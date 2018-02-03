using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackABSelectRegisters
    {
        ABSelectRegisterBits ReadbackABSelectRegister(int registerNumber);
    }

    internal class ReadbackABSelectRegisterCommand : IReadbackABSelectRegisters
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        private readonly AddressCodesForDataReadback[] AddressCodesForABSelectRegisters =
        {
            AddressCodesForDataReadback.ABSelect0Register,
            AddressCodesForDataReadback.ABSelect1Register,
            AddressCodesForDataReadback.ABSelect2Register,
            AddressCodesForDataReadback.ABSelect3Register,
            AddressCodesForDataReadback.ABSelect4Register
        };

        public ReadbackABSelectRegisterCommand(
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

        public ABSelectRegisterBits ReadbackABSelectRegister(int registerNumber)
        {
            if (registerNumber < 0 || registerNumber > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(registerNumber));
            }
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var deviceState = _evalBoard.DeviceState;
                if (deviceState.UseRegisterCache && deviceState.ABSelectRegisters[registerNumber].HasValue)
                {
                    return deviceState.ABSelectRegisters[registerNumber].Value;
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(
                    SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) AddressCodesForABSelectRegisters[registerNumber]);
                var spi = _readSPICommand.ReadSPI();
                var val = (ABSelectRegisterBits) spi;
                _evalBoard.DeviceState.ABSelectRegisters[registerNumber] = val;
                return val;
            }
        }
    }
}