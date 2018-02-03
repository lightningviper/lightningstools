namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackOFS0Register
    {
        ushort ReadbackOFS0Register();
    }

    internal class ReadbackOFS0RegisterCommand : IReadbackOFS0Register
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackOFS0RegisterCommand(
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

        public ushort ReadbackOFS0Register()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (_evalBoard.DeviceState.UseRegisterCache &&
                    _evalBoard.DeviceState.OFS0Register.HasValue)
                {
                    return _evalBoard.DeviceState.OFS0Register.Value;
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) AddressCodesForDataReadback.OFS0Register);
                var spi = _readSPICommand.ReadSPI();
                var val = (ushort) (spi & (ushort) BasicMasks.FourteenBits);
                _evalBoard.DeviceState.OFS0Register = val;
                return val;
            }
        }
    }
}