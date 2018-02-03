using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDacChannelDataSource
    {
        DacChannelDataSource GetDacChannelDataSource(ChannelAddress channel);
    }

    internal class GetDacChannelDataSourceCommand : IGetDacChannelDataSource
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackABSelectRegisters _readbackABSelectRegisterCommand;

        public GetDacChannelDataSourceCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackABSelectRegisters readbackABSelectRegisterCommand = null,
            ILockFactory lockFactory = null
        )
        {
            _readbackABSelectRegisterCommand = readbackABSelectRegisterCommand ??
                                               new ReadbackABSelectRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public DacChannelDataSource GetDacChannelDataSource(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress < ChannelAddress.Dac0 || channelAddress > ChannelAddress.Dac39)
                {
                    throw new ArgumentOutOfRangeException(nameof(channelAddress));
                }
                var channelNum = (byte) ((byte) channelAddress - 8);
                var currentSourceSelections =
                    _readbackABSelectRegisterCommand.ReadbackABSelectRegister(channelNum / 8);
                var channelOffset = (byte) (channelNum % 8);
                var channelMask = (ABSelectRegisterBits) (1 << channelOffset);
                var sourceIsDataValueB = (currentSourceSelections & channelMask) == channelMask;
                return sourceIsDataValueB ? DacChannelDataSource.DataValueB : DacChannelDataSource.DataValueA;
            }
        }
    }
}