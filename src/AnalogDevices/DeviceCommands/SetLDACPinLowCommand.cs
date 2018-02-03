namespace AnalogDevices.DeviceCommands
{
    internal interface ISetLDACPinLow
    {
        void SetLDACPinLow();
    }

    internal class SetLDACPinLowCommand : ISetLDACPinLow
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetLDACPinLowCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetLDACPinLow()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetLDACPinLow, 0);
            }
        }
    }
}