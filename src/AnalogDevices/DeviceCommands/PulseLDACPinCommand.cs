namespace AnalogDevices.DeviceCommands
{
    internal interface IPulseLDACPin
    {
        void PulseLDACPin();
    }

    internal class PulseLDACPinCommand : IPulseLDACPin
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public PulseLDACPinCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void PulseLDACPin()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.PulseLDACPin, 0);
            }
        }
    }
}