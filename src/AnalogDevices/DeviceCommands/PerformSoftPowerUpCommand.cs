namespace AnalogDevices.DeviceCommands
{
    internal interface IPerformSoftPowerUp
    {
        void PerformSoftPowerUp();
    }

    internal class PerformSoftPowerUpCommand : IPerformSoftPowerUp
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;
        private readonly IWriteControlRegister _writeControlRegisterCommand;

        public PerformSoftPowerUpCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            IWriteControlRegister writeControlRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _writeControlRegisterCommand = writeControlRegisterCommand ?? new WriteControlRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void PerformSoftPowerUp()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _evalBoard.DeviceState.ClearRegisterCache();
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister() &
                                          ControlRegisterBits.WritableBits;
                controlRegisterBits &= ~ControlRegisterBits.SoftPowerDown; //set bit F0=0 to perform soft power-up;
                _writeControlRegisterCommand.WriteControlRegister(controlRegisterBits);
                _evalBoard.DeviceState.ClearRegisterCache();
            }
        }
    }
}