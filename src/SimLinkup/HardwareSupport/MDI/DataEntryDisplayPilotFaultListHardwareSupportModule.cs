using System;
using System.Collections.Generic;
using System.Drawing;
using Common.HardwareSupport;
using Common.MacroProgramming;
using System.IO;
using System.IO.Ports;
using System.Linq;
using log4net;
using LightningGauges.Renderers.F16;
using System.Text;
using Common.IO.Ports;
using SerialPort = Common.IO.Ports.SerialPort;

namespace SimLinkup.HardwareSupport.MDI.DedPflHardwareSupportModule
{
    //MicroChip DED/PFL
    public class DataEntryDisplayPilotFaultListHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        //limits exceptions when we don't have the DED/PFL plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private const int BAUD_RATE = 19200;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 64 * 1024;
        private const int SERIAL_WRITE_TIMEOUT = 500;


        private static readonly ILog Log = LogManager.GetLogger(typeof(DataEntryDisplayPilotFaultListHardwareSupportModule));
        private readonly IDataEntryDisplayPilotFaultList _renderer = new DataEntryDisplayPilotFaultList();
        private bool _isDisposed;
        private readonly object _serialPortLock = new object();
        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts;

        private List<TextSignal> _displayLinesInputSignals = new List<TextSignal>();
        private List<TextSignal> _invertLinesInputSignals = new List<TextSignal>();

        //DEVICE CONFIG
        private readonly DeviceConfig _deviceConfig;

        private DataEntryDisplayPilotFaultListHardwareSupportModule(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
            if (_deviceConfig != null)
            {
                ConfigureDevice();
                CreateInputSignals();
            }
        }

        public override TextSignal[] TextInputs => Enumerable.Union(_displayLinesInputSignals, _invertLinesInputSignals).ToArray();

        public override string FriendlyName => $"Microchip F-16 DED/PFL on [ {_deviceConfig.COMPort ?? "<UNKNOWN>"} ]";

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "DataEntryDisplayPilotFaultListHardwareSupportModule.config");
                var hsmConfig = DataEntryDisplayPilotFaultListHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        var hsmInstance = new DataEntryDisplayPilotFaultListHardwareSupportModule(deviceConfiguration);
                        toReturn.Add(hsmInstance);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            return toReturn.ToArray();
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
        }

        private void ConfigureDeviceConnection()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_deviceConfig.COMPort))
                {
                    var comPort = (_deviceConfig.COMPort ?? "").Trim();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.Line1 = Encoding.Default.GetBytes(_displayLinesInputSignals[0].State);
            _renderer.InstrumentState.Line2 = Encoding.Default.GetBytes(_displayLinesInputSignals[1].State);
            _renderer.InstrumentState.Line3 = Encoding.Default.GetBytes(_displayLinesInputSignals[2].State);
            _renderer.InstrumentState.Line4 = Encoding.Default.GetBytes(_displayLinesInputSignals[3].State);
            _renderer.InstrumentState.Line5 = Encoding.Default.GetBytes(_displayLinesInputSignals[4].State);
            _renderer.InstrumentState.Line1Invert = Encoding.Default.GetBytes(_displayLinesInputSignals[0].State);
            _renderer.InstrumentState.Line2Invert = Encoding.Default.GetBytes(_displayLinesInputSignals[1].State);
            _renderer.InstrumentState.Line3Invert = Encoding.Default.GetBytes(_displayLinesInputSignals[2].State);
            _renderer.InstrumentState.Line4Invert = Encoding.Default.GetBytes(_displayLinesInputSignals[3].State);
            _renderer.InstrumentState.Line5Invert = Encoding.Default.GetBytes(_displayLinesInputSignals[4].State);
            _renderer.InstrumentState.PowerOn = true; //TODO: drive from input signal
            _renderer.Render(g, destinationRectangle);
        }

        private void CreateInputSignals()
        {
            for (var i = 0; i < 5; i++)
            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs",
                    CollectionName = "Text Inputs",
                    FriendlyName = $"DED/PFL Line {i + 1} Text",
                    Id = $"MDI_{Enum.GetName(typeof(DeviceType), _deviceConfig.DeviceType)}_Line_{i+1}_From_Sim",
                    Index = i,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = null,
                    State = string.Empty
                };
                _displayLinesInputSignals.Add(thisSignal);
            }
            for (var i = 0; i < 5; i++)
            {
                var thisSignal = new TextSignal
                {
                    Category = "Inputs",
                    CollectionName = "Text Inputs",
                    FriendlyName = $"DED/PFL Line {i + 1} Inverse Characters",
                    Id = $"MDI_{Enum.GetName(typeof(DeviceType), _deviceConfig.DeviceType)}_Invert_{i + 1}_From_Sim",
                    Index = i,
                    Source = this,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = null,
                    State = string.Empty
                };
                _displayLinesInputSignals.Add(thisSignal);
            }

        }

        public override void Synchronize()
        {
            base.Synchronize();
            UpdateDisplay();
        }
        private void UpdateDisplay()
        {
            var connected = EnsureSerialPortConnected();
            if (!connected) return;
            for (var i = 0; i < 5; i++)
            {
                _serialPort.WriteLine(_displayLinesInputSignals[i].State);

                //TODO: go byte-for-byte and check for inversion etc...
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
                        _serialPort = new SerialPort(_deviceConfig.COMPort, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, e);
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
                        Log.DebugFormat(
                            $"Opening serial port {_deviceConfig.COMPort}: Handshake:{HANDSHAKE}, WriteTimeout:{SERIAL_WRITE_TIMEOUT}, WriteBufferSize:{WRITE_BUFFER_SIZE}");
                        _serialPort.Open();
                        GC.SuppressFinalize(_serialPort.BaseStream);
                        _serialPort.DiscardOutBuffer();
                        _unsuccessfulConnectionAttempts = 0;
                        return true;
                    }
                    catch (Exception e)
                    {
                        _unsuccessfulConnectionAttempts++;
                        Log.Error(e.Message, e);
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
                            Log.DebugFormat($"Closing serial port {_deviceConfig.COMPort}");
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
                        Log.Error(e.Message, e);
                    }
                    _serialPort = null;
                }
                _unsuccessfulConnectionAttempts = 0; //reset unsuccessful connection attempts counter
            }
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_deviceConfig.COMPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                try
                {
                    CloseSerialPortConnection();
                    Common.Util.DisposeObject(_renderer);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                }
            }
            _isDisposed = true;
        }

        ~DataEntryDisplayPilotFaultListHardwareSupportModule()
        {
            Dispose(false);
        }

    }
}