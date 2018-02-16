using System;

namespace Henkie.Common
{
    public interface ICommandDispatcher:IDisposable
    {
        void SendCommand(byte subaddress, byte? data = null);
        byte[] SendQuery(byte subaddress, byte? data = null, int bytesToRead = 0);
    }
}
