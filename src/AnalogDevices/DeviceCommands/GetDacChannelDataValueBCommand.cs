using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface IGetDacChannelDataValueB
    {
        ushort GetDacChannelDataValueB(ChannelAddress channelAddress);
    }

    internal class GetDacChannelDataValueBCommand : IGetDacChannelDataValueB
    {
        private readonly ILockFactory _lockFactory;
        private readonly IReadbackX1BRegister _readbackX1BRegisterCommand;

        public GetDacChannelDataValueBCommand(
            IDenseDacEvalBoard evalBoard,
            IReadbackX1BRegister readbackX1BRegisterCommand = null,
            ILockFactory lockFactory = null)
        {
            _readbackX1BRegisterCommand = readbackX1BRegisterCommand ?? new ReadbackX1BRegisterCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ushort GetDacChannelDataValueB(ChannelAddress channelAddress)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channelAddress >= ChannelAddress.Dac0 && channelAddress <= ChannelAddress.Dac39)
                {
                        return _readbackX1BRegisterCommand.ReadbackX1BRegister(channelAddress);
                }
                throw new ArgumentOutOfRangeException(nameof(channelAddress));
            }
        }
    }
}