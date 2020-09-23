using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Drawing;
using Common.HardwareSupport;
using Common.IO.Ports;
using log4net;
using SimLinkup.PacketEncoding;
using SerialPort = Common.IO.Ports.SerialPort;
using Common.MacroProgramming;

namespace SimLinkup.HardwareSupport.TeensyEWMU
{
    public class TeensyEWMUHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const int BAUD_RATE = 115200;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 64 * 1024;
        private const int SERIAL_WRITE_TIMEOUT = 500;
        private const int MAX_UPDATE_FREQUENCY_HZ = 60;

        //limits exceptions when we don't have the EWMU plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(TeensyEWMUHardwareSupportModule));

        private readonly TeensyEWMUHardwareSupportModuleConfig _config;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts;
        private string _lastDisplayStringSent = null;
        private DateTime _lastDisplayStringSentTime = DateTime.MinValue;

        private readonly TextSignal[] _textSignals;
        private readonly AnalogSignal[] _analogSignals;
        private TextSignal _displayTextSignal;
        private AnalogSignal _bytesSentSignal;

        private TeensyEWMUHardwareSupportModule(TeensyEWMUHardwareSupportModuleConfig config)
        {
            _config = config;
            CreateSignals(out _analogSignals, out _textSignals);
        }

        public override AnalogSignal[] AnalogInputs => null;
        public override AnalogSignal[] AnalogOutputs => _analogSignals;
        public override DigitalSignal[] DigitalInputs => null;
        public override DigitalSignal[] DigitalOutputs => null;
        public override TextSignal[] TextInputs => _textSignals;
        public override TextSignal[] TextOutputs => null;

        public override string FriendlyName => $"Teensy EWMU module on {_config.COMPort}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TeensyEWMUHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "TeensyEWMUHardwareSupportModule.config");
                var hsmConfig = TeensyEWMUHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                IHardwareSupportModule thisHsm = new TeensyEWMUHardwareSupportModule(hsmConfig);
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
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_config.COMPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
        }


        private void CreateSignals(out AnalogSignal[] analogSignals, out TextSignal[] textSignals)
        {
            var textSignalsToReturn = new List<TextSignal>();
            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Management Unit",
                    FriendlyName = "Display Text",
                    Id = "TeensyEWMU__DisplayText",
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
                _displayTextSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }

            var analogSignalsToReturn = new List<AnalogSignal>();
            _bytesSentSignal = new AnalogSignal
            {
                Category = "Performance Tracking",
                CollectionName = "Data Throughput",
                FriendlyName = "# of Bytes Sent (last packet)",
                Id = "TeensyEWMU__Bytes_Sent_Last_Packet",
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
                MaxValue = 64 * 1024
            };
            analogSignalsToReturn.Add(_bytesSentSignal);

            analogSignals = analogSignalsToReturn.ToArray();
            textSignals = textSignalsToReturn.ToArray();
        }

        private void UpdateOutputs()
        {
            if (DateTime.Now.Subtract(_lastDisplayStringSentTime).TotalMilliseconds < (1000 / MAX_UPDATE_FREQUENCY_HZ))
            {
                return;
            }
            var connected = EnsureSerialPortConnected();
            if (!connected) return;
            string displayString = String.Format("THE TIME IS {0}", DateTime.Now.ToLongTimeString());
            SendDisplayString(displayString);
            _lastDisplayStringSentTime = DateTime.Now;
        }


        private void SendDisplayString(string displayString)
        {
            if (displayString == null || String.Equals(displayString, _lastDisplayStringSent, StringComparison.Ordinal)) return;

            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen || displayString == null) return;
                    var cobsEncodedPacket = COBS.Encode(Encoding.ASCII.GetBytes(displayString));
                    _serialPort.Write(cobsEncodedPacket, offset: 0, count: cobsEncodedPacket.Length);
                    _serialPort.Write(new byte[] { 0 }, 0, 1);
                    _serialPort.BaseStream.Flush();
                    _lastDisplayStringSent = displayString;
                    _bytesSentSignal.State = cobsEncodedPacket.Length;

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