using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;
using System.IO;
using log4net;
using System.Linq;
using Henkie.Common;
using Henkie.FuelFlow;
using System.Globalization;
using p = Phcc;
namespace SimLinkup.HardwareSupport.Henk.FuelFlow
{
    //Henkie F-16 Fuel Flow Indicator interface board
    public class HenkieF16FuelFlowIndicatorHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HenkieF16FuelFlowIndicatorHardwareSupportModule ));
        private readonly IFuelFlow _renderer = new LightningGauges.Renderers.F16.FuelFlow();

        private AnalogSignal _fuelFlowInputSignal;
        private readonly DeviceConfig _deviceConfig;
        private byte _deviceAddress;
        private List<DigitalSignal> _digitalOutputs = new List<DigitalSignal>();

        private bool _isDisposed;
        private AnalogSignal _positionOutputSignal;
        private Device _fuelFlowDeviceInterface;


        private CalibrationPoint[] _calibrationData;

        // Paths stashed at construction so the FileSystemWatchers (started
        // below) can reload calibration when either file changes.
        // _legacyConfigPath: HenkieF16FuelFlowIndicator.config (always present
        // when this HSM instantiates — that's where stator angles + DIG_OUTs
        // come from).
        // _unifiedConfigPath: HenkieF16FuelFlowHardwareSupportModule.config
        // (may not exist yet — the editor creates it the first time the user
        // saves a calibration. The watcher uses Path+Filter so it fires even
        // for file CREATE events).
        private string _legacyConfigPath;
        private string _unifiedConfigPath;
        private ConfigFileReloadWatcher _legacyConfigWatcher;
        private ConfigFileReloadWatcher _unifiedConfigWatcher;

        private HenkieF16FuelFlowIndicatorHardwareSupportModule (
            DeviceConfig deviceConfig,
            string legacyConfigPath,
            string unifiedConfigPath)
        {
            _deviceConfig = deviceConfig;
            _legacyConfigPath = legacyConfigPath;
            _unifiedConfigPath = unifiedConfigPath;
            if (_deviceConfig != null)
            {
                ConfigureDevice();
                CreateInputSignals();
                CreateOutputSignals();
                RegisterForEvents();
                StartConfigWatchers();
            }

        }

        public override AnalogSignal[] AnalogOutputs
        {
            get
            {
                return new[] { _positionOutputSignal }
                    .ToArray();
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] { _fuelFlowInputSignal};

        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                return _digitalOutputs
                    .OrderBy(x => x.FriendlyName)
                    .ToArray();
            }
        }

        public override DigitalSignal[] DigitalInputs => null;

        public override string FriendlyName =>
            $"Henkie Fuel Flow Drive Interface: 0x{_deviceAddress.ToString("X").PadLeft(2, '0')} on {_deviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_deviceConfig.COMPort ?? "<UNKNOWN>"} ]";

        private static OutputChannels[] DigitalOutputChannels => new[]
        {
            OutputChannels.DIG_OUT_1,
            OutputChannels.DIG_OUT_2,
            OutputChannels.DIG_OUT_3,
            OutputChannels.DIG_OUT_4,
            OutputChannels.DIG_OUT_5
        };

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HenkieF16FuelFlowIndicatorHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var legacyPath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkieF16FuelFlowIndicator.config");
                var unifiedPath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkieF16FuelFlowHardwareSupportModule.config");
                var hsmConfig = HenkieF16FuelFlowIndicatorHardwareSupportModuleConfig.Load(legacyPath);

                // Try the unified-schema calibration file (authored by the
                // SimLinkup Profile Editor). When present, its breakpoints
                // override the legacy <CalibrationData> block. See
                // HenkieF16FuelFlowHardwareSupportModuleConfig.cs for the
                // rationale and round-trip contract.
                var unifiedCalibration = TryLoadUnifiedCalibration(unifiedPath);

                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        if (unifiedCalibration != null)
                        {
                            deviceConfiguration.CalibrationData = unifiedCalibration;
                        }
                        var hsmInstance = new HenkieF16FuelFlowIndicatorHardwareSupportModule(
                            deviceConfiguration, legacyPath, unifiedPath);
                        toReturn.Add(hsmInstance);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            return toReturn.ToArray();
        }

        // Load + map the unified-schema calibration file's breakpoints onto
        // the existing Henkie.Common.CalibrationPoint shape. Returns null
        // when the file is absent, malformed, or doesn't contain the
        // expected channel — in any of those cases the caller falls back
        // to whatever's already on `deviceConfiguration.CalibrationData`
        // (typically the legacy <CalibrationData> block).
        private static CalibrationPoint[] TryLoadUnifiedCalibration(string unifiedPath)
        {
            try
            {
                if (!File.Exists(unifiedPath)) return null;
                var unifiedConfig = GaugeCalibrationConfig.Load<HenkieF16FuelFlowHardwareSupportModuleConfig>(unifiedPath);
                if (unifiedConfig == null) return null;
                // Synthetic channel id agreed with the editor
                // (src/js/gauges/henkie-fuelflow.js). The id is a round-trip
                // handle — not a real HSM port — so the editor and SimLinkup
                // can name the calibration table without inventing extra
                // signals on the gauge.
                var ch = unifiedConfig.FindChannel("HenkieF16FuelFlow_FuelFlow_To_Indicator");
                var bps = ch?.Transform?.Breakpoints;
                if (bps == null || bps.Length < 2) return null;
                // Editor writes <Point input="..." output="..."/> — both are
                // raw doubles, no volts/DAC conversion needed.
                var result = new CalibrationPoint[bps.Length];
                for (var i = 0; i < bps.Length; i++)
                {
                    result[i] = new CalibrationPoint(bps[i].Input, bps[i].Output);
                }
                return result;
            }
            catch (Exception e)
            {
                Log.Warn("Failed to load unified-schema calibration; falling back to legacy <CalibrationData>: " + e.Message);
                return null;
            }
        }

        // Watch both calibration files. Either changing triggers a reload of
        // just the calibration table — we never re-touch device identity or
        // stator angles at runtime (those would require disconnecting and
        // re-programming the hardware, which is the wrong thing to do
        // mid-flight). The watcher mirrors the pattern used by every other
        // HSM the editor authored a config for (Lilbern, AMI, Astronautics,
        // Simtek, …).
        // Hot-reload setup. Two ConfigFileReloadWatchers — one per
        // config file. Both invoke ReloadCalibration on change; the
        // helper's internal mtime dedup keeps duplicate events from
        // firing the reload twice. Unlike the per-watcher patterns
        // we used to have, the helper handles file-not-yet-existing
        // (it will fire Created when the editor first writes the
        // unified file) and self-heals from Windows watcher orphan
        // scenarios via periodic resubscribe.
        private void StartConfigWatchers()
        {
            try
            {
                if (!string.IsNullOrEmpty(_legacyConfigPath))
                {
                    _legacyConfigWatcher = new ConfigFileReloadWatcher(_legacyConfigPath, ReloadCalibration);
                }
            }
            catch (Exception e) { Log.Error(e.Message, e); }
            try
            {
                if (!string.IsNullOrEmpty(_unifiedConfigPath))
                {
                    _unifiedConfigWatcher = new ConfigFileReloadWatcher(_unifiedConfigPath, ReloadCalibration);
                }
            }
            catch (Exception e) { Log.Error(e.Message, e); }
        }

        // Re-evaluate calibration without touching the live device connection.
        // Preference: unified file if present (editor's source of truth);
        // legacy file's <CalibrationData> otherwise.
        private void ReloadCalibration()
        {
            CalibrationPoint[] next = null;
            try
            {
                next = TryLoadUnifiedCalibration(_unifiedConfigPath);
                if (next == null && !string.IsNullOrEmpty(_legacyConfigPath) && File.Exists(_legacyConfigPath))
                {
                    var legacy = HenkieF16FuelFlowIndicatorHardwareSupportModuleConfig.Load(_legacyConfigPath);
                    var dev = legacy?.Devices?.FirstOrDefault(d =>
                        d != null && string.Equals(d.Address, _deviceConfig?.Address, StringComparison.OrdinalIgnoreCase));
                    if (dev?.CalibrationData != null && dev.CalibrationData.Length >= 2)
                    {
                        next = dev.CalibrationData;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Calibration reload failed; keeping previous table: " + e.Message);
                return;
            }
            if (next != null && next.Length >= 2)
            {
                _calibrationData = next;
            }
        }

        private static int ChannelNumber(OutputChannels outputChannel)
        {
            var lastCharOfChannelName = outputChannel.ToString().Substring(outputChannel.ToString().Length - 1, 1);
            int.TryParse(lastCharOfChannelName, out var channelNumber);
            return channelNumber > 0 ? channelNumber : 8;
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            ConfigureStatorBaseAngles();
            ConfigureDiagnosticLEDBehavior();
            ConfigureOutputChannels();
            ConfigureCalibration();
        }

        private void ConfigureDeviceConnection()
        {
            try
            {
                if (
                    _deviceConfig?.ConnectionType != null &&
                    _deviceConfig.ConnectionType.Value == ConnectionType.USB &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.COMPort)
                )
                {
                    ConfigureUSBConnection();
                }
                else if (
                    _deviceConfig?.ConnectionType != null &&
                    _deviceConfig.ConnectionType.Value == ConnectionType.PHCC &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.COMPort) &&
                    !string.IsNullOrWhiteSpace(_deviceConfig.Address)
                )
                {
                    ConfigurePhccConnection();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureDiagnosticLEDBehavior()
        {
            if (_fuelFlowDeviceInterface == null) return;

            var diagnosticLEDBehavior = _deviceConfig?.DiagnosticLEDMode !=null && 
                _deviceConfig.DiagnosticLEDMode.HasValue
                ? _deviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _fuelFlowDeviceInterface.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureOutputChannels()
        {
            if (_fuelFlowDeviceInterface == null) return;

            try
            {
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_1,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_1));
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_2,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_2));
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_3,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_3));
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_4,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_4));
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_5,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_5));
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigurePhccConnection()
        {
            if
            (
                _deviceConfig?.ConnectionType == null ||
                _deviceConfig.ConnectionType.Value != ConnectionType.PHCC ||
                string.IsNullOrWhiteSpace(_deviceConfig.COMPort) || string.IsNullOrWhiteSpace(_deviceConfig.Address)
            )
            {
                return;
            }

            var addressString = (_deviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;

            _deviceAddress = addressByte;
            var comPort = (_deviceConfig.COMPort ?? "").Trim();

            try
            {
                var phccDevice = new p.Device(comPort, false);
                _fuelFlowDeviceInterface = new Device(phccDevice, addressByte);
                _fuelFlowDeviceInterface.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_fuelFlowDeviceInterface);
                _fuelFlowDeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureStatorBaseAngles()
        {
            if (_fuelFlowDeviceInterface == null) return;

            var s1StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S1BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 0.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            var s2StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S2BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 120.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            var s3StatorBaseAngle = _deviceConfig?.StatorBaseAnglesConfig?.S3BaseAngleDegrees / 360.000 *
                                    Device.STATOR_ANGLE_MAX_OFFSET ?? 240.0 / 360.000 * Device.STATOR_ANGLE_MAX_OFFSET;


            try
            {
                _fuelFlowDeviceInterface.SetStatorBaseAngle(StatorSignals.S1, (short)s1StatorBaseAngle);
                _fuelFlowDeviceInterface.SetStatorBaseAngle(StatorSignals.S2, (short)s2StatorBaseAngle);
                _fuelFlowDeviceInterface.SetStatorBaseAngle(StatorSignals.S3, (short)s3StatorBaseAngle);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureCalibration()
        {
            if (_fuelFlowDeviceInterface == null || _deviceConfig?.CalibrationData == null) return;
            _calibrationData = _deviceConfig?.CalibrationData;
            _fuelFlowDeviceInterface.SetPosition(0);
        }

        private void ConfigureUSBConnection()
        {
            if (
                _deviceConfig?.ConnectionType == null || _deviceConfig.ConnectionType.Value !=
                ConnectionType.USB &&
                string.IsNullOrWhiteSpace(_deviceConfig.COMPort)
            )
            {
                return;
            }

            var addressString = (_deviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;
            _deviceAddress = addressByte;

            try
            {
                var comPort = _deviceConfig.COMPort;
                _fuelFlowDeviceInterface = new Device(comPort);
                _fuelFlowDeviceInterface.DisableWatchdog();
                _fuelFlowDeviceInterface.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_fuelFlowDeviceInterface);
                _fuelFlowDeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private DigitalSignal CreateOutputSignalForOutputChannelConfiguredAsDigital(int channelNumber)
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"DIG_OUT_{channelNumber} (0=OFF, 1=ON)",
                Id = $"HenkieF16FuelFlow[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_OUT_{channelNumber}",
                Index = channelNumber,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = OutputChannelInitialValue(OutputChannel(channelNumber)) 
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _positionOutputSignal = CreatePositionOutputSignal();
            _digitalOutputs = CreateOutputSignalsForDigitalOutputChannels();
        }

        private List<DigitalSignal> CreateOutputSignalsForDigitalOutputChannels()
        {
            return DigitalOutputChannels
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsDigital(ChannelNumber(x)))
                .ToList();
        }

        private AnalogSignal CreatePositionOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Indicator Position",
                FriendlyName = "Indicator Position (0-4095)",
                Id = $"HenkieF16FuelFlow[{"0x" + _deviceAddress.ToString("X").PadLeft(2, '0')}]__Indicator_Position",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 4095
            };
            return thisSignal;
        }

        private void OutputSignalForDigitalOutputChannel_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_fuelFlowDeviceInterface == null) return;
            var signal = (DigitalSignal)sender;
            var channelNumber = signal.Index;
            var outputChannel = OutputChannel(channelNumber);
            try
            {
                _fuelFlowDeviceInterface.SetDigitalOutputChannelValue(outputChannel, args.CurrentState);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void SetPosition(ushort requestedPosition)
        {
            if (_fuelFlowDeviceInterface == null) return;
            try
            {
                _fuelFlowDeviceInterface.SetPosition((short)requestedPosition);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private static OutputChannels OutputChannel(int? channelNumber)
        {
            if (!channelNumber.HasValue) return OutputChannels.Unknown;
            if (channelNumber.Value == 1) return OutputChannels.DIG_OUT_1;
            if (channelNumber.Value == 2) return OutputChannels.DIG_OUT_2;
            if (channelNumber.Value == 3) return OutputChannels.DIG_OUT_3;
            if (channelNumber.Value == 4) return OutputChannels.DIG_OUT_4;
            if (channelNumber.Value == 5) return OutputChannels.DIG_OUT_5;
            return OutputChannels.Unknown;
        }

        private bool OutputChannelInitialValue(OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_1:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_1?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_2:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_2?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_3:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_3?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_4:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_4?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_5:
                    return _deviceConfig?.OutputChannelsConfig?.DIG_OUT_5?.InitialValue ?? false;

            }
            return false;
        }

        private void PositionOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_positionOutputSignal != null)
            {
                var requestedPosition = _positionOutputSignal.State;
                SetPosition((ushort)requestedPosition);
            }
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.FuelFlowPoundsPerHour = (float)_fuelFlowInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void FuelFlow_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateFuelFlowOutputValues();
        }

        private AnalogSignal CreateFuelFlowInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Fuel Flow (Pounds Per Hour)",
                Id = "HenkieF16FuelFlow_FuelFlow_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 80000
            };
            return thisSignal;
        }

        private void CreateInputSignals()
        {
            _fuelFlowInputSignal = CreateFuelFlowInputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForEvents();
                    Common.Util.DisposeObject(_fuelFlowDeviceInterface);
                    Common.Util.DisposeObject(_renderer);
                    if (_legacyConfigWatcher != null)
                    {
                        try { _legacyConfigWatcher.Dispose(); } catch { }
                        _legacyConfigWatcher = null;
                    }
                    if (_unifiedConfigWatcher != null)
                    {
                        try { _unifiedConfigWatcher.Dispose(); } catch { }
                        _unifiedConfigWatcher = null;
                    }
                }
            }
            _isDisposed = true;
        }

        private void RegisterForEvents()
        {
            if (_fuelFlowInputSignal != null)
            {
                _fuelFlowInputSignal.SignalChanged += FuelFlow_InputSignalChanged;
            }
            if (_positionOutputSignal != null)
            {
                _positionOutputSignal.SignalChanged += PositionOutputSignal_SignalChanged;
            }
            foreach (var digitalSignal in _digitalOutputs)
            {
                digitalSignal.SignalChanged += OutputSignalForDigitalOutputChannel_SignalChanged;
            }

        }

        private void UnregisterForEvents()
        {

            if (_fuelFlowInputSignal != null)
            {
                try
                {
                    _fuelFlowInputSignal.SignalChanged -= FuelFlow_InputSignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_positionOutputSignal != null)
            {
                try
                {
                    _positionOutputSignal.SignalChanged -= PositionOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
            foreach (var digitalSignal in _digitalOutputs)
            {
                try
                {
                    digitalSignal.SignalChanged -= OutputSignalForDigitalOutputChannel_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
        }
        private ushort CalibratedPosition(double fuelFlow)
        {
            ushort toReturn = 0;
            if (_calibrationData == null)
            {
                toReturn = (ushort)((fuelFlow / 80000.0) * 4095.0);
            }
            else
            {

                var lowerPoint = _calibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= fuelFlow) ??
                                 new CalibrationPoint(0, 0);
                var upperPoint =
                    _calibrationData
                        .OrderBy(x => x.Input)
                        .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(80000, 4095);
                var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
                var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
                var inputPct = inputRange != 0
                    ? (fuelFlow - lowerPoint.Input) / inputRange
                    : 1.00;
                toReturn = (ushort)((inputPct * outputRange) + lowerPoint.Output);
            }
            if (toReturn < 0) toReturn = 0;
            if (toReturn > 4095) toReturn = 4095;
            return toReturn;

        }
        private void UpdateFuelFlowOutputValues()
        {
            if (_fuelFlowInputSignal != null)
            {
                var fuelFlowFromSim= _fuelFlowInputSignal.State;
                if (fuelFlowFromSim >80000.0)
                {
                    fuelFlowFromSim = 80000;
                }
                else if (fuelFlowFromSim <0)
                {
                    fuelFlowFromSim = 0;
                }
                var positionOutput = CalibratedPosition(fuelFlowFromSim);
                _positionOutputSignal.State=positionOutput;
            }
        }
    }
}