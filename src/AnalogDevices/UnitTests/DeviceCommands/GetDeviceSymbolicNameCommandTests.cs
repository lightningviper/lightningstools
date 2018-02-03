using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetDeviceSymbolicNameCommandTests
    {
        [Test]
        [TestCase("Some device symbolic name")]
        public void ShouldReturnDeviceSymbolicName(string deviceSymbolicName)
        {
            GivenDeviceSymbolicNameIs(deviceSymbolicName);
            WhenGetDeviceSymbolicName();
            ThenReturnedValueShouldBe(deviceSymbolicName);
        }

        private void GivenDeviceSymbolicNameIs(string symbolicName)
        {
            Mock.Get(_fakeUsbDevice)
                .SetupGet(x => x.SymbolicName)
                .Returns(symbolicName);
        }

        private void WhenGetDeviceSymbolicName()
        {
            _deviceSymbolicName = _getDeviceSymbolicNameCommand.GetDeviceSymbolicName();
        }

        private void ThenReturnedValueShouldBe(string expected)
        {
            _deviceSymbolicName.Should().Be(expected);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeUsbDevice = Mock.Of<IUsbDevice>();

            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.UsbDevice)
                .Returns(_fakeUsbDevice);

            _getDeviceSymbolicNameCommand = new GetDeviceSymbolicNameCommand(_fakeEvalBoard);
        }

        private IGetDeviceSymbolicName _getDeviceSymbolicNameCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IUsbDevice _fakeUsbDevice;
        private string _deviceSymbolicName;
    }
}