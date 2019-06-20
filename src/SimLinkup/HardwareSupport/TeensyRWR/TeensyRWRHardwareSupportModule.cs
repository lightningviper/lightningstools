using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Common.HardwareSupport;
using Common.IO.Ports;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16.RWR;
using log4net;
using SimLinkup.PacketEncoding;
using SerialPort = Common.IO.Ports.SerialPort;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    public class TeensyRWRHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const int MAX_RWR_SYMBOLS_AS_INPUTS = 64;
        private const int BAUD_RATE = 115200;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 8*1024;
        private const int SERIAL_WRITE_TIMEOUT = 500;
        private const int MAX_UPDATE_FREQUENCY_HZ = 20;
        private const int VIEWBOX_WIDTH = 4095;
        private const int VIEWBOX_HEIGHT = 4095;
		private const int DAC_PRECISION=12;
		private const int BEZIER_CURVE_INTERPOLATION_STEPS=100;
        private const int SMALLEST_BEAM_MOVEMENT_DAC_STEPS = 10;
        private const bool USE_VECTOR_FONT = true;
        private readonly BMSRWRRenderer _drawingCommandRenderer = new BMSRWRRenderer { ActualWidth = VIEWBOX_WIDTH, ActualHeight = VIEWBOX_HEIGHT };
        private readonly BMSRWRRenderer _uiRenderer = new BMSRWRRenderer();

        //limits exceptions when we don't have the RWR plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(TeensyRWRHardwareSupportModule));

        private readonly TeensyRWRHardwareSupportModuleConfig _config;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts;
        private DateTime _lastCommandListSentTime = DateTime.MinValue;
        private String _lastCommandList;

        private readonly AnalogSignal[] _analogInputSignals;
        private readonly DigitalSignal[] _digitalInputSignals;
        private readonly TextSignal[] _textInputSignals;

        private readonly AnalogSignal[] _rwrObjectBearingInputSignals = new AnalogSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly AnalogSignal[] _rwrObjectLethalityInputSignals = new AnalogSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly DigitalSignal[] _rwrObjectMissileActivityFlagInputSignals = new DigitalSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly DigitalSignal[] _rwrObjectMissileLaunchFlagInputSignals =new DigitalSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly DigitalSignal[] _rwrObjectNewDetectionFlagInputSignals =new DigitalSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly DigitalSignal[] _rwrObjectSelectedFlagInputSignals =new DigitalSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private readonly AnalogSignal[] _rwrObjectSymbolIDInputSignals = new AnalogSignal[MAX_RWR_SYMBOLS_AS_INPUTS];
        private AnalogSignal _magneticHeadingDegreesInputSignal;
        private TextSignal _rwrInfoInputSignal;
        private AnalogSignal _rwrSymbolCountInputSignal;
        private AnalogSignal _chaffCountInputSignal;
        private AnalogSignal _flareCountInputSignal;

        private TeensyRWRHardwareSupportModule(TeensyRWRHardwareSupportModuleConfig config)
        {
            _config = config;
            CreateInputSignals(out _analogInputSignals, out _digitalInputSignals, out _textInputSignals);
        }

        public override AnalogSignal[] AnalogInputs => _analogInputSignals;
        public override AnalogSignal[] AnalogOutputs => null;
        public override DigitalSignal[] DigitalInputs => _digitalInputSignals;
        public override DigitalSignal[] DigitalOutputs => null;
        public override TextSignal[] TextInputs => _textInputSignals;
        public override TextSignal[] TextOutputs => null;

        public override string FriendlyName => $"Teensy RWR module for IP-1310/ALR Azimuth Indicator (RWR) on {_config.COMPort}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TeensyRWRHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "TeensyRWRHardwareSupportModule.config");
                var hsmConfig = TeensyRWRHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                IHardwareSupportModule thisHsm = new TeensyRWRHardwareSupportModule(hsmConfig);
                toReturn.Add(thisHsm);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return toReturn.ToArray();
        }

        public override void Synchronize()
        {
            base.Synchronize();
            UpdateOutputs();
        }
        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            var instrumentState = GetInstrumentState();
            _uiRenderer.Render(g, destinationRectangle, instrumentState, USE_VECTOR_FONT);
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_config.COMPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
        }

        private void CloseSerialPortConnection()
        {
            lock (_serialPortLock)
            {
                if (_serialPort != null)
                {
                    try
                    {
                        if (_serialPort.IsOpen)
                        {
                            _log.DebugFormat($"Closing serial port {_config.COMPort}");
                            _serialPort.DiscardOutBuffer();
                            _serialPort.Close();
                        }
                        GC.ReRegisterForFinalize(_serialPort.BaseStream);
                        _serialPort.Dispose();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
                    _serialPort = null;
                }
                _unsuccessfulConnectionAttempts = 0; //reset unsuccessful connection attempts counter
            }
        }

        private void CreateInputSignals(out AnalogSignal[] analogSignals, out DigitalSignal[] digitalSignals, out TextSignal[] textSignals)
        {
            var analogSignalsToReturn = new List<AnalogSignal>();
            var digitalSignalsToReturn = new List<DigitalSignal>();
            var textSignalsToReturn = new List<TextSignal>();

            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = "RWR Additional Info",
                    Id = "TeensyRWR__RWR_AdditionalInfo",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = string.Empty
                };
                _rwrInfoInputSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Navigation",
                    FriendlyName = "Magnetic Heading (Degrees)",
                    Id = "TeensyRWR__Magnetic_Heading_Degrees",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    IsAngle = true,
                    MinValue = 0,
                    MaxValue = 360
                };
                _magneticHeadingDegreesInputSignal = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Chaff/Flare",
                    FriendlyName = "Chaff Count (# remaining)",
                    Id = "TeensyRWR__Chaff_Count",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    IsAngle = false,
                    MinValue = 0,
                    MaxValue = 99
                };
                _chaffCountInputSignal = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Chaff/Flare",
                    FriendlyName = "Flare Count (# remaining)",
                    Id = "TeensyRWR__Flare_Count",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    IsAngle = false,
                    MinValue = 0,
                    MaxValue = 99
                };
                _flareCountInputSignal = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = "RWR Symbol Count",
                    Id = "TeensyRWR__RWR_Symbol_Count",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    MinValue = 0,
                    MaxValue = 64
                };
                _rwrSymbolCountInputSignal = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }


            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Symbol ID",
                    Id = $"TeensyRWR__RWR_Object_Symbol_ID[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = -1,
                    MinValue = Int32.MinValue,
                    MaxValue = Int32.MaxValue
                };
                _rwrObjectSymbolIDInputSignals[i] = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }
            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Bearing (degrees)",
                    Id = $"TeensyRWR__RWR_Object_Bearing_Degrees[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    IsAngle = true,
                    MinValue = 0,
                    MaxValue = 360
                };
                _rwrObjectBearingInputSignals[i] = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }

            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Lethality",
                    Id = $"TeensyRWR__RWR_Object_Lethality[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = 0,
                    MinValue = -1,
                    MaxValue = 3
                };
                _rwrObjectLethalityInputSignals[i] = thisSignal;
                analogSignalsToReturn.Add(thisSignal);
            }

            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Missile Activity Flag",
                    Id = $"TeensyRWR__RWR_Object_Missile_Activity_Flag[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _rwrObjectMissileActivityFlagInputSignals[i] = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);
            }
            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Missile Launch Flag",
                    Id = $"TeensyRWR__RWR_Object_Missile_Launch_Flag[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _rwrObjectMissileLaunchFlagInputSignals[i] = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);
            }
            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} Selected Flag",
                    Id = $"TeensyRWR__RWR_Object_Selected_Flag[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _rwrObjectSelectedFlagInputSignals[i] = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);
            }
            for (var i = 0; i < MAX_RWR_SYMBOLS_AS_INPUTS; i++)
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Radar Warning Receiver",
                    FriendlyName = $"RWR Object #{i} New Detection Flag",
                    Id = $"TeensyRWR__RWR_Object_New_Detection_Flag[{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _rwrObjectNewDetectionFlagInputSignals[i] = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);
            }

            analogSignals = analogSignalsToReturn.ToArray();
            digitalSignals = digitalSignalsToReturn.ToArray();
            textSignals = textSignalsToReturn.ToArray();
        }

        private InstrumentState GetInstrumentState()
        {
            const float RADIANS_PER_DEGREE = 0.01745329252F;
            var instrumentState = new InstrumentState
            {
                bearing = _rwrObjectBearingInputSignals.OrderBy(x => x.Index).Select(x => (float)(x.State * RADIANS_PER_DEGREE)).ToArray(),
                ChaffCount = (float)_chaffCountInputSignal.State,
                FlareCount = (float)_flareCountInputSignal.State,
                lethality = _rwrObjectLethalityInputSignals.OrderBy(x => x.Index).Select(x => (float)x.State).ToArray(),
                missileActivity = _rwrObjectMissileActivityFlagInputSignals.OrderBy(x => x.Index).Select(x => (x.State ? 1: 0)).ToArray(),
                missileLaunch = _rwrObjectMissileLaunchFlagInputSignals.OrderBy(x => x.Index).Select(x => (x.State ? 1 : 0)).ToArray(),
                newDetection = _rwrObjectNewDetectionFlagInputSignals.OrderBy(x => x.Index).Select(x => (x.State ? 1 : 0)).ToArray(),
                RwrInfo = Encoding.Default.GetBytes(_rwrInfoInputSignal.State ?? string.Empty),
                RwrObjectCount = (int)_rwrSymbolCountInputSignal.State,
                RWRsymbol = _rwrObjectSymbolIDInputSignals.OrderBy(x => x.Index).Select(x => (int)x.State).ToArray(),
                selected = _rwrObjectSelectedFlagInputSignals.OrderBy(x => x.Index).Select(x => (x.State ? 1: 0)).ToArray(),
                yaw = (float)(_magneticHeadingDegreesInputSignal.State * RADIANS_PER_DEGREE)
            };
            return instrumentState;
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                try
                {
                    CloseSerialPortConnection();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
            _isDisposed = true;
        }
        

        private bool EnsureSerialPortConnected()
        {
            lock (_serialPortLock)
            {
                if (_serialPort == null)
                {
                    try
                    {
                        _serialPort = new SerialPort(_config.COMPort, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                        return false;
                    }
                }
                if (_serialPort != null && !_serialPort.IsOpen &&
                    _unsuccessfulConnectionAttempts < MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS)
                {
                    try
                    {
                        _serialPort.Handshake = HANDSHAKE;
                        _serialPort.WriteTimeout = SERIAL_WRITE_TIMEOUT;
                        _serialPort.ErrorReceived += _serialPort_ErrorReceived;
                        _serialPort.WriteBufferSize = WRITE_BUFFER_SIZE;
                        _log.DebugFormat(
                            $"Opening serial port {_config.COMPort}: Handshake:{HANDSHAKE}, WriteTimeout:{SERIAL_WRITE_TIMEOUT}, WriteBufferSize:{WRITE_BUFFER_SIZE}");
                        _serialPort.Open();
                        GC.SuppressFinalize(_serialPort.BaseStream);
                        _serialPort.DiscardOutBuffer();
                        _unsuccessfulConnectionAttempts = 0;
                        return true;
                    }
                    catch (Exception e)
                    {
                        _unsuccessfulConnectionAttempts++;
                        _log.Error(e.Message, e);
                    }
                }
                else if (_serialPort !=null && _serialPort.IsOpen)
                {
                    return true;
                }
                return false;
            }
        }

        private string GenerateDrawingCommands()
        {
            var instrumentState = GetInstrumentState();
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Append();
            drawingContext.PushTransform(new RotateTransform(_config.RotationDegrees, VIEWBOX_WIDTH /2.0, VIEWBOX_HEIGHT/2.0));
            _drawingCommandRenderer.Render(drawingContext, instrumentState, USE_VECTOR_FONT);
            drawingContext.Close();
            return PathGeometry.CreateFromGeometry(drawingGroup.GetGeometry()).ToString();
        }

        private IEnumerable<DrawPoint> CalibrateDrawPoints(IEnumerable<DrawPoint> uncalibratedDrawPoints)
        {
            return uncalibratedDrawPoints.Select(drawPoint =>
                CalibrationHelper.CalibrateDrawPoint(drawPoint, _config.XAxisCalibrationData,
                    _config.YAxisCalibrationData));
        }
        private void SendDrawingCommands(string svgPathString)
        {
            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen || svgPathString == null) return;
                    var svgPathToDrawPointsConverter = new SVGPathToVectorScopePointsListConverter(dacPrecisionBits:DAC_PRECISION, bezierCurveInterpolationSteps:BEZIER_CURVE_INTERPOLATION_STEPS, stepSize: SMALLEST_BEAM_MOVEMENT_DAC_STEPS);
                    var uncalibratedDrawPoints = svgPathToDrawPointsConverter.ConvertToDrawPoints(svgPathString).ToList();
                    var calibratedDrawPoints = CalibrateDrawPoints(uncalibratedDrawPoints).ToList();
                    var drawPointsAsBytes = calibratedDrawPoints.Select(x=>(byte[])x).SelectMany(x => x).ToArray();
                    var cobsEncodedPacket = COBS.Encode(drawPointsAsBytes);
                    _serialPort.Write(cobsEncodedPacket, offset: 0, count: cobsEncodedPacket.Length);
                    _serialPort.Write(new byte[]{0},0,1);
                    _serialPort.BaseStream.Flush();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
        }

        private void UpdateOutputs()
        {
            if (DateTime.Now.Subtract(_lastCommandListSentTime).TotalMilliseconds < (1000/MAX_UPDATE_FREQUENCY_HZ))
            {
                return;
            }
            var connected = EnsureSerialPortConnected();
            if (!connected) return;
            var commandList = GenerateDrawingCommands();
            if (_lastCommandList != null && commandList == _lastCommandList) return;
            SendDrawingCommands(commandList);
            _lastCommandList = commandList;
            _lastCommandListSentTime = DateTime.Now;
        }
    }
}