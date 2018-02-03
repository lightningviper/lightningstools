using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetIsOverTemperatureCommandTests
    {
        [Test]
        [TestCase(byte.MinValue, false)]
        [TestCase((byte) ControlRegisterBits.OverTemperature, true)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, true)]
        [TestCase((byte) ControlRegisterBits.WritableBits, false)]
        [TestCase(byte.MaxValue, true)]
        [TestCase(byte.MaxValue & ~(byte) ControlRegisterBits.OverTemperature, false)]
        public void ShouldReturnTrueIfOverTemperatureBitIsTrueInControlRegisterBits(byte controlRegisterBits,
            bool expectedValue)
        {
            GivenControlRegisterBits((ControlRegisterBits) controlRegisterBits);
            WhenGetIsOverTemperature();
            ThenValueShouldBe(expectedValue);
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _deviceState.ControlRegister = controlRegisterBits;
            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackControlRegister())
                .Returns(controlRegisterBits);
        }

        private void WhenGetIsOverTemperature()
        {
            _isOverTemperature = _getIsOverTemperatureCommand.GetIsOverTemperature();
        }

        private void ThenValueShouldBe(bool expected)
        {
            _isOverTemperature.Should().Be(expected);
        }

        [SetUp]
        public void SetUp()
        {
            _getIsOverTemperatureCommand = new GetIsOverTemperatureCommand(_fakeEvalBoard, _fakeRb);

            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);
        }

        private IGetIsOverTemperature _getIsOverTemperatureCommand;
        private readonly IDenseDacEvalBoard _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
        private readonly IReadbackControlRegister _fakeRb = Mock.Of<IReadbackControlRegister>();
        private readonly DeviceState _deviceState = new DeviceState();
        private bool _isOverTemperature;
    }
}