using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class WriteOFS1RegisterCommandTests
    {
        [Test]
        [TestCase(null, ushort.MinValue)]
        [TestCase(null, ushort.MaxValue)]
        [TestCase(ushort.MinValue, ushort.MaxValue)]
        [TestCase(ushort.MaxValue, ushort.MinValue)]
        public void ShouldInvokeSendSpecialFunctionCommandWithNewOFS1RegisterBits(
            ushort? originalOFS1RegisterBits,
            ushort newOFS1RegisterBits)
        {
            GivenOriginalOFS1RegisterBits(originalOFS1RegisterBits);
            WhenWriteOFS1Register(newOFS1RegisterBits);
            ThenSendSpecialFunctionCommandWasInvokedWith(
                (ushort) (newOFS1RegisterBits & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        [TestCase(null, ushort.MinValue)]
        [TestCase(null, ushort.MaxValue)]
        [TestCase(ushort.MinValue, ushort.MaxValue)]
        [TestCase(ushort.MaxValue, ushort.MinValue)]
        [TestCase(ushort.MinValue, ushort.MinValue)]
        [TestCase(ushort.MaxValue, ushort.MaxValue)]
        public void ShouldUpdateDeviceState(
            ushort? originalOFS1RegisterBits,
            ushort newOFS1RegisterBits)
        {
            GivenOriginalOFS1RegisterBits(originalOFS1RegisterBits);
            WhenWriteOFS1Register(newOFS1RegisterBits);
            ThenDeviceStateWasUpdatedWithNewOFS1RegisterValues(
                (ushort) (newOFS1RegisterBits & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        [TestCase(ushort.MinValue, ushort.MinValue)]
        [TestCase(ushort.MaxValue, ushort.MaxValue)]
        public void ShouldNotInvokeSendSpecialFunctionCommandWhenNewValueWouldBeSameAsExistingValueInDevice(
            ushort? originalOFS1RegisterBits,
            ushort newOFS1RegisterBits)
        {
            GivenOriginalOFS1RegisterBits(originalOFS1RegisterBits);
            WhenWriteOFS1Register(newOFS1RegisterBits);
            ThenSendSpecialFunctionCommandWasNotInvoked();
        }

        private void GivenOriginalOFS1RegisterBits(ushort? originalOFS1RegisterBits)
        {
            _deviceState.OFS1Register = (ushort?) (originalOFS1RegisterBits & (ushort) BasicMasks.FourteenBits);
        }

        private void WhenWriteOFS1Register(ushort ofs1RegisterBits)
        {
            _writeOFS1RegisterCommand.WriteOFS1Register(ofs1RegisterBits);
        }

        private void ThenSendSpecialFunctionCommandWasInvokedWith(ushort ofs1RegisterBits)
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                        SpecialFunctionCode.WriteOFS1Register,
                        It.Is<ushort>(a => a == ofs1RegisterBits))
                    , Times.Once);
        }

        private void ThenDeviceStateWasUpdatedWithNewOFS1RegisterValues(ushort newOFS1RegisterBits)
        {
            _deviceState.OFS1Register.Should().Be(newOFS1RegisterBits);
        }

        private void ThenSendSpecialFunctionCommandWasNotInvoked()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(SpecialFunctionCode.WriteOFS1Register, It.IsAny<ushort>())
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

            _writeOFS1RegisterCommand = new WriteOFS1RegisterCommand(_fakeEvalBoard, _fakeSendSpecialFunctionCommand);
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private IWriteOFS1Register _writeOFS1RegisterCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
    }
}