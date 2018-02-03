using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetLDACPinLowCommandTests
    {
        [Test]
        public void ShouldSendAppropriateDeviceCommand()
        {
            WhenSetLDACPinLow();
            ThenCorrespondingDeviceCommandShouldBeSent();
        }

        private void WhenSetLDACPinLow()
        {
            _setLDACPinLowCommand.SetLDACPinLow();
        }

        private void ThenCorrespondingDeviceCommandShouldBeSent()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x =>
                        x.SendDeviceCommand(
                            DeviceCommand.SetLDACPinLow,
                            0),
                    Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();

            _fakeSendDeviceCommandCommand = Mock.Of<ISendDeviceCommand>();
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Setup(x => x.SendDeviceCommand(
                    It.IsAny<DeviceCommand>(),
                    It.IsAny<uint>()))
                .Verifiable();

            _setLDACPinLowCommand = new SetLDACPinLowCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private ISetLDACPinLow _setLDACPinLowCommand;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}