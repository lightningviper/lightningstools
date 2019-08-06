using System;
using System.Collections.Generic;
using Common.Math;
using Common.Networking;
using F4SharedMem;
using F4SharedMem.Headers;
using MFDExtractor.FlightDataAdapters;
using F4Utils.Terrain;
using LightningGauges.Renderers.F16;
using LightningGauges.Renderers.F16.AzimuthIndicator;
using LightningGauges.Renderers.F16.RWR;
using LightningGauges.Renderers.F16.EHSI;
using LightningGauges.Renderers.F16.HSI;
using LightningGauges.Renderers.F16.ISIS;
using Common.Drawing;

namespace MFDExtractor
{
    internal interface IFlightDataUpdater
    {
        void UpdateRendererStatesFromFlightData(
            IDictionary<InstrumentType, IInstrument> instruments,
            FlightData flightData,
            TerrainDB terrainDB,
            Action updateEHSIBrightnessLabelVisibility,
            F4TexSharedMem.IReader texSharedmemReader);
    }

    internal class FlightDataUpdater : IFlightDataUpdater
    {
        private readonly IFlightDataAdapterSet _flightDataAdapterSet;
        public FlightDataUpdater( 
            IFlightDataAdapterSet flightDataAdapterSet = null)
        {
            _flightDataAdapterSet = flightDataAdapterSet ?? new FlightDataAdapterSet();
        }
        public void UpdateRendererStatesFromFlightData(
            IDictionary<InstrumentType,IInstrument> instruments,
            FlightData flightData,
            TerrainDB terrainDB,
            Action updateEHSIBrightnessLabelVisibility,
            F4TexSharedMem.IReader texSharedmemReader)
        {
            if (flightData == null || (Extractor.State.NetworkMode != NetworkMode.Client && !Extractor.State.SimRunning))
            {
                flightData = new FlightData {hsiBits = Int32.MaxValue};
            }
            var hsi = instruments[InstrumentType.HSI].Renderer as IHorizontalSituationIndicator;
            var ehsi = instruments[InstrumentType.EHSI].Renderer as IEHSI;
            var adi = instruments[InstrumentType.ADI].Renderer as IADI;
            var isis = instruments[InstrumentType.ISIS].Renderer as IISIS;


            if (Extractor.State.SimRunning || Extractor.State.NetworkMode == NetworkMode.Client || Extractor.State.OptionsFormIsShowing)
            {
                var hsibits = ((HsiBits) flightData.hsiBits);
                _flightDataAdapterSet.ISIS.Adapt(instruments[InstrumentType.ISIS].Renderer as IISIS, flightData);
                _flightDataAdapterSet.VVI.Adapt(instruments[InstrumentType.VVI].Renderer as IVerticalVelocityIndicator, flightData);
                _flightDataAdapterSet.Altimeter.Adapt(instruments[InstrumentType.Altimeter].Renderer as IAltimeter, flightData);
                _flightDataAdapterSet.AirspeedIndicator.Adapt(instruments[InstrumentType.ASI].Renderer as IAirspeedIndicator, flightData);
                _flightDataAdapterSet.Compass.Adapt(instruments[InstrumentType.Compass].Renderer as ICompass, flightData);
                _flightDataAdapterSet.AOAIndicator.Adapt(instruments[InstrumentType.AOAIndicator].Renderer as IAngleOfAttackIndicator, flightData);
                _flightDataAdapterSet.AOAIndexer.Adapt(instruments[InstrumentType.AOAIndexer].Renderer as IAngleOfAttackIndexer, flightData);
                UpdateADI(instruments[InstrumentType.ADI].Renderer as IADI, hsibits);
                _flightDataAdapterSet.StandbyADI.Adapt(instruments[InstrumentType.BackupADI].Renderer as IStandbyADI, flightData);
                UpdateHSI(instruments[InstrumentType.HSI].Renderer as IHorizontalSituationIndicator, instruments[InstrumentType.EHSI].Renderer as IEHSI, hsibits, flightData);


                //***** UPDATE SOME COMPLEX HSI/ADI VARIABLES
                if (ADIIsTurnedOff(hsibits))
                {
                    SetADIToOffState(instruments[InstrumentType.ADI].Renderer as IADI);
                    SetISISToOffState(instruments[InstrumentType.ISIS].Renderer as IISIS);
                }
                else
                {
                    SetADIPitchAndRoll(instruments[InstrumentType.ADI].Renderer as IADI, flightData);
                    SetISISPitchAndRoll(instruments[InstrumentType.ISIS].Renderer as IISIS, flightData);

                    //The following floating data is also crossed up in the flightData.h File:
                    //float AdiIlsHorPos;       // Position of horizontal ILS bar ----Vertical
                    //float AdiIlsVerPos;       // Position of vertical ILS bar-----horizontal
                    var commandBarsOn = ((float) (Math.Abs(Math.Round(flightData.AdiIlsHorPos, 4))) != 0.1745f);
                    if ((Math.Abs((flightData.AdiIlsVerPos/Common.Math.Constants.RADIANS_PER_DEGREE)) > 1.0f)
                        ||
                        (Math.Abs((flightData.AdiIlsHorPos/Common.Math.Constants.RADIANS_PER_DEGREE)) > 5.0f))
                    {
                        commandBarsOn = false;
                    }
                    hsi.InstrumentState.ShowToFromFlag = true;
                    ehsi.InstrumentState.ShowToFromFlag = true;

                    //if the TOTALFLAGS flag is off, then we're most likely in NAV mode
                    if ((hsibits & HsiBits.TotalFlags) != HsiBits.TotalFlags)
                    {
                        hsi.InstrumentState.ShowToFromFlag = false;
                        ehsi.InstrumentState.ShowToFromFlag = false;
                    }
                        //if the TO/FROM flag is showing in shared memory, then we are most likely in TACAN mode 
                    else if ((((hsibits & HsiBits.ToTrue) == HsiBits.ToTrue)
                        ||
                        ((hsibits & HsiBits.FromTrue) == HsiBits.FromTrue)))
                    {
                        if (!commandBarsOn) //better make sure we're not in any ILS mode too though
                        {
                            hsi.InstrumentState.ShowToFromFlag = true;
                            ehsi.InstrumentState.ShowToFromFlag = true;
                        }
                    }

                    //if the glideslope or localizer flags on the ADI are turned on, then we must be in an ILS mode and therefore we 
                    //know we don't need to show the HSI TO/FROM flags.
                    if (((hsibits & HsiBits.ADI_GS) == HsiBits.ADI_GS)
                        ||
                        ((hsibits & HsiBits.ADI_LOC) == HsiBits.ADI_LOC))
                    {
                        hsi.InstrumentState.ShowToFromFlag = false;
                        ehsi.InstrumentState.ShowToFromFlag = false;
                    }
                    if (commandBarsOn)
                    {
                        hsi.InstrumentState.ShowToFromFlag = false;
                        ehsi.InstrumentState.ShowToFromFlag = false;
                    }
                    adi.InstrumentState.ShowCommandBars = commandBarsOn;
                    adi.InstrumentState.GlideslopeDeviationDegrees = flightData.AdiIlsVerPos/Common.Math.Constants.RADIANS_PER_DEGREE;
                    adi.InstrumentState.LocalizerDeviationDegrees = flightData.AdiIlsHorPos/Common.Math.Constants.RADIANS_PER_DEGREE;

                    isis.InstrumentState.ShowCommandBars = commandBarsOn;
                    isis.InstrumentState.GlideslopeDeviationDegrees = flightData.AdiIlsVerPos/Common.Math.Constants.RADIANS_PER_DEGREE;
                    isis.InstrumentState.LocalizerDeviationDegrees = flightData.AdiIlsHorPos/Common.Math.Constants.RADIANS_PER_DEGREE;
                }
                
                UpdateNavigationMode(hsi, ehsi, adi, isis, flightData);
                if (((hsibits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF))
                {
                    TurnOffHSI(hsi);
                    TurnOffEHSI(ehsi);
                }
                else
                {
                    UpdateHSIFlightData(hsi, flightData, hsibits);
                    UpdateEHSIFlightData(ehsi, flightData, hsibits);
                }

                UpdateHSIAndEHSICourseDeviationAndToFromFlags(hsi, ehsi);
                UpdateEHSI(updateEHSIBrightnessLabelVisibility);
                _flightDataAdapterSet.HYDA.Adapt(instruments[InstrumentType.HYDA].Renderer as IHydraulicPressureGauge, flightData);
                _flightDataAdapterSet.HYDB.Adapt(instruments[InstrumentType.HYDB].Renderer as IHydraulicPressureGauge, flightData);
                _flightDataAdapterSet.CabinPress.Adapt(instruments[InstrumentType.CabinPress].Renderer as ICabinPressureAltitudeIndicator, flightData);
                _flightDataAdapterSet.RollTrim.Adapt(instruments[InstrumentType.RollTrim].Renderer as IRollTrimIndicator, flightData);
                _flightDataAdapterSet.PitchTrim.Adapt(instruments[InstrumentType.PitchTrim].Renderer as IPitchTrimIndicator, flightData);
                _flightDataAdapterSet.AzimuthIndicator.Adapt(instruments[InstrumentType.AzimuthIndicator].Renderer as IAzimuthIndicator, flightData);
                _flightDataAdapterSet.RWR.Adapt(instruments[InstrumentType.RWR].Renderer as IRWRRenderer, flightData);
                _flightDataAdapterSet.CautionPanel.Adapt(instruments[InstrumentType.CautionPanel].Renderer as ICautionPanel, flightData);
                _flightDataAdapterSet.CMDS.Adapt(instruments[InstrumentType.CMDS].Renderer as ICMDSPanel, flightData);
                _flightDataAdapterSet.DED.Adapt(instruments[InstrumentType.DED].Renderer as IDataEntryDisplayPilotFaultList, flightData);
                _flightDataAdapterSet.PFL.Adapt(instruments[InstrumentType.PFL].Renderer as IDataEntryDisplayPilotFaultList, flightData);
                _flightDataAdapterSet.EPUFuel.Adapt(instruments[InstrumentType.EPUFuel].Renderer as IEPUFuelGauge, flightData);
                _flightDataAdapterSet.FuelFlow.Adapt(instruments[InstrumentType.FuelFlow].Renderer as IFuelFlow, flightData);
                _flightDataAdapterSet.FuelQuantity.Adapt(instruments[InstrumentType.FuelQuantity].Renderer as IFuelQuantityIndicator, flightData);
                _flightDataAdapterSet.LandingGearLights.Adapt(instruments[InstrumentType.GearLights].Renderer as ILandingGearWheelsLights, flightData);
                _flightDataAdapterSet.NWS.Adapt(instruments[InstrumentType.NWSIndexer].Renderer as INosewheelSteeringIndexer, flightData);
                _flightDataAdapterSet.Speedbrake.Adapt(instruments[InstrumentType.Speedbrake].Renderer as ISpeedbrakeIndicator, flightData);
                _flightDataAdapterSet.RPM1.Adapt(instruments[InstrumentType.RPM1].Renderer as ITachometer, flightData);
                _flightDataAdapterSet.RPM2.Adapt(instruments[InstrumentType.RPM2].Renderer as ITachometer, flightData);
                _flightDataAdapterSet.FTIT1.Adapt(instruments[InstrumentType.FTIT1].Renderer as IFanTurbineInletTemperature, flightData);
                _flightDataAdapterSet.FTIT2.Adapt(instruments[InstrumentType.FTIT2].Renderer as IFanTurbineInletTemperature, flightData);
                _flightDataAdapterSet.NOZ1.Adapt(instruments[InstrumentType.NOZ1].Renderer as INozzlePositionIndicator, flightData);
                _flightDataAdapterSet.NOZ2.Adapt(instruments[InstrumentType.NOZ2].Renderer as INozzlePositionIndicator, flightData);
                _flightDataAdapterSet.OIL1.Adapt(instruments[InstrumentType.OIL1].Renderer as IOilPressureGauge, flightData);
                _flightDataAdapterSet.OIL2.Adapt(instruments[InstrumentType.OIL2].Renderer as IOilPressureGauge, flightData);
                _flightDataAdapterSet.Accelerometer.Adapt(instruments[InstrumentType.Accelerometer].Renderer as IAccelerometer, flightData);
			}
            else //Falcon's not running
            {
                if (instruments[InstrumentType.VVI].Renderer is IVerticalVelocityIndicatorEU)
                {
                    ((IVerticalVelocityIndicatorEU) instruments[InstrumentType.VVI].Renderer).InstrumentState.OffFlag = true;
                }
                else if (instruments[InstrumentType.VVI].Renderer is IVerticalVelocityIndicatorUSA)
                {
                    ((IVerticalVelocityIndicatorUSA) instruments[InstrumentType.VVI].Renderer).InstrumentState.OffFlag = true;
                }
                ((IAngleOfAttackIndicator)(instruments[InstrumentType.AOAIndicator].Renderer)).InstrumentState.OffFlag = true;
                hsi.InstrumentState.OffFlag = true;
                ehsi.InstrumentState.NoDataFlag = true;
                ((IADI)instruments[InstrumentType.ADI].Renderer).InstrumentState.OffFlag = true;
                ((IStandbyADI)instruments[InstrumentType.BackupADI].Renderer).InstrumentState.OffFlag = true;
                ((IAzimuthIndicator)instruments[InstrumentType.AzimuthIndicator].Renderer).InstrumentState.RWRPowerOn = false;
                ((IISIS)instruments[InstrumentType.ISIS].Renderer).InstrumentState.RadarAltitudeAGL = 0;
                ((IISIS)instruments[InstrumentType.ISIS].Renderer).InstrumentState.OffFlag = true;
                updateEHSIBrightnessLabelVisibility();
            }
            var left = flightData.RTT_area[(int)RTT_areas.RTT_MFDLEFT * 4];
            var top = flightData.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 1];
            var right = flightData.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 2];
            var bottom = flightData.RTT_area[((int)RTT_areas.RTT_MFDLEFT * 4) + 3];
            var lmfdSourceRect = new Rectangle(left, top, right - left, bottom - top);                
            _flightDataAdapterSet.LMFD.Adapt(instruments[InstrumentType.LMFD], texSharedmemReader, lmfdSourceRect, InstrumentType.LMFD);

            left = flightData.RTT_area[(int)RTT_areas.RTT_MFDRIGHT * 4];
            top = flightData.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 1];
            right = flightData.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 2];
            bottom = flightData.RTT_area[((int)RTT_areas.RTT_MFDRIGHT * 4) + 3];
            var rmfdSourceRect = new Rectangle(left, top, right - left, bottom - top);
            _flightDataAdapterSet.RMFD.Adapt(instruments[InstrumentType.RMFD], texSharedmemReader, rmfdSourceRect, InstrumentType.RMFD);

            left = flightData.RTT_area[(int)RTT_areas.RTT_HUD * 4];
            top = flightData.RTT_area[((int)RTT_areas.RTT_HUD * 4) + 1];
            right = flightData.RTT_area[((int)RTT_areas.RTT_HUD * 4) + 2];
            bottom = flightData.RTT_area[((int)RTT_areas.RTT_HUD * 4) + 3];
            var hudSourceRect = new Rectangle(left, top, right - left, bottom - top);
            _flightDataAdapterSet.HUD.Adapt(instruments[InstrumentType.HUD], texSharedmemReader, hudSourceRect, InstrumentType.HUD);
		}

        private static void SetISISPitchAndRoll(IISIS isis, FlightData flightData)
        {
            isis.InstrumentState.PitchDegrees = ((flightData.pitch/Common.Math.Constants.RADIANS_PER_DEGREE));
            isis.InstrumentState.RollDegrees = ((flightData.roll /Common.Math.Constants.RADIANS_PER_DEGREE));
        }

        private static void SetADIPitchAndRoll(IADI adi, FlightData flightData)
        {
            adi.InstrumentState.PitchDegrees = ((flightData.pitch / Common.Math.Constants.RADIANS_PER_DEGREE));
            adi.InstrumentState.RollDegrees = ((flightData.roll / Common.Math.Constants.RADIANS_PER_DEGREE));
        }

        private static bool ADIIsTurnedOff(HsiBits hsibits)
        {
            return ((hsibits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF);
        }

        private static void SetISISToOffState(IISIS isis)
        {
            isis.InstrumentState.PitchDegrees = 0;
            isis.InstrumentState.RollDegrees = 0;
            isis.InstrumentState.GlideslopeDeviationDegrees = 0;
            isis.InstrumentState.LocalizerDeviationDegrees = 0;
            isis.InstrumentState.ShowCommandBars = false;
        }

        private static void SetADIToOffState(IADI adi)
        {
            adi.InstrumentState.PitchDegrees = 0;
            adi.InstrumentState.RollDegrees = 0;
            adi.InstrumentState.GlideslopeDeviationDegrees = 0;
            adi.InstrumentState.LocalizerDeviationDegrees = 0;
            adi.InstrumentState.ShowCommandBars = false;
        }

        private static void UpdateNavigationMode(IHorizontalSituationIndicator hsi, IEHSI ehsi, IADI adi, IISIS isis, FlightData flightData)
        {
            /*
                This value is called navMode and is unsigned char type with 4 possible values: ILS_TACAN = 0, and TACAN = 1,
                NAV = 2, ILS_NAV = 3
                */

            byte bmsNavMode = flightData.navMode;
            switch (bmsNavMode)
            {
                case 0: //NavModes.PlsTcn:
                    hsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.InstrumentMode = InstrumentModes.PlsTacan;
                    break;
                case 1: //NavModes.Tcn:
                    hsi.InstrumentState.ShowToFromFlag = true;
                    ehsi.InstrumentState.ShowToFromFlag = true;
                    ehsi.InstrumentState.InstrumentMode = InstrumentModes.Tacan;
                    adi.InstrumentState.ShowCommandBars = false;
                    isis.InstrumentState.ShowCommandBars = false;
                    break;
                case 2: //NavModes.Nav:
                    hsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.InstrumentMode = InstrumentModes.Nav;
                    adi.InstrumentState.ShowCommandBars = false;
                    isis.InstrumentState.ShowCommandBars = false;
                    break;
                case 3: //NavModes.PlsNav:
                    hsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.ShowToFromFlag = false;
                    ehsi.InstrumentState.InstrumentMode = InstrumentModes.PlsNav;
                    break;
            }
        }

        private static void UpdateHSIAndEHSICourseDeviationAndToFromFlags(IHorizontalSituationIndicator hsi, IEHSI ehsi)
        {
            var deviationLimitDecimalDegrees = hsi.InstrumentState.CourseDeviationLimitDegrees%180;
            var courseDeviationDecimalDegrees = hsi.InstrumentState.CourseDeviationDegrees;
            bool toFlag;
            bool fromFlag;
            if (Math.Abs(courseDeviationDecimalDegrees) <= 90)
            {
                toFlag = true;
                fromFlag = false;
            }
            else
            {
                toFlag = false;
                fromFlag = true;
            }

            if (courseDeviationDecimalDegrees < -90)
            {
                courseDeviationDecimalDegrees = Util.AngleDelta(Math.Abs(courseDeviationDecimalDegrees), 180)%180;
            }
            else if (courseDeviationDecimalDegrees > 90)
            {
                courseDeviationDecimalDegrees = -Util.AngleDelta(courseDeviationDecimalDegrees, 180)%180;
            }
            else
            {
                courseDeviationDecimalDegrees = -courseDeviationDecimalDegrees;
            }
            if (Math.Abs(courseDeviationDecimalDegrees) > deviationLimitDecimalDegrees)
            {
                courseDeviationDecimalDegrees = Math.Sign(courseDeviationDecimalDegrees)*deviationLimitDecimalDegrees;
            }

            hsi.InstrumentState.CourseDeviationDegrees = courseDeviationDecimalDegrees;
            hsi.InstrumentState.ToFlag = toFlag;
            hsi.InstrumentState.FromFlag = fromFlag;
            ehsi.InstrumentState.CourseDeviationDegrees = courseDeviationDecimalDegrees;
            ehsi.InstrumentState.ToFlag = toFlag;
            ehsi.InstrumentState.FromFlag = fromFlag;
        }

        private static void UpdateEHSIFlightData(IEHSI ehsi, FlightData flightData,
            HsiBits hsibits)
        {
            ehsi.InstrumentState.DmeInvalidFlag = ((hsibits & HsiBits.CourseWarning) == HsiBits.CourseWarning);
            ehsi.InstrumentState.DeviationInvalidFlag = ((hsibits & HsiBits.IlsWarning) == HsiBits.IlsWarning);
            ehsi.InstrumentState.CourseDeviationLimitDegrees = flightData.deviationLimit;
            ehsi.InstrumentState.CourseDeviationDegrees = flightData.courseDeviation;
            ehsi.InstrumentState.DesiredCourseDegrees = (int) flightData.desiredCourse;
            ehsi.InstrumentState.DesiredHeadingDegrees = (int) flightData.desiredHeading;
            ehsi.InstrumentState.BearingToBeaconDegrees = flightData.bearingToBeacon;
            ehsi.InstrumentState.DistanceToBeaconNauticalMiles = flightData.distanceToBeacon;
        }

        private static void UpdateHSIFlightData(IHorizontalSituationIndicator hsi, FlightData flightData, HsiBits hsibits)
        {
            hsi.InstrumentState.DmeInvalidFlag = ((hsibits & HsiBits.CourseWarning) ==HsiBits.CourseWarning);
            hsi.InstrumentState.DeviationInvalidFlag = ((hsibits &HsiBits.IlsWarning) == HsiBits.IlsWarning);
            hsi.InstrumentState.CourseDeviationLimitDegrees =flightData.deviationLimit;
            hsi.InstrumentState.CourseDeviationDegrees =flightData.courseDeviation;
            hsi.InstrumentState.DesiredCourseDegrees =(int) flightData.desiredCourse;
            hsi.InstrumentState.DesiredHeadingDegrees =(int) flightData.desiredHeading;
            hsi.InstrumentState.BearingToBeaconDegrees =flightData.bearingToBeacon;
            hsi.InstrumentState.DistanceToBeaconNauticalMiles =flightData.distanceToBeacon;
        }

        private static void TurnOffEHSI(IEHSI ehsi)
        {
            ehsi.InstrumentState.DmeInvalidFlag = true;
            ehsi.InstrumentState.DeviationInvalidFlag = false;
            ehsi.InstrumentState.CourseDeviationLimitDegrees = 0;
            ehsi.InstrumentState.CourseDeviationDegrees = 0;
            ehsi.InstrumentState.BearingToBeaconDegrees = 0;
            ehsi.InstrumentState.DistanceToBeaconNauticalMiles = 0;
        }

        private static void TurnOffHSI(IHorizontalSituationIndicator hsi)
        {
            hsi.InstrumentState.DmeInvalidFlag = true;
            hsi.InstrumentState.DeviationInvalidFlag = false;
            hsi.InstrumentState.CourseDeviationLimitDegrees = 0;
            hsi.InstrumentState.CourseDeviationDegrees = 0;
            hsi.InstrumentState.BearingToBeaconDegrees = 0;
            hsi.InstrumentState.DistanceToBeaconNauticalMiles = 0;
        }

        private static void UpdateEHSI(Action updateEHSIBrightnessLabelVisibility)
        {
            updateEHSIBrightnessLabelVisibility();
        }

        private static void UpdateHSI(IHorizontalSituationIndicator hsi, IEHSI ehsi, HsiBits hsibits, FlightData fromFalcon)
        {
            hsi.InstrumentState.OffFlag = ((hsibits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF) && !Extractor.State.OptionsFormIsShowing;
            hsi.InstrumentState.MagneticHeadingDegrees = (360 + (fromFalcon.yaw / Common.Math.Constants.RADIANS_PER_DEGREE)) % 360;
            ehsi.InstrumentState.NoDataFlag = ((hsibits & HsiBits.HSI_OFF) == HsiBits.HSI_OFF) && !Extractor.State.OptionsFormIsShowing; ;
            ehsi.InstrumentState.NoPowerFlag = ((fromFalcon.powerBits & (int)PowerBits.BusPowerBattery) != (int)PowerBits.BusPowerBattery) && !Extractor.State.OptionsFormIsShowing; 
            ehsi.InstrumentState.MagneticHeadingDegrees = (360 + (fromFalcon.yaw / Common.Math.Constants.RADIANS_PER_DEGREE)) % 360;
        }

        private static void UpdateADI(IADI adi, HsiBits hsibits)
        {
            
            adi.InstrumentState.OffFlag = ((hsibits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF);
            adi.InstrumentState.AuxFlag = ((hsibits & HsiBits.ADI_AUX) == HsiBits.ADI_AUX);
            adi.InstrumentState.GlideslopeFlag = ((hsibits & HsiBits.ADI_GS) == HsiBits.ADI_GS);
            adi.InstrumentState.LocalizerFlag = ((hsibits & HsiBits.ADI_LOC) == HsiBits.ADI_LOC);
        }

       
    }
}