namespace AnalogDevices.DeviceCommands
{
    internal interface ISetOffsetDAC1
    {
        void SetOffsetDAC1(ushort value);
    }

    internal class SetOffsetDAC1Command : ISetOffsetDAC1
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISetCLRPinHigh _setCLRPinHighCommand;
        private readonly ISetCLRPinLow _setCLRPinLowCommand;
        private readonly IWriteOFS1Register _writeOFS1RegisterCommand;

        public SetOffsetDAC1Command(
            IDenseDacEvalBoard evalBoard,
            ISetCLRPinLow setCLRPinLowCommand = null,
            ISetCLRPinHigh setCLRPinHighCommand = null,
            IWriteOFS1Register writeOFS1RegisterCommand = null,
            ILockFactory lockFactory = null
        )
        {
            _evalBoard = evalBoard;
            _setCLRPinLowCommand = setCLRPinLowCommand ?? new SetCLRPinLowCommand(evalBoard);
            _setCLRPinHighCommand = setCLRPinHighCommand ?? new SetCLRPinHighCommand(evalBoard);
            _writeOFS1RegisterCommand = writeOFS1RegisterCommand ?? new WriteOFS1RegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetOffsetDAC1(ushort value)
        {
            value &= (ushort) BasicMasks.FourteenBits;

            if (!_evalBoard.DeviceState.UseRegisterCache ||
                value != _evalBoard.DeviceState.OFS1Register)
            {
                using (_lockFactory.GetLock(LockType.CommandLock))
                {
                    _setCLRPinLowCommand.SetCLRPinLow();
                    _writeOFS1RegisterCommand.WriteOFS1Register(value);
                    _setCLRPinHighCommand.SetCLRPinHigh();
                }
            }
        }
    }
}