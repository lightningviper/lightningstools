namespace AnalogDevices.DeviceCommands
{
    internal interface ISetOffsetDAC0
    {
        void SetOffsetDAC0(ushort value);
    }

    internal class SetOffsetDAC0Command : ISetOffsetDAC0
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISetCLRPinHigh _setCLRPinHighCommand;
        private readonly ISetCLRPinLow _setCLRPinLowCommand;
        private readonly IWriteOFS0Register _writeOFS0RegisterCommand;

        public SetOffsetDAC0Command(
            IDenseDacEvalBoard evalBoard,
            ISetCLRPinLow setCLRPinLowCommand = null,
            ISetCLRPinHigh setCLRPinHighCommand = null,
            IWriteOFS0Register writeOFS0RegisterCommand = null,
            ILockFactory lockFactory = null
        )
        {
            _evalBoard = evalBoard;
            _setCLRPinLowCommand = setCLRPinLowCommand ?? new SetCLRPinLowCommand(evalBoard);
            _setCLRPinHighCommand = setCLRPinHighCommand ?? new SetCLRPinHighCommand(evalBoard);
            _writeOFS0RegisterCommand = writeOFS0RegisterCommand ?? new WriteOFS0RegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetOffsetDAC0(ushort value)
        {
            value &= (ushort) BasicMasks.FourteenBits;

            if (!_evalBoard.DeviceState.UseRegisterCache ||
                value != _evalBoard.DeviceState.OFS0Register)
            {
                using (_lockFactory.GetLock(LockType.CommandLock))
                {
                    _setCLRPinLowCommand.SetCLRPinLow();
                    _writeOFS0RegisterCommand.WriteOFS0Register(value);
                    _setCLRPinHighCommand.SetCLRPinHigh();
                }
            }
        }
    }
}