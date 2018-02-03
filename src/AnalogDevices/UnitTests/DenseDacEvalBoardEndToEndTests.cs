using System.Collections.Generic;
using System.Linq;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using NUnit.Framework;

namespace AnalogDevices.UnitTests
{
    [TestFixture]
    [Explicit]
    public class DenseDacEvalBoardEndToEndTests
    {
        [Test]
        [TestCase(DacPrecision.Unknown, DacPrecision.FourteenBit)]
        [TestCase(DacPrecision.Unknown, DacPrecision.SixteenBit)]
        [TestCase(DacPrecision.FourteenBit, DacPrecision.SixteenBit)]
        [TestCase(DacPrecision.SixteenBit, DacPrecision.FourteenBit)]
        public void ShouldSetAndGetDacPrecision
        (
            DacPrecision originalDacPrecision,
            DacPrecision newDacPrecision
        )
        {
            GivenDacPrecision(originalDacPrecision);
            WhenSetAndGetDacPrecision(newDacPrecision);
            ThenDacPrecisionShouldBe(newDacPrecision);
        }

        [Test]
        public void ShouldGetSymbolicName()
        {
            WhenGetSymbolicName();
            ThenSymbolicNameShouldNotBeNullOrEmpty();
        }

        [Test]
        public void ShouldGetIsOverTemperature()
        {
            WhenGetIsOverTemperature();
            ThenIsOverTemperatureShouldNotBeNull();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldGetIsThermalShutdownEnabled(bool value)
        {
            GivenIsThermalShutdownEnabled(value);
            WhenGetIsThermalShutdownEnabled();
            ThenIsThermalShutdownEnabledShouldBe(value);
        }

        [Test]
        public void ShouldSetAndGetOffsetDac0(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenOffsetDac0((ushort) ~newValue);
            WhenSetAndGetOffsetDac0((ushort) newValue);
            ThenOffsetDac0ValueShouldBe((ushort) (newValue & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        public void ShouldSetAndGetOffsetDac1(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenOffsetDac1((ushort) ~newValue);
            WhenSetAndGetOffsetDac1((ushort) newValue);
            ThenOffsetDac1ValueShouldBe((ushort) (newValue & (ushort) BasicMasks.FourteenBits));
        }

        [Test]
        public void ShouldGetPacketErrorCheckErrorOccurred()
        {
            WhenGetPacketErrorCheckErrorOccurred();
            ThenPacketErrorCheckErrorOccuredShouldNotBeNull();
        }

        [Test]
        public void ShouldSetAndGetDacChannelDataSource(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(DacChannelDataSource.DataValueA, DacChannelDataSource.DataValueB)]
            DacChannelDataSource dacChannelDataSource)
        {
            GivenDacChannelDataSource((ChannelAddress) channelAddress,
                dacChannelDataSource == DacChannelDataSource.DataValueA
                    ? DacChannelDataSource.DataValueB
                    : DacChannelDataSource.DataValueA);
            WhenSetAndGetDacChannelDataSource((ChannelAddress) channelAddress, dacChannelDataSource);
            ThenDacChannelDataSourceShouldBe(dacChannelDataSource);
        }

        [Test]
        public void ShouldSetAndGetDacChannelDataSourceAllChannels(
            [Values(DacChannelDataSource.DataValueA, DacChannelDataSource.DataValueB)]
            DacChannelDataSource dacChannelDataSource)
        {
            GivenDacChannelDataSourceAllChannels(dacChannelDataSource == DacChannelDataSource.DataValueA
                ? DacChannelDataSource.DataValueB
                : DacChannelDataSource.DataValueA);
            WhenSetDacChannelDataSourceAllChannels(dacChannelDataSource);
            ThenDacChannelDataSourceAllChannelsShouldBe(dacChannelDataSource);
        }

        [Test]
        public void ShouldSetAndGetDacChannelDataValueA(
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenDacChannelDataValueA((ChannelAddress) channelAddress, (ushort) ~newValue);
            WhenSetAndGetDacChannelDataValueA((ChannelAddress) channelAddress, (ushort) newValue);
            ThenDacChannelDataValueAShouldBe((ushort) (newValue & (ushort) (dacPrecision == DacPrecision.FourteenBit
                                                           ? BasicMasks.HighFourteenBits
                                                           : BasicMasks.SixteenBits)));
        }

        [Test]
        public void ShouldSetAndGetDacChannelDataValueB(
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenDacChannelDataValueB((ChannelAddress) channelAddress, (ushort) ~newValue);
            WhenSetAndGetDacChannelDataValueB((ChannelAddress) channelAddress, (ushort) newValue);
            ThenDacChannelDataValueBShouldBe((ushort) (newValue & (ushort) (dacPrecision == DacPrecision.FourteenBit
                                                           ? BasicMasks.HighFourteenBits
                                                           : BasicMasks.SixteenBits)));
        }

        [Test]
        public void ShouldSetAndGetDacChannelGain(
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenDacChannelGain((ChannelAddress) channelAddress, (ushort) ~newValue);
            WhenSetAndGetDacChannelGain((ChannelAddress) channelAddress, (ushort) newValue);
            ThenDacChannelGainShouldBe((ushort) (newValue & (ushort) (dacPrecision == DacPrecision.FourteenBit
                                                     ? BasicMasks.HighFourteenBits
                                                     : BasicMasks.SixteenBits)));
        }

        [Test]
        public void ShouldSetAndGetDacChannelOffset(
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int newValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenDacChannelOffset((ChannelAddress) channelAddress, (ushort) ~newValue);
            WhenSetAndGetDacChannelOffset((ChannelAddress) channelAddress, (ushort) newValue);
            ThenDacChannelOffsetShouldBe((ushort) (newValue & (ushort) (dacPrecision == DacPrecision.FourteenBit
                                                       ? BasicMasks.HighFourteenBits
                                                       : BasicMasks.SixteenBits)));
        }

        [Test]
        public void ShouldPerformSoftPowerDown()
        {
            GivenControlRegisterBits(ControlRegisterBits.ReadableBits & ~ControlRegisterBits.SoftPowerDown);
            WhenPerformSoftPowerDown();
            ThenSoftPowerDownBitShouldBeSetInControlRegister();
        }

        [Test]
        public void ShouldPerformSoftPowerUp()
        {
            GivenControlRegisterBits(ControlRegisterBits.SoftPowerDown);
            WhenPerformSoftPowerUp();
            ThenSoftPowerDownBitShouldBeClearedInControlRegister();
        }

        [Test]
        public void ShouldSetLDACPinHigh()
        {
            WhenSetLDACPinHigh();
            ThenLDACPinShouldBeSetHigh();
        }

        [Test]
        public void ShouldSetLDACPinLow()
        {
            WhenSetLDACPinLow();
            ThenLDACPinShouldBeSetLow();
        }

        [Test]
        public void ShouldPulseLDACPin()
        {
            WhenPulseLDACPin();
            ThenLDACPinShouldBePulsed();
        }

        [Test]
        public void ShouldSuspendAllDacOutputs()
        {
            WhenSuspendAllDacOutputs();
            ThenCLRPinShouldBeSetLow();
        }

        [Test]
        public void ShouldResumeAllDacOutputs()
        {
            WhenResumeAllDacOutputs();
            ThenCLRPinShouldBeSetHigh();
        }

        [Test]
        public void ShouldUpdateAllDacOutputs()
        {
            WhenUpdateAllDacOutputs();
            ThenLDACPinShouldBePulsed();
        }

        [Test]
        public void ShouldReset()
        {
            WhenReset();
            ThenResetPinWasSetHighAndThenLow();
        }

        [Test]
        public void ShouldRetainValuesWhenSet(
            [Range((int) ChannelAddress.Dac1, (int) ChannelAddress.Dac38)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int someValue)
        {
            GivenDacChannelDataValueA((ChannelAddress) channelAddress - 1, (ushort) ~someValue);
            GivenDacChannelDataValueA((ChannelAddress) channelAddress, (ushort) someValue);
            GivenDacChannelDataValueA((ChannelAddress) channelAddress + 1, (ushort) ~someValue);
            WhenGetDataValueA((ChannelAddress) channelAddress);
            ThenDacChannelDataValueAShouldBe((ushort) someValue);
        }

        private void GivenControlRegisterBits(ControlRegisterBits controlRegisterBits)
        {
            _evalBoard.DeviceState.ControlRegister = controlRegisterBits;
        }

        private void GivenDacChannelOffset(ChannelAddress channelAddress, ushort value)
        {
            _evalBoard.SetDacChannelOffset(channelAddress, value);
        }

        private void GivenDacChannelGain(ChannelAddress channelAddress, ushort value)
        {
            _evalBoard.SetDacChannelGain(channelAddress, value);
        }

        private void GivenDacChannelDataValueA(ChannelAddress channelAddress, ushort value)
        {
            _evalBoard.SetDacChannelDataValueA(channelAddress, value);
        }

        private void GivenDacChannelDataValueB(ChannelAddress channelAddress, ushort value)
        {
            _evalBoard.SetDacChannelDataValueB(channelAddress, value);
        }

        private void GivenDacChannelDataSourceAllChannels(DacChannelDataSource dacChannelDataSource)
        {
            _evalBoard.SetDacChannelDataSourceAllChannels(dacChannelDataSource);
        }

        private void GivenDacChannelDataSource(ChannelAddress channelAddress, DacChannelDataSource dacChannelDataSource)
        {
            _evalBoard.SetDacChannelDataSource(channelAddress, dacChannelDataSource);
        }

        private void GivenIsThermalShutdownEnabled(bool value)
        {
            _evalBoard.IsThermalShutdownEnabled = value;
        }

        private void GivenOffsetDac0(ushort value)
        {
            _evalBoard.OffsetDAC0 = value;
        }

        private void GivenOffsetDac1(ushort value)
        {
            _evalBoard.OffsetDAC1 = value;
        }

        private void GivenDacPrecision(DacPrecision dacPrecision)
        {
            _evalBoard.DacPrecision = dacPrecision;
        }

        private void WhenReset()
        {
            _evalBoard.Reset();
        }

        private void WhenUpdateAllDacOutputs()
        {
            _evalBoard.UpdateAllDacOutputs();
        }

        private void WhenSuspendAllDacOutputs()
        {
            _evalBoard.SuspendAllDacOutputs();
        }

        private void WhenResumeAllDacOutputs()
        {
            _evalBoard.ResumeAllDacOutputs();
        }

        private void WhenPerformSoftPowerDown()
        {
            _evalBoard.PerformSoftPowerDown();
        }

        private void WhenPerformSoftPowerUp()
        {
            _evalBoard.PerformSoftPowerUp();
        }

        private void WhenSetLDACPinHigh()
        {
            _evalBoard.SetLDACPinHigh();
        }

        private void WhenPulseLDACPin()
        {
            _evalBoard.PulseLDACPin();
        }

        private void WhenSetLDACPinLow()
        {
            _evalBoard.SetLDACPinLow();
        }

        private void WhenSetDacChannelDataSourceAllChannels(DacChannelDataSource dacChannelDataSource)
        {
            _evalBoard.SetDacChannelDataSourceAllChannels(dacChannelDataSource);
        }

        private void WhenSetAndGetDacChannelDataSource(ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource)
        {
            _evalBoard.SetDacChannelDataSource(channelAddress, dacChannelDataSource);
            _dacChannelDataSource = _evalBoard.GetDacChannelDataSource(channelAddress);
        }

        private void WhenGetPacketErrorCheckErrorOccurred()
        {
            _packetErrorCheckErrorOccured = _evalBoard.PacketErrorCheckErrorOccurred;
        }

        private void WhenGetIsThermalShutdownEnabled()
        {
            _isThermalShutdownEnabled = _evalBoard.IsThermalShutdownEnabled;
        }

        private void WhenSetAndGetDacPrecision(DacPrecision dacPrecision)
        {
            _evalBoard.DacPrecision = dacPrecision;
            _dacPrecisionReturned = _evalBoard.DacPrecision;
        }

        private void WhenSetAndGetOffsetDac0(ushort value)
        {
            _evalBoard.OffsetDAC0 = value;
            _offsetDac0ValueReturned = _evalBoard.OffsetDAC0;
        }

        private void WhenSetAndGetOffsetDac1(ushort value)
        {
            _evalBoard.OffsetDAC1 = value;
            _offsetDac1ValueReturned = _evalBoard.OffsetDAC1;
        }

        private void WhenSetAndGetDacChannelGain(ChannelAddress channelAddress, ushort newValue)
        {
            _evalBoard.SetDacChannelGain(channelAddress, newValue);
            _dacChannelGain = _evalBoard.GetDacChannelGain(channelAddress);
        }

        private void WhenSetAndGetDacChannelOffset(ChannelAddress channelAddress, ushort newValue)
        {
            _evalBoard.SetDacChannelOffset(channelAddress, newValue);
            _dacChannelOffset = _evalBoard.GetDacChannelOffset(channelAddress);
        }

        private void WhenSetAndGetDacChannelDataValueA(ChannelAddress channelAddress, ushort newValue)
        {
            _evalBoard.SetDacChannelDataValueA(channelAddress, newValue);
            _dacChannelDataValueA = _evalBoard.GetDacChannelDataValueA(channelAddress);
        }

        private void WhenGetDataValueA(ChannelAddress channelAddress)
        {
            _dacChannelDataValueA = _evalBoard.GetDacChannelDataValueA(channelAddress);
        }

        private void WhenSetAndGetDacChannelDataValueB(ChannelAddress channelAddress, ushort newValue)
        {
            _evalBoard.SetDacChannelDataValueB(channelAddress, newValue);
            _dacChannelDataValueB = _evalBoard.GetDacChannelDataValueB(channelAddress);
        }

        private void WhenGetSymbolicName()
        {
            _symbolicNameReturned = _evalBoard.SymbolicName;
        }

        private void WhenGetIsOverTemperature()
        {
            _isOverTemperature = _evalBoard.IsOverTemperature;
        }

        private void ThenResetPinWasSetHighAndThenLow()
        {
            var a = _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetRESETPinHigh);
            a.Should().NotBeNull();
            var b = _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetRESETPinLow);
            b.Should().NotBeNull();
            _usbControlTransfersSent.IndexOf(a).Should().BeLessThan(_usbControlTransfersSent.IndexOf(b));
        }

        private void ThenCLRPinShouldBeSetLow()
        {
            _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetCLRPinLow)
                .Should()
                .NotBeNull();
        }

        private void ThenCLRPinShouldBeSetHigh()
        {
            _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetCLRPinHigh)
                .Should()
                .NotBeNull();
        }

        private void ThenLDACPinShouldBePulsed()
        {
            _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.PulseLDACPin)
                .Should()
                .NotBeNull();
        }

        private void ThenLDACPinShouldBeSetHigh()
        {
            _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetLDACPinHigh)
                .Should()
                .NotBeNull();
        }

        private void ThenLDACPinShouldBeSetLow()
        {
            _usbControlTransfersSent.FirstOrDefault(x => x.Request == (byte) DeviceCommand.SetLDACPinLow)
                .Should()
                .NotBeNull();
        }

        private void ThenIsOverTemperatureShouldNotBeNull()
        {
            _isOverTemperature.Should().NotBeNull();
        }

        private void ThenIsThermalShutdownEnabledShouldBe(bool expected)
        {
            _isThermalShutdownEnabled.Should().Be(expected);
        }

        private void ThenDacPrecisionShouldBe(DacPrecision expected)
        {
            _dacPrecisionReturned.Should().Be(expected);
        }

        private void ThenSymbolicNameShouldNotBeNullOrEmpty()
        {
            _symbolicNameReturned.Should().NotBeNullOrEmpty();
        }

        private void ThenOffsetDac0ValueShouldBe(ushort expected)
        {
            _offsetDac0ValueReturned.Should().Be(expected);
        }

        private void ThenOffsetDac1ValueShouldBe(ushort expected)
        {
            _offsetDac1ValueReturned.Should().Be(expected);
        }

        private void ThenPacketErrorCheckErrorOccuredShouldNotBeNull()
        {
            _packetErrorCheckErrorOccured.Should().NotBeNull();
        }

        private void ThenDacChannelDataSourceShouldBe(DacChannelDataSource expected)
        {
            _dacChannelDataSource.Should().Be(expected);
        }

        private void ThenDacChannelDataSourceAllChannelsShouldBe(DacChannelDataSource expected)
        {
            for (var channel = ChannelAddress.Dac0; channel <= ChannelAddress.Dac39; channel++)
                _evalBoard.GetDacChannelDataSource(channel).Should().Be(expected);
        }

        private void ThenDacChannelGainShouldBe(ushort newValue)
        {
            _dacChannelGain.Should().Be(newValue);
        }

        private void ThenDacChannelOffsetShouldBe(ushort newValue)
        {
            _dacChannelOffset.Should().Be(newValue);
        }

        private void ThenDacChannelDataValueAShouldBe(ushort newValue)
        {
            _dacChannelDataValueA.Should().Be(newValue);
        }

        private void ThenDacChannelDataValueBShouldBe(ushort newValue)
        {
            _dacChannelDataValueB.Should().Be(newValue);
        }

        private void ThenSoftPowerDownBitShouldBeSetInControlRegister()
        {
            (_evalBoard.DeviceState.ControlRegister & ControlRegisterBits.SoftPowerDown).Should()
                .Be(ControlRegisterBits.SoftPowerDown);
        }

        private void ThenSoftPowerDownBitShouldBeClearedInControlRegister()
        {
            ((_evalBoard.DeviceState.ControlRegister ?? 0) & ControlRegisterBits.SoftPowerDown).Should()
                .Be((ControlRegisterBits) 0);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _evalBoard = new EnumerateDevicesCommand().EnumerateDevices().First();
            _evalBoard.DeviceState.UseRegisterCache = false;
            _evalBoard.UsbDevice.ControlTransferSent += (s, e) => _usbControlTransfersSent.Add(e);
        }

        [SetUp]
        public void SetUp()
        {
            _usbControlTransfersSent = new List<ControlTransferSentEventArgs>();
        }

        private IDenseDacEvalBoard _evalBoard;
        private DacPrecision _dacPrecisionReturned;
        private string _symbolicNameReturned;
        private bool? _isOverTemperature;
        private bool _isThermalShutdownEnabled;
        private ushort _offsetDac0ValueReturned;
        private ushort _offsetDac1ValueReturned;
        private bool? _packetErrorCheckErrorOccured;
        private DacChannelDataSource _dacChannelDataSource;
        private ushort _dacChannelDataValueA;
        private ushort _dacChannelDataValueB;
        private ushort _dacChannelGain;
        private ushort _dacChannelOffset;
        private List<ControlTransferSentEventArgs> _usbControlTransfersSent;
    }
}