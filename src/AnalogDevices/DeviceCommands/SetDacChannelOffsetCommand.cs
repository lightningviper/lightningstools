using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface ISetDacChannelOffset
    {
        void SetDacChannelOffset(ChannelAddress channelAddress, ushort value);
    }

    internal class SetDacChannelOffsetCommand : ISetDacChannelOffset
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSPI _sendSPICommand;
        private readonly ISetCLRPinHigh _setCLRPinHighCommand;
        private readonly ISetCLRPinLow _setCLRPinLowCommand;

        public SetDacChannelOffsetCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSPI sendSPICommand = null,
            ISetCLRPinLow setCLRPinLowCommand = null,
            ISetCLRPinHigh setCLRPinHighCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSPICommand = sendSPICommand ?? new SendSPICommand(evalBoard);
            _setCLRPinLowCommand = setCLRPinLowCommand ?? new SetCLRPinLowCommand(evalBoard);
            _setCLRPinHighCommand = setCLRPinHighCommand ?? new SetCLRPinHighCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetDacChannelOffset(ChannelAddress channelAddress, ushort value)
        {
            if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
            {
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }

            var val = _evalBoard.DeviceState.Precision == DacPrecision.SixteenBit
                ? value
                : (ushort) (value & (ushort) BasicMasks.HighFourteenBits);

            if (!_evalBoard.DeviceState.UseRegisterCache
                ||
                val != _evalBoard.DeviceState.CRegisters[channelAddress.ToChannelNumber()]
            )
            {
                using (_lockFactory.GetLock(LockType.CommandLock))
                {
                    _setCLRPinLowCommand.SetCLRPinLow();
                    _sendSPICommand.SendSPI((uint) SerialInterfaceModeBits.WriteToDACOffsetRegisterC |
                                            (uint) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 16) |
                                            val);
                    _evalBoard.DeviceState.CRegisters[channelAddress.ToChannelNumber()] = val;
                    _setCLRPinHighCommand.SetCLRPinHigh();
                }
            }
        }
    }
}