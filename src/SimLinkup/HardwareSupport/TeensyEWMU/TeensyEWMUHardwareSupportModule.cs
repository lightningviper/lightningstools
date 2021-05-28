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
        private byte[] _lastPacketSent = null;
        private DateTime _lastPacketSentTime = DateTime.MinValue;

        private readonly TextSignal[] _textSignals;
        private readonly AnalogSignal[] _analogSignals;
        private readonly DigitalSignal[] _digitalSignals;
        private TextSignal _ewmuDisplayTextLine1Signal;
        private TextSignal _ewmuDisplayTextLine2Signal;
        private TextSignal _ewpiChaffFlareDisplayTextSignal;
        private TextSignal _ewpiJMRDisplayTextSignal;
        private TextSignal _cmdsDisplayTextSignal;
        private DigitalSignal _cmdsNoGoSignal;
        private DigitalSignal _cmdsGoSignal;
        private DigitalSignal _cmdsDispenseReadySignal;
        private DigitalSignal _ewpiPRISignal;
        private DigitalSignal _ewpiUNKSignal;
        private DigitalSignal _ewpiMLSignal;
        private AnalogSignal _bytesSentSignal;
		private TeensyEWMUCommunicationProtocolHeaders.InvertBits _invertBits;

        private TeensyEWMUHardwareSupportModule(TeensyEWMUHardwareSupportModuleConfig config)
        {
            _config = config;
			_invertBits = GetInvertBits();
            CreateSignals(out _analogSignals, out _textSignals, out _digitalSignals);
        }

        public override AnalogSignal[] AnalogInputs => null;
        public override AnalogSignal[] AnalogOutputs => _analogSignals;
        public override DigitalSignal[] DigitalInputs => _digitalSignals;
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

    private TeensyEWMUCommunicationProtocolHeaders.InvertBits GetInvertBits()
    {
        TeensyEWMUCommunicationProtocolHeaders.InvertBits bits = 0;
        foreach (DXOutput output in this._config.DXOutputs)
        {
            TeensyEWMUCommunicationProtocolHeaders.InvertBits bits2;
            bool flag = Enum.TryParse<TeensyEWMUCommunicationProtocolHeaders.InvertBits>(output.ID, out bits2);
            if (flag && output.Invert)
            {
                bits |= bits2;
            }
        }
        return bits;
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


        private void CreateSignals(out AnalogSignal[] analogSignals, out TextSignal[] textSignals, out DigitalSignal[] digitalSignals)
        {
            var textSignalsToReturn = new List<TextSignal>();
            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Management Unit",
                    FriendlyName = "EWMU Display Text Line 1",
                    Id = "TeensyEWMU__Display_Text_Line1",
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
                _ewmuDisplayTextLine1Signal = thisSignal;
                textSignalsToReturn.Add(thisSignal);

                thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Management Unit",
                    FriendlyName = "EWMU Display Text Line 2",
                    Id = "TeensyEWMU__Display_Text_Line2",
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
                _ewmuDisplayTextLine2Signal = thisSignal;
                textSignalsToReturn.Add(thisSignal);

                thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Prime Panel",
                    FriendlyName = "EWPI Chaff/Flare Display Text",
                    Id = "TeensyEWMU__EWPI_Chaff_Flare_Display_Text",
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
                _ewpiChaffFlareDisplayTextSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);

                thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Prime Panel",
                    FriendlyName = "EWPI JMR Status Window Display Text",
                    Id = "TeensyEWMU__EWPI_JMR_Display_Text",
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
                _ewpiJMRDisplayTextSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);

                thisSignal = new TextSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Countermeasures Dispensing System (CMDS) Panel",
                    FriendlyName = "CMDS Status Window Display Text",
                    Id = "TeensyEWMU__CMDS_Display_Text",
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
                _cmdsDisplayTextSignal = thisSignal;
                textSignalsToReturn.Add(thisSignal);
            }

            var digitalSignalsToReturn = new List<DigitalSignal>();
            {
                var thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Countermeasures Dispensing System (CMDS) Panel",
                    FriendlyName = "CMDS NOGO Annunciator flag",
                    Id = "TeensyEWMU__CMDS_NOGO",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _cmdsNoGoSignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);

                thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Countermeasures Dispensing System (CMDS) Panel",
                    FriendlyName = "CMDS GO Annunciator flag",
                    Id = "TeensyEWMU__CMDS_GO",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _cmdsGoSignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);

                thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Countermeasures Dispensing System (CMDS) Panel",
                    FriendlyName = "CMDS DISPENSE RDY Annunciator flag",
                    Id = "TeensyEWMU__CMDS_DISPENSE_RDY",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _cmdsDispenseReadySignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);

                thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Prime Panel",
                    FriendlyName = "EWPI PRI Indicator flag",
                    Id = "TeensyEWMU__EWPI_PRI",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _ewpiPRISignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);

                thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Prime Panel",
                    FriendlyName = "EWPI UNK Indicator flag",
                    Id = "TeensyEWMU__EWPI_UNK",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _ewpiUNKSignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);

                thisSignal = new DigitalSignal
                {
                    Category = "Inputs from Simulation",
                    CollectionName = "Electronic Warfare Prime Panel",
                    FriendlyName = "EWPI ML Indicator flag",
                    Id = "TeensyEWMU__EWPI_ML",
                    Index = 0,
                    PublisherObject = this,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _config.COMPort,
                    SubSource = null,
                    SubSourceFriendlyName = null,
                    SubSourceAddress = null,
                    State = false
                };
                _ewpiMLSignal = thisSignal;
                digitalSignalsToReturn.Add(thisSignal);
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
            digitalSignals = digitalSignalsToReturn.ToArray();
        }
    
        private void UpdateOutputs()
        {
            if (DateTime.Now.Subtract(_lastPacketSentTime).TotalMilliseconds < (1000 / MAX_UPDATE_FREQUENCY_HZ))
            {
                return;
            }
            var connected = EnsureSerialPortConnected();
            if (!connected) return;

            byte ewpiLightbits=0;
            if (_ewpiPRISignal.State) ewpiLightbits |= (byte)EWPILightbits.PRI;
            if (_ewpiUNKSignal.State) ewpiLightbits |= (byte)EWPILightbits.UNK;
            if (_ewpiMLSignal.State) ewpiLightbits |= (byte)EWPILightbits.ML;

            byte cmdsLightbits = 0;
            if (_cmdsNoGoSignal.State) cmdsLightbits |= (byte)CMDSLightbits.NOGO;
            if (_cmdsGoSignal.State) cmdsLightbits |= (byte)CMDSLightbits.GO;
            if (_cmdsDispenseReadySignal.State) cmdsLightbits |= (byte)CMDSLightbits.DISPENSE_READY;

            string ewmuDisplayText = (_ewmuDisplayTextLine1Signal.State ?? "").PadRight(16) + (_ewmuDisplayTextLine2Signal.State ?? "").PadRight(16);
            string cmdsDisplayText = (_cmdsDisplayTextSignal.State ?? "").PadRight(16);
            string ewpiDisplayText = (_ewpiChaffFlareDisplayTextSignal.State ?? "").PadRight(8) + (_ewpiJMRDisplayTextSignal.State ?? "").PadRight(8);

            byte[] packet = null;
            using (var stream = new MemoryStream())
            {
				using (BinaryWriter writer = new BinaryWriter(stream))
                {

                    stream.WriteByte((byte)TeensyEWMUPacketFields.EWMU_DISPLAY_STRING);
                    stream.Write(Encoding.ASCII.GetBytes(ewmuDisplayText), 0, 32);

                    stream.WriteByte((byte)TeensyEWMUPacketFields.EWPI_DISPLAY_STRING);
                    stream.Write(Encoding.ASCII.GetBytes(ewpiDisplayText), 0, 16);

                    stream.WriteByte((byte)TeensyEWMUPacketFields.CMDS_DISPLAY_STRING);
                    stream.Write(Encoding.ASCII.GetBytes(cmdsDisplayText), 0, 16);

                    stream.WriteByte((byte)TeensyEWMUPacketFields.CMDS_LIGHTBITS);
                    stream.WriteByte(cmdsLightbits);

                    stream.WriteByte((byte)TeensyEWMUPacketFields.EWPI_LIGHTBITS);
                    stream.WriteByte(ewpiLightbits);
					
					stream.WriteByte((byte)TeensyEWMUPacketFields.INVERT_STATES);
					writer.Write((uint)_invertBits);

                    stream.WriteByte((byte)TeensyEWMUPacketFields.CMDS_CONDITIONAL_BLANKING_BITS);
                    writer.Write((byte)CMDSConditionalDisplayBlankingBits.ENABLE_CONDITIONAL_BLANKING);

                    writer.Flush();
                    stream.Flush();
                    packet = stream.ToArray();
				}
            }
            SendPacket(packet);
            _lastPacketSentTime = DateTime.Now;
        }

        private void SendPacket(byte[] packet)
        {
            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen || packet == null || packet.Length ==0 
                        || (_lastPacketSent != null && packet.SequenceEqual(_lastPacketSent))
                        ) return;
                    var cobsEncodedPacket = COBS.Encode(packet);
                    _serialPort.Write(cobsEncodedPacket, offset: 0, count: cobsEncodedPacket.Length);
                    _serialPort.Write(new byte[] { 0 }, 0, 1);
                    _serialPort.BaseStream.Flush();
                    _lastPacketSent = packet;
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

        [Flags]
        private enum TeensyEWMUPacketFields:byte
        {
            EWMU_DISPLAY_STRING       = 0x01,
            CMDS_DISPLAY_STRING       = 0x02,
            EWPI_DISPLAY_STRING       = 0x04,
            EWPI_LIGHTBITS            = 0x08,
            CMDS_LIGHTBITS            = 0x10,
            CMDS_CONDITIONAL_BLANKING_BITS = 0x20,
            INVERT_STATES = 0x40,
        };

        [Flags]
        private enum EWPILightbits:byte
        {
            PRI = 0x01,
            UNK = 0x02,
            ML  = 0x04,
        };

        [Flags]
        private enum CMDSLightbits:byte
        {
            NOGO           = 0x01,
            GO             = 0x02,
            DISPENSE_READY = 0x04,
        }

        [Flags]
        private enum SwitchAndButtonIDs:ulong
        {
            CMDS_O1 = 0x1UL,
            CMDS_O2 = 0x2UL,
            CMDS_CH = 0x4UL,
            CMDS_FL = 0x8UL,
            CMDS_AND_EWMU_JETTISON = 0x10UL,
            CMDS_PRGM_BIT = 0x20UL,
            CMDS_PRGM_1 = 0x40UL,
            CMDS_PRGM_2 = 0x80UL,
            CMDS_PRGM_3 = 0x100UL,
            CMDS_PRGM_4 = 0x200UL,
            CMDS_AND_EWMU_MWS = 0x400UL,
            CMDS_AND_EWMU_JMR = 0x800UL,
            CMDS_AND_EWMU_RWR = 0x1000UL,
            EWMU_DISP = 0x2000UL,
            CMDS_AND_EWMU_MODE_OFF = 0x4000UL,
            CMDS_AND_EWMU_MODE_STBY = 0x8000UL,
            CMDS_AND_EWMU_MODE_MAN = 0x10000UL,
            CMDS_AND_EWMU_MODE_SEMI = 0x20000UL,
            CMDS_AND_EWMU_MODE_AUTO = 0x40000UL,
            CMDS_MODE_BYP = 0x80000UL,
            EWMU_MWS_MENU = 0x100000UL,
            EWMU_JMR_MENU = 0x200000UL,
            EWMU_RWR_MENU = 0x400000UL,
            EWMU_DISP_MENU = 0x800000UL,
            EWPI_PRI = 0x100000UL,
            EWPI_SEP = 0x200000UL,
            EWPI_UNK = 0x400000UL,
            EWPI_MD = 0x800000UL,
            EWMU_SET1 = 0x1000000UL,
            EWMU_SET2 = 0x2000000UL,
            EWMU_SET3 = 0x4000000UL,
            EWMU_SET4 = 0x8000000UL,
            EWMU_NXT_UP = 0x10000000UL,
            EWMU_NXT_DOWN = 0x20000000UL,
            EWMU_RTN = 0x40000000UL,
        }
        
        [Flags]
        private enum CMDSConditionalDisplayBlankingBits:byte
        {
            DISABLE_CONDITIONAL_BLANKING = 0x00,
            ENABLE_CONDITIONAL_BLANKING = 0x01,
        }
    }

}

