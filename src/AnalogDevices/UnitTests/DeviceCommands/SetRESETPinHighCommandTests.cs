using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetRESETPinHighCommandTests
    {
        [Test]
        public void ShouldSendAppropriateDeviceCommand()
        {
            WhenSetRESETPinHigh();
            ThenCorrespondingDeviceCommandShouldBeSent();
        }

        private void WhenSetRESETPinHigh()
        {
            _setRESETPinHighCommand.SetRESETPinHigh();
        }

        private void ThenCorrespondingDeviceCommandShouldBeSent()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x =>
                        x.SendDeviceCommand(
                            DeviceCommand.SetRESETPinHigh,
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

            _setRESETPinHighCommand = new SetRESETPinHighCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private ISetRESETPinHigh _setRESETPinHighCommand;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}