using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackCRegisterCommandTests
    {
        [Test]
        public void ShouldReturnCRegisterValueFromCache(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort cRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenCRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, cRegisterBits);
            WhenReadbackCRegister((ChannelAddress) channelAddress);
            ThenTheValueFromCacheShouldBeTheValueReturned((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnCRegisterValueFromDeviceWhenCacheIsEmpty(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort cRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenCRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenCRegisterValueInDeviceMemory(cRegisterBits);
            WhenReadbackCRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldReturnCRegisterValueFromDeviceWhenCacheIsDisabled(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort cRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenCRegisterValueExistsInDeviceStateRegisterCache((ChannelAddress) channelAddress, cRegisterBits);
            GivenCRegisterValueInDeviceMemory(cRegisterBits);
            WhenReadbackCRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeReadFromTheDevice((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfCRegisterValue(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort cRegisterBits,
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision
        )
        {
            GivenCRegisterValueInDeviceMemory(cRegisterBits);
            GivenDacPrecision(dacPrecision);
            WhenReadbackCRegister((ChannelAddress) channelAddress);
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheCRegisterValueReturnedFromDevice(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort cRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenCRegisterValueDoesNotExistInDeviceStateRegisterCache((ChannelAddress) channelAddress);
            GivenCRegisterValueInDeviceMemory(cRegisterBits);
            WhenReadbackCRegister((ChannelAddress) channelAddress);
            ThenTheValueReadShouldBeCached((ChannelAddress) channelAddress);
        }

        [Test]
        public void ShouldThrowExceptionIfChannelAddressIsOutOfRange(
            [Values(ChannelAddress.Dac0 - 1, ChannelAddress.Dac39 + 1)] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenReadbackCRegister(channelAddress));
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

        private void GivenCRegisterValueExistsInDeviceStateRegisterCache(ChannelAddress channelAddress, ushort? value)
        {
            _deviceState.CRegisters[channelAddress.ToChannelNumber()] = value;
        }

        private void GivenCRegisterValueDoesNotExistInDeviceStateRegisterCache(ChannelAddress channelAddress)
        {
            _deviceState.CRegisters[(int) channelAddress - 8] = null;
        }

        private void GivenCRegisterValueInDeviceMemory(ushort cRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(cRegisterBits);
        }

        private void WhenReadbackCRegister(ChannelAddress channelAddress)
        {
            _resultOfReadback = _readbackCRegisterCommand.ReadbackCRegister(channelAddress);
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned(ChannelAddress channelAddress)
        {
            _deviceState.CRegisters[channelAddress.ToChannelNumber()].Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice(ChannelAddress channelAddress)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) ((ushort) AddressCodesForDataReadback.CRegister
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
            _deviceState.CRegisters[(int) channelAddress - 8].Should().Be(_resultOfReadback);
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

            _readbackCRegisterCommand = new ReadbackCRegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand,
                _fakeReadSPICommand);
        }

        private IReadbackCRegister _readbackCRegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}