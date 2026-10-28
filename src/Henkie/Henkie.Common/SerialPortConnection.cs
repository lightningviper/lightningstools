using System;
using System.IO.Ports;
using System.Threading;
using log4net;

namespace Henkie.Common
{
    public class SerialPortConnection:ISerialPortConnection
    {
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;
        private string _COMPort;
        private SerialPort _serialPort;
        private readonly ILog _log = LogManager.GetLogger(typeof(SerialPortConnection));
        private SerialPortConnection(){}
        public SerialPortConnection(string COMPort, bool openPort = false) :this()
        {
            _COMPort = COMPort;
            if (openPort)
            {
                lock (_serialPortLock)
                {
                    EnsurePortIsReady();
                }
            }
        }

        public event EventHandler<DeviceDataReceivedEventArgs> DataReceived;

        public string COMPort
        {
            get => _COMPort;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        "must contain a string that identifies a valid serial port on the machine (i.e. COM1, COM2, etc.)",
                        nameof(value));
                }
                ClosePort();
                _COMPort = value;
            }
        }

        public bool DiscardInputBuffer()
        {
            lock (_serialPortLock)
            {
                try
                {
                    EnsurePortIsReady();
                    _serialPort?.DiscardInBuffer();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("Serial port " + _serialPort?.PortName + " exception in DiscardInputBuffer(), disconnecting.", ex);
                    ClosePort();
                }
                return false;
            }
        }
        public int WriteThenRead(byte[] writeBuffer, int writeBufferIndex, int bytesToWrite, byte[] readBuffer, int readBufferIndex, int bytesToRead)
        {
            lock (_serialPortLock)
            {
                var success = Write(writeBuffer, writeBufferIndex, bytesToWrite);
                if (!success) return -1;
                return Read(readBuffer, readBufferIndex, bytesToRead);
            }
        }
        public bool Write(byte[] buffer, int index, int count)
        {
            lock (_serialPortLock)
            {
                try
                {
                    EnsurePortIsReady();
                    _serialPort.BaseStream.Write(buffer, index, count);
                    _serialPort.BaseStream.Flush();
                    return true;
                }
                catch (Exception ex) 
                {
                    _log.Error("Serial port " + _serialPort?.PortName + " exception in Write(), disconnecting.", ex);
                    ClosePort();
                }
            }
            return false;
        }
        public int BytesAvailable
        {
            get
            {
                lock (_serialPortLock)
                {
                    EnsurePortIsReady();
                    if (_serialPort == null) return 0;
                    try
                    {
                        return _serialPort.BytesToRead;
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Serial port " + _serialPort?.PortName + " exception in BytesAvailable(), disconnecting.", ex);
                        ClosePort();
                    }
                    return 0;
                }
            }
        }

        public int Read(byte[] buffer, int index, int count)
        {
            lock (_serialPortLock) 
            {
                EnsurePortIsReady();

                var timeout = _serialPort.ReadTimeout;
                var startTime = DateTime.UtcNow;
                var bytesAvailable = 0;

                while (bytesAvailable < count)
                {
                    bytesAvailable = BytesAvailable;
                    if (DateTime.UtcNow > startTime.AddMilliseconds(timeout) && timeout != Timeout.Infinite)
                    {
                        return -1;
                    }
                    Thread.Sleep(0);
                }

                if (BytesAvailable >= count)
                {
                    try
                    {
                        return _serialPort.BaseStream.Read(buffer, index, count);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Serial port " + _serialPort?.PortName + " exception in Read(), disconnecting.", ex);
                        ClosePort();
                    }
                }
                return -1;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void InitializeSerialPort()
        {
            lock (_serialPortLock)
            {
                ClosePort();
                _serialPort = new SerialPort
                {
                    PortName = _COMPort,
                    BaudRate = 115200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReceivedBytesThreshold = 1,
                    RtsEnable = true,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    ReadBufferSize = 64 * 1024,
                    WriteBufferSize = 64 * 1024
                };
                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.ErrorReceived += SerialPort_ErrorReceived;
                _serialPort.Open();
                GC.SuppressFinalize(_serialPort.BaseStream);
            }
        }

        private void EnsurePortIsReady()
        {
            lock (_serialPortLock)
            {
                if (_serialPort == null || !_serialPort.IsOpen)
                {
                    InitializeSerialPort();
                }
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                OnSerialPortDataReceived(sender, e);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
        private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, new DeviceDataReceivedEventArgs() { EventType = e.EventType == SerialData.Eof ? SerialPortDataReceivedEventType.Eof : SerialPortDataReceivedEventType.Chars });
        }
        private void ClosePort()
        {
            lock (_serialPortLock)
            {
                if (_serialPort == null) return;
                try
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.ErrorReceived -= SerialPort_ErrorReceived;
                }
                catch (Exception ex)
                {
                    _log.Error("Serial port " + _serialPort.PortName + " exception in ClosePort().", ex);
                }
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        try
                        {
                            GC.ReRegisterForFinalize(_serialPort.BaseStream);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Serial port " + _serialPort.PortName + " exception in ClosePort().", ex);
                        }
                        finally
                        {
                            try
                            {
                                _serialPort.Close();
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Serial port " + _serialPort.PortName + " exception in ClosePort().", ex);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Serial port " + _serialPort.PortName + " exception in ClosePort().", ex);
                }
                finally
                {
                    try
                    {
                        _serialPort.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Serial port " + _serialPort.PortName + " exception in ClosePort().", ex);
                    }
                }
                _serialPort = null;
                Thread.Sleep(500);
            }
        }
        private static void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e) { }

        ~SerialPortConnection()
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                _log.Error("Serial port " + _serialPort?.PortName + " exception in SerialPortConnection finalizer.", ex);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                try
                {
                    ClosePort();
                }
                catch (Exception ex)
                {
                    _log.Error("Serial port " + _serialPort?.PortName + " exception in Dispose().", ex);
                }
            }
            _isDisposed = true;
        }
    }
}