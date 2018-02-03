using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackOFS0RegisterCommandTests
    {
        [Test]
        public void ShouldReturnOFS0RegisterValueFromCache(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs0RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS0RegisterValueExistsInDeviceStateRegisterCache(ofs0RegisterBits);
            WhenReadbackOFS0Register();
            ThenTheValueFromCacheShouldBeTheValueReturned();
        }

        [Test]
        public void ShouldReturnOFS0RegisterValueFromDeviceWhenCacheIsEmpty(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs0RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS0RegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenOFS0RegisterValueInDeviceMemory(ofs0RegisterBits);
            WhenReadbackOFS0Register();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldReturnOFS0RegisterValueFromDeviceWhenCacheIsDisabled(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs0RegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenOFS0RegisterValueExistsInDeviceStateRegisterCache(ofs0RegisterBits);
            GivenOFS0RegisterValueInDeviceMemory(ofs0RegisterBits);
            WhenReadbackOFS0Register();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldOnlyReturnReadableBitsPortionOfOFS0RegisterValue(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs0RegisterBits
        )
        {
            GivenOFS0RegisterValueInDeviceMemory(ofs0RegisterBits);
            WhenReadbackOFS0Register();
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheOFS0RegisterValueReturnedFromDevice(
            [Values(ushort.MinValue, (ushort) BasicMasks.FourteenBits, (ushort) BasicMasks.SixteenBits)]
            ushort ofs0RegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenOFS0RegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenOFS0RegisterValueInDeviceMemory(ofs0RegisterBits);
            WhenReadbackOFS0Register();
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

        private void GivenOFS0RegisterValueExistsInDeviceStateRegisterCache(ushort? value)
        {
            _deviceState.OFS0Register = value;
        }

        private void GivenOFS0RegisterValueDoesNotExistInDeviceStateRegisterCache()
        {
            _deviceState.OFS0Register = null;
        }

        private void GivenOFS0RegisterValueInDeviceMemory(ushort ofs0RegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns(ofs0RegisterBits);
        }

        private void WhenReadbackOFS0Register()
        {
            _resultOfReadback = _readbackOFS0RegisterCommand.ReadbackOFS0Register();
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned()
        {
            _deviceState.OFS0Register.Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) AddressCodesForDataReadback.OFS0Register))
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
            _deviceState.OFS0Register.Should().Be(_resultOfReadback);
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

            _readbackOFS0RegisterCommand =
                new ReadbackOFS0RegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackOFS0Register _readbackOFS0RegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ushort _resultOfReadback;
    }
}