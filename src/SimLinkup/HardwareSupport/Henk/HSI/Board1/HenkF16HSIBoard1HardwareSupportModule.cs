using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16.HSI;
using Henkie.HSI.Board1;
using System.IO;
using System.Linq;
using Henkie.Common;
using System.Globalization;
using log4net;
using p = Phcc;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board1
{
    //Henk HSI 
    public class HenkF16HSIBoard1HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(HenkF16HSIBoard1HardwareSupportModule));
        private readonly IHorizontalSituationIndicator _renderer = new HorizontalSituationIndicator();
        private bool _isDisposed;

        //INPUT SIGNALS
        private AnalogSignal _magneticHeadingInputSignal;
        private AnalogSignal _bearingInputSignal;
        private AnalogSignal _rangeInputSignal;
        private DigitalSignal _rangeInvalidFlagInputSignal;
        private AnalogSignal _courseInputSignal;
        private AnalogSignal _courseDeviationInputSignal;
        private AnalogSignal _courseDeviationLimitInputSignal;
        private DigitalSignal _deviationFlagInputSignal;
        private AnalogSignal _desiredHeadingFromSimInputSignal;
        private DigitalSignal _toFlagInputSignal;
        private DigitalSignal _fromFlagInputSignal;
        private DigitalSignal _offFlagInputSignal;

        //OUTPUT SIGNALS
        private DigitalSignal _rangeInvalidFlagOutputSignal;
        private AnalogSignal _magneticHeadingOutputSignal;
        private AnalogSignal _bearingOutputSignal;
        private AnalogSignal _rangeHundredsDigitOutputSignal;
        private AnalogSignal _rangeTensDigitOutputSignal;
        private AnalogSignal _rangeOnesDigitOutputSignal;
        private List<DigitalSignal> _digitalOutputs = new List<DigitalSignal>();

        //DEVICE CONFIG
        private Device _hsiBoard1DeviceInterface;
        private readonly DeviceConfig _hsiBoard1DeviceConfig;
        private byte _hsiBoard1DeviceAddress;

        private CalibrationPoint[] _headingCalibrationData;
        private CalibrationPoint[] _bearingCalibrationData;
        private CalibrationPoint[] _rangeOnesDigitCalibrationData;
        private CalibrationPoint[] _rangeTensDigitCalibrationData;
        private CalibrationPoint[] _rangeHundredsDigitCalibrationData;

        // Editor-authored calibration plumbing. Both files are watched —
        // the legacy file (HenkF16HSIBoard1HardwareSupportModule.config)
        // continues to own identity / stator offsets / DIG_OUTs; the
        // unified file (HenkF16HSIBoard1Calibration.config) carries
        // editor-authored breakpoint tables that, when present, override
        // the per-channel CalibrationData arrays on the legacy device
        // config. Hot-reload on either file re-runs ReloadCalibration
        // without touching the live device connection. See
        // HenkF16HSIBoard1CalibrationConfig.cs for the contract.
        private string _legacyConfigPath;
        private string _unifiedConfigPath;
        private ConfigFileReloadWatcher _legacyConfigWatcher;
        private ConfigFileReloadWatcher _unifiedConfigWatcher;

        private HenkF16HSIBoard1HardwareSupportModule(DeviceConfig hsiBoard1DeviceConfig, string legacyConfigPath, string unifiedConfigPath)
        {
            _hsiBoard1DeviceConfig = hsiBoard1DeviceConfig;
            _legacyConfigPath = legacyConfigPath;
            _unifiedConfigPath = unifiedConfigPath;
            if (_hsiBoard1DeviceConfig != null)
            {
                ConfigureDevice();
                CreateInputSignals();
                CreateOutputSignals();
                RegisterForEvents();
                StartConfigWatchers();
            }
        }

        public override AnalogSignal[] AnalogInputs => new[]
        {
            _magneticHeadingInputSignal,
            _desiredHeadingFromSimInputSignal,
            _courseInputSignal,
            _courseDeviationInputSignal,
            _courseDeviationLimitInputSignal,
            _bearingInputSignal,
            _rangeInputSignal,
        };

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _magneticHeadingOutputSignal,  
            _bearingOutputSignal,
            _rangeHundredsDigitOutputSignal, _rangeTensDigitOutputSignal, _rangeOnesDigitOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => new[]
        {
            _offFlagInputSignal, _deviationFlagInputSignal, _rangeInvalidFlagInputSignal, _toFlagInputSignal,
            _fromFlagInputSignal
        };

        private static OutputChannels[] DigitalOutputChannels => new[]
        {
            OutputChannels.DIG_OUT_1,
            OutputChannels.DIG_OUT_2,
        };

        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                return _digitalOutputs
                    .OrderBy(x => x.FriendlyName)
                    .ToArray();
            }
        }

        public override string FriendlyName =>
                    $"Henkie F-16 HSI Interface Board #1: 0x{_hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')} on {_hsiBoard1DeviceConfig.ConnectionType?.ToString() ?? "UNKNOWN"} [ {_hsiBoard1DeviceConfig.COMPort ?? "<UNKNOWN>"} ]";


        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();

            try
            {
                var legacyPath  = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkF16HSIBoard1HardwareSupportModule.config");
                var unifiedPath = Path.Combine(Util.CurrentMappingProfileDirectory, "HenkF16HSIBoard1Calibration.config");
                var hsmConfig = HenkieF16HSIBoard1HardwareSupportModuleConfig.Load(legacyPath);

                // Try the unified-schema calibration file (authored by the
                // SimLinkup Profile Editor). When present, its per-channel
                // breakpoint tables override the legacy file's
                // <*CalibrationData> blocks. See
                // HenkF16HSIBoard1CalibrationConfig.cs for the rationale.
                var unifiedOverrides = TryLoadUnifiedOverrides(unifiedPath);

                if (hsmConfig != null)
                {
                    foreach (var deviceConfiguration in hsmConfig.Devices)
                    {
                        if (unifiedOverrides != null)
                        {
                            ApplyUnifiedOverrides(deviceConfiguration, unifiedOverrides);
                        }
                        var hsmInstance = new HenkF16HSIBoard1HardwareSupportModule(deviceConfiguration, legacyPath, unifiedPath);
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

        // Per-channel override bag returned by TryLoadUnifiedOverrides.
        // Each entry is null when the unified file doesn't supply that
        // channel, or a CalibrationPoint[] otherwise. Bundling the five
        // channels into a single struct lets ApplyUnifiedOverrides /
        // ReloadCalibration each touch the legacy device-config in one
        // place rather than five.
        private class UnifiedOverrides
        {
            public CalibrationPoint[] Heading;
            public CalibrationPoint[] Bearing;
            public CalibrationPoint[] RangeOnesDigit;
            public CalibrationPoint[] RangeTensDigit;
            public CalibrationPoint[] RangeHundredsDigit;
        }

        private static UnifiedOverrides TryLoadUnifiedOverrides(string unifiedPath)
        {
            try
            {
                if (!File.Exists(unifiedPath)) return null;
                var unified = GaugeCalibrationConfig.Load<HenkF16HSIBoard1CalibrationConfig>(unifiedPath);
                if (unified == null) return null;
                var result = new UnifiedOverrides
                {
                    Heading            = ExtractChannelBreakpoints(unified, "Henk_F16_HSI_Board1_Magnetic_Heading_To_Instrument"),
                    Bearing            = ExtractChannelBreakpoints(unified, "Henk_F16_HSI_Board1_Bearing_To_Instrument"),
                    RangeOnesDigit     = ExtractChannelBreakpoints(unified, "Henk_F16_HSI_Board1_Range_x1_To_Instrument"),
                    RangeTensDigit     = ExtractChannelBreakpoints(unified, "Henk_F16_HSI_Board1_Range_x10_To_Instrument"),
                    RangeHundredsDigit = ExtractChannelBreakpoints(unified, "Henk_F16_HSI_Board1_Range_x100_To_Instrument"),
                };
                return result;
            }
            catch (Exception e)
            {
                Log.Warn("Failed to load unified-schema calibration; falling back to legacy <*CalibrationData> blocks: " + e.Message);
                return null;
            }
        }

        private static CalibrationPoint[] ExtractChannelBreakpoints(GaugeCalibrationConfig config, string channelId)
        {
            var ch = config.FindChannel(channelId);
            var bps = ch?.Transform?.Breakpoints;
            if (bps == null || bps.Length < 2) return null;
            // Editor writes <Point input=".." output=".."/> — both raw doubles.
            var result = new CalibrationPoint[bps.Length];
            for (var i = 0; i < bps.Length; i++)
            {
                result[i] = new CalibrationPoint(bps[i].Input, bps[i].Output);
            }
            return result;
        }

        // Apply non-null overrides onto the legacy device config in place.
        // Null entries leave the legacy field untouched (so users can
        // partially override — calibrate just heading from the editor and
        // leave the range digits' existing legacy tables intact).
        private static void ApplyUnifiedOverrides(DeviceConfig device, UnifiedOverrides overrides)
        {
            if (device == null || overrides == null) return;
            if (overrides.Heading            != null) device.HeadingCalibrationData            = overrides.Heading;
            if (overrides.Bearing            != null) device.BearingCalibrationData            = overrides.Bearing;
            if (overrides.RangeOnesDigit     != null) device.RangeOnesDigitCalibrationData     = overrides.RangeOnesDigit;
            if (overrides.RangeTensDigit     != null) device.RangeTensDigitCalibrationData     = overrides.RangeTensDigit;
            if (overrides.RangeHundredsDigit != null) device.RangeHundredsDigitCalibrationData = overrides.RangeHundredsDigit;
        }

        // Watch both calibration files. Either changing triggers a reload
        // of just the calibration tables — we never re-touch device identity
        // / stator offsets / DIG_OUTs at runtime (those would require
        // disconnecting and re-programming the hardware mid-flight). Same
        // pattern as HenkieF16FuelFlowIndicatorHardwareSupportModule.
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

        // Re-evaluate the five calibration tables without touching the live
        // device connection. Preference per channel: unified file's value
        // when present (editor's source of truth); otherwise the legacy
        // file's matching <*CalibrationData> block; otherwise the C#
        // fallback math (= input range scaled linearly into 0..1023).
        private void ReloadCalibration()
        {
            try
            {
                var unified = TryLoadUnifiedOverrides(_unifiedConfigPath);
                CalibrationPoint[] heading            = unified?.Heading;
                CalibrationPoint[] bearing            = unified?.Bearing;
                CalibrationPoint[] rangeOnesDigit     = unified?.RangeOnesDigit;
                CalibrationPoint[] rangeTensDigit     = unified?.RangeTensDigit;
                CalibrationPoint[] rangeHundredsDigit = unified?.RangeHundredsDigit;

                // For any channel the unified file didn't override, fall
                // back to whatever the legacy file currently says (it may
                // have been hand-edited since startup).
                if ((heading == null || bearing == null || rangeOnesDigit == null
                        || rangeTensDigit == null || rangeHundredsDigit == null)
                    && !string.IsNullOrEmpty(_legacyConfigPath)
                    && File.Exists(_legacyConfigPath))
                {
                    var legacy = HenkieF16HSIBoard1HardwareSupportModuleConfig.Load(_legacyConfigPath);
                    var dev = legacy?.Devices?.FirstOrDefault(d =>
                        d != null && string.Equals(d.Address, _hsiBoard1DeviceConfig?.Address, StringComparison.OrdinalIgnoreCase));
                    if (dev != null)
                    {
                        if (heading            == null) heading            = dev.HeadingCalibrationData;
                        if (bearing            == null) bearing            = dev.BearingCalibrationData;
                        if (rangeOnesDigit     == null) rangeOnesDigit     = dev.RangeOnesDigitCalibrationData;
                        if (rangeTensDigit     == null) rangeTensDigit     = dev.RangeTensDigitCalibrationData;
                        if (rangeHundredsDigit == null) rangeHundredsDigit = dev.RangeHundredsDigitCalibrationData;
                    }
                }

                // Atomic per-field replacement — the Calibrated*Value
                // helpers read these fields directly, so partial updates
                // are safe; the worst case is the next gauge tick using
                // a mix of old + new which is no worse than the prior
                // tick using all-old.
                _headingCalibrationData            = heading            ?? _headingCalibrationData;
                _bearingCalibrationData            = bearing            ?? _bearingCalibrationData;
                _rangeOnesDigitCalibrationData     = rangeOnesDigit     ?? _rangeOnesDigitCalibrationData;
                _rangeTensDigitCalibrationData     = rangeTensDigit     ?? _rangeTensDigitCalibrationData;
                _rangeHundredsDigitCalibrationData = rangeHundredsDigit ?? _rangeHundredsDigitCalibrationData;

                // Re-fire every Update*OutputValue with the cached input
                // values so the user sees new calibration immediately,
                // without waiting for the next sim tick.
                UpdateMagneticHeadingOutputValue();
                UpdateBearingOutputValue();
                UpdateRangeOutputValue();
            }
            catch (Exception e)
            {
                Log.Warn("Calibration reload failed; keeping previous tables: " + e.Message);
            }
        }

        private List<DigitalSignal> CreateOutputSignalsForDigitalOutputChannels()
        {
            return DigitalOutputChannels
                .Select(x => CreateOutputSignalForOutputChannelConfiguredAsDigital(ChannelNumber(x)))
                .ToList();
        }
        private DigitalSignal CreateOutputSignalForOutputChannelConfiguredAsDigital(int channelNumber)
        {
            var thisSignal = new DigitalSignal
            {

                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = $"DIG_OUT_{channelNumber} (0=OFF, 1=ON)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__DIG_OUT_{channelNumber}",
                Index = channelNumber,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = OutputChannelInitialValue(OutputChannel(channelNumber))
            };
            return thisSignal;
        }

        private void ConfigureDevice()
        {
            ConfigureDeviceConnection();
            ConfigureStatorOffsets();
            ConfigureDiagnosticLEDBehavior();
            ConfigureOutputChannels();
            ConfigureCalibration();
        }
        private void ConfigureStatorOffsets()
        {
            if (_hsiBoard1DeviceInterface == null || _hsiBoard1DeviceConfig?.StatorOffsetsConfig == null) return;


            //HEADING stator offsets
            try
            {
                var headingS1StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.HeadingS1Offset ?? 241;
                var headingS2StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.HeadingS2Offset ?? 582;
                var headingS3StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.HeadingS3Offset ?? 923;

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)headingS1StatorOffset);
                _hsiBoard1DeviceInterface.LoadHeadingOffsetStatorCoilMask(StatorSignals.S1);
                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)headingS2StatorOffset);
                _hsiBoard1DeviceInterface.LoadHeadingOffsetStatorCoilMask(StatorSignals.S2);
                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)headingS3StatorOffset);
                _hsiBoard1DeviceInterface.LoadHeadingOffsetStatorCoilMask(StatorSignals.S3);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            //BEARING stator offsets
            try
            {
                var bearingS1StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.BearingS1Offset ?? 241;
                var bearingS2StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.BearingS2Offset ?? 582;
                var bearingS3StatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.BearingS3Offset ?? 923;

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)bearingS1StatorOffset);
                _hsiBoard1DeviceInterface.LoadBearingOffsetStatorCoilMask(StatorSignals.S1);
                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)bearingS2StatorOffset);
                _hsiBoard1DeviceInterface.LoadBearingOffsetStatorCoilMask(StatorSignals.S2);
                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)bearingS3StatorOffset);
                _hsiBoard1DeviceInterface.LoadBearingOffsetStatorCoilMask(StatorSignals.S3);

            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            //RANGE "ones" digit stator offsets
            try
            {
                var rangeOnesDigitXStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeOnesDigitXOffset ?? 85;
                var rangeOnesDigitYStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeOnesDigitYOffset ?? 171;

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeOnesDigitXStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_ONES_DIGIT_X);
                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeOnesDigitYStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_ONES_DIGIT_Y);

            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            //RANGE "tens" digit stator offsets
            try
            {
                var rangeTensDigitXStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeTensDigitXOffset ?? 85;
                var rangeTensDigitYStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeTensDigitYOffset ?? 171;

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeTensDigitXStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_TENS_DIGIT_X);

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeTensDigitYStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_TENS_DIGIT_Y);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            //RANGE "hundreds" digit stator offsets
            try
            {
                var rangeHundredsDigitXStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeHundredsDigitXOffset ?? 85;
                var rangeHundredsDigitYStatorOffset = _hsiBoard1DeviceConfig?.StatorOffsetsConfig?.RangeHundredsDigitYOffset ?? 171;

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeHundredsDigitXStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_HUNDREDS_DIGIT_X);

                _hsiBoard1DeviceInterface.SetStatorOffsetCoilMask((short)rangeHundredsDigitYStatorOffset);
                _hsiBoard1DeviceInterface.LoadRangeOffsetStatorCoilMask(RangeDigitStatorCoils.RANGE_HUNDREDS_DIGIT_Y);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

        }
        private void ConfigureCalibration()
        {
            if (_hsiBoard1DeviceInterface == null) return;
            _headingCalibrationData = _hsiBoard1DeviceConfig?.HeadingCalibrationData;
            _bearingCalibrationData = _hsiBoard1DeviceConfig?.BearingCalibrationData;
            _rangeOnesDigitCalibrationData = _hsiBoard1DeviceConfig?.RangeOnesDigitCalibrationData;
            _rangeTensDigitCalibrationData = _hsiBoard1DeviceConfig?.RangeTensDigitCalibrationData;
            _rangeHundredsDigitCalibrationData = _hsiBoard1DeviceConfig?.RangeHundredsDigitCalibrationData;
        }

        private void ConfigureDeviceConnection()
        {
            try
            {
                if (
                    _hsiBoard1DeviceConfig?.ConnectionType != null &&
                    _hsiBoard1DeviceConfig.ConnectionType.Value == ConnectionType.USB &&
                    !string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.COMPort)
                )
                {
                    ConfigureUSBConnection();
                }
                else if (
                    _hsiBoard1DeviceConfig?.ConnectionType != null &&
                    _hsiBoard1DeviceConfig.ConnectionType.Value == ConnectionType.PHCC &&
                    !string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.COMPort) &&
                    !string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.Address)
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
            if (_hsiBoard1DeviceInterface == null) return;

            var diagnosticLEDBehavior = _hsiBoard1DeviceConfig?.DiagnosticLEDMode != null &&
                _hsiBoard1DeviceConfig.DiagnosticLEDMode.HasValue
                ? _hsiBoard1DeviceConfig.DiagnosticLEDMode.Value
                : DiagnosticLEDMode.Heartbeat;
            try
            {
                _hsiBoard1DeviceInterface.ConfigureDiagnosticLEDBehavior(diagnosticLEDBehavior);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        private void ConfigureOutputChannels()
        {
            if (_hsiBoard1DeviceInterface == null) return;

            try
            {
                _hsiBoard1DeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_1,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_1));
                _hsiBoard1DeviceInterface.SetDigitalOutputChannelValue(OutputChannels.DIG_OUT_2,
                    OutputChannelInitialValue(OutputChannels.DIG_OUT_2));
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        private static int ChannelNumber(OutputChannels outputChannel)
        {
            var lastCharOfChannelName = outputChannel.ToString().Substring(outputChannel.ToString().Length - 1, 1);
            int.TryParse(lastCharOfChannelName, out var channelNumber);
            return channelNumber > 0 ? channelNumber : 0;
        }
        private static OutputChannels OutputChannel(int? channelNumber)
        {
            if (!channelNumber.HasValue) return OutputChannels.Unknown;
            if (channelNumber.Value == 1) return OutputChannels.DIG_OUT_1;
            if (channelNumber.Value == 2) return OutputChannels.DIG_OUT_2;
            return OutputChannels.Unknown;
        }
        private bool OutputChannelInitialValue(OutputChannels outputChannel)
        {
            switch (outputChannel)
            {
                case OutputChannels.DIG_OUT_1:
                    return _hsiBoard1DeviceConfig?.OutputChannelsConfig?.DIG_OUT_1?.InitialValue ?? false;
                case OutputChannels.DIG_OUT_2:
                    return _hsiBoard1DeviceConfig?.OutputChannelsConfig?.DIG_OUT_2?.InitialValue ?? false;
            }
            return false;
        }
        private void ConfigurePhccConnection()
        {
            if
            (
                _hsiBoard1DeviceConfig?.ConnectionType == null ||
                _hsiBoard1DeviceConfig.ConnectionType.Value != ConnectionType.PHCC ||
                string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.COMPort) || string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.Address)
            )
            {
                return;
            }

            var addressString = (_hsiBoard1DeviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;

            _hsiBoard1DeviceAddress = addressByte;
            var comPort = (_hsiBoard1DeviceConfig.COMPort ?? "").Trim();

            try
            {
                var phccDevice = new p.Device(comPort, false);
                _hsiBoard1DeviceInterface = new Device(phccDevice, addressByte);
                _hsiBoard1DeviceInterface.DisableWatchdog();
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_hsiBoard1DeviceInterface);
                _hsiBoard1DeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        private void ConfigureUSBConnection()
        {
            if (
                _hsiBoard1DeviceConfig?.ConnectionType == null || _hsiBoard1DeviceConfig.ConnectionType.Value !=
                ConnectionType.USB &&
                string.IsNullOrWhiteSpace(_hsiBoard1DeviceConfig.COMPort)
            )
            {
                return;
            }

            var addressString = (_hsiBoard1DeviceConfig.Address ?? "").ToLowerInvariant().Replace("0x", string.Empty).Trim();
            var addressIsValid = byte.TryParse(addressString, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var addressByte);
            if (!addressIsValid) return;
            _hsiBoard1DeviceAddress = addressByte;

            try
            {
                var comPort = _hsiBoard1DeviceConfig.COMPort;
                _hsiBoard1DeviceInterface = new Device(comPort);
                _hsiBoard1DeviceInterface.DisableWatchdog();
                _hsiBoard1DeviceInterface.ConfigureUsbDebug(false);
            }
            catch (Exception e)
            {
                Common.Util.DisposeObject(_hsiBoard1DeviceInterface);
                _hsiBoard1DeviceInterface = null;
                Log.Error(e.Message, e);
            }
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BearingToBeaconDegrees = (float)_bearingInputSignal.CorrelatedState;
            _renderer.InstrumentState.CourseDeviationDegrees = (float)_courseDeviationInputSignal.State;
            _renderer.InstrumentState.CourseDeviationLimitDegrees = (float)_courseDeviationLimitInputSignal.State;
            _renderer.InstrumentState.DesiredCourseDegrees = (int)_courseInputSignal.CorrelatedState;
            _renderer.InstrumentState.DesiredHeadingDegrees = (int)_desiredHeadingFromSimInputSignal.CorrelatedState;
            _renderer.InstrumentState.DeviationInvalidFlag = _deviationFlagInputSignal.State;
            _renderer.InstrumentState.DistanceToBeaconNauticalMiles = (float)_rangeInputSignal.State;
            _renderer.InstrumentState.DmeInvalidFlag = _rangeInvalidFlagInputSignal.State;
            _renderer.InstrumentState.FromFlag = _fromFlagInputSignal.State;
            _renderer.InstrumentState.MagneticHeadingDegrees = (float)_magneticHeadingInputSignal.CorrelatedState;
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.ShowToFromFlag = true;
            _renderer.InstrumentState.ToFlag = _toFlagInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void CreateInputSignals()
        {
            _offFlagInputSignal = CreateOffFlagInputSignal();
            _deviationFlagInputSignal = CreateDeviationFlagInputSignal();
            _rangeInvalidFlagInputSignal = CreateRangeInvalidFlagInputSignal();
            _toFlagInputSignal = CreateToFlagInputSignal();
            _fromFlagInputSignal = CreateFromFlagInputSignal();
            _magneticHeadingInputSignal = CreateMagneticHeadingInputSignal();
            _desiredHeadingFromSimInputSignal = CreateDesiredHeadingFromSimInputSignal();
            _courseInputSignal = CreateCourseInputSignal();
            _bearingInputSignal = CreateBearingInputSignal();
            _rangeInputSignal = CreateRangeInputSignal();
            _courseDeviationInputSignal = CreateCourseDeviationInputSignal();
            _courseDeviationLimitInputSignal = CreateCourseDeviationLimitInputSignal();
        }

        private AnalogSignal CreateMagneticHeadingInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Current Heading (from sim)",
                Id = $"Henk_F16_HSI_Board1__Magnetic_Heading_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateDesiredHeadingFromSimInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Desired Heading (from sim)",
                Id = $"Henk_F16_HSI_Board1__Desired_Heading_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }


        private AnalogSignal CreateBearingInputSignal()
        {

            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Bearing to Beacon (from sim)",
                Id = $"Henk_F16_HSI_Board1__Bearing_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course (from sim)",
                Id = $"Henk_F16_HSI_Board1__Course_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course Deviation (from sim)",
                Id = $"Henk_F16_HSI_Board1__Course_Deviation_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCourseDeviationLimitInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Course Deviation Limit (from sim)",
                Id = $"Henk_F16_HSI_Board1__Course_Deviation_Limit_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 10
            };
            return thisSignal;
        }
        private DigitalSignal CreateDeviationFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Deviation Invalid Flag (from sim)",
                Id = $"Henk_F16_HSI_Board1__Deviation_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateRangeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Range (from sim)",
                Id = $"Henk_F16_HSI_Board1__Range_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 999.9999
            };
            return thisSignal;
        }

        private DigitalSignal CreateRangeInvalidFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Range Invalid Flag (from sim)",
                Id = $"Henk_F16_HSI_Board1__Range_Invalid_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateToFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "TO flag (from sim)",
                Id = $"Henk_F16_HSI_Board1__TO_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateFromFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "FROM Flag (from sim)",
                Id = $"Henk_F16_HSI_Board1__FROM_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateOffFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF flag (from sim)",
                Id = $"Henk_F16_HSI_Board1__Off_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }




        private AnalogSignal CreateMagneticHeadingOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Magnetic Heading (0-1023)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Magnetic_Heading_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private AnalogSignal CreateBearingOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Bearing to Beacon (0-1023)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Bearing_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private AnalogSignal CreateRangex100OutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Range x100 (0-1023)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Range_x100_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private AnalogSignal CreateRangex10OutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Range x10 (0-1023)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Range_x10_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private AnalogSignal CreateRangex1OutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Range x1 (0-1023)",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Range_x1_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = false,
                IsSine = false,
                MinValue = 0,
                MaxValue = 1023
            };
            return thisSignal;
        }

        private DigitalSignal CreateRangeInvalidFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "Range Invalid Flag",
                Id = $"Henk_F16_HSI_Board1[{"0x" + _hsiBoard1DeviceAddress.ToString("X").PadLeft(2, '0')}]__Range_Invalid_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }
        
        private void CreateOutputSignals()
        {
            _digitalOutputs = CreateOutputSignalsForDigitalOutputChannels();

            _bearingOutputSignal = CreateBearingOutputSignal();

            _magneticHeadingOutputSignal = CreateMagneticHeadingOutputSignal();

            _rangeHundredsDigitOutputSignal = CreateRangex100OutputSignal();
            _rangeTensDigitOutputSignal = CreateRangex10OutputSignal();
            _rangeOnesDigitOutputSignal = CreateRangex1OutputSignal();
            _rangeInvalidFlagOutputSignal = CreateRangeInvalidFlagOutputSignal();
            _digitalOutputs.Add(_rangeInvalidFlagOutputSignal);
        }

        private void RegisterForEvents()
        {
            if (_magneticHeadingInputSignal != null)
            {
                _magneticHeadingInputSignal.SignalChanged += HSI_Directional_InputSignalsChanged;
            }
            if (_bearingInputSignal != null)
            {
                _bearingInputSignal.SignalChanged += HSI_Directional_InputSignalsChanged;
            }
            if (_rangeInputSignal != null)
            {
                _rangeInputSignal.SignalChanged += Range_InputSignalChanged;
            }
            if (_rangeInvalidFlagInputSignal != null)
            {
                _rangeInvalidFlagInputSignal.SignalChanged += RangeInvalidFlag_InputSignalChanged;
            }

            if (_magneticHeadingOutputSignal != null)
            {
                _magneticHeadingOutputSignal.SignalChanged += MagneticHeadingOutputSignal_SignalChanged;
            }

            if (_bearingOutputSignal != null)
            {
                _bearingOutputSignal.SignalChanged += BearingOutputSignal_SignalChanged;
            }

            if (_rangeOnesDigitOutputSignal != null)
            {
                _rangeOnesDigitOutputSignal.SignalChanged += RangeOnesDigitOutputSignal_SignalChanged;
            }

            if (_rangeTensDigitOutputSignal != null)
            {
                _rangeTensDigitOutputSignal.SignalChanged += RangeTensDigitOutputSignal_SignalChanged;
            }

            if (_rangeHundredsDigitOutputSignal != null)
            {
                _rangeHundredsDigitOutputSignal.SignalChanged += RangeHundredsDigitOutputSignal_SignalChanged;
            }

            if (_rangeInvalidFlagOutputSignal != null)
            {
                _rangeInvalidFlagOutputSignal.SignalChanged += RangeInvalidFlagOutputSignal_SignalChanged;
            }

            foreach (var digitalSignal in _digitalOutputs)
            {
                digitalSignal.SignalChanged += OutputSignalForDigitalOutputChannel_SignalChanged;
            }

        }

        private void UnregisterForEvents()
        {

            if (_magneticHeadingInputSignal != null)
            {
                try
                {
                    _magneticHeadingInputSignal.SignalChanged -= HSI_Directional_InputSignalsChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_bearingInputSignal != null )
            {
                try
                {
                    _bearingInputSignal.SignalChanged -= HSI_Directional_InputSignalsChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_rangeInputSignal != null )
            {
                try
                {
                    _rangeInputSignal.SignalChanged -= Range_InputSignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_rangeInvalidFlagInputSignal != null )
            {
                try
                {
                    _rangeInvalidFlagInputSignal.SignalChanged -= RangeInvalidFlag_InputSignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_magneticHeadingOutputSignal != null)
            {
                try
                {
                    _magneticHeadingOutputSignal.SignalChanged -= MagneticHeadingOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_bearingOutputSignal != null)
            {
                try
                {
                    _bearingOutputSignal.SignalChanged -= BearingOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_rangeInvalidFlagOutputSignal != null) 
            {
                try
                {
                    _rangeInvalidFlagOutputSignal.SignalChanged -= RangeInvalidFlagOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_rangeOnesDigitOutputSignal != null)
            {
                try
                {
                    _rangeOnesDigitOutputSignal.SignalChanged -= RangeOnesDigitOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }

            if (_rangeTensDigitOutputSignal != null)
            {
                try
                {
                    _rangeTensDigitOutputSignal.SignalChanged -= RangeTensDigitOutputSignal_SignalChanged;
                }
                catch (RemotingException)
                {
                }
            }
            if (_rangeHundredsDigitOutputSignal != null)
            {
                try
                {
                    _rangeHundredsDigitOutputSignal.SignalChanged -= RangeHundredsDigitOutputSignal_SignalChanged;
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

        private short? _lastMagneticHeadingIndicationState = null;
        private void MagneticHeadingOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _magneticHeadingOutputSignal != null)
            {
                var newMagneticHeadingIndicationState = (short)_magneticHeadingOutputSignal.State;
                if (newMagneticHeadingIndicationState != _lastMagneticHeadingIndicationState)
                {
                    _hsiBoard1DeviceInterface.SetHeadingIndication(newMagneticHeadingIndicationState);
                    _lastMagneticHeadingIndicationState = newMagneticHeadingIndicationState;
                }
            }
        }

        private short? _lastBearingIndicationState = null;
        private void BearingOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _bearingOutputSignal != null)
            {
                var newBearingIndicationState = (short)_bearingOutputSignal.State;
                if (newBearingIndicationState != _lastBearingIndicationState)
                {
                    _hsiBoard1DeviceInterface.SetBearingIndication(newBearingIndicationState);
                    _lastBearingIndicationState = newBearingIndicationState;
                }
            }
        }

        private short? _lastRangeOnesDigitIndicationState = null;
        private void RangeOnesDigitOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _rangeOnesDigitOutputSignal != null)
            {
                var newRangeOnesDigitIndicationState = (short)_rangeOnesDigitOutputSignal.State;
                if (newRangeOnesDigitIndicationState != _lastRangeOnesDigitIndicationState)
                {
                    _hsiBoard1DeviceInterface.SetRangeOnesDigitIndication(newRangeOnesDigitIndicationState);
                    _lastRangeOnesDigitIndicationState = newRangeOnesDigitIndicationState;
                }
            }
        }

        private short? _lastRangeTensDigitIndicationState = null;
        private void RangeTensDigitOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _rangeTensDigitOutputSignal != null)
            {
                var newRangeTensDigitIndicationState = (short)_rangeTensDigitOutputSignal.State;
                if (newRangeTensDigitIndicationState != _lastRangeTensDigitIndicationState)
                {
                    _hsiBoard1DeviceInterface.SetRangeTensDigitIndication(newRangeTensDigitIndicationState);
                    _lastRangeTensDigitIndicationState = newRangeTensDigitIndicationState;
                }
            }
        }

        private short? _lastRangeHundredsDigitIndicationState = null;
        private void RangeHundredsDigitOutputSignal_SignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _rangeHundredsDigitOutputSignal != null)
            {
                var newRangeHundredsDigitIndicationState = (short)_rangeHundredsDigitOutputSignal.State;
                if (newRangeHundredsDigitIndicationState != _lastRangeHundredsDigitIndicationState)
                {
                    _hsiBoard1DeviceInterface.SetRangeHundredsDigitIndication(newRangeHundredsDigitIndicationState);
                    _lastRangeHundredsDigitIndicationState = newRangeHundredsDigitIndicationState;
                }
            }
        }

        private void RangeInvalidFlagOutputSignal_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface != null && _rangeInvalidFlagOutputSignal != null)
            {
                _hsiBoard1DeviceInterface.SetRangeInvalidIndicatorVisible(_rangeInvalidFlagOutputSignal.State);
            }
        }

        private void OutputSignalForDigitalOutputChannel_SignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (_hsiBoard1DeviceInterface == null) return;
            var signal = (DigitalSignal)sender;
            var channelNumber = signal.Index;
            var outputChannel = OutputChannel(channelNumber);
            try
            {
                if (outputChannel > 0)
                {
                    _hsiBoard1DeviceInterface.SetDigitalOutputChannelValue(outputChannel, args.CurrentState);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
        
        private void Range_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRangeOutputValue();
        }

        private void RangeInvalidFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateRangeInvalidFlagOutputValue();
        }

        private void HSI_Directional_InputSignalsChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateDirectionalOutputs();
        }
        
        private void UpdateDirectionalOutputs()
        {
            UpdateMagneticHeadingOutputValue();
            UpdateBearingOutputValue();
        }
        
        private void UpdateBearingOutputValue()
        {
            if (_bearingInputSignal == null || _bearingOutputSignal == null || _magneticHeadingInputSignal == null) 
            {
                return;
            }
            var bearingToBeaconDegrees = double.IsInfinity(_bearingInputSignal.CorrelatedState) || double.IsNaN(_bearingInputSignal.CorrelatedState)
                ? 0
                : _bearingInputSignal.CorrelatedState;
            var magneticHeadingDegrees = double.IsInfinity(_magneticHeadingInputSignal.CorrelatedState) || double.IsNaN(_magneticHeadingInputSignal.CorrelatedState)
                ? 0
                : _magneticHeadingInputSignal.CorrelatedState % 360.00;

            var adjustedBearingValue = -(magneticHeadingDegrees - bearingToBeaconDegrees) % 360.00;
            if (adjustedBearingValue <0) adjustedBearingValue = 360.00 - Math.Abs(adjustedBearingValue);
            _bearingOutputSignal.State = CalibratedBearingValue(adjustedBearingValue);
        }

        private void UpdateMagneticHeadingOutputValue()
        {
            if (_magneticHeadingInputSignal == null || _magneticHeadingOutputSignal == null )
            {
                return;
            }

            var magneticHeadingDegrees = double.IsInfinity(_magneticHeadingInputSignal.CorrelatedState) ||double.IsNaN(_magneticHeadingInputSignal.CorrelatedState) 
                ? 0
                : _magneticHeadingInputSignal.CorrelatedState % 360.00;
            _magneticHeadingOutputSignal.State = CalibratedHeadingValue(magneticHeadingDegrees);
        }

        private void UpdateRangeOutputValue()
        {
            const double MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES = 999.9999d;
            if (_rangeInputSignal == null || _rangeHundredsDigitOutputSignal == null || 
                _rangeTensDigitOutputSignal == null || _rangeOnesDigitOutputSignal == null )
            {
                return;
            }
            var distanceToBeaconNauticalMiles = Math.Abs(double.IsInfinity(_rangeInputSignal.State) ||
                                                         double.IsNaN(_rangeInputSignal.State)
                ? 0
                : _rangeInputSignal.State);
            if (distanceToBeaconNauticalMiles > MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES)
            {
                distanceToBeaconNauticalMiles = MAX_DISTANCE_TO_BEACON_NAUTICAL_MILES;
            }
            var distanceToBeaconString = $"{distanceToBeaconNauticalMiles:000}";

            var rangeHundredsDigit = byte.Parse(distanceToBeaconString.Substring(0, 1));
            var rangeTensDigit = byte.Parse(distanceToBeaconString.Substring(1, 1));
            var rangeOnesDigit = byte.Parse(distanceToBeaconString.Substring(2, 1));

            _rangeHundredsDigitOutputSignal.State = CalibratedRangeHundredsDigitValue(rangeHundredsDigit);
            _rangeTensDigitOutputSignal.State = CalibratedRangeTensDigitValue(rangeTensDigit);
            _rangeOnesDigitOutputSignal.State = CalibratedRangeOnesDigitValue(rangeOnesDigit);

        }

        private void UpdateRangeInvalidFlagOutputValue()
        {
            if (_rangeInvalidFlagInputSignal != null && _rangeInvalidFlagOutputSignal != null)
            {
                _rangeInvalidFlagOutputSignal.State = _rangeInvalidFlagInputSignal.State;
            }
        }

        private ushort CalibratedHeadingValue(double heading)
        {
            if (double.IsNaN(heading) || double.IsInfinity(heading)) heading = 0;
            if (_headingCalibrationData == null) return (ushort)((heading / 360.0) * 1023.0);

            var lowerPoint = _headingCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= heading) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _headingCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(360.0, 1023.0);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (heading - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        private ushort CalibratedBearingValue(double bearing)
        {
            if (double.IsInfinity(bearing) || double.IsNaN(bearing)) bearing = 0;
            if (_bearingCalibrationData == null) return (ushort)((bearing / 360.0) * 1023.0);

            var lowerPoint = _bearingCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= bearing) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _bearingCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(360.0, 1023.0);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input) * 1.00;
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output) * 1.00;
            var inputPct = inputRange != 0
                ? (bearing - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        private ushort CalibratedRangeOnesDigitValue(double rangeOnesDigit)
        {
            if (_rangeOnesDigitCalibrationData == null) return (ushort)((rangeOnesDigit / 10.0) * 1023.0);

            var lowerPoint = _rangeOnesDigitCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= rangeOnesDigit) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _rangeOnesDigitCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(10.0, 1023.0);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (rangeOnesDigit - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        private ushort CalibratedRangeTensDigitValue(double rangeTensDigit)
        {
            if (_rangeTensDigitCalibrationData == null) return (ushort)((rangeTensDigit / 10.0) * 1023.0);

            var lowerPoint = _rangeTensDigitCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= rangeTensDigit) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _rangeTensDigitCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(10.0, 1023.0);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (rangeTensDigit - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        private ushort CalibratedRangeHundredsDigitValue(double rangeHundredsDigit)
        {
            if (_rangeHundredsDigitCalibrationData == null) return (ushort)((rangeHundredsDigit / 10.0) * 1023.0);

            var lowerPoint = _rangeHundredsDigitCalibrationData.OrderBy(x => x.Input).LastOrDefault(x => x.Input <= rangeHundredsDigit) ??
                             new CalibrationPoint(0, 0);
            var upperPoint =
                _rangeHundredsDigitCalibrationData
                    .OrderBy(x => x.Input)
                    .FirstOrDefault(x => x != lowerPoint && x.Input >= lowerPoint.Input) ?? new CalibrationPoint(10.0, 1023.0);
            var inputRange = Math.Abs(upperPoint.Input - lowerPoint.Input);
            var outputRange = Math.Abs(upperPoint.Output - lowerPoint.Output);
            var inputPct = inputRange != 0
                ? (rangeHundredsDigit - lowerPoint.Input) / inputRange
                : 1.00;
            return (ushort)((inputPct * outputRange) + lowerPoint.Output);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForEvents();
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

        ~HenkF16HSIBoard1HardwareSupportModule()
        {
            Dispose(false);
        }

    }
}