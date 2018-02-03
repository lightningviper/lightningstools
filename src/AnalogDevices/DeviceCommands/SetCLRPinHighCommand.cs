namespace AnalogDevices.DeviceCommands
{
    internal interface ISetCLRPinHigh
    {
        void SetCLRPinHigh();
    }

    internal class SetCLRPinHighCommand : ISetCLRPinHigh
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetCLRPinHighCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetCLRPinHigh()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetCLRPinHigh, 0);
            }
        }
    }
}