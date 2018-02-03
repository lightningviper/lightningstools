namespace AnalogDevices.DeviceCommands
{
    internal interface IInitializeSPIPins
    {
        void InitializeSPIPins();
    }

    internal class InitializeSPIPinsCommand : IInitializeSPIPins
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendDeviceCommand _sendDeviceCommandCommand;

        public InitializeSPIPinsCommand(
            IDenseDacEvalBoard evalBoard,
            ISendDeviceCommand sendDeviceCommandCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendDeviceCommandCommand = sendDeviceCommandCommand ?? new SendDeviceCommandCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void InitializeSPIPins()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _sendDeviceCommandCommand.SendDeviceCommand(DeviceCommand.InitializeSPIPins, 0);
                _evalBoard.DeviceState.SPI_Initialized = true;
            }
        }
    }
}