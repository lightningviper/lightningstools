using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetOffsetDAC1CommandTests
    {
        [Test]
        public void ShouldSetClrPinLowBeforeSettingOffsetDACValue()
        {
            WhenSetOffsetDAC1(SomeValue);
            ThenClrPinWasSetLowBeforeSetOffsetDAC1WasCalled();
        }

        [Test]
        public void ShouldInvokeSetOFS1RegisterCommand(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int value)
        {
            WhenSetOffsetDAC1((ushort) value);
            ThenWriteOFS1RegisterCommandWasInvoked((ushort) (value & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        public void ShouldSetClrPinHighAfterSettingOffsetDACValue()
        {
            WhenSetOffsetDAC1(SomeValue);
            ThenClrPinWasSetHighAfterSetOffsetDAC1WasCalled();
        }

        private void WhenSetOffsetDAC1(ushort value)
        {
            _setOffsetDAC1Command.SetOffsetDAC1(value);
        }

        private void ThenClrPinWasSetHighAfterSetOffsetDAC1WasCalled()
        {
            Mock.Get(_fakeSetCLRPinHighCommand)
                .Verify(x => x.SetCLRPinHigh(), Times.Once);

            _setCLRPinHighCallOrder.Should().BeGreaterThan(_writeOFS1RegisterCallOrder);
        }

        private void ThenWriteOFS1RegisterCommandWasInvoked(ushort value)
        {
            Mock.Get(_fakeWriteOFS1RegisterCommand)
                .Verify(x => x.WriteOFS1Register(value));
        }

        private void ThenClrPinWasSetLowBeforeSetOffsetDAC1WasCalled()
        {
            Mock.Get(_fakeSetCLRPinLowCommand)
                .Verify(x => x.SetCLRPinLow(), Times.Once);

            _setCLRPinLowCallOrder.Should().BeLessThan(_writeOFS1RegisterCallOrder);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);


            _fakeSetCLRPinHighCommand = Mock.Of<ISetCLRPinHigh>();
            Mock.Get(_fakeSetCLRPinHighCommand)
                .Setup(x => x.SetCLRPinHigh())
                .Callback(() => _setCLRPinHighCallOrder = ++_callOrder)
                .Verifiable();

            _fakeSetCLRPinLowCommand = Mock.Of<ISetCLRPinLow>();
            Mock.Get(_fakeSetCLRPinLowCommand)
                .Setup(x => x.SetCLRPinLow())
                .Callback(() => _setCLRPinLowCallOrder = ++_callOrder)
                .Verifiable();

            _fakeWriteOFS1RegisterCommand = Mock.Of<IWriteOFS1Register>();
            Mock.Get(_fakeWriteOFS1RegisterCommand)
                .Setup(x => x.WriteOFS1Register(It.IsAny<ushort>()))
                .Callback(() => _writeOFS1RegisterCallOrder = ++_callOrder)
                .Verifiable();

            _setOffsetDAC1Command = new SetOffsetDAC1Command(_fakeEvalBoard, _fakeSetCLRPinLowCommand,
                _fakeSetCLRPinHighCommand, _fakeWriteOFS1RegisterCommand);
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private ISetCLRPinHigh _fakeSetCLRPinHighCommand;
        private ISetCLRPinLow _fakeSetCLRPinLowCommand;
        private IWriteOFS1Register _fakeWriteOFS1RegisterCommand;
        private ISetOffsetDAC1 _setOffsetDAC1Command;
        private int _setCLRPinHighCallOrder;
        private int _setCLRPinLowCallOrder;
        private int _writeOFS1RegisterCallOrder;
        private int _callOrder;
        private readonly ushort SomeValue = ushort.MaxValue;
    }
}