using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDacChannelOffset
    {
        ushort GetDacChannelOffset(ChannelAddress channelAddress);
    }

    internal class GetDacChannelOffsetCommand : IGetDacChannelOffset
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackCRegister _readbackCRegisterCommand;

        public GetDacChannelOffsetCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackCRegister readbackCRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackCRegisterCommand = readbackCRegisterCommand ?? new ReadbackCRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ushort GetDacChannelOffset(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress >= ChannelAddress.Dac0 && channelAddress <= ChannelAddress.Dac39)
                {
                    return _readbackCRegisterCommand.ReadbackCRegister(channelAddress);
                }
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }
        }
    }
}