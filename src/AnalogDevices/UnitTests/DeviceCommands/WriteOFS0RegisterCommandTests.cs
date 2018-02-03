using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class WriteOFS0RegisterCommandTests
    {
        [Test]
        [TestCase(null, ushort.MinValue)]
        [TestCase(null, ushort.MaxValue)]
        [TestCase(ushort.MinValue, ushort.MaxValue)]
        [TestCase(ushort.MaxValue, ushort.MinValue)]
        public void ShouldInvokeSendSpecialFunctionCommandWithNewOFS0RegisterBits(
            ushort? originalOFS0RegisterBits,
            ushort newOFS0RegisterBits)
        {
            GivenOriginalOFS0RegisterBits(originalOFS0RegisterBits);
            WhenWriteOFS0Register(newOFS0RegisterBits);
            ThenSendSpecialFunctionCommandWasInvokedWith(
                (ushort) (newOFS0RegisterBits & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        [TestCase(null, ushort.MinValue)]
        [TestCase(null, ushort.MaxValue)]
        [TestCase(ushort.MinValue, ushort.MaxValue)]
        [TestCase(ushort.MaxValue, ushort.MinValue)]
        [TestCase(ushort.MinValue, ushort.MinValue)]
        [TestCase(ushort.MaxValue, ushort.MaxValue)]
        public void ShouldUpdateDeviceState(
            ushort? originalOFS0RegisterBits,
            ushort newOFS0RegisterBits)
        {
            GivenOriginalOFS0RegisterBits(originalOFS0RegisterBits);
            WhenWriteOFS0Register(newOFS0RegisterBits);
            ThenDeviceStateWasUpdatedWithNewOFS0RegisterValues(
                (ushort) (newOFS0RegisterBits & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        [TestCase(ushort.MinValue, ushort.MinValue)]
        [TestCase(ushort.MaxValue, ushort.MaxValue)]
        public void ShouldNotInvokeSendSpecialFunctionCommandWhenNewValueWouldBeSameAsExistingValueInDevice(
            ushort? originalOFS0RegisterBits,
            ushort newOFS0RegisterBits)
        {
            GivenOriginalOFS0RegisterBits(originalOFS0RegisterBits);
            WhenWriteOFS0Register(newOFS0RegisterBits);
            ThenSendSpecialFunctionCommandWasNotInvoked();
        }

        private void GivenOriginalOFS0RegisterBits(ushort? originalOFS0RegisterBits)
        {
            _deviceState.OFS0Register = (ushort?) (originalOFS0RegisterBits & (ushort) BasicMasks.FourteenBits);
        }

        private void WhenWriteOFS0Register(ushort ofs0RegisterBits)
        {
            _writeOFS0RegisterCommand.WriteOFS0Register(ofs0RegisterBits);
        }

        private void ThenSendSpecialFunctionCommandWasInvokedWith(ushort ofs0RegisterBits)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.WriteOFS0Register,
                        It.Is<ushort>(a => a == ofs0RegisterBits))
                    , Times.Once);
        }

        private void ThenDeviceStateWasUpdatedWithNewOFS0RegisterValues(ushort newOFS0RegisterBits)
        {
            _deviceState.OFS0Register.Should().Be(newOFS0RegisterBits);
        }

        private void ThenSendSpecialFunctionCommandWasNotInvoked()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(SpecialFunctionCode.WriteOFS0Register, It.IsAny<ushort>())
                    , Times.Never);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);
            _deviceState.UseRegisterCache = true;

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Setup(x => x.SendSpecialFunction(It.IsAny<SpecialFunctionCode>(), It.IsAny<ushort>()))
                .Verifiable();

            _writeOFS0RegisterCommand = new WriteOFS0RegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand);
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IWriteOFS0Register _writeOFS0RegisterCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
    }
}