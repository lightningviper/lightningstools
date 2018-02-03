using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackOFS1RegisterCommandTests
    {
        [Test]
        public void ShouldReturnOFS1RegisterValueFromCache(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs1RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS1RegisterValueExistsInDeviceStateRegisterCache(ofs1RegisterBits);
            WhenReadbackOFS1Register();
            ThenTheValueFromCacheShouldBeTheValueReturned();
        }

        [Test]
        public void ShouldReturnOFS1RegisterValueFromDeviceWhenCacheIsEmpty(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs1RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS1RegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenOFS1RegisterValueInDeviceMemory(ofs1RegisterBits);
            WhenReadbackOFS1Register();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldReturnOFS1RegisterValueFromDeviceWhenCacheIsDisabled(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs1RegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenOFS1RegisterValueExistsInDeviceStateRegisterCache(ofs1RegisterBits);
            GivenOFS1RegisterValueInDeviceMemory(ofs1RegisterBits);
            WhenReadbackOFS1Register();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfOFS1RegisterValue(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs1RegisterBits
        )
        {
            GivenOFS1RegisterValueInDeviceMemory(ofs1RegisterBits);
            WhenReadbackOFS1Register();
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheOFS1RegisterValueReturnedFromDevice(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs1RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS1RegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenOFS1RegisterValueInDeviceMemory(ofs1RegisterBits);
            WhenReadbackOFS1Register();
            ThenTheValueReadShouldBeCached();
        }

        private void GivenRegisterCachesAreEnabled()
        {
            _deviceState.UseRegisterCache = true;
        }

        private void GivenRegisterCachesAreDisabled()
        {
            _deviceState.UseRegisterCache = false;
        }

        private void GivenOFS1RegisterValueExistsInDeviceStateRegisterCache(ushort? value)
        {
            _deviceState.OFS1Register = value;
        }

        private void GivenOFS1RegisterValueDoesNotExistInDeviceStateRegisterCache()
        {
            _deviceState.OFS1Register = null;
        }

        private void GivenOFS1RegisterValueInDeviceMemory(ushort ofs1RegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(ofs1RegisterBits);
        }

        private void WhenReadbackOFS1Register()
        {
            _resultOfReadback = _readbackOFS1RegisterCommand.ReadbackOFS1Register();
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned()
        {
            _deviceState.OFS1Register.Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) AddressCodesForDataReadback.OFS1Register))
                    , Times.Once);

            Mock.Get(_fakeReadSPICommand)
                .Verify(x => x.ReadSPI(), Times.Once);
        }

        private void ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice()
        {
            ((ushort) (_resultOfReadback & ~(ushort) BasicMasks.FourteenBits)).Should().Be(0);
        }

        private void ThenTheValueReadShouldBeCached()
        {
            _deviceState.OFS1Register.Should().Be(_resultOfReadback);
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

            _readbackOFS1RegisterCommand =
                new ReadbackOFS1RegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackOFS1Register _readbackOFS1RegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}