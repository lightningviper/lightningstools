using System;

namespace Henkie.Common
{
    public class PhccCommandDispatcher:ICommandDispatcher
    {
        private bool _isDisposed;
        private readonly Phcc.Device _phccDevice;
        private readonly byte _address;
        public PhccCommandDispatcher(Phcc.Device phccDevice, byte address)
        {
            _phccDevice = phccDevice;
            _address = address;
        }
        public byte[] SendQuery(byte subaddress, byte? data = null, int bytesToRead=0)
        {
            throw new InvalidOperationException();
        }

        public void SendCommand(byte subaddress, byte? data=null)
        {
            _phccDevice?.DoaSendRaw(_address, subaddress, data ?? 0x00);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PhccCommandDispatcher()
        {
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _phccDevice.Dispose();
            }
            _isDisposed = true;
        }

    }
}
