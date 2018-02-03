using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetDacChannelDataSourceAllChannelsCommandTests
    {
        [Test]
        public void ShouldSendSpecialFunctionCodeToSetDataSourceForAllChannelsToDataValueA()
        {
            WhenSetDacChannelDataSourceAllChannels(DacChannelDataSource.DataValueA);
            ThenSpecialFunctionCodeIsSentToBlockWriteABSelectRegistersAndSetTheDataSourceForAllChannelsToDataValueA();
        }

        [Test]
        public void ShouldSendSpecialFunctionCodeToSetDataSourceForAllChannelsToDataValueB()
        {
            WhenSetDacChannelDataSourceAllChannels(DacChannelDataSource.DataValueB);
            ThenSpecialFunctionCodeIsSentToBlockWriteABSelectRegistersAndSetTheDataSourceForAllChannelsToDataValueB();
        }

        [Test]
        public void ShouldUpdateABSelectRegistersInDeviceStateCache(
            [Values] DacChannelDataSource dacChannelDataSource)
        {
            WhenSetDacChannelDataSourceAllChannels(dacChannelDataSource);
            ThenABSelectRegistersInDeviceStateCacheAreSetAppropriately(dacChannelDataSource);
        }

        [Test]
        public void ShouldThrowArgumentOutOfRangeExceptionWhenSourceParameterIsOutOfRange(
            [Values(-1, 2)] int source)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WhenSetDacChannelDataSourceAllChannels((DacChannelDataSource) source));
        }

        private void WhenSetDacChannelDataSourceAllChannels(DacChannelDataSource dacChannelDataSource)
        {
            _setDacChannelDataSourceAllChannelsCommand.SetDacChannelDataSourceAllChannels(dacChannelDataSource);
        }

        private void
            ThenSpecialFunctionCodeIsSentToBlockWriteABSelectRegistersAndSetTheDataSourceForAllChannelsToDataValueA()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.BlockWriteABSelectRegisters,
                        (ushort) ABSelectRegisterBits.AllChannelsA),
                    Times.Once);
        }

        private void
            ThenSpecialFunctionCodeIsSentToBlockWriteABSelectRegistersAndSetTheDataSourceForAllChannelsToDataValueB()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.BlockWriteABSelectRegisters,
                        (ushort) ABSelectRegisterBits.AllChannelsB),
                    Times.Once);
        }

        private void ThenABSelectRegistersInDeviceStateCacheAreSetAppropriately(
            DacChannelDataSource dacChannelDataSource)
        {
            var theExpectedValue = dacChannelDataSource == DacChannelDataSource.DataValueA
                ? ABSelectRegisterBits.AllChannelsA
                : ABSelectRegisterBits.AllChannelsB;

            _deviceState.ABSelectRegisters[0].Should().Be(theExpectedValue);
            _deviceState.ABSelectRegisters[1].Should().Be(theExpectedValue);
            _deviceState.ABSelectRegisters[2].Should().Be(theExpectedValue);
            _deviceState.ABSelectRegisters[3].Should().Be(theExpectedValue);
            _deviceState.ABSelectRegisters[4].Should().Be(theExpectedValue);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Setup(x => x.SendSpecialFunction(
                    It.IsAny<SpecialFunctionCode>(),
                    It.IsAny<ushort>()))
                .Verifiable();


            _setDacChannelDataSourceAllChannelsCommand =
                new SetDacChannelDataSourceAllChannelsCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand);
        }

        private DeviceState _deviceState;
        private ISetDacChannelDataSourceAllChannels _setDacChannelDataSourceAllChannelsCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}