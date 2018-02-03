namespace AnalogDevices.DeviceCommands
{
    internal interface ISetCLRPinLow
    {
        void SetCLRPinLow();
    }

    internal class SetCLRPinLowCommand : ISetCLRPinLow
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetCLRPinLowCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetCLRPinLow()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetCLRPinLow, 0);
            }
        }
    }
}