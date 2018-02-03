using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackMRegister
    {
        ushort ReadbackMRegister(ChannelAddress channelAddress);
    }

    internal class ReadbackMRegisterCommand : IReadbackMRegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackMRegisterCommand(
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

        public ushort ReadbackMRegister(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }

                if (_evalBoard.DeviceState.UseRegisterCache &&
                    _evalBoard.DeviceState.MRegisters[channelAddress.ToChannelNumber()].HasValue)
                {
                    var deviceStateMRegister = _evalBoard.DeviceState.MRegisters[channelAddress.ToChannelNumber()];
                    if (deviceStateMRegister != null)
                    {
                        return deviceStateMRegister.Value;
                    }
                }

                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) ((ushort) AddressCodesForDataReadback.MRegister |
                              (ushort) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 7)));
                var val = _readSPICommand.ReadSPI();
                if (_evalBoard.DeviceState.Precision == DacPrecision.FourteenBit)
                {
                    val &= (ushort) BasicMasks.HighFourteenBits;
                }
                _evalBoard.DeviceState.MRegisters[channelAddress.ToChannelNumber()] = val;
                return val;
            }
        }
    }
}