using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class PerformSoftPowerDownCommandTests
    {
        [Test]
        public void ShouldPerformSoftPowerDown()
        {
            GivenControlRegisterBits(ControlRegisterBits.WritableBits & ~ControlRegisterBits.SoftPowerDown);
            WhenPerformSoftPowerDown();
            ThenControlRegisterIsUpdatedWithSoftPowerDownFlagEnabled();
        }

        [Test]
        public void DeviceStateCacheShouldBeCleared()
        {
            GivenControlRegisterBitsExistInDeviceStateCache();
            WhenPerformSoftPowerDown();
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

        private void WhenPerformSoftPowerDown()
        {
            _performSoftPowerDownCommand.PerformSoftPowerDown();
        }

        private void ThenControlRegisterIsUpdatedWithSoftPowerDownFlagEnabled()
        {
            var differenceInControlRegisterBits = _newControlRegisterBits & ~_originalControlRegisterBits;
            differenceInControlRegisterBits.Should().Be(ControlRegisterBits.SoftPowerDown);
        }

        private void ThenDeviceStateCacheIsCleared()
        {
            _deviceState.ControlRegister.HasValue.Should().BeFalse();
        }

        [SetUp]
        public void SetUp()
        {
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            Mock.Get(_fakeControlRegisterWriter)
                .Setup(x => x.WriteControlRegister(It.IsAny<ControlRegisterBits>()))
                .Callback<ControlRegisterBits>(x => _newControlRegisterBits = x)
                .Verifiable();

            _performSoftPowerDownCommand = new PerformSoftPowerDownCommand(_fakeEvalBoard, _fakeControlRegisterReader,
                _fakeControlRegisterWriter);
        }

        private IPerformSoftPowerDown _performSoftPowerDownCommand;
        private readonly IDenseDacEvalBoard _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
        private readonly IReadbackControlRegister _fakeControlRegisterReader = Mock.Of<IReadbackControlRegister>();
        private readonly IWriteControlRegister _fakeControlRegisterWriter = Mock.Of<IWriteControlRegister>();
        private readonly DeviceState _deviceState = new DeviceState();
        private ControlRegisterBits _originalControlRegisterBits;
        private ControlRegisterBits _newControlRegisterBits;
    }
}