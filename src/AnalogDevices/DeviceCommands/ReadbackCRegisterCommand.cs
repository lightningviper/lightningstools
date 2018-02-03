using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IReadbackCRegister
    {
        ushort ReadbackCRegister(ChannelAddress channelAddress);
    }

    internal class ReadbackCRegisterCommand : IReadbackCRegister
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadSPI _readSPICommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public ReadbackCRegisterCommand(
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

        public ushort ReadbackCRegister(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }

                if (_evalBoard.DeviceState.UseRegisterCache &&
                    _evalBoard.DeviceState.CRegisters[channelAddress.ToChannelNumber()].HasValue)
                {
                    var deviceStateCRegister = _evalBoard.DeviceState.CRegisters[channelAddress.ToChannelNumber()];
                    if (deviceStateCRegister != null)
                    {
                        return deviceStateCRegister.Value;
                    }
                }
                _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.SelectRegisterForReadback,
                    (ushort) ((ushort) AddressCodesForDataReadback.CRegister |
                              (ushort) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 7)));
                var val = _readSPICommand.ReadSPI();
                if (_evalBoard.DeviceState.Precision == DacPrecision.FourteenBit)
                {
                    val &= (ushort) BasicMasks.HighFourteenBits;
                }
                _evalBoard.DeviceState.CRegisters[channelAddress.ToChannelNumber()] = val;
                return val;
            }
        }
    }
}