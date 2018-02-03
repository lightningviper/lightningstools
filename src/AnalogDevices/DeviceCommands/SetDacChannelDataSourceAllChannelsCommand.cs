using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface ISetDacChannelDataSourceAllChannels
    {
        void SetDacChannelDataSourceAllChannels(DacChannelDataSource source);
    }

    internal class SetDacChannelDataSourceAllChannelsCommand : ISetDacChannelDataSourceAllChannels
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;

        public SetDacChannelDataSourceAllChannelsCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetDacChannelDataSourceAllChannels(DacChannelDataSource source)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                switch (source)
                {
                    case DacChannelDataSource.DataValueA:
                        _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.BlockWriteABSelectRegisters,
                            (byte) ABSelectRegisterBits.AllChannelsA);
                        _evalBoard.DeviceState.ABSelectRegisters[0] = ABSelectRegisterBits.AllChannelsA;
                        _evalBoard.DeviceState.ABSelectRegisters[1] = ABSelectRegisterBits.AllChannelsA;
                        _evalBoard.DeviceState.ABSelectRegisters[2] = ABSelectRegisterBits.AllChannelsA;
                        _evalBoard.DeviceState.ABSelectRegisters[3] = ABSelectRegisterBits.AllChannelsA;
                        _evalBoard.DeviceState.ABSelectRegisters[4] = ABSelectRegisterBits.AllChannelsA;
                        break;
                    case DacChannelDataSource.DataValueB:
                        _sendSpecialFunctionCommand.SendSpecialFunction(SpecialFunctionCode.BlockWriteABSelectRegisters,
                            (byte) ABSelectRegisterBits.AllChannelsB);
                        _evalBoard.DeviceState.ABSelectRegisters[0] = ABSelectRegisterBits.AllChannelsB;
                        _evalBoard.DeviceState.ABSelectRegisters[1] = ABSelectRegisterBits.AllChannelsB;
                        _evalBoard.DeviceState.ABSelectRegisters[2] = ABSelectRegisterBits.AllChannelsB;
                        _evalBoard.DeviceState.ABSelectRegisters[3] = ABSelectRegisterBits.AllChannelsB;
                        _evalBoard.DeviceState.ABSelectRegisters[4] = ABSelectRegisterBits.AllChannelsB;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source));
                }
            }
        }
    }
}