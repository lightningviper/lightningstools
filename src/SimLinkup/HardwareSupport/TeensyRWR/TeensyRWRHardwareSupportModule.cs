using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media;
using Common.HardwareSupport;
using Common.IO.Ports;
using Common.MacroProgramming;
using log4net;
using SerialPort = Common.IO.Ports.SerialPort;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    public class TeensyRWRRHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const int BAUD_RATE = 9600;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 2048;
        private const int SERIAL_WRITE_TIMEOUT = 500;

        private BMSRWRRenderer _renderer = new BMSRWRRenderer();

        //limits exceptions when we don't have the RWR plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(TeensyRWRRHardwareSupportModule));

        private readonly string _comPort;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts = 3;

        private TeensyRWRRHardwareSupportModule(string comPort)
        {
            _comPort = comPort;
        }

        public override AnalogSignal[] AnalogInputs => null;
        public override AnalogSignal[] AnalogOutputs => null;
        public override DigitalSignal[] DigitalInputs => null;
        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => $"Teensy RWR module for IP-1310/ALR Azimuth Indicator (RWR): {_comPort}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TeensyRWRRHardwareSupportModule()
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
                IHardwareSupportModule thisHsm = new TeensyRWRRHardwareSupportModule(hsmConfig.COMPort);
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
                    }
                    catch (Exception e)
                    {
                        _unsuccessfulConnectionAttempts++;
                        _log.Error(e.Message, e);
                        return false;
                    }
                }
                return true;
            }
        }

        private string GenerateDrawingCommands()
        {
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Append();
            drawingContext.PushTransform(new ScaleTransform(1, -1));
            _renderer.Render(drawingContext);
            drawingContext.Close();
            Rect bounds = new Rect(0, -500, 500, 500);
            const uint dacPrecisionBits = 12;
            using (var stream = new MemoryStream())
            using (var streamReader = new StreamReader(stream))
            {
                VectorEncoder.Serialize(drawingGroup, bounds, stream, dacPrecisionBits,
                    new PreprocessorOptions
                    {
                        EqualizeBrightness = true
                    }
                );
                stream.Seek(0, SeekOrigin.Begin);
                return streamReader.ReadToEnd();
            }
        }
        private void SendDrawingCommands(string drawingCommands)
        {
            lock (_serialPortLock)
            {
                try
                {
                    _serialPort.Write(drawingCommands);
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
            var connected = EnsureSerialPortConnected();
            if (connected)
            {
                var commandList = GenerateDrawingCommands();
                SendDrawingCommands(commandList);
            }
        }
    }
}