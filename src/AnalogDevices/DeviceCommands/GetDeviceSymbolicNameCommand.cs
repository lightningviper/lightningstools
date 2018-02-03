namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDeviceSymbolicName
    {
        string GetDeviceSymbolicName();
    }

    internal class GetDeviceSymbolicNameCommand : IGetDeviceSymbolicName
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;

        public GetDeviceSymbolicNameCommand(
            IDenseDacEvalBoard evalBoard,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public string GetDeviceSymbolicName()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                return _evalBoard.UsbDevice?.SymbolicName;
            }
        }
    }
}