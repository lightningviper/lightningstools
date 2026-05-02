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
    //Simtek 10-1084 F-16 Standby ADI (pitch piecewise_resolver + roll resolver + OFF flag digital_invert)
    public class Simtek101084HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101084HardwareSupportModule));
        private readonly IStandbyADI _renderer = new StandbyADI();

        // Editor-authored calibration. Same hot-reload contract as the other
        // gauges, but with three independent overrides:
        //   - pitch (piecewise_resolver pair): _pitchTransform + _pitchSinChannel + _pitchCosChannel
        //   - roll  (resolver pair):           _rollTransform  + _rollSinChannel  + _rollCosChannel
        //   - OFF flag (digital_invert):       _offFlagChannel (carries Invert bool)
        // Each may be null independently — Update*OutputValues falls through
        // to the hardcoded path for any unconfigured channel.
        private Simtek101084HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _pitchTransform;
        private GaugeChannelConfig _pitchSinChannel;
        private GaugeChannelConfig _pitchCosChannel;
        private GaugeTransformConfig _rollTransform;
        private GaugeChannelConfig _rollSinChannel;
        private GaugeChannelConfig _rollCosChannel;
        private GaugeChannelConfig _offFlagChannel;

        // Caged-rest behaviour. Picked once at construction time when the
        // editor enabled it on the SIN channel; nulled out when disabled.
        // While the OFF flag input is TRUE, the HSM drives sin/cos × peak
        // V to the picked rest angle instead of evaluating the transform
        // — modelling a real standby ADI's gimbal-locked random rest
        // position when the gyro is at rest. See GaugeChannelConfig
        // CagedRestEnabled / CagedRestRangeMin/MaxDegrees for the schema.
        // Static Random shared across HSM instances so two ADIs in the
        // same profile get independent rests instead of all picking the
        // same seed.
        private static readonly Random _cagedRestRandom = new Random();
        private double? _pitchCagedRestAngleDegrees;
        private double? _rollCagedRestAngleDegrees;

        private ConfigFileReloadWatcher _configWatcher;

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

        public Simtek101084HardwareSupportModule(Simtek101084HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Resolve all three channel groups in one pass. Each pair-resolver
        // returns null when the config doesn't supply a usable transform;
        // the per-method override branches check for null individually.
        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            ResolvePiecewiseResolverPair(config,
                "101084_Pitch_SIN_To_Instrument",
                "101084_Pitch_COS_To_Instrument",
                out _pitchTransform, out _pitchSinChannel, out _pitchCosChannel);
            ResolvePiecewiseResolverPair(config,
                "101084_Roll_SIN_To_Instrument",
                "101084_Roll_COS_To_Instrument",
                out _rollTransform, out _rollSinChannel, out _rollCosChannel);
            // Caged-rest random angles. Picked from the SIN channel of each
            // resolver pair — that's where the editor writes the cage
            // settings (the COS channel only carries trim). Defaults
            // ±20° pitch, ±40° roll match the editor's defaults so a
            // user who enables the feature without supplying a range gets
            // sensible behaviour.
            _pitchCagedRestAngleDegrees = PickCagedRestAngle(_pitchSinChannel, defaultRangeDegrees: 20.0);
            _rollCagedRestAngleDegrees  = PickCagedRestAngle(_rollSinChannel,  defaultRangeDegrees: 40.0);
            _offFlagChannel = config != null
                ? config.FindChannel("101084_OFF_Flag_To_Instrument")
                : null;
            // Only honour the OFF flag config if it's actually a digital_invert
            // record with the Invert field set. Otherwise fall through to the
            // hardcoded behaviour.
            if (_offFlagChannel != null
                && (_offFlagChannel.Transform == null
                    || _offFlagChannel.Transform.Kind != "digital_invert"
                    || !_offFlagChannel.Invert.HasValue))
            {
                // Channel record exists but isn't a usable digital_invert
                // override — null it out so UpdateOFFFlagOutputValue takes
                // the hardcoded path. Note: the Transform may be missing
                // entirely (the editor writes <Transform kind="digital_invert">
                // with an empty body); we treat absent kind as "use hardcoded"
                // to avoid honouring a malformed config.
                if (_offFlagChannel.Transform == null
                    || _offFlagChannel.Transform.Kind != "digital_invert")
                {
                    _offFlagChannel = null;
                }
            }
        }

        // Same resolver-pair resolver as Simtek 10-1088. See that class for
        // the full contract: returns null transform when the config doesn't
        // carry a usable resolver, and the HSM falls back to the hardcoded
        // path in that case.
        private static void ResolveResolverPair(
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
                || t.Kind != "resolver"
                || !t.InputMin.HasValue
                || !t.InputMax.HasValue
                || t.InputMax.Value <= t.InputMin.Value
                || !t.AngleMinDegrees.HasValue
                || !t.AngleMaxDegrees.HasValue
                || !t.PeakVolts.HasValue)
            {
                return;
            }
            transform = t;
            sinCh = s;
            cosCh = c;
        }

        // Pick a random caged-rest angle for one resolver pair, OR null
        // when the editor hasn't opted in. Picked once at construction
        // time; the same angle is used for as long as the OFF flag input
        // remains TRUE this session. Re-resolved on every config reload
        // (a hot-reload that toggles enabled / changes the range gives a
        // new random rest), but NOT re-rolled while enabled stays true
        // through the same reload.
        //
        // defaultRangeDegrees: the symmetric ±range used when CagedRest
        // is enabled but Min/Max aren't both supplied. Pitch defaults
        // to ±20°, roll defaults to ±40° per the editor's defaults.
        private static double? PickCagedRestAngle(GaugeChannelConfig sinChannel, double defaultRangeDegrees)
        {
            if (sinChannel == null) return null;
            if (!sinChannel.CagedRestEnabled.GetValueOrDefault(false)) return null;
            var min = sinChannel.CagedRestRangeMinDegrees ?? -defaultRangeDegrees;
            var max = sinChannel.CagedRestRangeMaxDegrees ??  defaultRangeDegrees;
            if (max < min)
            {
                // Defensive: degenerate range means the editor wrote
                // something nonsensical. Fall back to the default
                // symmetric range rather than throw / NaN at runtime.
                min = -defaultRangeDegrees;
                max =  defaultRangeDegrees;
            }
            // Min == Max: caller wants a fixed (non-random) rest angle.
            if (max == min) return min;
            return min + _cagedRestRandom.NextDouble() * (max - min);
        }

        // Same shape as ResolveResolverPair but for the piecewise_resolver
        // kind: SIN side carries a breakpoint table (input → angle°) plus
        // PeakVolts; COS side just points back via partnerChannel.
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
                var reloaded = Simtek101084HardwareSupportModuleConfig.Load(configFile);
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

        public override string FriendlyName => "Simtek P/N 10-1084 - Indicator - Simulated Standby Attitude";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101084HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101084HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101084HardwareSupportModule.config");
                hsmConfig = Simtek101084HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101084HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.PitchDegrees = (float) _pitchInputSignal.State;
            _renderer.InstrumentState.RollDegrees = (float) _rollInputSignal.State;

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
                Id = "101084_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
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
                Id = "101084_OFF_Flag_To_Instrument",
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
                Id = "101084_Pitch_COS_To_Instrument",
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
                Id = "101084_Pitch_From_Sim",
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
                Id = "101084_Pitch_SIN_To_Instrument",
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
                Id = "101084_Roll_COS_To_Instrument",
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
                Id = "101084_Roll_From_Sim",
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
                Id = "101084_Roll_SIN_To_Instrument",
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
            // When caged-rest is enabled, the OFF flag transition gates
            // pitch/roll output between the rest-angle path and the
            // configured transform path — so we have to re-evaluate
            // both whenever the flag toggles. No-op when caged-rest is
            // disabled (the rest-angle branch in UpdatePitchOutputValues /
            // UpdateRollOutputValues falls through immediately).
            if (_pitchCagedRestAngleDegrees.HasValue) UpdatePitchOutputValues();
            if (_rollCagedRestAngleDegrees.HasValue)  UpdateRollOutputValues();
        }

        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_pitchInputSignal != null)
            {
                _pitchInputSignal.SignalChanged += _pitchInputSignalChangedEventHandler;
            }
            if (_rollInputSignal != null)
            {
                _rollInputSignal.SignalChanged += _rollInputSignalChangedEventHandler;
            }
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
        }

        private void UnregisterForInputEvents()
        {
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
        }

        private void UpdateOFFFlagOutputValue()
        {
            // Editor-authored override: when a digital_invert config is set,
            // honour its Invert bool. Without it, fall through to the
            // hardcoded inversion (matches pre-config behaviour).
            if (_offFlagChannel != null && _offFlagChannel.Invert.HasValue)
            {
                _offFlagOutputSignal.State = _offFlagChannel.Invert.Value
                    ? !_offFlagInputSignal.State
                    : _offFlagInputSignal.State;
                return;
            }
            _offFlagOutputSignal.State = !_offFlagInputSignal.State;
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal == null) return;
            var pitchInputDegrees = _pitchInputSignal.State;

            // Caged-rest override: when the editor enabled CagedRest on
            // the pitch SIN channel AND the OFF flag input is currently
            // visible (gauge is caged / spinning down), drive sin/cos to
            // the random rest angle picked at construction time. Skips
            // the transform entirely for as long as the OFF flag stays
            // TRUE; falls through to the normal transform when the OFF
            // flag goes FALSE (gauge spun up + uncaged).
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

            // Editor-authored override: when a piecewise_resolver config is
            // set, evaluate via the generic helper and per-channel trim.
            // Falls through to the hardcoded if/else below when no config
            // is present.
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

            double pitchOutputRefAngleDegrees = 0;

            if (pitchInputDegrees >= 0 && pitchInputDegrees < 10)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees / 10) * 21.176;
            }
            else if (pitchInputDegrees >= 10 && pitchInputDegrees < 20)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees - 10) / 10) * 21.177) + 21.176;
            }
            else if (pitchInputDegrees >= 20 && pitchInputDegrees < 30)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees - 20) / 10) * 21.176) + 42.353;
            }
            else if (pitchInputDegrees >= 30 && pitchInputDegrees < 60)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees - 30) / 30) * 63.53) + 63.526;
            }
            else if (pitchInputDegrees >= 60 && pitchInputDegrees <= 90)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees - 60) / 30) * 67.76) + 127.059;
            }
            else if (pitchInputDegrees <= -60 && pitchInputDegrees >= -90)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees - -90) / 30) * 38.122) + 194.819;
            }
            else if (pitchInputDegrees <= -30 && pitchInputDegrees >= -60)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees + 60) / 30) * 63.53) + 232.941;
            }
            else if (pitchInputDegrees <= -20 && pitchInputDegrees >= -30)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees + 30) / 10) * 21.176) + 296.471;
            }
            else if (pitchInputDegrees <= -10 && pitchInputDegrees >= -20)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees + 20) / 10) * 21.177) + 317.647;
            }
            else if (pitchInputDegrees <= 0 && pitchInputDegrees >= -10)
            {
                pitchOutputRefAngleDegrees = (((pitchInputDegrees + 10) / 10) * 21.176) + 338.824;
            }


            var pitchSinOutputValue = 10.0000 * Math.Sin(pitchOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);
            var pitchCosOutputValue = 10.0000 * Math.Cos(pitchOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);

            if (_pitchSinOutputSignal != null)
            {
                if (pitchSinOutputValue < -10)
                {
                    pitchSinOutputValue = -10;
                }
                else if (pitchSinOutputValue > 10)
                {
                    pitchSinOutputValue = 10;
                }

                _pitchSinOutputSignal.State = pitchSinOutputValue;
            }

            if (_pitchCosOutputSignal == null) return;
            if (pitchCosOutputValue < -10)
            {
                pitchCosOutputValue = -10;
            }
            else if (pitchCosOutputValue > 10)
            {
                pitchCosOutputValue = 10;
            }

            _pitchCosOutputSignal.State = pitchCosOutputValue;
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

            // Editor-authored override: when a piecewise_resolver config is
            // set, evaluate via the generic helper and per-channel trim.
            // Falls through to the hardcoded sin/cos blocks below when no
            // config is present.
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

            var rollOutputRefAngleDegrees = rollInputDegrees;

            var rollSinOutputValue = 10.0000 * Math.Sin(rollOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);
            var rollCosOutputValue = 10.0000 * Math.Cos(rollOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);

            if (_rollSinOutputSignal != null)
            {
                if (rollSinOutputValue < -10)
                {
                    rollSinOutputValue = -10;
                }
                else if (rollSinOutputValue > 10)
                {
                    rollSinOutputValue = 10;
                }

                _rollSinOutputSignal.State = rollSinOutputValue;
            }

            if (_rollCosOutputSignal == null) return;
            if (rollCosOutputValue < -10)
            {
                rollCosOutputValue = -10;
            }
            else if (rollCosOutputValue > 10)
            {
                rollCosOutputValue = 10;
            }

            _rollCosOutputSignal.State = rollCosOutputValue;
        }

        // Final-resort clamp for the caged-rest path when the editor
        // hasn't supplied a SIN/COS GaugeChannelConfig with ApplyTrim.
        // The rest of the class uses inline ±10 V clamps; this is the
        // same logic factored out for the cage block.
        private static double Clamp10(double v)
        {
            if (v < -10) return -10;
            if (v > 10) return 10;
            return v;
        }
    }
}