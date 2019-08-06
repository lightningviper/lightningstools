using System;
using System.Collections.Generic;
using System.Linq;
using Common.Drawing;
using Common.SimSupport;
using F16CPD.Mfd.Controls;
using F16CPD.Networking;
using F16CPD.Properties;
using F16CPD.SimSupport.Falcon4.EventHandlers;
using F16CPD.SimSupport.Falcon4.MovingMap;
using F16CPD.SimSupport.Falcon4.Networking;
using F4KeyFile;
using F4SharedMem;
using F4SharedMem.Headers;
using F4Utils.Process;
using F4Utils.SimSupport;
using F4Utils.Terrain;
using log4net;

namespace F16CPD.SimSupport.Falcon4
{
    //TODO: PRIO blank RALTs in certain attitudes
    internal sealed class Falcon4Support : ISimSupportModule, IDisposable
    {
        private const TacanBand BackupTacanBand = TacanBand.X;
        private static readonly ILog Log = LogManager.GetLogger(typeof (Falcon4Support));
        private readonly IClientSideInboundMessageProcessor _clientSideInboundMessageProcessor;
        private readonly F4Utils.SimSupport.IIndicatedRateOfTurnCalculator _indicatedRateOfTurnCalculator =
            new IndicatedRateOfTurnCalculator();
        private readonly IInputControlEventHandler _inputControlEventHandler;
        private readonly MorseCode _morseCodeGenerator;
        private readonly IServerSideInboundMessageProcessor _serverSideInboundMessageProcessor;
        private readonly ITerrainDBFactory _terrainDBFactory = new TerrainDBFactory();

        private bool _isDisposed;
        private KeyFile _keyFile;
        private bool _morseCodeSignalLineValue;
        private IMovingMap _movingMap;
        private KeyWithModifiers _pendingComboKeys;
        private Queue<bool> _pendingMorseCodeUnits = new Queue<bool>();
        private F4SharedMem.Reader _sharedMemReader;
        private F4TexSharedMem.Reader _texSharedMemReader;
        private TacanChannelSource TacanChannelSource = TacanChannelSource.Ufc;
        private TerrainDB _terrainDB;
        private ITheaterMapRetriever _theaterMapRetriever;
        public Falcon4Support(F16CpdMfdManager manager)
        {
            Manager = manager;

            InitializeFlightData();
            _morseCodeGenerator = new MorseCode {CharactersPerMinute = 53};
            _morseCodeGenerator.UnitTimeTick += MorseCodeUnitTimeTick;
            _inputControlEventHandler = new InputControlEventHandler(Manager);

            _clientSideInboundMessageProcessor = new ClientSideInboundMessageProcessor();
            _serverSideInboundMessageProcessor = new ServerSideInboundMessageProcessor(Manager);
        }

        #region ISimSupportModule Members

        public bool IsSendingInput
        {
            get { return FalconCallbackSender.IsSendingInput; }
        }

        public F16CpdMfdManager Manager { get; set; }

        public bool IsSimRunning { get; private set; }

        public void InitializeTestMode()
        {
            if (!Settings.Default.RunAsClient)
            {
                LoadCurrentKeyFile();
                EnsureTerrainIsLoaded();
            }
        }

        #endregion

        public void HandleInputControlEvent(CpdInputControls eventSource, MfdInputControl control)
        {
            _inputControlEventHandler.HandleInputControlEvent(eventSource, control);
        }

        public bool ProcessPendingMessageToClientFromServer(Message message)
        {
            return _clientSideInboundMessageProcessor.ProcessPendingMessage(message);
        }

        public bool ProcessPendingMessageToServerFromClient(Message message)
        {
            return _serverSideInboundMessageProcessor.ProcessPendingMessage(message);
        }


        public void RenderMap(Graphics g, Rectangle renderRect, float mapZoom, 
            int rangeRingRadiusInNauticalMiles,
            MapRotationMode rotationMode)
        {
            if (_movingMap == null)
            {
                _movingMap = new MovingMap.MovingMap(_terrainDB, Manager.Client);
            }
            _movingMap.RenderMap(
                g, renderRect,
                mapZoom,
                Manager.FlightData.MapCoordinateFeetEast, Manager.FlightData.MapCoordinateFeetNorth,Manager.FlightData.MagneticHeadingInDecimalDegrees, 
                rangeRingRadiusInNauticalMiles, rotationMode);
        }

        #region Destructors

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Falcon4Support()
        {
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //dispose of managed resources here
                    Common.Util.DisposeObject(_keyFile);
                    _keyFile = null;
                    Common.Util.DisposeObject(_sharedMemReader);
                    _sharedMemReader = null;
                    Common.Util.DisposeObject(_texSharedMemReader);
                    _texSharedMemReader = null;
                    Common.Util.DisposeObject(_pendingComboKeys);
                    _pendingComboKeys = null;
                    Common.Util.DisposeObject(_terrainDB);
                    _terrainDB = null;
                }
            }
            // Code to dispose the un-managed resources of the class
            _isDisposed = true;
        }

        #endregion

        private void MorseCodeUnitTimeTick(object sender, UnitTimeTickEventArgs e)
        {
            _pendingMorseCodeUnits.Enqueue(e.CurrentSignalLineState);
        }

        private void GetNextMorseCodeUnit()
        {
            if (_pendingMorseCodeUnits.Count > 0)
            {
                bool nextUnit = _pendingMorseCodeUnits.Dequeue();
                _morseCodeSignalLineValue = nextUnit;
            }
            if (_pendingMorseCodeUnits.Count <= 1000) return;
            var units = _pendingMorseCodeUnits.ToArray();
            _pendingMorseCodeUnits.Clear();
            var newUnits = new Queue<bool>();
            for (var i = 0; i < 100; i++)
            {
                newUnits.Enqueue(units[i]);
            }
            _pendingMorseCodeUnits = newUnits;
        }

        #region Flight Data/State Management

        public void InitializeFlightData()
        {
            FlightData flightData = Manager.FlightData;
            flightData.AltimeterMode = AltimeterMode.Electronic;
            flightData.AutomaticLowAltitudeWarningInFeet = 300;
            flightData.BarometricPressure = 29.92f;
            flightData.AltimeterUnits = AltimeterUnits.Hg;
            flightData.HsiCourseDeviationLimitInDecimalDegrees = 10;
            flightData.HsiDesiredCourseInDegrees = 0;
            flightData.HsiDesiredHeadingInDegrees = 0;
            flightData.HsiDeviationInvalidFlag = false;
            flightData.TransitionAltitudeInFeet = 18000;
            flightData.RateOfTurnInDecimalDegreesPerSecond = 0;
            flightData.VviOffFlag = false;
            flightData.AoaOffFlag = false;
            flightData.HsiOffFlag = false;
            flightData.AdiOffFlag = false;
            flightData.PfdOffFlag = false;
            flightData.RadarAltimeterOffFlag = false;
            flightData.CpdPowerOnFlag = true;
            flightData.MarkerBeaconOuterMarkerFlag = false;
            flightData.MarkerBeaconMiddleMarkerFlag = false;
            flightData.AdiEnableCommandBars = false;
            flightData.TacanChannel = "106X";
            flightData.ActiveMFD = "LMFD";
            _morseCodeSignalLineValue = false;
            _indicatedRateOfTurnCalculator.Reset();
            IsSimRunning = false;
        }

        public void UpdateManagerFlightData()
        {
            GetNextMorseCodeUnit();

            if (Settings.Default.RunAsClient)
            {
                PerformClientSideFlightDataUpdates();
                return;
            }

            FlightData flightData = Manager.FlightData;

            CreateSharedMemReaderIfNotExists();
            F4SharedMem.FlightData fromFalcon = ReadF4SharedMem();

            if (_keyFile == null)
            {
                LoadCurrentKeyFile();
            }
            EnsureTerrainIsLoaded();

            if (_sharedMemReader != null && _sharedMemReader.IsFalconRunning)
            {
                IsSimRunning = true;
                if (fromFalcon == null) fromFalcon = new F4SharedMem.FlightData();
                var hsibits = ((HsiBits) fromFalcon.hsiBits);

                flightData.VviOffFlag = ((hsibits & HsiBits.VVI) == HsiBits.VVI);
                flightData.AoaOffFlag = ((hsibits & HsiBits.AOA) == HsiBits.AOA);
                flightData.HsiOffFlag = ((hsibits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF);
                flightData.AdiOffFlag = ((hsibits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF);
                flightData.PfdOffFlag = false;

                UpdateCpdPowerState(flightData, fromFalcon);

                flightData.RadarAltimeterOffFlag = ((fromFalcon.lightBits & (int) LightBits.RadarAlt) ==
                                                    (int) LightBits.RadarAlt);
                flightData.BarometricPressure = (fromFalcon.AltCalReading/100.00f);
                flightData.AltimeterUnits = (((AltBits)fromFalcon.altBits & AltBits.CalType) == AltBits.CalType) ? AltimeterUnits.Hg : AltimeterUnits.hPa;
                flightData.AltimeterMode = (((AltBits)fromFalcon.altBits & AltBits.PneuFlag) == AltBits.PneuFlag) ? AltimeterMode.Pneumatic : AltimeterMode.Electronic;
                switch (fromFalcon.navMode) 
                { 
                    case (byte)F4SharedMem.Headers.NavModes.ILS_NAV:
                        flightData.NavMode = NavModes.IlsNav;
                        break;
                    case (byte)F4SharedMem.Headers.NavModes.ILS_TACAN:
                        flightData.NavMode = NavModes.IlsTcn;
                        break;
                    case (byte)F4SharedMem.Headers.NavModes.TACAN:
                        flightData.NavMode = NavModes.Tcn;
                        break;
                    case (byte)F4SharedMem.Headers.NavModes.NAV:
                        flightData.NavMode = NavModes.Nav;
                        break;
                }
                UpdateIndicatedAltitude(flightData, fromFalcon);
                UpdateAltitudeAGL(flightData, fromFalcon);
                UpdateIndicatedAirspeed(flightData, fromFalcon);
                UpdateALOW(flightData, fromFalcon);

                flightData.TrueAirspeedInDecimalFeetPerSecond = fromFalcon.vt;
                flightData.MachNumber = fromFalcon.mach;
                flightData.GroundSpeedInDecimalFeetPerSecond =
                    (float) Math.Sqrt((fromFalcon.xDot*fromFalcon.xDot) + (fromFalcon.yDot*fromFalcon.yDot));

                flightData.MagneticHeadingInDecimalDegrees = (360 +
                                                              (fromFalcon.yaw/Common.Math.Constants.RADIANS_PER_DEGREE))%
                                                             360;


                UpdateVerticalVelocity(flightData, fromFalcon, hsibits);

                flightData.AngleOfAttackInDegrees = ((hsibits & HsiBits.AOA) == HsiBits.AOA) ? 0 : fromFalcon.alpha;

                flightData.AdiAuxFlag = ((hsibits & HsiBits.ADI_AUX) == HsiBits.ADI_AUX);
                flightData.AdiGlideslopeInvalidFlag = ((hsibits & HsiBits.ADI_GS) == HsiBits.ADI_GS);
                flightData.AdiLocalizerInvalidFlag = ((hsibits & HsiBits.ADI_LOC) == HsiBits.ADI_LOC);

                UpdateMarkerBeaconLight(flightData, fromFalcon);

                if (((hsibits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF))
                {
                    TurnOffADI(flightData);
                }
                else
                {
                    flightData.PitchAngleInDecimalDegrees = ((fromFalcon.pitch/Common.Math.Constants.RADIANS_PER_DEGREE));
                    flightData.RollAngleInDecimalDegrees = fromFalcon.roll/Common.Math.Constants.RADIANS_PER_DEGREE;
                    flightData.BetaAngleInDecimalDegrees = fromFalcon.beta;
                    flightData.GammaAngleInDecimalDegrees = fromFalcon.gamma/Common.Math.Constants.RADIANS_PER_DEGREE;
                    flightData.WindOffsetToFlightPathMarkerInDecimalDegrees = fromFalcon.windOffset/
                                                                              Common.Math.Constants.RADIANS_PER_DEGREE;

                    flightData.AdiIlsGlideslopeDeviationInDecimalDegrees = fromFalcon.AdiIlsVerPos/
                                                                           Common.Math.Constants.RADIANS_PER_DEGREE;
                    flightData.AdiIlsLocalizerDeviationInDecimalDegrees = fromFalcon.AdiIlsHorPos/
                                                                          Common.Math.Constants.RADIANS_PER_DEGREE;

                    UpdateHSIToFromFlagVisibilityAndADICommandBarsVisibilityBasedOnBMS4NavMode(flightData,fromFalcon);
                   
                }


                UpdateHSIData(flightData, fromFalcon, hsibits);

                flightData.RateOfTurnInDecimalDegreesPerSecond= _indicatedRateOfTurnCalculator.DetermineIndicatedRateOfTurn(flightData.MagneticHeadingInDecimalDegrees);

                UpdateTACANChannel(flightData, fromFalcon);
                UpdateMapPosition(flightData, fromFalcon);
                flightData.LMFDImage = GetLMFDImage();
                flightData.RMFDImage = GetRMFDImage();
            }
            else //Falcon's not running
            {
                IsSimRunning = false;
                if (Settings.Default.ShutoffIfFalconNotRunning)
                {
                    TurnOffAllInstruments(flightData);
                }


                if (_sharedMemReader != null)
                {
                    Common.Util.DisposeObject(_sharedMemReader);
                    _sharedMemReader = null;
                }
                if (_texSharedMemReader != null)
                {
                    Common.Util.DisposeObject(_texSharedMemReader);
                    _texSharedMemReader = null;
                }

                Common.Util.DisposeObject(_keyFile);
                Common.Util.DisposeObject(_terrainDB);
                _keyFile = null;
                _terrainDB = null;
            }

            //if running in server mode, send updated flight data to client 
            if (Settings.Default.RunAsServer)
            {
                F16CPDServer.SetSimProperty("F4FlightData", Common.Serialization.Util.ToRawBytes(flightData));
            }
        }

        private static void NotifyClientThatSimIsNotRunningOnServer()
        {
            if (Settings.Default.RunAsServer)
            {
                F16CPDServer.SetSimProperty("SimName", null);
                F16CPDServer.SetSimProperty("SimVersion", null);
                F16CPDServer.SetSimProperty("F4FlightData", null);
            }
        }

        private void UpdateCpdPowerState(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            
            flightData.CpdPowerOnFlag = ((fromFalcon.lightBits3 & (int) LightBits3.Power_Off) !=
                                            (int) LightBits3.Power_Off);
            
        }

        private void UpdateIndicatedAltitude(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            
            flightData.IndicatedAltitudeAboveMeanSeaLevelInDecimalFeet = -fromFalcon.aauz;
            
        }

        private static void UpdateHSIData(FlightData flightData, F4SharedMem.FlightData fromFalcon, HsiBits hsibits)
        {
            if (((hsibits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF))
            {
                TurnOffHSI(flightData);
            }
            else
            {
                flightData.HsiDistanceInvalidFlag = ((hsibits & HsiBits.CourseWarning) == HsiBits.CourseWarning);
                flightData.HsiDeviationInvalidFlag = ((hsibits & HsiBits.IlsWarning) == HsiBits.IlsWarning);
                flightData.HsiCourseDeviationLimitInDecimalDegrees = fromFalcon.deviationLimit;
                flightData.HsiCourseDeviationInDecimalDegrees = fromFalcon.courseDeviation;
                flightData.HsiLocalizerDeviationInDecimalDegrees = fromFalcon.localizerCourse;
                flightData.HsiDesiredCourseInDegrees = (int) fromFalcon.desiredCourse;
                flightData.HsiDesiredHeadingInDegrees = (int) fromFalcon.desiredHeading;
                flightData.HsiBearingToBeaconInDecimalDegrees = fromFalcon.bearingToBeacon;
                flightData.HsiDistanceToBeaconInNauticalMiles = fromFalcon.distanceToBeacon;
            }
        }


        private void UpdateTACANChannel(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            if (TacanChannelSource == TacanChannelSource.Backup)
            {
                flightData.TacanChannel = fromFalcon.AUXTChan + Enum.GetName(typeof (TacanBand), BackupTacanBand);
            }
            else if (TacanChannelSource == TacanChannelSource.Ufc)
            {
                flightData.TacanChannel = fromFalcon.UFCTChan.ToString();
            }
        }

        private void UpdateMarkerBeaconLight(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            bool outerMarkerFromFalcon = ((fromFalcon.hsiBits & (int) HsiBits.OuterMarker) == (int) HsiBits.OuterMarker);
            bool middleMarkerFromFalcon = ((fromFalcon.hsiBits & (int) HsiBits.MiddleMarker) ==
                                           (int) HsiBits.MiddleMarker);

            if (Settings.Default.RunAsServer)
            {
                flightData.MarkerBeaconOuterMarkerFlag = outerMarkerFromFalcon;
                flightData.MarkerBeaconMiddleMarkerFlag = middleMarkerFromFalcon;
            }
            else
            {
                flightData.MarkerBeaconOuterMarkerFlag = outerMarkerFromFalcon & _morseCodeSignalLineValue;
                flightData.MarkerBeaconMiddleMarkerFlag = middleMarkerFromFalcon & _morseCodeSignalLineValue;

                if (outerMarkerFromFalcon)
                {
                    _morseCodeGenerator.PlainText = "T"; //dot
                }
                else if (middleMarkerFromFalcon)
                {
                    _morseCodeGenerator.PlainText = "A"; //dot-dash
                }
                if ((outerMarkerFromFalcon || middleMarkerFromFalcon) && !_morseCodeGenerator.Sending)
                {
                    _morseCodeGenerator.StartSending();
                }
                else if (!outerMarkerFromFalcon && !middleMarkerFromFalcon)
                {
                    _morseCodeGenerator.StopSending();
                }
            }
        }

        private static void UpdateVerticalVelocity(FlightData flightData, F4SharedMem.FlightData fromFalcon,
            HsiBits hsibits)
        {
            if (((hsibits & HsiBits.VVI) == HsiBits.VVI))
            {
                flightData.VerticalVelocityInDecimalFeetPerSecond = 0;
            }
            else
            {
                flightData.VerticalVelocityInDecimalFeetPerSecond = -fromFalcon.zDot;
            }
        }

        private void UpdateALOW(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            flightData.AutomaticLowAltitudeWarningInFeet = fromFalcon.caraAlow;
        }

        private static void UpdateIndicatedAirspeed(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            if (Settings.Default.UseAsiLockoutSpeed && (fromFalcon.kias < Settings.Default.AsiLockoutSpeedKnots))
            {
                //lockout airspeed if under 60 knots
                {
                    flightData.IndicatedAirspeedInDecimalFeetPerSecond = 0;
                }
            }
            else
            {
                flightData.IndicatedAirspeedInDecimalFeetPerSecond = fromFalcon.kias*Common.Math.Constants.FPS_PER_KNOT;
            }
        }

        private void UpdateAltitudeAGL(FlightData flightData, F4SharedMem.FlightData fromFalcon, uint lod=2)
        {
            try
            {
                float terrainHeight = _terrainDB.CalculateTerrainHeight(fromFalcon.x, fromFalcon.y, lod);
                float agl = -fromFalcon.z - terrainHeight;

                //reset AGL altitude to zero if we're on the ground
                if (
                    ((fromFalcon.lightBits & (int) LightBits.ONGROUND) == (int) LightBits.ONGROUND)
                    ||
                    (
                        ((fromFalcon.lightBits3 & (int) LightBits3.OnGround) == (int) LightBits3.OnGround)
                        
                        )
                    )
                {
                    agl = 0;
                }
                if (agl < 0) agl = 0;
                flightData.AltitudeAboveGroundLevelInDecimalFeet = agl;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message, e);
                flightData.AltitudeAboveGroundLevelInDecimalFeet =
                    flightData.TrueAltitudeAboveMeanSeaLevelInDecimalFeet;
            }
        }


        private static void UpdateHSIToFromFlagVisibilityAndADICommandBarsVisibilityBasedOnBMS4NavMode(
            FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            var showToFromFlag = true;
            var showCommandBars =
                           ((Math.Abs((fromFalcon.AdiIlsVerPos / Common.Math.Constants.RADIANS_PER_DEGREE)) <= (fromFalcon.deviationLimit / 5.0f))
                               &&
                           (Math.Abs((fromFalcon.AdiIlsHorPos / Common.Math.Constants.RADIANS_PER_DEGREE)) <= fromFalcon.deviationLimit))
                               &&
                           !(((HsiBits)fromFalcon.hsiBits & HsiBits.ADI_GS) == HsiBits.ADI_GS)
                           &&
                           !(((HsiBits)fromFalcon.hsiBits & HsiBits.ADI_LOC) == HsiBits.ADI_LOC)
                           &&
                           !(((HsiBits)fromFalcon.hsiBits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF);

            switch (fromFalcon.navMode)
            {
                case 0: //NavModes.PlsTcn:
                    showToFromFlag = false;
                    break;
                case 1: //NavModes.Tcn:
                    showToFromFlag = true;
                    showCommandBars = false;
                    break;
                case 2: //NavModes.Nav:
                    showToFromFlag = false;
                    showCommandBars = false;
                    break;
                case 3: //NavModes.PlsNav:
                    showToFromFlag = false;
                    break;
            }

            if (showCommandBars)
            {
                showToFromFlag = false;
            }
            flightData.AdiEnableCommandBars = showCommandBars;
            flightData.HsiDisplayToFromFlag = showToFromFlag;
        }

        private void PerformClientSideFlightDataUpdates()
        {
            try
            {
                var serializedFlightData = (string) Manager.Client.GetSimProperty("F4FlightData");
                FlightData fromServer = null;
                if (!String.IsNullOrEmpty(serializedFlightData))
                {
                    IsSimRunning = true;
                    fromServer = (FlightData) Common.Serialization.Util.FromRawBytes(serializedFlightData);
                    UpdateNewServerFlightDataWithCertainExistingClientFlightData(fromServer);
                    Manager.FlightData = fromServer;

                    bool outerMarkerFromFalcon = Manager.FlightData.MarkerBeaconOuterMarkerFlag;
                    bool middleMarkerFromFalcon = Manager.FlightData.MarkerBeaconMiddleMarkerFlag;

                    Manager.FlightData.MarkerBeaconOuterMarkerFlag &= _morseCodeSignalLineValue;
                    Manager.FlightData.MarkerBeaconMiddleMarkerFlag &= _morseCodeSignalLineValue;

                    if (outerMarkerFromFalcon)
                    {
                        if (_morseCodeGenerator != null)
                            _morseCodeGenerator.PlainText = "T"; //dot
                    }
                    else if (middleMarkerFromFalcon)
                    {
                        if (_morseCodeGenerator != null)
                            _morseCodeGenerator.PlainText = "A"; //dot-dash
                    }
                    if (_morseCodeGenerator != null)
                        if ((outerMarkerFromFalcon || middleMarkerFromFalcon) && !_morseCodeGenerator.Sending)
                        {
                            if (!_morseCodeGenerator.Sending)
                            {
                                _pendingMorseCodeUnits.Clear();
                                _morseCodeGenerator.StartSending();
                            }
                        }
                        else if (!outerMarkerFromFalcon && !middleMarkerFromFalcon)
                        {
                            _morseCodeGenerator.StopSending();
                            _pendingMorseCodeUnits.Clear();
                        }
                }
                else
                {
                    IsSimRunning = false;
                }
                if (fromServer == null)
                {
                    fromServer = new FlightData();
                    InitializeFlightData();
                    Manager.FlightData = fromServer;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private static void TurnOffAllInstruments(FlightData flightData)
        {
            flightData.VviOffFlag = true;
            flightData.AoaOffFlag = true;
            flightData.HsiOffFlag = true;
            flightData.AdiOffFlag = true;
            flightData.CpdPowerOnFlag = false;
            flightData.RadarAltimeterOffFlag = true;
            flightData.PfdOffFlag = true;
        }

        private void UpdateMapPosition(FlightData flightData, F4SharedMem.FlightData fromFalcon)
        {
            int latWholeDegrees;
            float latMinutes;
            int longWholeDegrees;
            float longMinutes;
            _terrainDB.CalculateLatLong(fromFalcon.x, fromFalcon.y, out latWholeDegrees,
                out latMinutes,
                out longWholeDegrees, out longMinutes);
            flightData.LatitudeInDecimalDegrees = latWholeDegrees + (latMinutes/60.0f);
            flightData.LongitudeInDecimalDegrees = longWholeDegrees + (longMinutes/60.0f);
            flightData.MapCoordinateFeetEast = fromFalcon.y;
            flightData.MapCoordinateFeetNorth = fromFalcon.x;
        }

        private static void TurnOffHSI(FlightData flightData)
        {
            flightData.HsiDistanceInvalidFlag = true;
            flightData.HsiDeviationInvalidFlag = false;
            flightData.HsiCourseDeviationLimitInDecimalDegrees = 0;
            flightData.HsiCourseDeviationInDecimalDegrees = 0;
            flightData.HsiLocalizerDeviationInDecimalDegrees = 0;
            flightData.HsiBearingToBeaconInDecimalDegrees = 0;
            flightData.HsiDistanceToBeaconInNauticalMiles = 0;
        }

        private static void TurnOffADI(FlightData flightData)
        {
            //if the ADI is off
            flightData.PitchAngleInDecimalDegrees = 0;
            flightData.RollAngleInDecimalDegrees = 0;
            flightData.BetaAngleInDecimalDegrees = 0;
            flightData.GammaAngleInDecimalDegrees = 0;
            flightData.AdiIlsGlideslopeDeviationInDecimalDegrees = 0;
            flightData.AdiIlsLocalizerDeviationInDecimalDegrees = 0;
            flightData.AdiEnableCommandBars = false;
            flightData.WindOffsetToFlightPathMarkerInDecimalDegrees = 0;
        }

        private void UpdateNewServerFlightDataWithCertainExistingClientFlightData(FlightData newServerFlightData)
        {
            //TODO: move all these variables to private state inside the manager
            var existingFlightData = Manager.FlightData;
            newServerFlightData.TransitionAltitudeInFeet = existingFlightData.TransitionAltitudeInFeet;
        }

        private void EnsureTerrainIsLoaded()
        {
            if (_terrainDB == null)
            {
                var curFlightData = ReadF4SharedMem();
                if (curFlightData == null) return;
                string bmsBaseDir = curFlightData.StringData.data.Where(x => (x.strId == (uint)StringIdentifier.BmsBasedir)).First().value;
                if (string.IsNullOrEmpty(bmsBaseDir)) return;
                _terrainDB = _terrainDBFactory.Create(bmsBaseDir, false);
                if (_terrainDB != null)
                {
                    _theaterMapRetriever = new TheaterMapRetriever(_terrainDB, Manager.Client);
                    PublishTheaterMapForClients();
                }
            }
        }

        private void PublishTheaterMapForClients()
        {
            float mapWidthInFeet = float.NaN;
            _theaterMapRetriever.GetTheaterMapImage(ref mapWidthInFeet);
        }

        #endregion

        #region Falcon Process Detection and Manipulation Functions

        private F4SharedMem.FlightData ReadF4SharedMem()
        {
            var toReturn = new F4SharedMem.FlightData();

            CreateSharedMemReaderIfNotExists();
            if (_sharedMemReader != null)
            {
                toReturn = _sharedMemReader.GetCurrentData();
            }
            return toReturn;
        }
        private byte[] GetMFDImage(Rectangle sourceRectangle)
        {
            CreateTexSharedMemReaderIfNotExists();
            if (_texSharedMemReader != null && _texSharedMemReader.IsDataAvailable)
            {
                using (var image = _texSharedMemReader.GetImage(sourceRectangle))
                {
                    return Common.Imaging.Util.BytesFromBitmap(image, "RLE", "PNG");
                }
            }
            return null;
        }
        private byte[] GetLMFDImage()
        {
            var latestSharedMem = ReadF4SharedMem();
            if (latestSharedMem != null)
            {
                var left = latestSharedMem.RTT_area[(int)RTT_areas.RTT_MFDLEFT * 4];
                var top = latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 1];
                var right = latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 2];
                var bottom = latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 3];
                var rect = new Rectangle(left, top, right - left, bottom - top);
                return GetMFDImage(rect);
            }
            return Array.Empty<byte>();
        }
        private byte[] GetRMFDImage()
        {
            var latestSharedMem = ReadF4SharedMem();
            if (latestSharedMem != null)
            {
                var left = latestSharedMem.RTT_area[(int)RTT_areas.RTT_MFDRIGHT * 4];
                var top = latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 1];
                var right= latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 2];
                var bottom = latestSharedMem.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 3];
                var rect = new Rectangle(left, top, right-left, bottom-top);
                return GetMFDImage(rect);
            }
            return Array.Empty<byte>();
        }

        private void CreateSharedMemReaderIfNotExists()
        {
            if (_sharedMemReader == null && !Settings.Default.RunAsClient)
            {
                _sharedMemReader = new F4SharedMem.Reader();
            }
        }
        private void CreateTexSharedMemReaderIfNotExists()
        {
            if (_texSharedMemReader == null && !Settings.Default.RunAsClient)
            {
                _texSharedMemReader = new F4TexSharedMem.Reader();
            }
        }

        private void LoadCurrentKeyFile()
        {
            if (!Settings.Default.RunAsClient)
            {
                _keyFile = KeyFileUtils.GetCurrentKeyFile();
            }
        }

        #endregion
    }
}