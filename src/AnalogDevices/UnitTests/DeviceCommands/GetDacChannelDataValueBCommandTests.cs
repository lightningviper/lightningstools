using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetDacChannelDataValueBCommandTests
    {
        [Test]
        public void ShouldGetCorrectDacChannelDataValueB(
            [Values(DacPrecision.SixteenBit, DacPrecision.FourteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int x1bRegisterValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenX1BRegisterBits((ChannelAddress) channelAddress, (ushort) x1bRegisterValue);
            WhenGetDacChannelDataValueB((ChannelAddress) channelAddress);
            ThenValueReturnedShouldBe((ushort) (x1bRegisterValue &
                                                (dacPrecision == DacPrecision.SixteenBit
                                                    ? (ushort) BasicMasks.SixteenBits
                                                    : (ushort) BasicMasks.HighFourteenBits)));
        }

        [Test]
        public void ShouldThrowExceptionIfChannelNumberIsOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenGetDacChannelDataValueB(channelAddress));
        }

        private void GivenDacPrecision(DacPrecision dacPrecision)
        {
            _deviceState.Precision = dacPrecision;
        }

        private void GivenX1BRegisterBits(ChannelAddress channelAddress, ushort x1bRegisterValue)
        {
            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackX1BRegister(channelAddress))
                .Returns((ushort) (x1bRegisterValue & (_deviceState.Precision == DacPrecision.SixteenBit
                                       ? (ushort) BasicMasks.SixteenBits
                                       : (ushort) BasicMasks.HighFourteenBits)));
        }

        private void WhenGetDacChannelDataValueB(ChannelAddress channelAddress)
        {
            _dacChannelDataValueB = _getDacChannelDataValueBCommand.GetDacChannelDataValueB(channelAddress);
        }

        private void ThenValueReturnedShouldBe(ushort expectedValue)
        {
            _dacChannelDataValueB.Should().Be(expectedValue);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeRb = Mock.Of<IReadbackX1BRegister>();

            _getDacChannelDataValueBCommand = new GetDacChannelDataValueBCommand(_fakeEvalBoard, _fakeRb);
        }

        private IGetDacChannelDataValueB _getDacChannelDataValueBCommand;
        private IReadbackX1BRegister _fakeRb;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ushort _dacChannelDataValueB;
    }
}