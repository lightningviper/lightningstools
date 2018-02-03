namespace AnalogDevices.DeviceCommands
{
    internal interface IGetThermalShutdownEnabled
    {
        bool GetThermalShutdownEnabled();
    }

    internal class GetThermalShutdownEnabledCommand : IGetThermalShutdownEnabled
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;

        public GetThermalShutdownEnabledCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public bool GetThermalShutdownEnabled()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister();
                return (controlRegisterBits & ControlRegisterBits.ThermalShutdownEnabled) ==
                       ControlRegisterBits.ThermalShutdownEnabled; //if bit 1=1, thermal shutdown is enabled
            }
        }
    }
}