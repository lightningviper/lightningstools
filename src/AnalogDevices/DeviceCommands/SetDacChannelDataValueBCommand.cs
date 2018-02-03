using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface ISetDacChannelDataValueB
    {
        void SetDacChannelDataValueB(ChannelAddress channelAddress, ushort value);
    }

    internal class SetDacChannelDataValueBCommand : ISetDacChannelDataValueB
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackControlRegister _readbackControlRegisterCommand;
        private readonly ISendSPI _sendSPICommand;
        private readonly IWriteControlRegister _writeControlRegisterCommand;

        public SetDacChannelDataValueBCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackControlRegister readbackControlRegisterCommand = null,
            IWriteControlRegister writeControlRegisterCommand = null,
            ISendSPI sendSPICommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _readbackControlRegisterCommand = readbackControlRegisterCommand ??
                                              new ReadbackControlRegisterCommand(evalBoard);
            _writeControlRegisterCommand = writeControlRegisterCommand ?? new WriteControlRegisterCommand(evalBoard);
            _sendSPICommand = sendSPICommand ?? new SendSPICommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetDacChannelDataValueB(ChannelAddress channelAddress, ushort value)
        {
            if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
            {
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }

            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var val = _evalBoard.DeviceState.Precision == DacPrecision.SixteenBit
                    ? value
                    : (ushort) (value & (ushort) BasicMasks.HighFourteenBits);

                if (!_evalBoard.DeviceState.UseRegisterCache ||
                    val != _evalBoard.DeviceState.X1BRegisters[channelAddress.ToChannelNumber()])
                {
                    var controlRegisterBits = _readbackControlRegisterCommand.ReadbackControlRegister() &
                                              ControlRegisterBits.WritableBits;
                    controlRegisterBits |=
                        ControlRegisterBits
                            .InputRegisterSelect; //set control register bit F2 =1 to select register X1B for input
                    _writeControlRegisterCommand.WriteControlRegister(controlRegisterBits);

                    _sendSPICommand.SendSPI((uint) SerialInterfaceModeBits.WriteToDACInputDataRegisterX |
                                            (uint) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 16) |
                                            val);
                    _evalBoard.DeviceState.X1BRegisters[channelAddress.ToChannelNumber()] = val;
                }
            }
        }
    }
}