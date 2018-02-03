using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackX1BRegister
    {
        ushort ReadbackX1BRegister(ChannelAddress channelAddress);
    }

    internal class ReadbackX1BRegisterCommand : IReadbackX1BRegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackX1BRegisterCommand(
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

        public ushort ReadbackX1BRegister(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }
                if (_evalBoard.DeviceState.UseRegisterCache &&
                    _evalBoard.DeviceState.X1BRegisters[channelAddress.ToChannelNumber()].HasValue)
                {
                    var deviceStateX1BRegister = _evalBoard.DeviceState.X1BRegisters[channelAddress.ToChannelNumber()];
                    if (deviceStateX1BRegister != null)
                    {
                        return deviceStateX1BRegister.Value;
                    }
                }
                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) ((ushort) AddressCodesForDataReadback.X1BRegister |
                              (ushort) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 7)));
                var val = _readSPICommand.ReadSPI();
                if (_evalBoard.DeviceState.Precision == DacPrecision.FourteenBit)
                {
                    val &= (ushort) BasicMasks.HighFourteenBits;
                }
                _evalBoard.DeviceState.X1BRegisters[channelAddress.ToChannelNumber()] = val;
                return val;
            }
        }
    }
}