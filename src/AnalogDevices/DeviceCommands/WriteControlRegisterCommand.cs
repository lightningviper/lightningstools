namespace AnalogDevices.DeviceCommands
{
    internal interface IWriteControlRegister
    {
        void WriteControlRegister(ControlRegisterBits controlRegisterBits);
    }

    internal class WriteControlRegisterCommand : IWriteControlRegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public WriteControlRegisterCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void WriteControlRegister(ControlRegisterBits controlRegisterBits)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                controlRegisterBits &= ControlRegisterBits.WritableBits;
                if (
                    !_evalBoard.DeviceState.UseRegisterCache ||
                    controlRegisterBits != (_evalBoard.DeviceState.ControlRegister & ControlRegisterBits.WritableBits)
                )
                {
                    _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.WriteControlRegister,
                        (ushort) controlRegisterBits);
                    _evalBoard.DeviceState.ControlRegister =
                    (_evalBoard.DeviceState.ControlRegister & ControlRegisterBits.ReadableBits &
                     ~ControlRegisterBits.WritableBits) | (controlRegisterBits & ControlRegisterBits.WritableBits);
                }
            }
        }
    }
}