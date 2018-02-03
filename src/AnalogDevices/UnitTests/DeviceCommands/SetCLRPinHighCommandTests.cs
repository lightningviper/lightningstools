using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetCLRPinHighCommandTests
    {
        [Test]
        public void ShouldSendAppropriateDeviceCommand()
        {
            WhenSetCLRPinHigh();
            ThenCorrespondingDeviceCommandShouldBeSent();
        }

        private void WhenSetCLRPinHigh()
        {
            _setCLRPinHighCommand.SetCLRPinHigh();
        }

        private void ThenCorrespondingDeviceCommandShouldBeSent()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x =>
                        x.SendDeviceCommand(
                            DeviceCommand.SetCLRPinHigh,
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

            _setCLRPinHighCommand = new SetCLRPinHighCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private ISetCLRPinHigh _setCLRPinHighCommand;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}