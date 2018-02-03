using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadSPICommandTests
    {
        [Test]
        public void ShouldInitializeSPIPinsWhenNotAlreadyInitialized()
        {
            GivenSPIPinsHaveNotBeenInitialized();
            WhenReadSPI();
            ThenSPIPinsShouldBeInitialized();
        }

        [Test]
        public void ShouldNotInitializeSPIPinsWhenAlreadyInitialized()
        {
            GivenSPIPinsHaveAlreadyBeenInitialized();
            WhenReadSPI();
            ThenSPIPinsShouldNotBeInitialized();
        }

        [Test]
        [TestCase(new byte[] {0xFF, 0x00, 0xAA})]
        public void ShouldPerformUsbControlTransferCorrectly(byte[] pendingSPIData)
        {
            GivenPendingSPIDataOnDevice(pendingSPIData);
            WhenReadSPI();
            ThenUsbControlTransferOccurredCorrectly();
        }

        private void GivenPendingSPIDataOnDevice(byte[] pendingSPIData)
        {
            _pendingSPIData = pendingSPIData;
        }

        private void GivenSPIPinsHaveNotBeenInitialized()
        {
            _deviceState.SPI_Initialized = false;
        }

        private void GivenSPIPinsHaveAlreadyBeenInitialized()
        {
            _deviceState.SPI_Initialized = true;
        }

        private void WhenReadSPI()
        {
            _valueReturnedFromReadSPI = _readSPICommand.ReadSPI();
        }

        private void ThenSPIPinsShouldBeInitialized()
        {
            Mock.Get(_fakeInitializeSPIPinsCommand)
                .Verify(x => x.InitializeSPIPins(), Times.Once);
        }

        private void ThenSPIPinsShouldNotBeInitialized()
        {
            Mock.Get(_fakeInitializeSPIPinsCommand)
                .Verify(x => x.InitializeSPIPins(), Times.Never);
        }

        private void ThenUsbControlTransferOccurredCorrectly()
        {
            Mock.Get(_fakeUSBControlTransferCommand)
                .Verify(x => x.UsbControlTransfer(
                    It.IsAny<byte>(),
                    It.IsAny<byte>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<int?>()), Times.Once);

            _controlTransferArgs.Should().NotBeNull();
            _controlTransferArgs.Length.Should().Be(3);
            _controlTransferArgs.RequestType.Should()
                .Be((byte) UsbRequestType.TypeVendor | (byte) UsbCtrlFlags.Direction_In);
            _controlTransferArgs.Request.Should().Be((byte) DeviceCommand.SendSPI);
            _controlTransferArgs.Index.Should().Be(0);
            _controlTransferArgs.Value.Should().Be(0);
            _controlTransferArgs.Buffer.Should().NotBeNull();
            _controlTransferArgs.Buffer.Length.Should().Be(3);
            _valueReturnedFromReadSPI.Should()
                .Be((ushort) (_controlTransferArgs.Buffer[0] | (_controlTransferArgs.Buffer[1] << 8)));
        }

        [SetUp]
        public void SetUp()
        {
            _deviceState = new DeviceState();
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeInitializeSPIPinsCommand = Mock.Of<IInitializeSPIPins>();
            Mock.Get(_fakeInitializeSPIPinsCommand)
                .Setup(x => x.InitializeSPIPins())
                .Verifiable();

            _fakeUSBControlTransferCommand = Mock.Of<IUSBControlTransfer>();
            Mock.Get(_fakeUSBControlTransferCommand)
                .Setup(x => x.UsbControlTransfer(
                    It.IsAny<byte>(),
                    It.IsAny<byte>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<int?>())
                )
                .Callback<byte, byte, int, int, byte[], int?>(
                    (a, b, c, d, e, f) =>
                    {
                        _controlTransferArgs = new ControlTransferSentEventArgs(a, b, c, d, e, f);

                        if (_pendingSPIData != null)
                        {
                            Array.Copy(_pendingSPIData, _controlTransferArgs.Buffer, _controlTransferArgs.Length ?? 0);
                        }
                    })
                .Verifiable();

            _readSPICommand = new ReadSPICommand(_fakeEvalBoard, _fakeUSBControlTransferCommand,
                _fakeInitializeSPIPinsCommand);

            _pendingSPIData = new byte[3];
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IInitializeSPIPins _fakeInitializeSPIPinsCommand;
        private IUSBControlTransfer _fakeUSBControlTransferCommand;
        private IReadSPI _readSPICommand;
        private ControlTransferSentEventArgs _controlTransferArgs;
        private byte[] _pendingSPIData;
        private ushort _valueReturnedFromReadSPI;
    }
}