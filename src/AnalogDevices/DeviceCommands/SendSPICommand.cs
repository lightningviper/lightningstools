namespace AnalogDevices.DeviceCommands
{
    internal interface ISendSPI
    {
        void SendSPI(uint data);
    }

    internal class SendSPICommand : ISendSPI
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public SendSPICommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _sendDeviceCommandCommand = sendDeviceCommandCommand
                                        ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SendSPI(uint data)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.SendSPI, data);
            }
        }
    }
}