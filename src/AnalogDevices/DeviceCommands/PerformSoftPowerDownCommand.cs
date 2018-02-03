namespace AnalogDevices.DeviceCommands
{
    internal interface IPerformSoftPowerDown
    {
        void PerformSoftPowerDown();
    }

    internal class PerformSoftPowerDownCommand : IPerformSoftPowerDown
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;
        private readonly IWriteControlRegister _writeControlRegisterCommand;

        public PerformSoftPowerDownCommand(
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

        public void PerformSoftPowerDown()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _evalBoard.DeviceState.ClearRegisterCache();
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister() &
                                          ControlRegisterBits.WritableBits;
                controlRegisterBits |= ControlRegisterBits.SoftPowerDown; //set bit F0=1 to perform soft power-down
                _writeControlRegisterCommand.WriteControlRegister(controlRegisterBits);
            }
        }
    }
}