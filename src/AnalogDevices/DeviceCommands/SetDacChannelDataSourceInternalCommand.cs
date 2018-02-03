using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface ISetDacChannelDataSourceInternal
    {
        void SetDacChannelDataSourceInternal(ChannelAddress channelAddress, DacChannelDataSource dacChannelDataSource);
    }

    internal class SetDacChannelDataSourceInternalCommand : ISetDacChannelDataSourceInternal
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackABSelectRegisters _readbackABSelectRegisterCommand;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public SetDacChannelDataSourceInternalCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackABSelectRegisters readbackABSelectRegisterCommand = null,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null
        )
        {
            _evalBoard = evalBoard;
            _readbackABSelectRegisterCommand = readbackABSelectRegisterCommand ??
                                               new ReadbackABSelectRegisterCommand(evalBoard,
                                                   _sendSpecialFunctionCommand);
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetDacChannelDataSourceInternal(
            ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }
                if ((int) dacChannelDataSource < (int) DacChannelDataSource.DataValueA
                    || (int) dacChannelDataSource > (int) DacChannelDataSource.DataValueB)
                {
                    throw new ArgumentOutOfRangeException(nameof(dacChannelDataSource));
                }

                var channelNum = (byte) ((byte) channelAddress - 8);
                var currentSourceSelections = ABSelectRegisterBits.AllChannelsA;
                var specialFunctionCode = SpecialFunctionCode.NOP;
                if (channelNum < 8)
                {
                    specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister0;
                    currentSourceSelections = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(0);
                }
                else if (channelNum >= 8 && channelNum < 16)
                {
                    specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister1;
                    currentSourceSelections = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(1);
                }
                else if (channelNum >= 16 && channelNum < 24)
                {
                    specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister2;
                    currentSourceSelections = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(2);
                }
                else if (channelNum >= 24 && channelNum < 32)
                {
                    specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister3;
                    currentSourceSelections = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(3);
                }
                else if (channelNum >= 32 && channelNum <= 39)
                {
                    specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister4;
                    currentSourceSelections = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(4);
                }
                var newSourceSelections = currentSourceSelections;

                var channelOffset = (byte) (channelNum % 8);
                var channelMask = (ABSelectRegisterBits) (1 << channelOffset);

                if (dacChannelDataSource == DacChannelDataSource.DataValueA)
                {
                    newSourceSelections &= ~channelMask;
                }
                else
                {
                    newSourceSelections |= channelMask;
                }

                if (!_evalBoard.DeviceState.UseRegisterCache ||
                    newSourceSelections != currentSourceSelections
                )
                {
                    _sendSpecialFunctionCommand.SendSpecialFunction(specialFunctionCode,
                        (byte) newSourceSelections);
                    switch (specialFunctionCode)
                    {
                        case SpecialFunctionCode.WriteToABSelectRegister0:
                            _evalBoard.DeviceState.ABSelectRegisters[0] = newSourceSelections;
                            break;
                        case SpecialFunctionCode.WriteToABSelectRegister1:
                            _evalBoard.DeviceState.ABSelectRegisters[1] = newSourceSelections;
                            break;
                        case SpecialFunctionCode.WriteToABSelectRegister2:
                            _evalBoard.DeviceState.ABSelectRegisters[2] = newSourceSelections;
                            break;
                        case SpecialFunctionCode.WriteToABSelectRegister3:
                            _evalBoard.DeviceState.ABSelectRegisters[3] = newSourceSelections;
                            break;
                        case SpecialFunctionCode.WriteToABSelectRegister4:
                            _evalBoard.DeviceState.ABSelectRegisters[4] = newSourceSelections;
                            break;
                    }
                }
            }
        }
    }
}