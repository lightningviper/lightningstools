using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetThermalShutdownEnabledCommandTests
    {
        [Test]
        [TestCase(byte.MinValue, false)]
        [TestCase((byte) ControlRegisterBits.WritableBits, true)]
        [TestCase((byte) ControlRegisterBits.ThermalShutdownEnabled, true)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, true)]
        [TestCase(byte.MaxValue, true)]
        [TestCase(byte.MaxValue & ~(byte) ControlRegisterBits.ThermalShutdownEnabled, false)]
        public void Test(byte controlRegisterBits, bool expectedResult)
        {
            GivenControlRegisterBits((ControlRegisterBits) controlRegisterBits);
            WhenGetThermalShutdownEnabled();
            ThenResultShouldBe(expectedResult);
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _deviceState.ControlRegister = controlRegisterBits;

            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackControlRegister())
                .Returns(controlRegisterBits);
        }

        private void WhenGetThermalShutdownEnabled()
        {
            _result = _GetThermalShutdownEnabledStatusCommand.GetThermalShutdownEnabled();
        }

        private void ThenResultShouldBe(bool expectedResult)
        {
            _result.Should().Be(expectedResult);
        }

        [SetUp]
        public void SetUp()
        {
            _GetThermalShutdownEnabledStatusCommand
                = new GetThermalShutdownEnabledCommand(_fakeEvalBoard, _fakeRb);

            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);
        }

        private IGetThermalShutdownEnabled _GetThermalShutdownEnabledStatusCommand;
        private readonly IDenseDacEvalBoard _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
        private readonly IReadbackControlRegister _fakeRb = Mock.Of<IReadbackControlRegister>();
        private readonly DeviceState _deviceState = new DeviceState();
        private bool _result;
    }
}