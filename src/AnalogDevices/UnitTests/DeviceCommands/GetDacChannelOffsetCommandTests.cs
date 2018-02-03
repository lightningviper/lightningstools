using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetDacChannelOffsetCommandTests
    {
        [Test]
        public void ShouldGetCorrectDacChannelOffset(
            [Values(DacPrecision.SixteenBit, DacPrecision.FourteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int cRegisterValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenCRegisterBits((ChannelAddress) channelAddress, (ushort) cRegisterValue);
            WhenGetDacChannelOffset((ChannelAddress) channelAddress);
            ThenValueReturnedShouldBe((ushort) (cRegisterValue &
                                                (dacPrecision == DacPrecision.SixteenBit
                                                    ? (ushort) BasicMasks.SixteenBits
                                                    : (ushort) BasicMasks.HighFourteenBits)));
        }

        [Test]
        public void ShouldThrowExceptionIfChannelNumberIsOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenGetDacChannelOffset(channelAddress));
        }

        private void GivenDacPrecision(DacPrecision dacPrecision)
        {
            _deviceState.Precision = dacPrecision;
        }

        private void GivenCRegisterBits(ChannelAddress channelAddress, ushort cRegisterValue)
        {
            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackCRegister(channelAddress))
                .Returns((ushort) (cRegisterValue & (_deviceState.Precision == DacPrecision.SixteenBit
                                       ? (ushort) BasicMasks.SixteenBits
                                       : (ushort) BasicMasks.HighFourteenBits)));
        }

        private void WhenGetDacChannelOffset(ChannelAddress channelAddress)
        {
            _dacChannelOffset = _getDacChannelOffsetCommand.GetDacChannelOffset(channelAddress);
        }

        private void ThenValueReturnedShouldBe(ushort expectedValue)
        {
            _dacChannelOffset.Should().Be(expectedValue);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeRb = Mock.Of<IReadbackCRegister>();

            _getDacChannelOffsetCommand = new GetDacChannelOffsetCommand(_fakeEvalBoard, _fakeRb);
        }

        private IGetDacChannelOffset _getDacChannelOffsetCommand;
        private IReadbackCRegister _fakeRb;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ushort _dacChannelOffset;
    }
}