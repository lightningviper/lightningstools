using AnalogDevices.DeviceCommands;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetCLRPinLowCommandTests
    {
        [Test]
        public void ShouldSendAppropriateDeviceCommand()
        {
            WhenSetCLRPinLow();
            ThenCorrespondingDeviceCommandShouldBeSent();
        }

        private void WhenSetCLRPinLow()
        {
            _setCLRPinLowCommand.SetCLRPinLow();
        }

        private void ThenCorrespondingDeviceCommandShouldBeSent()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x =>
                        x.SendDeviceCommand(
                            DeviceCommand.SetCLRPinLow,
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

            _setCLRPinLowCommand = new SetCLRPinLowCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private ISetCLRPinLow _setCLRPinLowCommand;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}