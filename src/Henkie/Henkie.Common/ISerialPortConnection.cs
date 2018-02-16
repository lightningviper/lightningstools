using System;

namespace Henkie.Common
{
    public interface ISerialPortConnection:IDisposable
    {
        string COMPort { get; set; }
        int BytesAvailable { get; }
        event EventHandler<DeviceDataReceivedEventArgs> DataReceived;
        void Read(byte[] buffer, int index, int count);
        void Write(byte[] buffer, int index, int count);
        void DiscardInputBuffer();
    }
}