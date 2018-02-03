using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class WriteControlRegisterCommandTests
    {
        [Test]
        [TestCase(null, (byte) ControlRegisterBits.ReadableBits)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, byte.MinValue)]
        [TestCase(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)]
        public void ShouldInvokeSendSpecialFunctionCommandWithNewControlRegisterBits(
            byte? originalControlRegisterBits,
            byte newControlRegisterBits)
        {
            GivenOriginalControlRegisterBits((ControlRegisterBits?) originalControlRegisterBits);
            WhenWriteControlRegister((ControlRegisterBits) newControlRegisterBits);
            ThenSendSpecialFunctionCommandWasInvokedWith((ControlRegisterBits) newControlRegisterBits &
                                                         ControlRegisterBits.WritableBits);
        }

        [Test]
        [TestCase(byte.MinValue, byte.MinValue)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, (byte) ControlRegisterBits.ReadableBits)]
        [TestCase((byte) ControlRegisterBits.WritableBits, (byte) ControlRegisterBits.WritableBits)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, (byte) ControlRegisterBits.WritableBits)]
        [TestCase(byte.MaxValue, byte.MaxValue)]
        public void ShouldNotInvokeSendSpecialFunctionCommandWhenNewValueWouldBeSameAsExistingValueInDevice(
            byte? originalControlRegisterBits,
            byte newControlRegisterBits)
        {
            GivenOriginalControlRegisterBits((ControlRegisterBits?) originalControlRegisterBits);
            WhenWriteControlRegister((ControlRegisterBits) newControlRegisterBits);
            ThenSendSpecialFunctionCommandWasNotInvoked();
        }

        [Test]
        [TestCase(null, (byte) ControlRegisterBits.ReadableBits)]
        [TestCase((byte) ControlRegisterBits.ReadableBits, byte.MinValue)]
        [TestCase(byte.MinValue, (byte) ControlRegisterBits.ReadableBits)]
        public void ShouldUpdateDeviceState(
            byte? originalControlRegisterBits,
            byte newControlRegisterBits)
        {
            GivenOriginalControlRegisterBits((ControlRegisterBits?) originalControlRegisterBits);
            WhenWriteControlRegister((ControlRegisterBits) newControlRegisterBits);
            ThenDeviceStateWasUpdatedWithNewControlRegisterValues(
                (ControlRegisterBits) newControlRegisterBits & ControlRegisterBits.WritableBits);
        }

        private void GivenOriginalControlRegisterBits(ControlRegisterBits? originalControlRegisterBits)
        {
            _originalControlRegisterBits = originalControlRegisterBits;
            _deviceState.ControlRegister = originalControlRegisterBits;
        }

        private void WhenWriteControlRegister(ControlRegisterBits controlRegisterBits)
        {
            _writeControlRegisterCommand.WriteControlRegister(controlRegisterBits);
        }

        private void ThenSendSpecialFunctionCommandWasInvokedWith(ControlRegisterBits controlRegisterBits)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.WriteControlRegister,
                        It.Is<ushort>(a => a == (ushort) controlRegisterBits))
                    , Times.Once);
        }

        private void ThenDeviceStateWasUpdatedWithNewControlRegisterValues(ControlRegisterBits newControlRegisterBits)
        {
            _deviceState.ControlRegister.Should()
                .Be(
                    (_originalControlRegisterBits & ControlRegisterBits.ReadableBits &
                     ~ControlRegisterBits.WritableBits) | (newControlRegisterBits & ControlRegisterBits.WritableBits));
        }

        private void ThenSendSpecialFunctionCommandWasNotInvoked()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(SpecialFunctionCode.WriteControlRegister, It.IsAny<ushort>())
                    , Times.Never);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState {UseRegisterCache = true};
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Setup(x => x.SendSpecialFunction(It.IsAny<SpecialFunctionCode>(), It.IsAny<ushort>()))
                .Verifiable();

            _writeControlRegisterCommand =
                new WriteControlRegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand);
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IWriteControlRegister _writeControlRegisterCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private ControlRegisterBits? _originalControlRegisterBits;
    }
}