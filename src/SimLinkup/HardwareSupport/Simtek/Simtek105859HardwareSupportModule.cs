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
    //Simtek 10-5859 F-16 Standby Attitude Indicator
    // (pitch piecewise_resolver + roll piecewise_resolver)
    //
    // Per the spec sheet (PL20-5859, dwg 10-5859):
    //   Drive type: DUAL DC SERVO
    //   Signal input: 10 VDC × sin/cos of reference angle
    //   Pitch table: 11 points 0°..±90° → 0..338.82° non-linearly
    //   Roll table:  9 points 90°L..0..90°R → 270..90° linearly
    //
    // Modeled on Simtek101084HardwareSupportModule (the other F-16 standby
    // ADI in the catalog). Both pitch and roll are encoded as
    // piecewise_resolver — pitch is genuinely non-linear; roll is linear
    // but expressed as a piecewise table for editor consistency and to
    // give users the ability to correct local synchro drift.
    //
    // Renderer reuse: borrows IStandbyADI from 10-1084. The visual
    // preview will look like the other standby ADI; calibration and
    // signal routing are independent.
    public class Simtek105859HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek105859HardwareSupportModule));
        private readonly IStandbyADI _renderer = new StandbyADI();

        // Editor-authored calibration. Two independent overrides:
        //   - pitch (piecewise_resolver pair)
        //   - roll  (piecewise_resolver pair)
        // Each may be null independently — Update*OutputValues falls
        // through to the hardcoded path for any unconfigured channel.
        private Simtek105859HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _pitchTransform;
        private GaugeChannelConfig _pitchSinChannel;
        private GaugeChannelConfig _pitchCosChannel;
        private GaugeTransformConfig _rollTransform;
        private GaugeChannelConfig _rollSinChannel;
        private GaugeChannelConfig _rollCosChannel;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;

        private AnalogSignal _pitchCosOutputSignal;
        private AnalogSignal _pitchInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _pitchInputSignalChangedEventHandler;
        private AnalogSignal _pitchSinOutputSignal;
        private AnalogSignal _rollCosOutputSignal;
        private AnalogSignal _rollInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rollInputSignalChangedEventHandler;
        private AnalogSignal _rollSinOutputSignal;

        public Simtek105859HardwareSupportModule(Simtek105859HardwareSupportModuleConfig config)
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
                "105859_Pitch_SIN_To_Instrument",
                "105859_Pitch_COS_To_Instrument",
                out _pitchTransform, out _pitchSinChannel, out _pitchCosChannel);
            ResolvePiecewiseResolverPair(config,
                "105859_Roll_SIN_To_Instrument",
                "105859_Roll_COS_To_Instrument",
                out _rollTransform, out _rollSinChannel, out _rollCosChannel);
        }

        // SIN side carries the shared transform body (breakpoints + PeakVolts);
        // COS side carries only its own per-channel trim. Returns null
        // transform when the config doesn't carry a usable record; HSM falls
        // back to the hardcoded path in that case.
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
                var reloaded = Simtek105859HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate both outputs with cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven loop won't fire until the
                // simulator next pushes a new pitch or roll value.
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

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-5859 - Indicator - Standby Attitude";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek105859HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek105859HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek105859HardwareSupportModule.config");
                hsmConfig = Simtek105859HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek105859HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.PitchDegrees = (float) _pitchInputSignal.State;
            _renderer.InstrumentState.RollDegrees = (float) _rollInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = null;
            _rollInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = pitch_InputSignalChanged;
            _rollInputSignalChangedEventHandler = roll_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _pitchInputSignal = CreatePitchInputSignal();
            _rollInputSignal = CreateRollInputSignal();
        }

        private void CreateOutputSignals()
        {
            _pitchSinOutputSignal = CreatePitchSinOutputSignal();
            _pitchCosOutputSignal = CreatePitchCosOutputSignal();
            _rollSinOutputSignal = CreateRollSinOutputSignal();
            _rollCosOutputSignal = CreateRollCosOutputSignal();
        }

        private AnalogSignal CreatePitchInputSignal()
        {
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Pitch (Degrees)",
                Id = "105859_Pitch_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -90,
                MaxValue = 90
            };
        }

        private AnalogSignal CreateRollInputSignal()
        {
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Roll (Degrees)",
                Id = "105859_Roll_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -180,
                MaxValue = 180
            };
        }

        private AnalogSignal CreatePitchSinOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (SIN)",
                Id = "105859_Pitch_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
        }

        private AnalogSignal CreatePitchCosOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (COS)",
                Id = "105859_Pitch_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00,
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
        }

        private AnalogSignal CreateRollSinOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (SIN)",
                Id = "105859_Roll_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
        }

        private AnalogSignal CreateRollCosOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (COS)",
                Id = "105859_Roll_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00,
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
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

        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
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
        }

        private void UnregisterForInputEvents()
        {
            if (_pitchInputSignalChangedEventHandler != null && _pitchInputSignal != null)
            {
                try { _pitchInputSignal.SignalChanged -= _pitchInputSignalChangedEventHandler; }
                catch (RemotingException) { }
            }
            if (_rollInputSignalChangedEventHandler != null && _rollInputSignal != null)
            {
                try { _rollInputSignal.SignalChanged -= _rollInputSignalChangedEventHandler; }
                catch (RemotingException) { }
            }
        }

        // Pitch piecewise_resolver. Reference angle table from PL20-5859
        // Table 1 (sheet 5). Hardcoded path matches the editor's defaults
        // exactly via linear interpolation between adjacent test points.
        // Outside the [-90, +90] range the table clamps to the endpoint
        // angle (±90° both reach the inverted "DOT" position at 194.82°).
        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal == null) return;
            var pitchInputDegrees = _pitchInputSignal.State;

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

            // Hardcoded fallback: same per-segment linear interpolation as
            // 10-1084 (the angles match the spec to within rounding).
            double refAngle;
            if (pitchInputDegrees >= 0 && pitchInputDegrees < 10)
                refAngle = (pitchInputDegrees / 10.0) * 21.18;
            else if (pitchInputDegrees >= 10 && pitchInputDegrees < 20)
                refAngle = (((pitchInputDegrees - 10) / 10.0) * (42.35 - 21.18)) + 21.18;
            else if (pitchInputDegrees >= 20 && pitchInputDegrees < 30)
                refAngle = (((pitchInputDegrees - 20) / 10.0) * (63.53 - 42.35)) + 42.35;
            else if (pitchInputDegrees >= 30 && pitchInputDegrees < 60)
                refAngle = (((pitchInputDegrees - 30) / 30.0) * (127.10 - 63.53)) + 63.53;
            else if (pitchInputDegrees >= 60 && pitchInputDegrees <= 90)
                refAngle = (((pitchInputDegrees - 60) / 30.0) * (194.82 - 127.10)) + 127.10;
            else if (pitchInputDegrees < 0 && pitchInputDegrees >= -10)
                refAngle = (((pitchInputDegrees + 10) / 10.0) * (360.00 - 338.82)) + 338.82;
            else if (pitchInputDegrees < -10 && pitchInputDegrees >= -20)
                refAngle = (((pitchInputDegrees + 20) / 10.0) * (338.82 - 317.65)) + 317.65;
            else if (pitchInputDegrees < -20 && pitchInputDegrees >= -30)
                refAngle = (((pitchInputDegrees + 30) / 10.0) * (317.65 - 296.47)) + 296.47;
            else if (pitchInputDegrees < -30 && pitchInputDegrees >= -60)
                refAngle = (((pitchInputDegrees + 60) / 30.0) * (296.47 - 232.94)) + 232.94;
            else if (pitchInputDegrees < -60 && pitchInputDegrees >= -90)
                refAngle = (((pitchInputDegrees + 90) / 30.0) * (232.94 - 194.82)) + 194.82;
            else
                refAngle = pitchInputDegrees > 90 ? 194.82 : 194.82;

            var sin = 10.0 * Math.Sin(refAngle * Constants.RADIANS_PER_DEGREE);
            var cos = 10.0 * Math.Cos(refAngle * Constants.RADIANS_PER_DEGREE);
            _pitchSinOutputSignal.State = Clamp(sin);
            _pitchCosOutputSignal.State = Clamp(cos);
        }

        // Roll piecewise_resolver. Reference angle table from PL20-5859
        // Table 2 (sheet 6). Roll left → angle 270..360 (CCW from 12
        // o'clock); roll right → 0..90 (CW). Encoded monotonically in
        // the editor as 270..450 (right side wraps past 360).
        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal == null) return;
            var rollInputDegrees = _rollInputSignal.State;

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

            // Hardcoded fallback: roll input in degrees == reference angle
            // (linear 1:1 mapping). The Table 2 mapping (90L→270, 0→0,
            // 90R→90) is identical to "reference angle = -roll degrees mod
            // 360" for roll left being negative input — we treat the C#
            // input convention as positive=right, negative=left, so
            // reference angle = roll degrees works for both directions
            // (90L = -90° input → -90° angle = 270°; 90R = +90° input
            // → 90° angle).
            var refAngle = rollInputDegrees;
            var sin = 10.0 * Math.Sin(refAngle * Constants.RADIANS_PER_DEGREE);
            var cos = 10.0 * Math.Cos(refAngle * Constants.RADIANS_PER_DEGREE);
            _rollSinOutputSignal.State = Clamp(sin);
            _rollCosOutputSignal.State = Clamp(cos);
        }

        private static double Clamp(double v)
        {
            if (v < -10) return -10;
            if (v > 10) return 10;
            return v;
        }
    }
}
