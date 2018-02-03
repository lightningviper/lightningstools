namespace AnalogDevices.DeviceCommands
{
    internal interface IGetPacketErrorCheckErrorOccurredStatus
    {
        bool GetPacketErrorCheckErrorOccurredStatus();
    }

    internal class GetPacketErrorCheckErrorOccurredStatusCommand : IGetPacketErrorCheckErrorOccurredStatus
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;

        public GetPacketErrorCheckErrorOccurredStatusCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public bool GetPacketErrorCheckErrorOccurredStatus()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister();
                return (controlRegisterBits & ControlRegisterBits.PacketErrorCheckErrorOccurred) ==
                       ControlRegisterBits.PacketErrorCheckErrorOccurred;
            }
        }
    }
}