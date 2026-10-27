using System;

namespace Henkie.Common
{
    public interface ISerialPortConnection:IDisposable
    {
        string COMPort { get; set; }
        int BytesAvailable { get; }
        event EventHandler<DeviceDataReceivedEventArgs> DataReceived;
        bool Read(byte[] buffer, int index, int count);
        bool Write(byte[] buffer, int index, int count);
        bool WriteThenRead(byte[] writeBuffer, int writeBufferIndex, int bytesToWrite, byte[] readBuffer, int readBufferIndex, int bytesToRead);
        bool DiscardInputBuffer();
    }
}