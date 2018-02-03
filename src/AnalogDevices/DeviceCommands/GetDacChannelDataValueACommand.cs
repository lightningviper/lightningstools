using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDacChannelDataValueA
    {
        ushort GetDacChannelDataValueA(ChannelAddress channelAddress);
    }

    internal class GetDacChannelDataValueACommand : IGetDacChannelDataValueA
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackX1ARegister _readbackX1ARegisterCommand;

        public GetDacChannelDataValueACommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackX1ARegister readbackX1ARegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackX1ARegisterCommand = readbackX1ARegisterCommand ?? new ReadbackX1ARegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ushort GetDacChannelDataValueA(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress >= ChannelAddress.Dac0 && channelAddress <= ChannelAddress.Dac39)
                {
                    return _readbackX1ARegisterCommand.ReadbackX1ARegister(channelAddress);
                }
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }
        }
    }
}