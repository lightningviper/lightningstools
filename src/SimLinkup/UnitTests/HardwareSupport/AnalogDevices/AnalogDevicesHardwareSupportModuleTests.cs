using AnalogDevices;
using Moq;
using NUnit.Framework;
using SimLinkup.HardwareSupport.AnalogDevices;

namespace SimLinkup.UnitTests.HardwareSupport.AnalogDevices
{
    [TestFixture]
    public class AnalogDevicesHardwareSupportModuleTests
    {
        [Test]
        public void ShouldConfigureInitialDacChannelDataValueA(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int dacChannelDataValueA)
        {
            GivenDeviceConfig(WithInitialDacChannelDataValueA((ChannelAddress) channelAddress,
                (ushort) dacChannelDataValueA));
            WhenCreate();
            ThenDacChannelDataValueAShouldBe((ChannelAddress) channelAddress, (ushort) dacChannelDataValueA);
        }

        [Test]
        public void ShouldConfigureDefaultInitialDacChannelDataValueA(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress)
        {
            GivenDeviceConfig(WithInitialDacChannelDataValueA((ChannelAddress) channelAddress, null));
            WhenCreate();
            ThenDacChannelDataValueAShouldBe((ChannelAddress) channelAddress, 0x8000);
        }

        [Test]
        public void ShouldConfigureInitialDacChannelDataValueB(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int dacChannelDataValueB)
        {
            GivenDeviceConfig(WithInitialDacChannelDataValueB((ChannelAddress) channelAddress,
                (ushort) dacChannelDataValueB));
            WhenCreate();
            ThenDacChannelDataValueBShouldBe((ChannelAddress) channelAddress, (ushort) dacChannelDataValueB);
        }

        [Test]
        public void ShouldConfigureDefaultInitialDacChannelDataValueB(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress)
        {
            GivenDeviceConfig(WithInitialDacChannelDataValueB((ChannelAddress) channelAddress, null));
            WhenCreate();
            ThenDacChannelDataValueBShouldBe((ChannelAddress) channelAddress, 0x8000);
        }

        [Test]
        public void ShouldConfigureInitialDacChannelOffset(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int dacChannelOffset)
        {
            GivenDeviceConfig(WithInitialDacChannelOffset((ChannelAddress) channelAddress, (ushort) dacChannelOffset));
            WhenCreate();
            ThenDacChannelOffsetShouldBe((ChannelAddress) channelAddress, (ushort) dacChannelOffset);
        }

        [Test]
        public void ShouldConfigureDefaultInitialDacChannelOffset(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress)
        {
            GivenDeviceConfig(WithInitialDacChannelOffset((ChannelAddress) channelAddress, null));
            WhenCreate();
            ThenDacChannelOffsetShouldBe((ChannelAddress) channelAddress, 0x8000);
        }

        [Test]
        public void ShouldConfigureInitialDacChannelGain(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int dacChannelGain)
        {
            GivenDeviceConfig(WithInitialDacChannelGain((ChannelAddress) channelAddress, (ushort) dacChannelGain));
            WhenCreate();
            ThenDacChannelGainShouldBe((ChannelAddress) channelAddress, (ushort) dacChannelGain);
        }

        [Test]
        public void ShouldConfigureDefaultInitialDacChannelGain(
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress)
        {
            GivenDeviceConfig(WithInitialDacChannelGain((ChannelAddress) channelAddress, null));
            WhenCreate();
            ThenDacChannelGainShouldBe((ChannelAddress) channelAddress, 0xFFFF);
        }

        [Test]
        public void ShouldConfigureDacPrecision(
            [Values(DacPrecision.FourteenBit, DacPrecision.SixteenBit)] DacPrecision dacPrecision)
        {
            GivenDeviceConfig(WithDacPrecision(dacPrecision));
            WhenCreate();
            ThenDacPrecisionShouldBe(dacPrecision);
        }

        [Test]
        public void ShouldConfigureDefaultDacPrecision()
        {
            GivenDeviceConfig(WithDacPrecision(DacPrecision.Unknown));
            WhenCreate();
            ThenDacPrecisionShouldBe(DacPrecision.SixteenBit);
        }

        [Test]
        public void ShouldConfigureOffsetDac0(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int offsetDac0
        )
        {
            GivenDeviceConfig(WithOffsetDac0((ushort) offsetDac0));
            WhenCreate();
            ThenOffsetDac0ShouldBe((ushort) offsetDac0);
        }

        [Test]
        public void ShouldConfigureDefaultOffsetDac0()
        {
            GivenDeviceConfig(WithOffsetDac0(null));
            WhenCreate();
            ThenOffsetDac0ShouldBe(0x2000);
        }

        [Test]
        public void ShouldConfigureOffsetDac1(
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15)] int offsetDac1
        )
        {
            GivenDeviceConfig(WithOffsetDac1((ushort) offsetDac1));
            WhenCreate();
            ThenOffsetDac1ShouldBe((ushort) offsetDac1);
        }

        [Test]
        public void ShouldConfigureDefaultOffsetDac1()
        {
            GivenDeviceConfig(WithOffsetDac1(null));
            WhenCreate();
            ThenOffsetDac1ShouldBe(0x2000);
        }

        [Test]
        public void ShouldReset()
        {
            WhenCreate();
            ThenDeviceShouldBeReset();
        }

        [Test]
        public void ShouldUpdateAllDacOutputs()
        {
            WhenCreate();
            ThenShouldUpdateAllDacOutputs();
        }

        [Test]
        public void ShouldClearThermalShutdownEnabledFlag()
        {
            GivenDeviceIsOverTemperature();
            WhenCreate();
            ThenThermalShutdownEnabledFlagShouldBeCleared();
        }

        [Test]
        public void ShouldSetThermalShutdownEnabledFlag()
        {
            WhenCreate();
            ThenThermalShutdownEnabledFlagShouldBeSet();
        }

        [Test]
        public void ShouldSetDacChannelDataSourceAllChannelsToDataSourceA()
        {
            WhenCreate();
            ThenDacChannelDataSourceAllChannelsShouldBeSetToDataSourceA();
        }

        private void GivenDeviceIsOverTemperature()
        {
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.IsOverTemperature)
                .Returns(true);
        }

        private void GivenDeviceConfig(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
        }

        private static DeviceConfig WithInitialDacChannelDataValueA(ChannelAddress channelAddress,
            ushort? dacChannelDataValueA)
        {
            var deviceConfig = new DeviceConfig {DACChannelConfig = new DACChannelConfigurations()};
            var propertyInfo = deviceConfig.DACChannelConfig.GetType()
                .GetProperty($"DAC{(int) channelAddress - 8}");
            if (propertyInfo == null) return deviceConfig;
            propertyInfo
                .SetValue(deviceConfig.DACChannelConfig,
                    new DACChannelConfiguration
                    {
                        InitialState = new InitialDACChannelState {DataValueA = dacChannelDataValueA}
                    }
                );
            return deviceConfig;
        }

        private static DeviceConfig WithInitialDacChannelDataValueB(ChannelAddress channelAddress,
            ushort? dacChannelDataValueB)
        {
            var deviceConfig = new DeviceConfig {DACChannelConfig = new DACChannelConfigurations()};
            var propertyInfo = deviceConfig.DACChannelConfig.GetType()
                .GetProperty($"DAC{(int) channelAddress - 8}");
            if (propertyInfo == null) return deviceConfig;
            propertyInfo
                .SetValue(deviceConfig.DACChannelConfig,
                    new DACChannelConfiguration
                    {
                        InitialState = new InitialDACChannelState {DataValueB = dacChannelDataValueB}
                    }
                );
            return deviceConfig;
        }

        private static DeviceConfig WithInitialDacChannelOffset(ChannelAddress channelAddress, ushort? dacChannelOffset)
        {
            var deviceConfig = new DeviceConfig {DACChannelConfig = new DACChannelConfigurations()};
            var propertyInfo = deviceConfig.DACChannelConfig.GetType()
                .GetProperty($"DAC{(int) channelAddress - 8}");
            if (propertyInfo == null) return deviceConfig;
            propertyInfo
                .SetValue(deviceConfig.DACChannelConfig,
                    new DACChannelConfiguration
                    {
                        Calibration = new DACChannelCalibration {Offset = dacChannelOffset}
                    }
                );
            return deviceConfig;
        }

        private static DeviceConfig WithInitialDacChannelGain(ChannelAddress channelAddress, ushort? dacChannelGain)
        {
            var deviceConfig = new DeviceConfig {DACChannelConfig = new DACChannelConfigurations()};
            var propertyInfo = deviceConfig.DACChannelConfig.GetType()
                .GetProperty($"DAC{(int) channelAddress - 8}");
            if (propertyInfo == null) return deviceConfig;
            propertyInfo
                .SetValue(deviceConfig.DACChannelConfig,
                    new DACChannelConfiguration
                    {
                        Calibration = new DACChannelCalibration {Gain = dacChannelGain}
                    }
                );
            return deviceConfig;
        }

        private static DeviceConfig WithDacPrecision(DacPrecision dacPrecision)
        {
            return new DeviceConfig {DACPrecision = dacPrecision};
        }

        private static DeviceConfig WithOffsetDac0(ushort? offsetDac0)
        {
            return new DeviceConfig
            {
                Calibration = new DeviceCalibration
                {
                    OffsetDAC0 = offsetDac0
                }
            };
        }

        private static DeviceConfig WithOffsetDac1(ushort? offsetDac1)
        {
            return new DeviceConfig
            {
                Calibration = new DeviceCalibration
                {
                    OffsetDAC1 = offsetDac1
                }
            };
        }

        private void WhenCreate()
        {
            AnalogDevicesHardwareSupportModule.Create(_fakeEvalBoard, 0, _deviceConfig);
        }

        private void ThenDacChannelDataValueAShouldBe(ChannelAddress channelAddress, ushort? dataValueA)
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.SetDacChannelDataValueA(channelAddress, dataValueA.Value));
        }

        private void ThenDacChannelDataValueBShouldBe(ChannelAddress channelAddress, ushort? dataValueB)
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.SetDacChannelDataValueB(channelAddress, dataValueB.Value));
        }

        private void ThenDacChannelOffsetShouldBe(ChannelAddress channelAddress, ushort? dacChannelOffset)
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.SetDacChannelOffset(channelAddress, dacChannelOffset.Value));
        }

        private void ThenDacChannelGainShouldBe(ChannelAddress channelAddress, ushort? dacChannelGain)
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.SetDacChannelGain(channelAddress, dacChannelGain.Value));
        }

        private void ThenDacPrecisionShouldBe(DacPrecision expectedDacPrecision)
        {
            Mock.Get(_fakeEvalBoard)
                .VerifySet(x => x.DacPrecision = expectedDacPrecision);
        }

        private void ThenDeviceShouldBeReset()
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.Reset());
        }

        private void ThenThermalShutdownEnabledFlagShouldBeCleared()
        {
            Mock.Get(_fakeEvalBoard)
                .VerifySet(x => x.IsThermalShutdownEnabled = false);
        }

        private void ThenThermalShutdownEnabledFlagShouldBeSet()
        {
            Mock.Get(_fakeEvalBoard)
                .VerifySet(x => x.IsThermalShutdownEnabled = true);
        }

        private void ThenDacChannelDataSourceAllChannelsShouldBeSetToDataSourceA()
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.SetDacChannelDataSourceAllChannels(DacChannelDataSource.DataValueA));
        }

        private void ThenOffsetDac0ShouldBe(ushort expectedOffsetDac0)
        {
            Mock.Get(_fakeEvalBoard)
                .VerifySet(x => x.OffsetDAC0 = expectedOffsetDac0);
        }

        private void ThenOffsetDac1ShouldBe(ushort expectedOffsetDac1)
        {
            Mock.Get(_fakeEvalBoard)
                .VerifySet(x => x.OffsetDAC1 = expectedOffsetDac1);
        }

        private void ThenShouldUpdateAllDacOutputs()
        {
            Mock.Get(_fakeEvalBoard)
                .Verify(x => x.UpdateAllDacOutputs());
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();

            Mock.Get(_fakeEvalBoard)
                .Setup(x => x.SetDacChannelDataSource(It.IsAny<ChannelAddress>(), It.IsAny<DacChannelDataSource>()))
                .Verifiable();
        }

        private DeviceConfig _deviceConfig;
        private IDenseDacEvalBoard _fakeEvalBoard;
    }
}