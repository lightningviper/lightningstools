namespace AnalogDevices.DeviceCommands
{
    internal interface IWriteOFS0Register
    {
        void WriteOFS0Register(ushort value);
    }

    internal class WriteOFS0RegisterCommand : IWriteOFS0Register
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public WriteOFS0RegisterCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void WriteOFS0Register(ushort value)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                value &= (ushort) BasicMasks.FourteenBits;
                if (!_evalBoard.DeviceState.UseRegisterCache ||
                    value != _evalBoard.DeviceState.OFS0Register)
                {
                    _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.WriteOFS0Register, value);
                    _evalBoard.DeviceState.OFS0Register = value;
                }
            }
        }
    }
}