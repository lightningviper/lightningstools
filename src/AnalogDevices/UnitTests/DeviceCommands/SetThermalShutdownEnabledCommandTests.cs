using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetThermalShutdownEnabledCommandTests
    {
        [Test]
        public void ShouldSetThermalShutdownEnabled(
            [Range(byte.MinValue, byte.MaxValue)] byte controlRegisterBits)
        {
            GivenControlRegisterBits((ControlRegisterBits) controlRegisterBits);
            WhenSetThermalShutdownEnabled(true);
            ThenControlRegisterIsUpdatedWithThermalShutdownEnabledFlagSet();
        }

        [Test]
        public void ShouldSetThermalShutdownDisabled(
            [Range(byte.MinValue, byte.MaxValue)] byte controlRegisterBits)
        {
            GivenControlRegisterBits((ControlRegisterBits) controlRegisterBits);
            WhenSetThermalShutdownEnabled(false);
            ThenControlRegisterIsUpdatedWithThermalShutdownEnabledFlagCleared();
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _originalControlRegisterBits = controlRegisterBits & ControlRegisterBits.ReadableBits;

            Mock.Get(_fakeControlRegisterReader)
                .Setup(x => x.ReadbackControlRegister())
                .Returns(_originalControlRegisterBits);
        }

        private void WhenSetThermalShutdownEnabled(bool value)
        {
            _setThermalShutdownEnabledCommand.SetThermalShutdownEnabled(value);
        }

        private void ThenControlRegisterIsUpdatedWithThermalShutdownEnabledFlagCleared()
        {
            _newControlRegisterBits.Should()
                .Be(_originalControlRegisterBits & ControlRegisterBits.WritableBits &
                    ~ControlRegisterBits.ThermalShutdownEnabled);
        }

        private void ThenControlRegisterIsUpdatedWithThermalShutdownEnabledFlagSet()
        {
            _newControlRegisterBits.Should()
                .Be((_originalControlRegisterBits & ControlRegisterBits.WritableBits) |
                    ControlRegisterBits.ThermalShutdownEnabled);
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

            _setThermalShutdownEnabledCommand = new SetThermalShutdownEnabledCommand(_fakeEvalBoard,
                _fakeControlRegisterReader, _fakeControlRegisterWriter);
        }

        private ISetThermalShutdownEnabled _setThermalShutdownEnabledCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IReadbackControlRegister _fakeControlRegisterReader;
        private IWriteControlRegister _fakeControlRegisterWriter;
        private DeviceState _deviceState;
        private ControlRegisterBits _originalControlRegisterBits;
        private ControlRegisterBits _newControlRegisterBits;
    }
}