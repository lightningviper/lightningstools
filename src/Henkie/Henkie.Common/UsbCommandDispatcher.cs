using System;

namespace Henkie.Common
{
    public class UsbCommandDispatcher : ICommandDispatcher
    {
        private bool _isDisposed = false;
        private ISerialPortConnection SerialPortConnection { get; set; }

        public UsbCommandDispatcher(ISerialPortConnection serialPortConnection)
        {
            SerialPortConnection = serialPortConnection;
        }
        public UsbCommandDispatcher(string COMPort)
        {
            SerialPortConnection = new SerialPortConnection(COMPort);
        }
        public void SendCommand(byte subaddress, byte? data=null)
        {
            if (SerialPortConnection != null)
            {
                SerialPortConnection.Write(new[] { subaddress }, 0, 1);
                if (data != null)
                {
                    SerialPortConnection.Write(new[] { data.Value }, 0, 1);
                }
            }
        }
        public byte[] SendQuery(byte subaddress, byte? data = null, int bytesToRead = 0)
        {
            if (bytesToRead <0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesToRead));
            }
            if (SerialPortConnection != null)
            {
                if (bytesToRead > 0)
                {
                    SerialPortConnection.DiscardInputBuffer();
                }
                SendCommand(subaddress, data);
                if (bytesToRead > 0)
                {
                    var readBuffer = new byte[bytesToRead];
                    SerialPortConnection.Read(readBuffer, 0, bytesToRead);
                    return readBuffer;
                }
            }
            return null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UsbCommandDispatcher()
        {
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (SerialPortConnection != null)
                {
                    SerialPortConnection.Dispose();
                }
            }
            _isDisposed = true;
        }
    }
}
