namespace AnalogDevices.DeviceCommands
{
    internal interface ISetThermalShutdownEnabled
    {
        void SetThermalShutdownEnabled(bool value);
    }

    internal class SetThermalShutdownEnabledCommand : ISetThermalShutdownEnabled
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;
        private readonly IWriteControlRegister _writeControlRegisterCommand;

        public SetThermalShutdownEnabledCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            IWriteControlRegister writeControlRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _writeControlRegisterCommand = writeControlRegisterCommand ?? new WriteControlRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetThermalShutdownEnabled(bool value)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister() &
                                          ControlRegisterBits.WritableBits;
                if (value)
                {
                    controlRegisterBits |= ControlRegisterBits.ThermalShutdownEnabled;
                }
                else
                {
                    controlRegisterBits &= ~ControlRegisterBits.ThermalShutdownEnabled;
                }
                _writeControlRegisterCommand.WriteControlRegister(controlRegisterBits);
            }
        }
    }
}