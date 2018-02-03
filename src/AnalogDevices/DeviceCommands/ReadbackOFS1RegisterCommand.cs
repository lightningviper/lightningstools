namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackOFS1Register
    {
        ushort ReadbackOFS1Register();
    }

    internal class ReadbackOFS1RegisterCommand : IReadbackOFS1Register
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackOFS1RegisterCommand(
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

        public ushort ReadbackOFS1Register()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (_evalBoard.DeviceState.UseRegisterCache && _evalBoard.DeviceState.OFS1Register.HasValue)
                {
                    return _evalBoard.DeviceState.OFS1Register.Value;
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) AddressCodesForDataReadback.OFS1Register);
                var spi = _readSPICommand.ReadSPI();
                var val = (ushort) (spi & (ushort) BasicMasks.FourteenBits);
                _evalBoard.DeviceState.OFS1Register = val;
                return val;
            }
        }
    }
}