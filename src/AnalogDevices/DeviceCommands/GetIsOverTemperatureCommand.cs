namespace AnalogDevices.DeviceCommands
{
    internal interface IGetIsOverTemperature
    {
        bool GetIsOverTemperature();
    }

    internal class GetIsOverTemperatureCommand : IGetIsOverTemperature
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;

        public GetIsOverTemperatureCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public bool GetIsOverTemperature()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister();
                return (controlRegisterBits & ControlRegisterBits.OverTemperature) ==
                       ControlRegisterBits.OverTemperature;
            }
        }
    }
}