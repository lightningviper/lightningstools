using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetPacketErrorCheckErrorOccurredCommandTests
    {
        [Test]
        [TestCase(byte.MinValue, false)]
        [TestCase((byte) ControlRegisterBits.WritableBits, false)]
        [TestCase((byte) ControlRegisterBits.PacketErrorCheckErrorOccurred, true)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, true)]
        [TestCase(byte.MaxValue, true)]
        [TestCase(byte.MaxValue & ~(byte) ControlRegisterBits.PacketErrorCheckErrorOccurred, false)]
        public void Test(byte controlRegisterBits, bool expectedResult)
        {
            GivenControlRegisterBits((ControlRegisterBits) controlRegisterBits);
            WhenGetPacketErrorCheckErrorOccurred();
            ThenResultShouldBe(expectedResult);
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _deviceState.ControlRegister = controlRegisterBits;

            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackControlRegister())
                .Returns(controlRegisterBits);
        }

        private void WhenGetPacketErrorCheckErrorOccurred()
        {
            _result = _getPacketErrorCheckErrorOccurredStatusCommand.GetPacketErrorCheckErrorOccurredStatus();
        }

        private void ThenResultShouldBe(bool expectedResult)
        {
            _result.Should().Be(expectedResult);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeRb = Mock.Of<IReadbackControlRegister>();

            _getPacketErrorCheckErrorOccurredStatusCommand
                = new GetPacketErrorCheckErrorOccurredStatusCommand(_fakeEvalBoard, _fakeRb);

            _result = false;
        }

        private IGetPacketErrorCheckErrorOccurredStatus _getPacketErrorCheckErrorOccurredStatusCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IReadbackControlRegister _fakeRb;
        private DeviceState _deviceState;
        private bool _result;
    }
}