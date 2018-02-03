using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetDacChannelDataSourceCommandTests
    {
        [Test]
        public void ShouldThrowExceptionIfArgumentsAreOutOfRange(
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1))]
            ChannelAddress channelAddress,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource dacChannelDataSource
        )
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WhenSetDacChannelDataSource(channelAddress, dacChannelDataSource));
        }

        [Test]
        public void ShouldSetDacChannelDataSourceAllGroupsAllChannels(
            [Values] DacChannelDataSource dacChannelDataSource
        )
        {
            WhenSetDacChannelDataSource(ChannelAddress.AllGroupsAllChannels, dacChannelDataSource);
            ThenCallIsDelegatedToSetDacChannelDataSourceAllChannelsCommand(dacChannelDataSource);
        }

        [Test]
        [TestCase(ChannelAddress.Group0AllChannels, DacChannelDataSource.DataValueA,
            (byte) SpecialFunctionCode.WriteToABSelectRegister0, (ushort) ABSelectRegisterBits.AllChannelsA)]
        [TestCase(ChannelAddress.Group0AllChannels, DacChannelDataSource.DataValueB,
            (byte) SpecialFunctionCode.WriteToABSelectRegister0, (ushort) ABSelectRegisterBits.AllChannelsB)]
        [TestCase(ChannelAddress.Group1AllChannels, DacChannelDataSource.DataValueA,
            (byte) SpecialFunctionCode.WriteToABSelectRegister1, (ushort) ABSelectRegisterBits.AllChannelsA)]
        [TestCase(ChannelAddress.Group1AllChannels, DacChannelDataSource.DataValueB,
            (byte) SpecialFunctionCode.WriteToABSelectRegister1, (ushort) ABSelectRegisterBits.AllChannelsB)]
        [TestCase(ChannelAddress.Group2AllChannels, DacChannelDataSource.DataValueA,
            (byte) SpecialFunctionCode.WriteToABSelectRegister2, (ushort) ABSelectRegisterBits.AllChannelsA)]
        [TestCase(ChannelAddress.Group2AllChannels, DacChannelDataSource.DataValueB,
            (byte) SpecialFunctionCode.WriteToABSelectRegister2, (ushort) ABSelectRegisterBits.AllChannelsB)]
        [TestCase(ChannelAddress.Group3AllChannels, DacChannelDataSource.DataValueA,
            (byte) SpecialFunctionCode.WriteToABSelectRegister3, (ushort) ABSelectRegisterBits.AllChannelsA)]
        [TestCase(ChannelAddress.Group3AllChannels, DacChannelDataSource.DataValueB,
            (byte) SpecialFunctionCode.WriteToABSelectRegister3, (ushort) ABSelectRegisterBits.AllChannelsB)]
        [TestCase(ChannelAddress.Group4AllChannels, DacChannelDataSource.DataValueA,
            (byte) SpecialFunctionCode.WriteToABSelectRegister4, (ushort) ABSelectRegisterBits.AllChannelsA)]
        [TestCase(ChannelAddress.Group4AllChannels, DacChannelDataSource.DataValueB,
            (byte) SpecialFunctionCode.WriteToABSelectRegister4, (ushort) ABSelectRegisterBits.AllChannelsB)]
        public void ShouldSetDacChannelDataSourceAllChannelsInSameGroup(
            ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource,
            byte specialFunctionCode,
            ushort data
        )
        {
            WhenSetDacChannelDataSource(channelAddress, dacChannelDataSource);
            ThenSendSpecialFunctionCommandShouldBeCalled((SpecialFunctionCode) specialFunctionCode, data);
        }

        [Test]
        [TestCase(ChannelAddress.Group0Through4Channel0, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel0,
            ChannelAddress.Group1Channel0, ChannelAddress.Group2Channel0, ChannelAddress.Group3Channel0,
            ChannelAddress.Group4Channel0)]
        [TestCase(ChannelAddress.Group0Through4Channel0, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel0,
            ChannelAddress.Group1Channel0, ChannelAddress.Group2Channel0, ChannelAddress.Group3Channel0,
            ChannelAddress.Group4Channel0)]
        [TestCase(ChannelAddress.Group0Through4Channel1, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel1,
            ChannelAddress.Group1Channel1, ChannelAddress.Group2Channel1, ChannelAddress.Group3Channel1,
            ChannelAddress.Group4Channel1)]
        [TestCase(ChannelAddress.Group0Through4Channel1, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel1,
            ChannelAddress.Group1Channel1, ChannelAddress.Group2Channel1, ChannelAddress.Group3Channel1,
            ChannelAddress.Group4Channel1)]
        [TestCase(ChannelAddress.Group0Through4Channel2, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel2,
            ChannelAddress.Group1Channel2, ChannelAddress.Group2Channel2, ChannelAddress.Group3Channel2,
            ChannelAddress.Group4Channel2)]
        [TestCase(ChannelAddress.Group0Through4Channel2, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel2,
            ChannelAddress.Group1Channel2, ChannelAddress.Group2Channel2, ChannelAddress.Group3Channel2,
            ChannelAddress.Group4Channel2)]
        [TestCase(ChannelAddress.Group0Through4Channel3, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel3,
            ChannelAddress.Group1Channel3, ChannelAddress.Group2Channel3, ChannelAddress.Group3Channel3,
            ChannelAddress.Group4Channel3)]
        [TestCase(ChannelAddress.Group0Through4Channel3, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel3,
            ChannelAddress.Group1Channel3, ChannelAddress.Group2Channel3, ChannelAddress.Group3Channel3,
            ChannelAddress.Group4Channel3)]
        [TestCase(ChannelAddress.Group0Through4Channel4, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel4,
            ChannelAddress.Group1Channel4, ChannelAddress.Group2Channel4, ChannelAddress.Group3Channel4,
            ChannelAddress.Group4Channel4)]
        [TestCase(ChannelAddress.Group0Through4Channel4, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel4,
            ChannelAddress.Group1Channel4, ChannelAddress.Group2Channel4, ChannelAddress.Group3Channel4,
            ChannelAddress.Group4Channel4)]
        [TestCase(ChannelAddress.Group0Through4Channel5, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel5,
            ChannelAddress.Group1Channel5, ChannelAddress.Group2Channel5, ChannelAddress.Group3Channel5,
            ChannelAddress.Group4Channel5)]
        [TestCase(ChannelAddress.Group0Through4Channel5, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel5,
            ChannelAddress.Group1Channel5, ChannelAddress.Group2Channel5, ChannelAddress.Group3Channel5,
            ChannelAddress.Group4Channel5)]
        [TestCase(ChannelAddress.Group0Through4Channel6, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel6,
            ChannelAddress.Group1Channel6, ChannelAddress.Group2Channel6, ChannelAddress.Group3Channel6,
            ChannelAddress.Group4Channel6)]
        [TestCase(ChannelAddress.Group0Through4Channel6, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel6,
            ChannelAddress.Group1Channel6, ChannelAddress.Group2Channel6, ChannelAddress.Group3Channel6,
            ChannelAddress.Group4Channel6)]
        [TestCase(ChannelAddress.Group0Through4Channel7, DacChannelDataSource.DataValueA, ChannelAddress.Group0Channel7,
            ChannelAddress.Group1Channel7, ChannelAddress.Group2Channel7, ChannelAddress.Group3Channel7,
            ChannelAddress.Group4Channel7)]
        [TestCase(ChannelAddress.Group0Through4Channel7, DacChannelDataSource.DataValueB, ChannelAddress.Group0Channel7,
            ChannelAddress.Group1Channel7, ChannelAddress.Group2Channel7, ChannelAddress.Group3Channel7,
            ChannelAddress.Group4Channel7)]
        public void ShouldSetDacChannelDataSourceSameChannelInGroups0To4(
            ChannelAddress groupChannelAddress,
            DacChannelDataSource dacChannelDataSource,
            ChannelAddress group0ChannelAddress,
            ChannelAddress group1ChannelAddress,
            ChannelAddress group2ChannelAddress,
            ChannelAddress group3ChannelAddress,
            ChannelAddress group4ChannelAddress
        )
        {
            WhenSetDacChannelDataSource(groupChannelAddress, dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group0ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group1ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group2ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group3ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group4ChannelAddress,
                dacChannelDataSource);
        }


        [Test]
        public void ShouldSetDacChannelDataSource
        (
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] ChannelAddress channelAddress,
            [Values] DacChannelDataSource dacChannelDataSource
        )
        {
            WhenSetDacChannelDataSource(channelAddress, dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(channelAddress, dacChannelDataSource);
        }

        [Test]
        [TestCase(ChannelAddress.Group0Through4Channel0, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel0,
            ChannelAddress.Group2Channel0, ChannelAddress.Group3Channel0, ChannelAddress.Group4Channel0)]
        [TestCase(ChannelAddress.Group0Through4Channel0, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel0,
            ChannelAddress.Group2Channel0, ChannelAddress.Group3Channel0, ChannelAddress.Group4Channel0)]
        [TestCase(ChannelAddress.Group0Through4Channel1, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel1,
            ChannelAddress.Group2Channel1, ChannelAddress.Group3Channel1, ChannelAddress.Group4Channel1)]
        [TestCase(ChannelAddress.Group0Through4Channel1, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel1,
            ChannelAddress.Group2Channel1, ChannelAddress.Group3Channel1, ChannelAddress.Group4Channel1)]
        [TestCase(ChannelAddress.Group0Through4Channel2, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel2,
            ChannelAddress.Group2Channel2, ChannelAddress.Group3Channel2, ChannelAddress.Group4Channel2)]
        [TestCase(ChannelAddress.Group0Through4Channel2, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel2,
            ChannelAddress.Group2Channel2, ChannelAddress.Group3Channel2, ChannelAddress.Group4Channel2)]
        [TestCase(ChannelAddress.Group0Through4Channel3, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel3,
            ChannelAddress.Group2Channel3, ChannelAddress.Group3Channel3, ChannelAddress.Group4Channel3)]
        [TestCase(ChannelAddress.Group0Through4Channel3, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel3,
            ChannelAddress.Group2Channel3, ChannelAddress.Group3Channel3, ChannelAddress.Group4Channel3)]
        [TestCase(ChannelAddress.Group0Through4Channel4, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel4,
            ChannelAddress.Group2Channel4, ChannelAddress.Group3Channel4, ChannelAddress.Group4Channel4)]
        [TestCase(ChannelAddress.Group0Through4Channel4, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel4,
            ChannelAddress.Group2Channel4, ChannelAddress.Group3Channel4, ChannelAddress.Group4Channel4)]
        [TestCase(ChannelAddress.Group0Through4Channel5, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel5,
            ChannelAddress.Group2Channel5, ChannelAddress.Group3Channel5, ChannelAddress.Group4Channel5)]
        [TestCase(ChannelAddress.Group0Through4Channel5, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel5,
            ChannelAddress.Group2Channel5, ChannelAddress.Group3Channel5, ChannelAddress.Group4Channel5)]
        [TestCase(ChannelAddress.Group0Through4Channel6, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel6,
            ChannelAddress.Group2Channel6, ChannelAddress.Group3Channel6, ChannelAddress.Group4Channel6)]
        [TestCase(ChannelAddress.Group0Through4Channel6, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel6,
            ChannelAddress.Group2Channel6, ChannelAddress.Group3Channel6, ChannelAddress.Group4Channel6)]
        [TestCase(ChannelAddress.Group0Through4Channel7, DacChannelDataSource.DataValueA, ChannelAddress.Group1Channel7,
            ChannelAddress.Group2Channel7, ChannelAddress.Group3Channel7, ChannelAddress.Group4Channel7)]
        [TestCase(ChannelAddress.Group0Through4Channel7, DacChannelDataSource.DataValueB, ChannelAddress.Group1Channel7,
            ChannelAddress.Group2Channel7, ChannelAddress.Group3Channel7, ChannelAddress.Group4Channel7)]
        public void ShouldSetDacChannelDataSourceSameChannelInGroups1To4(
            ChannelAddress groupChannelAddress,
            DacChannelDataSource dacChannelDataSource,
            ChannelAddress group1ChannelAddress,
            ChannelAddress group2ChannelAddress,
            ChannelAddress group3ChannelAddress,
            ChannelAddress group4ChannelAddress
        )
        {
            WhenSetDacChannelDataSource(groupChannelAddress, dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group1ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group2ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group3ChannelAddress,
                dacChannelDataSource);
            ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(group4ChannelAddress,
                dacChannelDataSource);
        }

        [Test]
        public void ShouldSetDacChannelDataSourceSimultaneouslyForChannelsInSameGroup(
            [Values] ChannelGroup group,
            [Values] DacChannelDataSource channel0,
            [Values] DacChannelDataSource channel1,
            [Values] DacChannelDataSource channel2,
            [Values] DacChannelDataSource channel3,
            [Values] DacChannelDataSource channel4,
            [Values] DacChannelDataSource channel5,
            [Values] DacChannelDataSource channel6,
            [Values] DacChannelDataSource channel7)
        {
            WhenSetDacChannelDataSource(group, channel0, channel1, channel2, channel3, channel4, channel5, channel6,
                channel7);
            ThenSendSpecialFunctionCommandShouldBeCalled(
                SpecialFunctionCode.WriteToABSelectRegister0 + (int) group,
                (ushort) (
                    (channel0 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel0 : 0) |
                    (channel1 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel1 : 0) |
                    (channel2 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel2 : 0) |
                    (channel3 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel3 : 0) |
                    (channel4 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel4 : 0) |
                    (channel5 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel5 : 0) |
                    (channel6 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel6 : 0) |
                    (channel7 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel7 : 0)
                )
            );
        }

        [Test]
        public void ShouldUpdateABSelectRegistersInDeviceStateCache(
            [Values] ChannelGroup group,
            [Values] DacChannelDataSource channel0,
            [Values] DacChannelDataSource channel1,
            [Values] DacChannelDataSource channel2,
            [Values] DacChannelDataSource channel3,
            [Values] DacChannelDataSource channel4,
            [Values] DacChannelDataSource channel5,
            [Values] DacChannelDataSource channel6,
            [Values] DacChannelDataSource channel7)
        {
            WhenSetDacChannelDataSource(group, channel0, channel1, channel2, channel3, channel4, channel5, channel6,
                channel7);
            ThenABSelectRegistersInDeviceStateCacheAreUpdated(
                (int) group,
                (channel0 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel0 : 0) |
                (channel1 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel1 : 0) |
                (channel2 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel2 : 0) |
                (channel3 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel3 : 0) |
                (channel4 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel4 : 0) |
                (channel5 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel5 : 0) |
                (channel6 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel6 : 0) |
                (channel7 == DacChannelDataSource.DataValueB ? ABSelectRegisterBits.Channel7 : 0)
            );
        }

        public void ShouldThrowExceptionIfArgumentsAreOutOfRange(
            [Values((int) (ChannelGroup.Group0 - 1), (int) (ChannelGroup.Group4 + 1))] ChannelGroup channelGroup,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel0,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel1,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel2,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel3,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel4,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel5,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel6,
            [Values((int) (ChannelAddress.AllGroupsAllChannels - 1), (int) (ChannelAddress.Group1Through4Channel7 + 1),
                6, 7)] DacChannelDataSource channel7)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WhenSetDacChannelDataSource(channelGroup, channel0, channel1, channel2, channel3, channel4,
                    channel5, channel6, channel7));
        }

        private void WhenSetDacChannelDataSource(ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource)
        {
            _setDacChannelDataSourceCommand.SetDacChannelDataSource(channelAddress, dacChannelDataSource);
        }

        private void WhenSetDacChannelDataSource(ChannelGroup group, DacChannelDataSource channel0,
            DacChannelDataSource channel1, DacChannelDataSource channel2,
            DacChannelDataSource channel3, DacChannelDataSource channel4,
            DacChannelDataSource channel5, DacChannelDataSource channel6,
            DacChannelDataSource channel7)
        {
            _setDacChannelDataSourceCommand.SetDacChannelDataSource(group, channel0, channel1, channel2, channel3,
                channel4, channel5, channel6, channel7);
        }

        private void ThenABSelectRegistersInDeviceStateCacheAreUpdated(int abSelectRegisterNum,
            ABSelectRegisterBits expectedValue)
        {
            _deviceState
                .ABSelectRegisters[abSelectRegisterNum]
                .GetValueOrDefault(ABSelectRegisterBits.AllChannelsA)
                .Should()
                .Be(expectedValue);
        }

        private void ThenCallIsDelegatedToSetDacChannelDataSourceAllChannelsCommand(
            DacChannelDataSource dacChannelDataSource)
        {
            Mock.Get(_fakeSetDacChannelDataSourceAllChannelsCommand)
                .Verify(x => x.SetDacChannelDataSourceAllChannels(dacChannelDataSource)
                    , Times.Once);
        }

        private void ThenSendSpecialFunctionCommandShouldBeCalled(SpecialFunctionCode specialFunctionCode, ushort data)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(specialFunctionCode, data), Times.Once);
        }

        private void ThenCallShouldBeDelegatedToSetDacChannelDataSourceInternalCommand(ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource)
        {
            Mock.Get(_fakeSetDacChannelDataSourceInternalCommand)
                .Verify(x => x.SetDacChannelDataSourceInternal(channelAddress, dacChannelDataSource), Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSetDacChannelDataSourceAllChannelsCommand = Mock.Of<ISetDacChannelDataSourceAllChannels>();
            Mock.Get(_fakeSetDacChannelDataSourceAllChannelsCommand)
                .Setup(x => x.SetDacChannelDataSourceAllChannels(It.IsAny<DacChannelDataSource>()))
                .Verifiable();

            _fakeSetDacChannelDataSourceInternalCommand = Mock.Of<ISetDacChannelDataSourceInternal>();
            Mock.Get(_fakeSetDacChannelDataSourceInternalCommand)
                .Setup(x => x.SetDacChannelDataSourceInternal(It.IsAny<ChannelAddress>(),
                    It.IsAny<DacChannelDataSource>()))
                .Verifiable();

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Setup(x => x.SendSpecialFunction(It.IsAny<SpecialFunctionCode>(), It.IsAny<ushort>()))
                .Verifiable();

            _setDacChannelDataSourceCommand = new SetDacChannelDataSourceCommand(
                _fakeEvalBoard,
                _fakeSetDacChannelDataSourceAllChannelsCommand,
                _fakeSetDacChannelDataSourceInternalCommand,
                _fakeSendSpecialFunctionCommand);
        }

        private ISetDacChannelDataSource _setDacChannelDataSourceCommand;
        private ISetDacChannelDataSourceAllChannels _fakeSetDacChannelDataSourceAllChannelsCommand;
        private ISetDacChannelDataSourceInternal _fakeSetDacChannelDataSourceInternalCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
    }
}