using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class PulseLDACPinCommandTests
    {
        [Test]
        public void ShouldSendCommandToPulseLDACPin()
        {
            WhenPulseLDACPin();
            ThenCommandWasSentToDeviceToPulseLDACPin();
        }

        private void WhenPulseLDACPin()
        {
            _pulseLDACPinCommand.PulseLDACPin();
        }

        private void ThenCommandWasSentToDeviceToPulseLDACPin()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x => x.SendDeviceCommand(DeviceCommand.PulseLDACPin, It.IsAny<uint>()), Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();

            _fakeSendDeviceCommandCommand = Mock.Of<ISendDeviceCommand>();

            Mock.Get(_fakeSendDeviceCommandCommand)
                .Setup(x => x.SendDeviceCommand(DeviceCommand.PulseLDACPin, It.IsAny<uint>()))
                .Verifiable();

            _pulseLDACPinCommand = new PulseLDACPinCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private IPulseLDACPin _pulseLDACPinCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
    }
}