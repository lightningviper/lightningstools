using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackX1BRegisterCommandTests
    {
        [Test]
        public void ShouldReturnX1BRegisterValueFromCache(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1bRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1BRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, x1bRegisterBits);
            WhenReadbackX1BRegister((ChannelAddress) channelAddress);
            ThenTheValueFromCacheShouldBeTheValueReturned((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnX1BRegisterValueFromDeviceWhenCacheIsEmpty(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1bRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1BRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenX1BRegisterValueInDeviceMemory(x1bRegisterBits);
            WhenReadbackX1BRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnX1BRegisterValueFromDeviceWhenCacheIsDisabled(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1bRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenX1BRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, x1bRegisterBits);
            GivenX1BRegisterValueInDeviceMemory(x1bRegisterBits);
            WhenReadbackX1BRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfX1BRegisterValue(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1bRegisterBits,
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision
        )
        {
            GivenX1BRegisterValueInDeviceMemory(x1bRegisterBits);
            GivenDacPrecision(dacPrecision);
            WhenReadbackX1BRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheX1BRegisterValueReturnedFromDevice(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1bRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1BRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenX1BRegisterValueInDeviceMemory(x1bRegisterBits);
            WhenReadbackX1BRegister((ChannelAddress) channelAddress);
            ThenTheValueReadShouldBeCached((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldThrowExceptionIfChannelAddressIsOutOfRange(
            [Values(ChannelAddress.Dac0 - 1, ChannelAddress.Dac39 + 1)] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenReadbackX1BRegister(channelAddress));
        }

        private void GivenDacPrecision(DacPrecision dacPrecision)
        {
            _deviceState.Precision = dacPrecision;
        }

        private void GivenRegisterCachesAreEnabled()
        {
            _deviceState.UseRegisterCache = true;
        }

        private void GivenRegisterCachesAreDisabled()
        {
            _deviceState.UseRegisterCache = false;
        }

        private void GivenX1BRegisterValueExistsInDeviceStateRegisterCache(ChannelAddress channelAddress, ushort? value)
        {
            _deviceState.X1BRegisters[channelAddress.ToChannelNumber()] = value;
        }

        private void GivenX1BRegisterValueDoesNotExistInDeviceStateRegisterCache(ChannelAddress channelAddress)
        {
            _deviceState.X1BRegisters[channelAddress.ToChannelNumber()] = null;
        }

        private void GivenX1BRegisterValueInDeviceMemory(ushort x1bRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(x1bRegisterBits);
        }

        private void WhenReadbackX1BRegister(ChannelAddress channelAddress)
        {
            _resultOfReadback = _readbackX1BRegisterCommand.ReadbackX1BRegister(channelAddress);
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned(ChannelAddress channelAddress)
        {
            _deviceState.X1BRegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice(ChannelAddress channelAddress)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) ((ushort) AddressCodesForDataReadback.X1BRegister
                                                          |
                                                          (ushort) (((byte) channelAddress &
                                                                     (byte) BasicMasks.SixBits) << 7))))
                    , Times.Once);

            Mock.Get(_fakeReadSPICommand)
                .Verify(x => x.ReadSPI(), Times.Once);
        }

        private void ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice()
        {
            var readableBits =
                _deviceState.Precision == DacPrecision.FourteenBit
                    ? (ushort) BasicMasks.HighFourteenBits
                    : (ushort) BasicMasks.SixteenBits;
            ((ushort) (_resultOfReadback & ~readableBits)).Should().Be(0);
        }

        private void ThenTheValueReadShouldBeCached(ChannelAddress channelAddress)
        {
            _deviceState.X1BRegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
        }

        [SetUp]
        public void SetUp()
        {
            _deviceState = new DeviceState();

            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            _fakeReadSPICommand = Mock.Of<IReadSPI>();

            _readbackX1BRegisterCommand =
                new ReadbackX1BRegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackX1BRegister _readbackX1BRegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}