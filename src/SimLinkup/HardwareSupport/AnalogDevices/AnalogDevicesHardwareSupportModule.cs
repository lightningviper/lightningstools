using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.HardwareSupport;
using Common.MacroProgramming;
using log4net;
using a = AnalogDevices;

namespace SimLinkup.HardwareSupport.AnalogDevices
{
    public class AnalogDevicesHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AnalogDevicesHardwareSupportModule));
        private readonly List<Task> _pendingTasks = new List<Task>();

        private AnalogSignal[] _analogOutputSignals;
        private a.IDenseDacEvalBoard _device;
        private int _deviceIndex;
        private bool _isDisposed;

        private AnalogDevicesHardwareSupportModule()
        {
        }

        public override AnalogSignal[] AnalogInputs => null;

        public override AnalogSignal[] AnalogOutputs => _analogOutputSignals;

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName =>
            $"Analog Devices AD536x/AD537x on {(_device == null ? $"{{FAKE{_deviceIndex}}}" : _device.SymbolicName)}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AnalogDevicesHardwareSupportModule()
        {
            Dispose();
        }

        public static AnalogDevicesHardwareSupportModule Create(a.IDenseDacEvalBoard device, int deviceIndex = 0,
            DeviceConfig deviceConfig = null)
        {
            var module = new AnalogDevicesHardwareSupportModule();
            module.Initialize(device, deviceIndex, deviceConfig);
            return module;
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory,
                    "AnalogDevicesHardwareSupportModule.config");
                var hsmConfig = AnalogDevicesHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig.Devices != null && hsmConfig.Devices == null && hsmConfig.Devices.Length == 0)
                {
                    return toReturn.ToArray();
                }
                var index = 0;
                var devices = a.DenseDacEvalBoard.Enumerate();

                if (hsmConfig.Devices != null)
                {
                    foreach (var deviceConfig in hsmConfig.Devices)
                    {
                        var thisDeviceConfig = hsmConfig.Devices.Length > index
                            ? hsmConfig.Devices[index]
                            : null;

                        var device = devices != null && devices.Length > index ? devices[index] : null;
                        if (device != null)
                        {
                            var hsmInstance = Create(device, index, thisDeviceConfig);
                            toReturn.Add(hsmInstance);
                        }
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }


            return toReturn.ToArray();
        }

        public override void Synchronize()
        {
            try
            {
                Task.WaitAll(_pendingTasks.ToArray());
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            finally
            {
                _pendingTasks.Clear();
            }
        }

        private static void ConfigureDevice(a.IDenseDacEvalBoard device, DeviceConfig deviceConfig)
        {
            if (device == null) return;
            device.DeviceState.UseRegisterCache = true;
            device.DacPrecision = deviceConfig?.DACPrecision != null &&
                                  deviceConfig.DACPrecision.Value != a.DacPrecision.Unknown
                ? deviceConfig.DACPrecision.Value
                : a.DacPrecision.SixteenBit;

            device.Reset();
            if (device.IsOverTemperature)
            {
                device.IsThermalShutdownEnabled = false;
                //reset temperature shutdown after previous over-temperature event
            }
            device.IsThermalShutdownEnabled = true; //enable over-temperature auto shutdown

            device.SetDacChannelDataSourceAllChannels(a.DacChannelDataSource.DataValueA);

            device.OffsetDAC0 = deviceConfig?.Calibration?.OffsetDAC0 ?? (ushort) 0x2000;

            device.OffsetDAC1 = deviceConfig?.Calibration?.OffsetDAC1 ?? (ushort) 0x2000;

            for (var channel = a.ChannelAddress.Dac0; channel <= a.ChannelAddress.Dac39; channel++)
                ConfigureDeviceChannel(device, deviceConfig, channel);
            device.UpdateAllDacOutputs();
        }

        private static void ConfigureDeviceChannel(a.IDenseDacEvalBoard device, DeviceConfig deviceConfig,
            a.ChannelAddress channel)
        {
            var dacChannelConfiguration = GetDACChannelConfiguration(channel, deviceConfig);

            device.SetDacChannelDataValueA(channel,
                dacChannelConfiguration?.InitialState?.DataValueA ?? (ushort) 0x8000);

            device.SetDacChannelDataValueB(channel,
                dacChannelConfiguration?.InitialState?.DataValueB ?? (ushort) 0x8000);

            device.SetDacChannelOffset(channel,
                dacChannelConfiguration?.Calibration?.Offset ?? (ushort) 0x8000);

            device.SetDacChannelGain(channel,
                dacChannelConfiguration?.Calibration?.Gain ?? (ushort) 0xFFFF);
        }

        private AnalogSignal[] CreateOutputSignals(a.IDenseDacEvalBoard device, int deviceIndex)
        {
            var analogSignalsToReturn = new List<AnalogSignal>();
            for (var i = 0; i < 40; i++)
            {
                var thisSignal = new AnalogSignal
                {
                    Category = "Outputs",
                    CollectionName = "DAC Outputs",
                    FriendlyName = $"DAC #{i}",
                    Id = $"AnalogDevices_AD536x/537x__DAC_OUTPUT[{deviceIndex}][{i}]",
                    Index = i,
                    PublisherObject = this,
                    Source = device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = device?.SymbolicName,
                    SubSource = (a.ChannelAddress) i + 8
                };
                thisSignal.SubSourceFriendlyName = ((a.ChannelAddress) thisSignal.SubSource).ToString();
                thisSignal.SubSourceAddress = null;
                thisSignal.State = 0; //O Volts
                thisSignal.SignalChanged += DAC_OutputSignalChanged;
                thisSignal.Precision = -1; //arbitrary decimal precision (limited to 14-16 bits output precision)
                thisSignal.IsVoltage = true;
                thisSignal.MinValue = -10;
                thisSignal.MaxValue = 10;
                analogSignalsToReturn.Add(thisSignal);
            }
            return analogSignalsToReturn.ToArray();
        }

        private void DAC_OutputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            var task =
                Task.Run(() => SetDACChannelOutputValue((AnalogSignal) sender, a.DacChannelDataSource.DataValueA));
            _pendingTasks.Add(task);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Common.Util.DisposeObject(_device); //disconnect
                    _device = null;
                }
            }
            _isDisposed = true;
        }

        private static DACChannelConfiguration GetDACChannelConfiguration(a.ChannelAddress channel,
            DeviceConfig deviceConfig)
        {
            var type = typeof(DACChannelConfigurations);
            DACChannelConfiguration toReturn = null;
            try
            {
                if (
                    deviceConfig?.DACChannelConfig != null
                )
                {
                    var propInfo = type.GetProperty($"DAC{(int) channel - 8}");
                    toReturn = propInfo != null
                        ? propInfo.GetMethod.Invoke(deviceConfig.DACChannelConfig, null) as DACChannelConfiguration
                        : null;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            return toReturn;
        }

        private void Initialize(a.IDenseDacEvalBoard device, int deviceIndex, DeviceConfig deviceConfig)
        {
            _device = device;
            _deviceIndex = deviceIndex;
            ConfigureDevice(_device, deviceConfig);
            _analogOutputSignals = CreateOutputSignals(_device, _deviceIndex);
            InitializeOutputs();
        }

        private void InitializeOutputs()
        {
            _analogOutputSignals.ToList()
                .ForEach(signal =>
                    SetDACChannelOutputValue(signal, a.DacChannelDataSource.DataValueA));
            _device.SetLDACPinLow();
            Synchronize();
        }

        private void SetDACChannelOutputValue(AnalogSignal outputSignal, a.DacChannelDataSource dacChannelDataSource)
        {
            if (!outputSignal.Index.HasValue || _device == null) return;

            var value = (ushort) ((outputSignal.State + 10.0000) / 20.0000 * 0xFFFF);
            var channelAddress = (a.ChannelAddress) outputSignal.SubSource;
            if (dacChannelDataSource == a.DacChannelDataSource.DataValueA)
            {
                _device.SetDacChannelDataValueA(channelAddress, value);
            }
            else
            {
                _device.SetDacChannelDataValueB(channelAddress, value);
            }
        }
    }
}