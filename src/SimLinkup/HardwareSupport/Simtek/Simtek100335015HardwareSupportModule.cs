using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;
using log4net;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0335-01 F-16 Standby ADI
    public class Simtek100335015HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100335015HardwareSupportModule));
        private readonly IStandbyADI _renderer = new StandbyADI();

        private bool _isDisposed;

        private DigitalSignal _offFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _offFlagInputSignalChangedEventHandler;
        private DigitalSignal _offFlagOutputSignal;
        private AnalogSignal _pitchCosOutputSignal;
        private AnalogSignal _pitchInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _pitchInputSignalChangedEventHandler;
        private AnalogSignal _pitchSinOutputSignal;
        private AnalogSignal _rollCosOutputSignal;
        private AnalogSignal _rollInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rollInputSignalChangedEventHandler;
        private AnalogSignal _rollSinOutputSignal;

        // Editor-authored calibration. Three override surfaces:
        //   - pitch (piecewise_resolver pair): _pitchTransform + _pitchSinChannel + _pitchCosChannel
        //   - roll  (piecewise_resolver pair): _rollTransform  + _rollSinChannel  + _rollCosChannel
        //   - OFF flag (digital_invert):       _offFlagChannel (carries Invert bool)
        // Any may be null independently — Update*OutputValues falls through
        // to the hardcoded path for any unconfigured channel.
        private Simtek100335015HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _pitchTransform;
        private GaugeChannelConfig _pitchSinChannel;
        private GaugeChannelConfig _pitchCosChannel;
        private GaugeTransformConfig _rollTransform;
        private GaugeChannelConfig _rollSinChannel;
        private GaugeChannelConfig _rollCosChannel;
        private GaugeChannelConfig _offFlagChannel;

        // Caged-rest behaviour. See Simtek 10-1084 for the full contract;
        // identical implementation. Picked once per HSM construction
        // (and re-rolled on hot-reload via ResolveAllChannels). Static
        // shared Random so two ADIs in the same profile get independent
        // rest angles.
        private static readonly Random _cagedRestRandom = new Random();
        private double? _pitchCagedRestAngleDegrees;
        private double? _rollCagedRestAngleDegrees;

        private ConfigFileReloadWatcher _configWatcher;

        public Simtek100335015HardwareSupportModule(Simtek100335015HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            ResolvePiecewiseResolverPair(config,
                "10033501_Pitch_SIN_To_Instrument",
                "10033501_Pitch_COS_To_Instrument",
                out _pitchTransform, out _pitchSinChannel, out _pitchCosChannel);
            ResolvePiecewiseResolverPair(config,
                "10033501_Roll_SIN_To_Instrument",
                "10033501_Roll_COS_To_Instrument",
                out _rollTransform, out _rollSinChannel, out _rollCosChannel);
            _pitchCagedRestAngleDegrees = PickCagedRestAngle(_pitchSinChannel, defaultRangeDegrees: 20.0);
            _rollCagedRestAngleDegrees  = PickCagedRestAngle(_rollSinChannel,  defaultRangeDegrees: 40.0);
            _offFlagChannel = config != null
                ? config.FindChannel("10033501_OFF_Flag_To_Instrument")
                : null;
            // Only honour the OFF flag config if it's actually a digital_invert
            // record with the Invert field set. Otherwise fall through to the
            // hardcoded behaviour (= same as 10-1084 pattern).
            if (_offFlagChannel != null
                && (_offFlagChannel.Transform == null
                    || _offFlagChannel.Transform.Kind != "digital_invert"
                    || !_offFlagChannel.Invert.HasValue))
            {
                if (_offFlagChannel.Transform == null
                    || _offFlagChannel.Transform.Kind != "digital_invert")
                {
                    _offFlagChannel = null;
                }
            }
        }

        // Pick a random caged-rest angle for one resolver pair, OR null
        // when the editor hasn't opted in. See Simtek 10-1084 for the
        // full contract; identical implementation here.
        private static double? PickCagedRestAngle(GaugeChannelConfig sinChannel, double defaultRangeDegrees)
        {
            if (sinChannel == null) return null;
            if (!sinChannel.CagedRestEnabled.GetValueOrDefault(false)) return null;
            var min = sinChannel.CagedRestRangeMinDegrees ?? -defaultRangeDegrees;
            var max = sinChannel.CagedRestRangeMaxDegrees ??  defaultRangeDegrees;
            if (max < min)
            {
                min = -defaultRangeDegrees;
                max =  defaultRangeDegrees;
            }
            if (max == min) return min;
            return min + _cagedRestRandom.NextDouble() * (max - min);
        }

        // SIN side carries a breakpoint table (input → angle°) plus
        // PeakVolts; COS side just points back via partnerChannel.
        // Same shape as Simtek 10-1084.
        private static void ResolvePiecewiseResolverPair(
            GaugeCalibrationConfig config,
            string sinChannelId,
            string cosChannelId,
            out GaugeTransformConfig transform,
            out GaugeChannelConfig sinCh,
            out GaugeChannelConfig cosCh)
        {
            transform = null;
            sinCh = null;
            cosCh = null;
            if (config == null) return;
            var s = config.FindChannel(sinChannelId);
            var c = config.FindChannel(cosChannelId);
            if (s == null || c == null) return;
            var t = s.Transform;
            if (t == null
                || t.Kind != "piecewise_resolver"
                || t.Breakpoints == null
                || t.Breakpoints.Length < 2
                || !t.PeakVolts.HasValue)
            {
                return;
            }
            transform = t;
            sinCh = s;
            cosCh = c;
        }

        private void StartConfigWatcher()
        {
            if (_config == null || string.IsNullOrEmpty(_config.FilePath)) return;
            try
            {
                _configWatcher = new ConfigFileReloadWatcher(_config.FilePath, ReloadConfig);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private void ReloadConfig()
        {
            try
            {
                var configFile = _config != null ? _config.FilePath : null;
                if (string.IsNullOrEmpty(configFile)) return;
                var reloaded = Simtek100335015HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateOFFFlagOutputValue();
                UpdatePitchOutputValues();
                UpdateRollOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_pitchInputSignal, _rollInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_pitchSinOutputSignal, _pitchCosOutputSignal, _rollSinOutputSignal, _rollCosOutputSignal};

        public override DigitalSignal[] DigitalInputs => new[] {_offFlagInputSignal};

        public override DigitalSignal[] DigitalOutputs => new[] {_offFlagOutputSignal};

        public override string FriendlyName => "Simtek P/N 10-0335-01 - Indicator - Simulated Standby Attitude";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100335015HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100335015HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100335015HardwareSupportModule.config");
                hsmConfig = Simtek100335015HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100335015HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.PitchDegrees = (float) _pitchInputSignal.State;
            _renderer.InstrumentState.RollDegrees = (float) _rollInputSignal.State;
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = null;
            _rollInputSignalChangedEventHandler = null;
            _offFlagInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler =
                pitch_InputSignalChanged;
            _rollInputSignalChangedEventHandler =
                roll_InputSignalChanged;
            _offFlagInputSignalChangedEventHandler =
                offFlag_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _pitchInputSignal = CreatePitchInputSignal();
            _rollInputSignal = CreateRollInputSignal();
            _offFlagInputSignal = CreateOFFFlagInputSignal();
        }

        private DigitalSignal CreateOFFFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag Visible (0=Hidden; 1=Visible)",
                Id = "10033501_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateOFFFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "OFF Flag Hidden (0=Visible; 1=Hidden)",
                Id = "10033501_OFF_Flag_To_Instrument",
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
            _pitchSinOutputSignal = CreatePitchSinOutputSignal();
            _pitchCosOutputSignal = CreatePitchCosOutputSignal();
            _rollSinOutputSignal = CreateRollSinOutputSignal();
            _rollCosOutputSignal = CreateRollCosOutputSignal();
            _offFlagOutputSignal = CreateOFFFlagOutputSignal();
        }

        private AnalogSignal CreatePitchCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (COS)",
                Id = "10033501_Pitch_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }


        private AnalogSignal CreatePitchInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Pitch (Degrees)",
                Id = "10033501_Pitch_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -90,
                MaxValue = 90
            };
            return thisSignal;
        }

        private AnalogSignal CreatePitchSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (SIN)",
                Id = "10033501_Pitch_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (COS)",
                Id = "10033501_Roll_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Roll (Degrees)",
                Id = "10033501_Roll_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -180,
                MaxValue = 180
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (SIN)",
                Id = "10033501_Roll_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_renderer);
                    if (_configWatcher != null)
                    {
                        try { _configWatcher.Dispose(); } catch { }
                        _configWatcher = null;
                    }
                }
            }
            _isDisposed = true;
        }

        private void offFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOFFFlagOutputValue();
            // OFF flag transitions gate the caged-rest path; re-evaluate
            // pitch/roll when it toggles. No-op when caged-rest is
            // disabled (the rest-angle branch falls through immediately).
            if (_pitchCagedRestAngleDegrees.HasValue) UpdatePitchOutputValues();
            if (_rollCagedRestAngleDegrees.HasValue)  UpdateRollOutputValues();
        }

        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
            if (_pitchInputSignal != null)
            {
                _pitchInputSignal.SignalChanged += _pitchInputSignalChangedEventHandler;
            }
            if (_rollInputSignal != null)
            {
                _rollInputSignal.SignalChanged += _rollInputSignalChangedEventHandler;
            }
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_offFlagInputSignalChangedEventHandler != null && _offFlagInputSignal != null)
            {
                try
                {
                    _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_pitchInputSignalChangedEventHandler != null && _pitchInputSignal != null)
            {
                try
                {
                    _pitchInputSignal.SignalChanged -= _pitchInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_rollInputSignalChangedEventHandler != null && _rollInputSignal != null)
            {
                try
                {
                    _rollInputSignal.SignalChanged -= _rollInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOFFFlagOutputValue()
        {
            if (_offFlagOutputSignal == null) return;
            // Editor override: Invert bool from the digital_invert config.
            if (_offFlagChannel != null && _offFlagChannel.Invert.HasValue)
            {
                _offFlagOutputSignal.State = _offFlagChannel.Invert.Value
                    ? !_offFlagInputSignal.State
                    : _offFlagInputSignal.State;
                return;
            }
            // Hardcoded fallback — match the original behaviour (always invert).
            _offFlagOutputSignal.State = !_offFlagInputSignal.State;
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal == null) return;
            var pitchInputDegrees = _pitchInputSignal.State;

            // Caged-rest override: when the editor enabled CagedRest on
            // the pitch SIN channel AND the OFF flag input is currently
            // visible, drive sin/cos to the random rest angle picked at
            // construction time. See Simtek 10-1084 for the contract.
            if (_pitchCagedRestAngleDegrees.HasValue
                && _offFlagInputSignal != null
                && _offFlagInputSignal.State
                && _pitchSinOutputSignal != null
                && _pitchCosOutputSignal != null)
            {
                var peakVolts = (_pitchTransform != null && _pitchTransform.PeakVolts.HasValue)
                    ? _pitchTransform.PeakVolts.Value
                    : 10.0;
                var rad = _pitchCagedRestAngleDegrees.Value * Constants.RADIANS_PER_DEGREE;
                var sin = peakVolts * Math.Sin(rad);
                var cos = peakVolts * Math.Cos(rad);
                _pitchSinOutputSignal.State = _pitchSinChannel != null
                    ? _pitchSinChannel.ApplyTrim(sin, _pitchSinOutputSignal.MinValue, _pitchSinOutputSignal.MaxValue)
                    : Clamp10(sin);
                _pitchCosOutputSignal.State = _pitchCosChannel != null
                    ? _pitchCosChannel.ApplyTrim(cos, _pitchCosOutputSignal.MinValue, _pitchCosOutputSignal.MaxValue)
                    : Clamp10(cos);
                return;
            }

            // Editor override: piecewise_resolver pair.
            if (_pitchTransform != null
                && _pitchSinChannel != null
                && _pitchCosChannel != null
                && _pitchSinOutputSignal != null
                && _pitchCosOutputSignal != null)
            {
                var t = _pitchTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    pitchInputDegrees, t.Breakpoints, t.PeakVolts.Value);
                _pitchSinOutputSignal.State = _pitchSinChannel.ApplyTrim(sinCos[0], _pitchSinOutputSignal.MinValue, _pitchSinOutputSignal.MaxValue);
                _pitchCosOutputSignal.State = _pitchCosChannel.ApplyTrim(sinCos[1], _pitchCosOutputSignal.MinValue, _pitchCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — straight 10·sin/cos(input°).
            var pitchSinOutputValue = 10.0000 * Math.Sin(pitchInputDegrees * Constants.RADIANS_PER_DEGREE);
            var pitchCosOutputValue = 10.0000 * Math.Cos(pitchInputDegrees * Constants.RADIANS_PER_DEGREE);

            if (_pitchSinOutputSignal != null)
            {
                if (pitchSinOutputValue < -10) pitchSinOutputValue = -10;
                else if (pitchSinOutputValue > 10) pitchSinOutputValue = 10;
                _pitchSinOutputSignal.State = pitchSinOutputValue;
            }

            if (_pitchCosOutputSignal != null)
            {
                if (pitchCosOutputValue < -10) pitchCosOutputValue = -10;
                else if (pitchCosOutputValue > 10) pitchCosOutputValue = 10;
                _pitchCosOutputSignal.State = pitchCosOutputValue;
            }
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal == null) return;
            var rollInputDegrees = _rollInputSignal.State;

            // Caged-rest override (see UpdatePitchOutputValues for the
            // contract; same pattern, roll-specific defaults).
            if (_rollCagedRestAngleDegrees.HasValue
                && _offFlagInputSignal != null
                && _offFlagInputSignal.State
                && _rollSinOutputSignal != null
                && _rollCosOutputSignal != null)
            {
                var peakVolts = (_rollTransform != null && _rollTransform.PeakVolts.HasValue)
                    ? _rollTransform.PeakVolts.Value
                    : 10.0;
                var rad = _rollCagedRestAngleDegrees.Value * Constants.RADIANS_PER_DEGREE;
                var sin = peakVolts * Math.Sin(rad);
                var cos = peakVolts * Math.Cos(rad);
                _rollSinOutputSignal.State = _rollSinChannel != null
                    ? _rollSinChannel.ApplyTrim(sin, _rollSinOutputSignal.MinValue, _rollSinOutputSignal.MaxValue)
                    : Clamp10(sin);
                _rollCosOutputSignal.State = _rollCosChannel != null
                    ? _rollCosChannel.ApplyTrim(cos, _rollCosOutputSignal.MinValue, _rollCosOutputSignal.MaxValue)
                    : Clamp10(cos);
                return;
            }

            // Editor override: piecewise_resolver pair.
            if (_rollTransform != null
                && _rollSinChannel != null
                && _rollCosChannel != null
                && _rollSinOutputSignal != null
                && _rollCosOutputSignal != null)
            {
                var t = _rollTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    rollInputDegrees, t.Breakpoints, t.PeakVolts.Value);
                _rollSinOutputSignal.State = _rollSinChannel.ApplyTrim(sinCos[0], _rollSinOutputSignal.MinValue, _rollSinOutputSignal.MaxValue);
                _rollCosOutputSignal.State = _rollCosChannel.ApplyTrim(sinCos[1], _rollCosOutputSignal.MinValue, _rollCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — straight 10·sin/cos(input°).
            var rollSinOutputValue = 10.0000 * Math.Sin(rollInputDegrees * Constants.RADIANS_PER_DEGREE);
            var rollCosOutputValue = 10.0000 * Math.Cos(rollInputDegrees * Constants.RADIANS_PER_DEGREE);

            if (_rollSinOutputSignal != null)
            {
                if (rollSinOutputValue < -10) rollSinOutputValue = -10;
                else if (rollSinOutputValue > 10) rollSinOutputValue = 10;
                _rollSinOutputSignal.State = rollSinOutputValue;
            }

            if (_rollCosOutputSignal != null)
            {
                if (rollCosOutputValue < -10) rollCosOutputValue = -10;
                else if (rollCosOutputValue > 10) rollCosOutputValue = 10;
                _rollCosOutputSignal.State = rollCosOutputValue;
            }
        }

        // Final-resort clamp for the caged-rest path when no SIN/COS
        // GaugeChannelConfig is supplied (so ApplyTrim isn't available).
        private static double Clamp10(double v)
        {
            if (v < -10) return -10;
            if (v > 10) return 10;
            return v;
        }
    }
}
