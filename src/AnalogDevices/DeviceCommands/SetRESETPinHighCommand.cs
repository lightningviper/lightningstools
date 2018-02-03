namespace AnalogDevices.DeviceCommands
{
    internal interface ISetRESETPinHigh
    {
        void SetRESETPinHigh();
    }

    internal class SetRESETPinHighCommand : ISetRESETPinHigh
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetRESETPinHighCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetRESETPinHigh()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetRESETPinHigh, 0);
            }
        }
    }
}