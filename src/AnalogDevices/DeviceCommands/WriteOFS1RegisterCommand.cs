namespace AnalogDevices.DeviceCommands
{
    internal interface IWriteOFS1Register
    {
        void WriteOFS1Register(ushort value);
    }

    internal class WriteOFS1RegisterCommand : IWriteOFS1Register
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public WriteOFS1RegisterCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void WriteOFS1Register(ushort value)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                value &= (ushort) BasicMasks.FourteenBits;

                if (!_evalBoard.DeviceState.UseRegisterCache ||
                    value != _evalBoard.DeviceState.OFS1Register)
                {
                    _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.WriteOFS1Register, value);
                    _evalBoard.DeviceState.OFS1Register = value;
                }
            }
        }
    }
}