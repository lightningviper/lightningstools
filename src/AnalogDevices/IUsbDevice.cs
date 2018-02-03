using System;

namespace AnalogDevices
{
    public interface IUsbDevice : IDisposable
    {
        int Vid { get; }
        int Pid { get; }
        string SymbolicName { get; }

        void ControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer = null,
            int? length = null);

        event EventHandler<ControlTransferSentEventArgs> ControlTransferSent;
    }
}