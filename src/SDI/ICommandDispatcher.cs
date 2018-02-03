using System;

namespace SDI
{
    internal interface ICommandDispatcher:IDisposable
    {
        string SendCommand(CommandSubaddress subaddress, byte data);
    }
}
