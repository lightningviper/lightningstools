using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackControlRegisterCommandTests
    {
        [Test]
        public void ShouldReturnControlRegisterValueFromCache(
            [Range(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)] byte controlRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenControlRegisterValueExistsInDeviceStateRegisterCache((ControlRegisterBits) controlRegisterBits);
            WhenReadbackControlRegister();
            ThenTheValueFromCacheShouldBeTheValueReturned();
        }

        [Test]
        public void ShouldReturnControlRegisterValueFromDeviceWhenCacheIsEmpty(
            [Range(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)] byte controlRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenControlRegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenControlRegisterValueInDeviceMemory((ControlRegisterBits) controlRegisterBits);
            WhenReadbackControlRegister();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldReturnControlRegisterValueFromDeviceWhenCacheIsDisabled(
            [Range(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)] byte controlRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenControlRegisterValueExistsInDeviceStateRegisterCache((ControlRegisterBits) controlRegisterBits);
            GivenControlRegisterValueInDeviceMemory((ControlRegisterBits) controlRegisterBits);
            WhenReadbackControlRegister();
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        [TestCase(byte.MaxValue)]
        public void ShouldOnlyReturnReadableBitsPortionOfControlRegisterValue(byte controlRegisterBits)
        {
            GivenControlRegisterValueInDeviceMemory((ControlRegisterBits) controlRegisterBits);
            WhenReadbackControlRegister();
            ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice();
        }

        [Test]
        public void ShouldCacheControlRegisterValueReturnedFromDevice(
            [Range(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)] byte controlRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenControlRegisterValueDoesNotExistInDeviceStateRegisterCache();
            GivenControlRegisterValueInDeviceMemory((ControlRegisterBits) controlRegisterBits);
            WhenReadbackControlRegister();
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

        private void GivenControlRegisterValueExistsInDeviceStateRegisterCache(ControlRegisterBits? value)
        {
            _deviceState.ControlRegister = value;
        }

        private void GivenControlRegisterValueDoesNotExistInDeviceStateRegisterCache()
        {
            _deviceState.ControlRegister = null;
        }

        private void GivenControlRegisterValueInDeviceMemory(ControlRegisterBits controlRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns((ushort) controlRegisterBits);
        }

        private void WhenReadbackControlRegister()
        {
            _resultOfReadback = _readbackControlRegisterCommand.ReadbackControlRegister();
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned()
        {
            _deviceState.ControlRegister.Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.SelectRegisterForReadback,
                        It.Is<ushort>(y => y == (ushort) AddressCodesForDataReadback.ControlRegister)),
                    Times.Once);

            Mock.Get(_fakeReadSPICommand)
                .Verify(x => x.ReadSPI(), Times.Once);
        }

        private void ThenTheValueShouldBeTheReadableBitsPartOfTheValueOnTheDevice()
        {
            ((byte) (_resultOfReadback & ~ControlRegisterBits.ReadableBits)).Should().Be(0);
        }

        private void ThenTheValueReadShouldBeCached()
        {
            _deviceState.ControlRegister.Should().Be(_resultOfReadback);
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

            _readbackControlRegisterCommand = new ReadbackControlRegisterCommand(_fakeEvalBoard,
                _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackControlRegister _readbackControlRegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ControlRegisterBits _resultOfReadback;
    }
}