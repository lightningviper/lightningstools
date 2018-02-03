namespace AnalogDevices.DeviceCommands
{
    internal interface ISetLDACPinHigh
    {
        void SetLDACPinHigh();
    }

    internal class SetLDACPinHighCommand : ISetLDACPinHigh
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetLDACPinHighCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetLDACPinHigh()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetLDACPinHigh, 0);
            }
        }
    }
}