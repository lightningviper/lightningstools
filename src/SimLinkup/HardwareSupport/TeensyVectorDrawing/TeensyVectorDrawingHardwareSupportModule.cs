using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Media;
using Common.HardwareSupport;
using Common.IO.Ports;
using Common.MacroProgramming;
using LightningGauges.Renderers.VectorDrawing;
using log4net;
using SimLinkup.PacketEncoding;
using SimLinkup.HardwareSupport.TeensyRWR;
using SerialPort = Common.IO.Ports.SerialPort;
using static SimLinkup.HardwareSupport.TeensyVectorDrawing.TeensyVectorDrawingHardwareSupportModuleConfig;

namespace SimLinkup.HardwareSupport.TeensyVectorDrawing
{
    public class TeensyVectorDrawingHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const int BAUD_RATE = 115200;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 64*1024;
        private const int SERIAL_WRITE_TIMEOUT = 500;
        private const int MAX_UPDATE_FREQUENCY_HZ = 60;
        private const int VIEWBOX_WIDTH = 4095;
        private const int VIEWBOX_HEIGHT = 4095;
		private const int BEZIER_CURVE_INTERPOLATION_STEPS=25;
        private readonly IVectorDrawingRenderer _drawingCommandRenderer = new VectorDrawingRenderer { };
        private readonly IVectorDrawingRenderer _uiRenderer = new VectorDrawingRenderer();

        //limits exceptions when we don't have the device plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(TeensyVectorDrawingHardwareSupportModule));

        private readonly TeensyVectorDrawingHardwareSupportModuleConfig _config;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts;
        private DateTime _lastCommandListSentTime = DateTime.MinValue;
        private String _lastCommandList;

        private readonly AnalogSignal[] _analogInputSignals;
        private readonly DigitalSignal[] _digitalInputSignals;
        private readonly TextSignal[] _textInputSignals;

        private TextSignal _hudVectorDrawingCommandsInputSignal;
        private TextSignal _rwrVectorDrawingCommandsInputSignal;
        private TextSignal _hmsVectorDrawingCommandsInputSignal;
        private TextSignal _cockpitArtDirectoryInputSignal;

        private AnalogSignal _numDrawPointsSentSignal;
        private AnalogSignal _bytesSent;

        private TeensyVectorDrawingHardwareSupportModule(TeensyVectorDrawingHardwareSupportModuleConfig config)
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

        public override string FriendlyName => $"Teensy Vector Drawing module for BMS {_config?.DeviceType.ToString() ?? "RWR/HUD/HMS" } on {(_config !=null ? _config.COMPort : "UNKNOWN")}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TeensyVectorDrawingHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "TeensyVectorDrawingHardwareSupportModule.config");
                var hsmConfig = TeensyVectorDrawingHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                IHardwareSupportModule thisHsm = new TeensyVectorDrawingHardwareSupportModule(hsmConfig);
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
            switch (_config.TestPattern)
            {
                default:
                    _uiRenderer.InstrumentState = instrumentState;
                    _uiRenderer.Render(g, destinationRectangle);
                    break;
            }
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_config.COMPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
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
                    CollectionName = "Vector Drawing Commands",
                    FriendlyName = "HUD Drawing Commands",
                    Id = "TeensyVectorDrawing__HUD_Drawing_Commands",
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
                _hudVectorDrawingCommandsInputSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }


            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Vector Drawing Commands",
                    FriendlyName = "RWR Drawing Commands",
                    Id = "TeensyVectorDrawing__RWR_Drawing_Commands",
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
                _rwrVectorDrawingCommandsInputSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }



            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Vector Drawing Commands",
                    FriendlyName = "HMS Drawing Commands",
                    Id = "TeensyVectorDrawing__HMS_Drawing_Commands",
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
                _hmsVectorDrawingCommandsInputSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }

            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Simulation Data",
                    FriendlyName = "BMS Cockpit Art Directory",
                    Id = "TeensyVectorDrawing__BMS_Cockpit_Art_Directory",
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
                _cockpitArtDirectoryInputSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }


            _numDrawPointsSentSignal = new AnalogSignal
            {
                Category = "Performance Tracking",
                CollectionName = "Data Throughput",
                FriendlyName = "# of Draw Points Sent (last packet)",
                Id = "TeensyVectorDrawing__Num_Draw_Points_Sent_Last_Packet",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 21*1024
            };
            analogSignalsToReturn.Add(_numDrawPointsSentSignal);

            _bytesSent = new AnalogSignal
            {
                Category = "Performance Tracking",
                CollectionName = "Data Throughput",
                FriendlyName = "# of Bytes Sent (last packet)",
                Id = "TeensyVectorDrawing__Bytes_Sent_Last_Packet",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 64*1024
            };
            analogSignalsToReturn.Add(_bytesSent);


            analogSignals = analogSignalsToReturn.ToArray();
            digitalSignals = digitalSignalsToReturn.ToArray();
            textSignals = textSignalsToReturn.ToArray();
        }
        private InstrumentState GetInstrumentState()
        {
            TextSignal vectorDrawingCommandsInputSignal;
            switch (_config?.DeviceType ?? VectorDrawingDeviceType.RWR)
            {
                case VectorDrawingDeviceType.HUD:
                    vectorDrawingCommandsInputSignal = _hudVectorDrawingCommandsInputSignal;
                    break;
                case VectorDrawingDeviceType.HMS:
                    vectorDrawingCommandsInputSignal = _hmsVectorDrawingCommandsInputSignal;
                    break;
                case VectorDrawingDeviceType.RWR:
                default:
                    vectorDrawingCommandsInputSignal = _rwrVectorDrawingCommandsInputSignal;
                    break;
            }

            var drawingCommands = vectorDrawingCommandsInputSignal.State;
            drawingCommands = !string.IsNullOrWhiteSpace(drawingCommands) ? drawingCommands : "";

            var cockpitArtDirectory = _cockpitArtDirectoryInputSignal.State;
            cockpitArtDirectory = !string.IsNullOrWhiteSpace(cockpitArtDirectory) ? cockpitArtDirectory : "";

            var instrumentState = new InstrumentState
            {
                DrawingCommands = drawingCommands,
                FontDirectory = string.IsNullOrWhiteSpace(cockpitArtDirectory) ? "" : Path.Combine(cockpitArtDirectory, "3DFont")
            };
            return instrumentState;
        }

        private void UpdateOutputs()
        {
            var millisSinceLastCommand = DateTime.Now.Subtract(_lastCommandListSentTime).TotalMilliseconds;
            if (millisSinceLastCommand < (1000 / MAX_UPDATE_FREQUENCY_HZ))
            {
                return;
            }
            else if (millisSinceLastCommand > 5000)
            {
                _lastCommandList = string.Empty;
            }
            var connected = EnsureSerialPortConnected();
            if (!connected) return;
            System.Windows.Size dimensions;
            var commandList = GenerateDrawingCommands(out dimensions);
            if (_lastCommandList != null && commandList == _lastCommandList && _config.TestPattern ==0) return;
            SendDrawingCommands(commandList, dimensions);
            _lastCommandList = commandList;
            _lastCommandListSentTime = DateTime.Now;
        }


        private string GenerateDrawingCommands(out System.Windows.Size dimensions)
        {
            var instrumentState = GetInstrumentState();
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Append();
            switch (_config.TestPattern)
            {
                default:
                    _drawingCommandRenderer.InstrumentState = instrumentState;
                    _drawingCommandRenderer.Render(drawingContext);
                    dimensions = _drawingCommandRenderer.GetResolution();
                    break;
            }
            drawingContext.Close();
            return PathGeometry.CreateFromGeometry(drawingGroup.GetGeometry()).ToString();
        }

        private void SendDrawingCommands(string svgPathString, System.Windows.Size sourceDrawingDimensions)
        {
            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen || svgPathString == null) return;
                    var drawPoints = new SVGPathToVectorScopePointsListConverter(bezierCurveInterpolationSteps: BEZIER_CURVE_INTERPOLATION_STEPS)
                                        .ConvertToDrawPoints(svgPathString)
                                        .ApplyScaling(VIEWBOX_WIDTH / sourceDrawingDimensions.Width, VIEWBOX_HEIGHT / sourceDrawingDimensions.Width)
                                        .ApplyCentering(_config.Centering.OffsetX, _config.Centering.OffsetY)
                                        .ApplyInversion(VIEWBOX_WIDTH, VIEWBOX_HEIGHT, invertX: false, invertY: true)
                                        .ApplyRotation(VIEWBOX_WIDTH / 2.0, VIEWBOX_HEIGHT / 2.0, _config.RotationDegrees)
                                        .ApplyScaling(_config.Scaling.ScaleX, _config.Scaling.ScaleY)
                                        .ApplyCalibration(_config.XAxisCalibrationData, _config.YAxisCalibrationData)
                                        .ApplyClipping(VIEWBOX_WIDTH, VIEWBOX_HEIGHT)
                                      ;

                    var drawPointsAsBytes = drawPoints.Select(x=>(byte[])x).SelectMany(x => x).ToArray();

                    var cobsEncodedPacket = COBS.Encode(drawPointsAsBytes);

                    _serialPort.Write(cobsEncodedPacket, offset: 0, count: cobsEncodedPacket.Length);
                    _serialPort.Write(new byte[]{0},0,1);
                    _serialPort.BaseStream.Flush();

                    _numDrawPointsSentSignal.State = drawPoints.Count();
                    _bytesSent.State = cobsEncodedPacket.Length;

                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
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
                else if (_serialPort != null && _serialPort.IsOpen)
                {
                    return true;
                }
                return false;
            }
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
                        try
                        {
                            GC.ReRegisterForFinalize(_serialPort.BaseStream);
                        }
                        catch { }
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

    }
}