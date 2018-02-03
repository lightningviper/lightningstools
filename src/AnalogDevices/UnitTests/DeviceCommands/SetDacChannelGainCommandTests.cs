using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetDacChannelGainCommandTests
    {
        [Test]
        public void ShouldThrowExceptionIfChannelAddressIsOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WhenSetDacChannelGain(channelAddress, ushort.MaxValue));
        }

        [Test]
        public void ShouldSetCLRPinLowBeforeCallingSendSPI()
        {
            WhenSetDacChannelGain(ChannelAddress.Dac0, ushort.MinValue);
            ThenCLRPinIsSetLowBeforeCallingSendSPI();
        }

        [Test]
        public void ShouldSetCLRPinHighAfterCallingSendSPI()
        {
            WhenSetDacChannelGain(ChannelAddress.Dac0, ushort.MinValue);
            ThenCLRPinIsSetHighAfterCallingSendSPI();
        }

        [Test]
        public void ShouldSendSPI(
            [Values(DacPrecision.SixteenBit, DacPrecision.FourteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] ChannelAddress channelAddress,
            [Values(ushort.MinValue, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10,
                1 << 11, 1 << 12, 1 << 13, 1 << 14, 1 << 15, ushort.MaxValue)] int dacChannelGain)
        {
            GivenDACPrecision(dacPrecision);
            WhenSetDacChannelGain(channelAddress, (ushort) dacChannelGain);
            ThenSPICommandIsSent(DataForSPI(dacPrecision, channelAddress, (ushort) dacChannelGain));
        }

        [Test]
        public void ShouldCacheMRegisterValues(
            [Values(DacPrecision.SixteenBit, DacPrecision.FourteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] ChannelAddress channelAddress,
            [Values(ushort.MinValue, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10,
                1 << 11, 1 << 12, 1 << 13, 1 << 14, 1 << 15, ushort.MaxValue)] int dacChannelGain)
        {
            GivenDACPrecision(dacPrecision);
            WhenSetDacChannelGain(channelAddress, (ushort) dacChannelGain);
            ThenMRegisterValueInDeviceStateCacheIsSetTo(
                channelAddress.ToChannelNumber(),
                (ushort) (dacChannelGain &
                          (dacPrecision == DacPrecision.SixteenBit
                              ? (ushort) BasicMasks.SixteenBits
                              : (ushort) BasicMasks.HighFourteenBits))
            );
        }

        private void GivenDACPrecision(DacPrecision dacPrecision)
        {
            _deviceState.Precision = dacPrecision;
        }

        private void WhenSetDacChannelGain(ChannelAddress channelAddress, ushort dacChannelGain)
        {
            _setDacChannelGainCommand.SetDacChannelGain(channelAddress, dacChannelGain);
        }

        private void ThenCLRPinIsSetLowBeforeCallingSendSPI()
        {
            Mock.Get(_fakeSetCLRPinLowCommand)
                .Verify(x => x.SetCLRPinLow(), Times.Once);
            _setCLRPinLowCallOrder.Should().BeLessThan(_sendSPICallOrder);
        }

        private void ThenSPICommandIsSent(uint data)
        {
            Mock.Get(_fakeSendSPICommand)
                .Verify(x => x.SendSPI(data), Times.Once);
        }

        private void ThenCLRPinIsSetHighAfterCallingSendSPI()
        {
            Mock.Get(_fakeSetCLRPinHighCommand)
                .Verify(x => x.SetCLRPinHigh(), Times.Once);
            _setCLRPinHighCallOrder.Should().BeGreaterThan(_sendSPICallOrder);
        }

        private void ThenMRegisterValueInDeviceStateCacheIsSetTo(int registerNum, ushort data)
        {
            _deviceState.MRegisters[registerNum].Should().Be(data);
        }

        private static uint DataForSPI(DacPrecision dacPrecision, ChannelAddress channelAddress, ushort dacChannelGain)
        {
            return
                (uint) SerialInterfaceModeBits.WriteToDACGainRegisterM
                | (uint) (((byte) channelAddress & (byte) BasicMasks.SixBits) << 16)
                | (dacChannelGain &
                   (dacPrecision == DacPrecision.SixteenBit
                       ? (uint) BasicMasks.SixteenBits
                       : (uint) BasicMasks.HighFourteenBits));
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendSPICommand = Mock.Of<ISendSPI>();
            Mock.Get(_fakeSendSPICommand)
                .Setup(x => x.SendSPI(It.IsAny<uint>()))
                .Callback<uint>(
                    a =>
                    {
                        _sendSPICallOrder = _callOrder;
                        _callOrder++;
                    }
                )
                .Verifiable();

            _fakeSetCLRPinLowCommand = Mock.Of<ISetCLRPinLow>();
            Mock.Get(_fakeSetCLRPinLowCommand)
                .Setup(x => x.SetCLRPinLow())
                .Callback(() =>
                    {
                        _setCLRPinLowCallOrder = _callOrder;
                        _callOrder++;
                    }
                )
                .Verifiable();

            _fakeSetCLRPinHighCommand = Mock.Of<ISetCLRPinHigh>();
            Mock.Get(_fakeSetCLRPinHighCommand)
                .Setup(x => x.SetCLRPinHigh())
                .Callback(() =>
                    {
                        _setCLRPinHighCallOrder = _callOrder;
                        _callOrder++;
                    }
                )
                .Verifiable();

            _setDacChannelGainCommand = new SetDacChannelGainCommand(
                _fakeEvalBoard, _fakeSendSPICommand, _fakeSetCLRPinLowCommand, _fakeSetCLRPinHighCommand);
        }

        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSPI _fakeSendSPICommand;
        private ISetCLRPinLow _fakeSetCLRPinLowCommand;
        private ISetCLRPinHigh _fakeSetCLRPinHighCommand;
        private ISetDacChannelGain _setDacChannelGainCommand;
        private int _sendSPICallOrder;
        private int _setCLRPinLowCallOrder;
        private int _setCLRPinHighCallOrder;
        private int _callOrder;
    }
}