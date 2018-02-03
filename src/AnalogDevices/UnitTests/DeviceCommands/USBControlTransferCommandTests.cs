using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class USBControlTransferCommandTests
    {
        [Test]
        [TestCase(byte.MaxValue, byte.MaxValue, int.MaxValue, int.MaxValue, new byte[] {0, 1, 2}, 3)]
        [TestCase(byte.MinValue, byte.MinValue, int.MinValue, int.MinValue, null, null)]
        public void ShouldCallUsbControlTransferOnUnderlyingUsbDevice(
            byte requestType,
            byte request,
            int value,
            int index,
            byte[] buffer = null,
            int? length = null)
        {
            WhenUsbControlTransfer(requestType, request, value, index, buffer, length);
            ThenUsbControlTransferWasCalledOnUnderlyingUsbDevice();
        }

        private void WhenUsbControlTransfer(
            byte requestType,
            byte request,
            int value,
            int index,
            byte[] buffer = null,
            int? length = null)
        {
            Mock.Get(_fakeUsbDevice)
                .Setup(x => x.ControlTransfer(
                    requestType,
                    request,
                    value,
                    index,
                    buffer,
                    length))
                .Callback(() => _wasCalled = true);

            _usbControlTransferCommand.UsbControlTransfer(
                requestType,
                request,
                value,
                index,
                buffer,
                length);
        }

        private void ThenUsbControlTransferWasCalledOnUnderlyingUsbDevice()
        {
            _wasCalled.Should().BeTrue();
        }

        [SetUp]
        public void SetUp()
        {
            _fakeUsbDevice = Mock.Of<IUsbDevice>();

            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.UsbDevice)
                .Returns(_fakeUsbDevice);

            _usbControlTransferCommand = new USBControlTransferCommand(_fakeEvalBoard);
        }

        private bool _wasCalled;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private IUsbDevice _fakeUsbDevice;
        private IUSBControlTransfer _usbControlTransferCommand;
    }
}