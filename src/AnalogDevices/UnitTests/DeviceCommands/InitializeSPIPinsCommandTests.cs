using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class InitializeSPIPinsCommandTests
    {
        [Test]
        public void ShouldInitializeSPIPins()
        {
            GivenSPIPinsAreNotInitialized();
            WhenInitializeSPIPins();
            ThenSPIPinsAreInitialized();
        }

        private void GivenSPIPinsAreNotInitialized()
        {
            _deviceState.SPI_Initialized = false;
        }

        private void WhenInitializeSPIPins()
        {
            _initializeSPIPinsCommand.InitializeSPIPins();
        }

        private void ThenSPIPinsAreInitialized()
        {
            Mock.Get(_fakeSendDeviceCommandCommand)
                .Verify(x => x.SendDeviceCommand(DeviceCommand.InitializeSPIPins, It.IsAny<uint>()));

            _deviceState.SPI_Initialized.Should().BeTrue();
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendDeviceCommandCommand = Mock.Of<ISendDeviceCommand>();
            _initializeSPIPinsCommand = new InitializeSPIPinsCommand(_fakeEvalBoard, _fakeSendDeviceCommandCommand);
        }

        private IInitializeSPIPins _initializeSPIPinsCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private ISendDeviceCommand _fakeSendDeviceCommandCommand;
        private DeviceState _deviceState;
    }
}