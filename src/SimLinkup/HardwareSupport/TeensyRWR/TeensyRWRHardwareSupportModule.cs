using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Common.HardwareSupport;
using Common.IO.Ports;
using Common.MacroProgramming;
using log4net;
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
        private const int WRITE_BUFFER_SIZE = 64*1024;
        private const int SERIAL_WRITE_TIMEOUT = 1000;

        private BMSRWRRenderer _drawingCommandRenderer = new BMSRWRRenderer() { ActualWidth = 4096, ActualHeight = 4096 };
        private BMSRWRRenderer _uiRenderer = new BMSRWRRenderer();

        //limits exceptions when we don't have the RWR plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(TeensyRWRHardwareSupportModule));

        private readonly string _comPort;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts = 0;
        private DateTime _lastSynchronizedAt = DateTime.MinValue;
        private byte[] _lastCommandList=null;

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

        private TeensyRWRHardwareSupportModule(string comPort)
        {
            _comPort = comPort;
            CreateInputSignals(out _analogInputSignals, out _digitalInputSignals, out _textInputSignals);
        }

        public override AnalogSignal[] AnalogInputs => _analogInputSignals;
        public override AnalogSignal[] AnalogOutputs => null;
        public override DigitalSignal[] DigitalInputs => _digitalInputSignals;
        public override DigitalSignal[] DigitalOutputs => null;
        public override TextSignal[] TextInputs => _textInputSignals;
        public override TextSignal[] TextOutputs => null;

        public override string FriendlyName => $"Teensy RWR module for IP-1310/ALR Azimuth Indicator (RWR) on {_comPort}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TeensyRWRHardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "TeensyRWRHardwareSupportModule.config");
                var hsmConfig = TeensyRWRHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                IHardwareSupportModule thisHsm = new TeensyRWRHardwareSupportModule(hsmConfig.COMPort);
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
            _uiRenderer.Render(g, destinationRectangle, instrumentState);
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_comPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
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
                            _log.DebugFormat($"Closing serial port {_comPort}");
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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
                    SourceAddress = _comPort,
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

        private LightningGauges.Renderers.F16.RWR.InstrumentState GetInstrumentState()
        {
            const float RADIANS_PER_DEGREE = 0.01745329252F;
            var instrumentState = new LightningGauges.Renderers.F16.RWR.InstrumentState
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
                        _serialPort = new SerialPort(_comPort, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);
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
                            $"Opening serial port {_comPort}: Handshake:{HANDSHAKE}, WriteTimeout:{SERIAL_WRITE_TIMEOUT}, WriteBufferSize:{WRITE_BUFFER_SIZE}");
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

        private byte[] GenerateDrawingCommands()
        {
            var instrumentState = GetInstrumentState();
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Append();
            drawingContext.PushTransform(new ScaleTransform(1, -1));
            _drawingCommandRenderer.Render(drawingContext, instrumentState);
            drawingContext.Close();
            Rect bounds = new Rect(0, 0, _drawingCommandRenderer.ActualWidth, _drawingCommandRenderer.ActualHeight);
            using (var stream = new MemoryStream())
            { 
                VectorEncoder.Serialize(drawingGroup, bounds, stream);
                var length = (int)stream.Length;
                stream.Seek(0, SeekOrigin.Begin);
                var toReturn = new byte[length];
                stream.Read(toReturn, 0, length);
                return toReturn;
            }
        }
        private byte[] PacketMarker = new[] { (byte)'\0' };
        private void SendDrawingCommands(byte[] drawingCommands)
        {
            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort != null && _serialPort.IsOpen && drawingCommands !=null)
                    {
                        var cobsEncodedPacket = PacketEncoding.COBS.Encode(drawingCommands);
                        if (cobsEncodedPacket != null)
                        {
                            _serialPort.Write(cobsEncodedPacket, 0, cobsEncodedPacket.Count());
                            _serialPort.Write(PacketMarker, 0, 1);
                            _serialPort.BaseStream.Flush();
                            System.Threading.Thread.Sleep(30);
                        }

                    }
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
        }

        private void UpdateOutputs()
        {
            var connected = EnsureSerialPortConnected();
            if (connected || true)
            {
                var commandList = GenerateDrawingCommands();
                if (_lastCommandList == null || commandList != _lastCommandList)
                {
                    SendDrawingCommands(commandList);
                    _lastCommandList = commandList;
                }
            }
        }
    }
}