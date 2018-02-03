using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackX1ARegister
    {
        ushort ReadbackX1ARegister(ChannelAddress channelAddress);
    }

    internal class ReadbackX1ARegisterCommand : IReadbackX1ARegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackX1ARegisterCommand(
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

        public ushort ReadbackX1ARegister(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }

                if (_evalBoard.DeviceState.UseRegisterCache &&
                    _evalBoard.DeviceState.X1ARegisters[channelAddress.ToChannelNumber()].HasValue)
                {
                    var deviceStateX1ARegister = _evalBoard.DeviceState.X1ARegisters[channelAddress.ToChannelNumber()];
                    if (deviceStateX1ARegister != null)
                    {
                        return deviceStateX1ARegister.Value;
                    }
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 7));
                var val = _readSPICommand.ReadSPI();
                if (_evalBoard.DeviceState.Precision == DacPrecision.FourteenBit)
                {
                    val &= (ushort) BasicMasks.HighFourteenBits;
                }
                _evalBoard.DeviceState.X1ARegisters[channelAddress.ToChannelNumber()] = val;
                return val;
            }
        }
    }
}