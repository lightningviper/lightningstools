using System;

namespace AnalogDevices.DeviceCommands
{
    internal interface ISetDacChannelDataSource
    {
        void SetDacChannelDataSource(ChannelAddress channel, DacChannelDataSource value);

        void SetDacChannelDataSource(ChannelGroup group, DacChannelDataSource channel0,
            DacChannelDataSource channel1, DacChannelDataSource channel2,
            DacChannelDataSource channel3, DacChannelDataSource channel4,
            DacChannelDataSource channel5, DacChannelDataSource channel6,
            DacChannelDataSource channel7);
    }

    internal class SetDacChannelDataSourceCommand : ISetDacChannelDataSource
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISendSpecialFunction _sendSpecialFunctionCommand;
        private readonly ISetDacChannelDataSourceAllChannels _setDacChannelDataSourceAllChannelsCommand;
        private readonly ISetDacChannelDataSourceInternal _setDacChannelDataSourceInternalCommand;

        public SetDacChannelDataSourceCommand(
            IDenseDacEvalBoard evalBoard,
            ISetDacChannelDataSourceAllChannels setDacChannelDataSourceAllChannelsCommand = null,
            ISetDacChannelDataSourceInternal setDacChannelDataSourceInternalCommand = null,
            ISendSpecialFunction sendSpecialFunctionCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _setDacChannelDataSourceAllChannelsCommand = setDacChannelDataSourceAllChannelsCommand ??
                                                         new SetDacChannelDataSourceAllChannelsCommand(evalBoard);
            _setDacChannelDataSourceInternalCommand = setDacChannelDataSourceInternalCommand ??
                                                      new SetDacChannelDataSourceInternalCommand(evalBoard);
            _sendSpecialFunctionCommand = sendSpecialFunctionCommand ?? new SendSpecialFunctionCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SetDacChannelDataSource(ChannelAddress channel, DacChannelDataSource value)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (channel < ChannelAddress.Dac0 || channel > ChannelAddress.Dac39)
                {
                    if (channel == ChannelAddress.AllGroupsAllChannels)
                    {
                        _setDacChannelDataSourceAllChannelsCommand.SetDacChannelDataSourceAllChannels(value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0AllChannels)
                    {
                        SetDacChannelDataSource(ChannelGroup.Group0, value, value, value, value, value, value, value,
                            value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1AllChannels)
                    {
                        SetDacChannelDataSource(ChannelGroup.Group1, value, value, value, value, value, value, value,
                            value);
                        return;
                    }
                    if (channel == ChannelAddress.Group2AllChannels)
                    {
                        SetDacChannelDataSource(ChannelGroup.Group2, value, value, value, value, value, value, value,
                            value);
                        return;
                    }
                    if (channel == ChannelAddress.Group3AllChannels)
                    {
                        SetDacChannelDataSource(ChannelGroup.Group3, value, value, value, value, value, value, value,
                            value);
                        return;
                    }
                    if (channel == ChannelAddress.Group4AllChannels)
                    {
                        SetDacChannelDataSource(ChannelGroup.Group4, value, value, value, value, value, value, value,
                            value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel0)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel0, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel1)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel1, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel2)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel2, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel3)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel3, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel4)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel4, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel5)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel5, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel6)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel6, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group0Through4Channel7)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group0Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group1Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel7, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel0)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel0, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel0, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel1)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel1, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel1, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel2)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel2, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel2, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel3)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel3, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel3, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel4)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel4, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel4, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel5)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel5, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel5, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel6)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel6, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel6, value);
                        return;
                    }
                    if (channel == ChannelAddress.Group1Through4Channel7)
                    {
                        SetDacChannelDataSource(ChannelAddress.Group1Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group2Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group3Channel7, value);
                        SetDacChannelDataSource(ChannelAddress.Group4Channel7, value);
                        return;
                    }
                    throw new ArgumentOutOfRangeException(nameof(channel));
                }
                _setDacChannelDataSourceInternalCommand.SetDacChannelDataSourceInternal(channel, value);
            }
        }

        public void SetDacChannelDataSource(ChannelGroup group, DacChannelDataSource channel0,
            DacChannelDataSource channel1, DacChannelDataSource channel2,
            DacChannelDataSource channel3, DacChannelDataSource channel4,
            DacChannelDataSource channel5, DacChannelDataSource channel6,
            DacChannelDataSource channel7)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var abSelectRegisterBits = ABSelectRegisterBits.AllChannelsA;

                if (channel0 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel0;
                }
                if (channel1 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel1;
                }
                if (channel2 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel2;
                }
                if (channel3 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel3;
                }
                if (channel4 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel4;
                }
                if (channel5 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel5;
                }
                if (channel6 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel6;
                }
                if (channel7 == DacChannelDataSource.DataValueB)
                {
                    abSelectRegisterBits |= ABSelectRegisterBits.Channel7;
                }

                var specialFunctionCode = SpecialFunctionCode.NOP;
                var index = 0;
                switch (group)
                {
                    case ChannelGroup.Group0:
                        specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister0;
                        index = 0;
                        break;
                    case ChannelGroup.Group1:
                        specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister1;
                        index = 1;
                        break;
                    case ChannelGroup.Group2:
                        specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister2;
                        index = 2;
                        break;
                    case ChannelGroup.Group3:
                        specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister3;
                        index = 3;
                        break;
                    case ChannelGroup.Group4:
                        specialFunctionCode = SpecialFunctionCode.WriteToABSelectRegister4;
                        index = 4;
                        break;
                }
                if (_evalBoard.DeviceState.UseRegisterCache ||
                    _evalBoard.DeviceState.ABSelectRegisters[index] != abSelectRegisterBits)
                {
                    _sendSpecialFunctionCommand.SendSpecialFunction(specialFunctionCode,
                        (byte) abSelectRegisterBits);
                    _evalBoard.DeviceState.ABSelectRegisters[index] = abSelectRegisterBits;
                }
            }
        }
    }
}