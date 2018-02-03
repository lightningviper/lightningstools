namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackControlRegister
    {
        ControlRegisterBits ReadbackControlRegister();
    }

    internal class ReadbackControlRegisterCommand : IReadbackControlRegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackControlRegisterCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            IReadSPI readSPICommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _readSPICommand = readSPICommand ?? new ReadSPICommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ControlRegisterBits ReadbackControlRegister()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (_evalBoard.DeviceState.UseRegisterCache && _evalBoard.DeviceState.ControlRegister.HasValue)
                {
                    return (ControlRegisterBits) ((ushort) _evalBoard.DeviceState.ControlRegister.Value &
                                                  (ushort) ControlRegisterBits.ReadableBits);
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) AddressCodesForDataReadback.ControlRegister);
                var val = (ControlRegisterBits) (_readSPICommand.ReadSPI() & (ushort) ControlRegisterBits.ReadableBits);
                _evalBoard.DeviceState.ControlRegister = val;
                return val;
            }
        }
    }
}