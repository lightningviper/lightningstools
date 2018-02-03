using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetLDACPinHighCommandTests
    {
        [Test]
        public void ShouldSendAppropriateDeviceCommand()
        {
            WhenSetLDACPinHigh();
            ThenCorrespondingDeviceCommandShouldBeSent();
        }

        private void WhenSetLDACPinHigh()
        {
            _setLDACPinHighCommand.SetLDACPinHigh();
        }

        private void ThenCorrespondingDeviceCommandShouldBeSent()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x =>
                        x.SendDeviceCommand(
                            DeviceCommand.SetLDACPinHigh,
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

            _setLDACPinHighCommand = new SetLDACPinHighCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private ISetLDACPinHigh _setLDACPinHighCommand;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}