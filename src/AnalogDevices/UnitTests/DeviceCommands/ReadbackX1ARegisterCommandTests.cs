using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackX1ARegisterCommandTests
    {
        [Test]
        public void ShouldReturnX1ARegisterValueFromCache(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1aRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1ARegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, x1aRegisterBits);
            WhenReadbackX1ARegister((ChannelAddress) channelAddress);
            ThenTheValueFromCacheShouldBeTheValueReturned((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnX1ARegisterValueFromDeviceWhenCacheIsEmpty(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1aRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1ARegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenX1ARegisterValueInDeviceMemory(x1aRegisterBits);
            WhenReadbackX1ARegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnX1ARegisterValueFromDeviceWhenCacheIsDisabled(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1aRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenX1ARegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, x1aRegisterBits);
            GivenX1ARegisterValueInDeviceMemory(x1aRegisterBits);
            WhenReadbackX1ARegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfX1ARegisterValue(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1aRegisterBits,
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision
        )
        {
            GivenX1ARegisterValueInDeviceMemory(x1aRegisterBits);
            GivenDacPrecision(dacPrecision);
            WhenReadbackX1ARegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheX1ARegisterValueReturnedFromDevice(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort x1aRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenX1ARegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenX1ARegisterValueInDeviceMemory(x1aRegisterBits);
            WhenReadbackX1ARegister((ChannelAddress) channelAddress);
            ThenTheValueReadShouldBeCached((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldThrowExceptionIfChannelAddressIsOutOfRange(
            [Values(ChannelAddress.Dac0 - 1, ChannelAddress.Dac39 + 1)] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenReadbackX1ARegister(channelAddress));
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

        private void GivenX1ARegisterValueExistsInDeviceStateRegisterCache(ChannelAddress channelAddress, ushort? value)
        {
            _deviceState.X1ARegisters[channelAddress.ToChannelNumber()] = value;
        }

        private void GivenX1ARegisterValueDoesNotExistInDeviceStateRegisterCache(ChannelAddress channelAddress)
        {
            _deviceState.X1ARegisters[channelAddress.ToChannelNumber()] = null;
        }

        private void GivenX1ARegisterValueInDeviceMemory(ushort x1aRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(x1aRegisterBits);
        }

        private void WhenReadbackX1ARegister(ChannelAddress channelAddress)
        {
            _resultOfReadback = _readbackX1ARegisterCommand.ReadbackX1ARegister(channelAddress);
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned(ChannelAddress channelAddress)
        {
            _deviceState.X1ARegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice(ChannelAddress channelAddress)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) ((ushort) AddressCodesForDataReadback.X1ARegister
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
            _deviceState.X1ARegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
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

            _readbackX1ARegisterCommand =
                new ReadbackX1ARegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackX1ARegister _readbackX1ARegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}