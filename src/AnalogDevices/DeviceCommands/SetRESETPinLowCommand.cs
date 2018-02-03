namespace AnalogDevices.DeviceCommands
{
    internal interface ISetRESETPinLow
    {
        void SetRESETPinLow();
    }

    internal class SetRESETPinLowCommand : ISetRESETPinLow
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SetRESETPinLowCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetRESETPinLow()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SetRESETPinLow, 0);
            }
        }
    }
}