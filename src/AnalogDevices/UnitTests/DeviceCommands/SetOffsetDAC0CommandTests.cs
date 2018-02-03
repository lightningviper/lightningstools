using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetOffsetDAC0CommandTests
    {
        [Test]
        public void ShouldSetClrPinLowBeforeSettingOffsetDACValue()
        {
            WhenSetOffsetDAC0(SomeValue);
            ThenClrPinWasSetLowBeforeSetOffsetDAC0WasCalled();
        }

        [Test]
        public void ShouldInvokeSetOFS0RegisterCommand(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int value)
        {
            WhenSetOffsetDAC0((ushort) value);
            ThenWriteOFS0RegisterCommandWasInvoked((ushort) (value & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        public void ShouldSetClrPinHighAfterSettingOffsetDACValue()
        {
            WhenSetOffsetDAC0(SomeValue);
            ThenClrPinWasSetHighAfterSetOffsetDAC0WasCalled();
        }

        private void WhenSetOffsetDAC0(ushort value)
        {
            _setOffsetDAC0Command.SetOffsetDAC0(value);
        }

        private void ThenClrPinWasSetHighAfterSetOffsetDAC0WasCalled()
        {
            Mock.Get(_fakeSetCLRPinHighCommand)
                .Verify(x => x.SetCLRPinHigh(), Times.Once);

            _setCLRPinHighCallOrder.Should().BeGreaterThan(_writeOFS0RegisterCallOrder);
        }

        private void ThenWriteOFS0RegisterCommandWasInvoked(ushort value)
        {
            Mock.Get(_fakeWriteOFS0RegisterCommand)
                .Verify(x => x.WriteOFS0Register(value));
        }

        private void ThenClrPinWasSetLowBeforeSetOffsetDAC0WasCalled()
        {
            Mock.Get(_fakeSetCLRPinLowCommand)
                .Verify(x => x.SetCLRPinLow(), Times.Once);

            _setCLRPinLowCallOrder.Should().BeLessThan(_writeOFS0RegisterCallOrder);
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

            _fakeWriteOFS0RegisterCommand = Mock.Of<IWriteOFS0Register>();
            Mock.Get(_fakeWriteOFS0RegisterCommand)
                .Setup(x => x.WriteOFS0Register(It.IsAny<ushort>()))
                .Callback(() => _writeOFS0RegisterCallOrder = ++_callOrder)
                .Verifiable();

            _setOffsetDAC0Command = new SetOffsetDAC0Command(_fakeEvalBoard, _fakeSetCLRPinLowCommand,
                _fakeSetCLRPinHighCommand, _fakeWriteOFS0RegisterCommand);
        }

        private DeviceState _deviceState;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private ISetCLRPinHigh _fakeSetCLRPinHighCommand;
        private ISetCLRPinLow _fakeSetCLRPinLowCommand;
        private IWriteOFS0Register _fakeWriteOFS0RegisterCommand;
        private ISetOffsetDAC0 _setOffsetDAC0Command;
        private int _setCLRPinHighCallOrder;
        private int _setCLRPinLowCallOrder;
        private int _writeOFS0RegisterCallOrder;
        private int _callOrder;
        private readonly ushort SomeValue = ushort.MaxValue;
    }
}