using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDacChannelGain
    {
        ushort GetDacChannelGain(ChannelAddress channel);
    }

    internal class GetDacChannelGainCommand : IGetDacChannelGain
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackMRegister _readbackMRegisterCommand;

        public GetDacChannelGainCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackMRegister readbackMRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackMRegisterCommand = readbackMRegisterCommand ?? new ReadbackMRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ushort GetDacChannelGain(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress >= ChannelAddress.Dac0 && channelAddress <= ChannelAddress.Dac39)
                {
                    var value = _readbackMRegisterCommand.ReadbackMRegister(channelAddress);
                    return value;
                }
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }
        }
    }
}