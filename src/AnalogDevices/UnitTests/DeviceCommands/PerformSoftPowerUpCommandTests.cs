using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class PerformSoftPowerUpCommandTests
    {
        [Test]
        public void ShouldPerformSoftPowerUp()
        {
            GivenControlRegisterBits(ControlRegisterBits.SoftPowerDown);
            WhenPerformSoftPowerUp();
            ThenControlRegisterIsUpdatedWithSoftPowerDownFlagCleared();
        }

        [Test]
        public void DeviceStateCacheShouldBeCleared()
        {
            GivenControlRegisterBitsExistInDeviceStateCache();
            WhenPerformSoftPowerUp();
            ThenDeviceStateCacheIsCleared();
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _originalControlRegisterBits = controlRegisterBits;

            Mock.Get(_fakeControlRegisterReader)
                .Setup(x => x.ReadbackControlRegister())
                .Returns(controlRegisterBits);
        }

        private void GivenControlRegisterBitsExistInDeviceStateCache()
        {
            _deviceState.ControlRegister = byte.MinValue;
        }

        private void WhenPerformSoftPowerUp()
        {
            _performSoftPowerUpCommand.PerformSoftPowerUp();
        }

        private void ThenControlRegisterIsUpdatedWithSoftPowerDownFlagCleared()
        {
            var differenceInControlRegisterBits = _originalControlRegisterBits & ~_newControlRegisterBits;
            differenceInControlRegisterBits.Should().Be(ControlRegisterBits.SoftPowerDown);
        }

        private void ThenDeviceStateCacheIsCleared()
        {
            _deviceState.ControlRegister.HasValue.Should().BeFalse();
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeControlRegisterReader = Mock.Of<IReadbackControlRegister>();

            _fakeControlRegisterWriter = Mock.Of<IWriteControlRegister>();
            Mock.Get(_fakeControlRegisterWriter)
                .Setup(x => x.WriteControlRegister(It.IsAny<ControlRegisterBits>()))
                .Callback<ControlRegisterBits>(x => _newControlRegisterBits = x)
                .Verifiable();

            _performSoftPowerUpCommand = new PerformSoftPowerUpCommand(_fakeEvalBoard, _fakeControlRegisterReader,
                _fakeControlRegisterWriter);
        }

        private IPerformSoftPowerUp _performSoftPowerUpCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IReadbackControlRegister _fakeControlRegisterReader;
        private IWriteControlRegister _fakeControlRegisterWriter;
        private DeviceState _deviceState;
        private ControlRegisterBits _originalControlRegisterBits;
        private ControlRegisterBits _newControlRegisterBits;
    }
}