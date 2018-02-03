using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackMRegisterCommandTests
    {
        [Test]
        public void ShouldReturnMRegisterValueFromCache(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort mRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenMRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, mRegisterBits);
            WhenReadbackMRegister((ChannelAddress) channelAddress);
            ThenTheValueFromCacheShouldBeTheValueReturned((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnMRegisterValueFromDeviceWhenCacheIsEmpty(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort mRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenMRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenMRegisterValueInDeviceMemory(mRegisterBits);
            WhenReadbackMRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnMRegisterValueFromDeviceWhenCacheIsDisabled(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort mRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenMRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, mRegisterBits);
            GivenMRegisterValueInDeviceMemory(mRegisterBits);
            WhenReadbackMRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfMRegisterValue(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort mRegisterBits,
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision
        )
        {
            GivenMRegisterValueInDeviceMemory(mRegisterBits);
            GivenDacPrecision(dacPrecision);
            WhenReadbackMRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheMRegisterValueReturnedFromDevice(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort mRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenMRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenMRegisterValueInDeviceMemory(mRegisterBits);
            WhenReadbackMRegister((ChannelAddress) channelAddress);
            ThenTheValueReadShouldBeCached((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldThrowExceptionIfChannelAddressIsOutOfRange(
            [Values(ChannelAddress.Dac0 - 1, ChannelAddress.Dac39 + 1)] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenReadbackMRegister(channelAddress));
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

        private void GivenMRegisterValueExistsInDeviceStateRegisterCache(ChannelAddress channelAddress, ushort? value)
        {
            _deviceState.MRegisters[channelAddress.ToChannelNumber()] = value;
        }

        private void GivenMRegisterValueDoesNotExistInDeviceStateRegisterCache(ChannelAddress channelAddress)
        {
            _deviceState.MRegisters[channelAddress.ToChannelNumber()] = null;
        }

        private void GivenMRegisterValueInDeviceMemory(ushort mRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(mRegisterBits);
        }

        private void WhenReadbackMRegister(ChannelAddress channelAddress)
        {
            _resultOfReadback = _readbackMRegisterCommand.ReadbackMRegister(channelAddress);
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned(ChannelAddress channelAddress)
        {
            _deviceState.MRegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice(ChannelAddress channelAddress)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) ((ushort) AddressCodesForDataReadback.MRegister
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
            _deviceState.MRegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
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

            _readbackMRegisterCommand = new ReadbackMRegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand,
                _fakeReadSPICommand);
        }

        private IReadbackMRegister _readbackMRegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}