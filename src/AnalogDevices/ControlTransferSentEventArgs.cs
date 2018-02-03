using System;

namespace AnalogDevices
{
    public class ControlTransferSentEventArgs : EventArgs
    {
        public ControlTransferSentEventArgs(
            byte requestType,
            byte request,
            int value,
            int index,
            byte[] buffer,
            int? length)
        {
            RequestType = requestType;
            Request = request;
            Value = value;
            Index = index;
            Buffer = buffer;
            Length = length;
        }

        public byte RequestType { get; }
        public byte Request { get; }
        public int Value { get; }
        public int Index { get; }
        public byte[] Buffer { get; }
        public int? Length { get; }
    }
}