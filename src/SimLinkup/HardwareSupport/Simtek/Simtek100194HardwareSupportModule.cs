using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;
using log4net;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0194 F-16 Mach/Airspeed Indicator
    public class Simtek100194HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100194HardwareSupportModule));
        private readonly IAirspeedIndicator _renderer = new AirspeedIndicator();

        // Editor-authored calibration. Both channel overrides may be null
        // independently — UpdateAirspeedOutputValues uses the airspeed
        // breakpoint table when present; UpdateMachOutputValues applies the
        // Mach trim when present. Missing/malformed file = full hardcoded
        // behaviour preserved. Pattern matches ArduinoSeatHardwareSupportModule:
        // load + inject in GetInstances, store the whole config object, watch
        // the file for hot reload.
        //
        // Not `readonly` because the hot-reload handler reassigns these on
        // file change. Reads are atomic reference assignments — Update-
        // OutputValues sees either the old or new pointer, never a torn
        // value, so no locking is needed.
        //
        // _airspeedCalibration overrides the 43-knot piecewise table.
        // _machCalibration carries a piecewise table for the MACH REFERENCE
        // VOLTAGE (the value that feeds the cross-coupling math), plus the
        // usual ZeroTrim/Gain that apply to the final cross-coupled output.
        // The cross-coupled angle math itself (gauge geometry: 262°/V scale,
        // Mach 1.0 = 131° reference) stays hardcoded — that's not user-
        // editable hardware.
        private Simtek100194HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _airspeedCalibration;
        private GaugeChannelConfig _machCalibration;

        // Hot-reload plumbing. Same shape as ArduinoSeatHardwareSupportModule.
        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _airspeedInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _airspeedInputSignalChangedEventHandler;
        private AnalogSignal _airspeedOutputSignal;
        private bool _isDisposed;

        private AnalogSignal _machInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _machInputSignalChangedEventHandler;
        private AnalogSignal _machOutputSignal;

        // Public ctor takes the (optionally-null) config. Null = no editor
        // override; the existing hardcoded transforms handle everything.
        public Simtek100194HardwareSupportModule(Simtek100194HardwareSupportModuleConfig config)
        {
            _config = config;
            _airspeedCalibration = ResolvePiecewiseChannel(config, "100194_Airspeed_To_Instrument");
            // Mach: piecewise reference-voltage table + ZeroTrim/GainTrim.
            // We store the channel record regardless of its Transform shape
            // (a malformed file should still apply the trim fields and let
            // the hardcoded reference-voltage table run). UpdateMachOutputValues
            // checks the Transform shape before honouring the breakpoints.
            _machCalibration = config != null ? config.FindChannel("100194_Mach_To_Instrument") : null;
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Set up a ConfigFileReloadWatcher on the config file so editor
        // saves take effect within ~5 s on the running SimLinkup. Skipped
        // when no config was loaded (nothing to watch). The shared helper
        // handles the unreliable bits — silent watcher orphaning under
        // antivirus / OneDrive / SMB filter drivers, internal buffer
        // overflow, mtime dedup — so this HSM just supplies the reload
        // callback. See Common.HardwareSupport.Calibration.ConfigFileReloadWatcher.
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

        // Reload the config and re-resolve both channel pointers. Wrapped in
        // a try/catch so a partial-write race (the watcher can fire mid-save)
        // doesn't escape into the runtime — we just log and wait for the
        // next change event with a complete file.
        private void ReloadConfig()
        {
            try
            {
                var configFile = _config != null ? _config.FilePath : null;
                if (string.IsNullOrEmpty(configFile)) return;
                var reloaded = Simtek100194HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _airspeedCalibration = ResolvePiecewiseChannel(reloaded, "100194_Airspeed_To_Instrument");
                _machCalibration = reloaded.FindChannel("100194_Mach_To_Instrument");
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateAirspeedOutputValues();
                UpdateMachOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        // Same helper as Simtek100207: only return the channel when it carries
        // a usable piecewise table; null otherwise.
        private static GaugeChannelConfig ResolvePiecewiseChannel(GaugeCalibrationConfig config, string channelId)
        {
            if (config == null) return null;
            var ch = config.FindChannel(channelId);
            if (ch != null
                && ch.Transform != null
                && ch.Transform.Kind == "piecewise"
                && ch.Transform.Breakpoints != null
                && ch.Transform.Breakpoints.Length >= 2)
            {
                return ch;
            }
            return null;
        }

        public override AnalogSignal[] AnalogInputs => new[] {_machInputSignal, _airspeedInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_machOutputSignal, _airspeedOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0194 - Indicator, Simulated Mach Airspeed";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100194HardwareSupportModule()
        {
            Dispose(false);
        }

        // Match the ArduinoSeat pattern: build path, deserialize, inject the
        // possibly-null config. Missing/malformed file = null = hardcoded
        // behaviour preserved.
        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100194HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100194HardwareSupportModule.config");
                hsmConfig = Simtek100194HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    // Stash the path on the config so the instance's
                    // ConfigFileReloadWatcher knows which file to watch.
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100194HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            //_renderer.InstrumentState.AirspeedIndexKnots = _airspeedIndexInputSignal;
            //_renderer.InstrumentState.NeverExceedSpeedKnots = _neverExceedSpeedInputSignal;
            _renderer.InstrumentState.AirspeedKnots = (float) _airspeedInputSignal.State;
            _renderer.InstrumentState.MachNumber = (float) _machInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _machInputSignalChangedEventHandler = null;
            _airspeedInputSignalChangedEventHandler = null;
        }

        private void airspeed_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAirspeedOutputValues();
            UpdateMachOutputValues();
        }

        private AnalogSignal CreateAirspeedInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Airspeed",
                Id = "100194_Airspeed_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 850
            };
            return thisSignal;
        }

        private AnalogSignal CreateAirspeedOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Airspeed",
                Id = "100194_Airspeed_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _machInputSignalChangedEventHandler =
                mach_InputSignalChanged;
            _airspeedInputSignalChangedEventHandler =
                airspeed_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _machInputSignal = CreateMachInputSignal();
            _airspeedInputSignal = CreateAirspeedInputSignal();
        }

        private AnalogSignal CreateMachInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Mach",
                Id = "100194_Mach_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 2.4
            };
            return thisSignal;
        }

        private AnalogSignal CreateMachOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Mach",
                Id = "100194_Mach_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _machOutputSignal = CreateMachOutputSignal();
            _airspeedOutputSignal = CreateAirspeedOutputSignal();
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

        private void mach_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAirspeedOutputValues();
            UpdateMachOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_machInputSignal != null)
            {
                _machInputSignal.SignalChanged += _machInputSignalChangedEventHandler;
            }
            if (_airspeedInputSignal != null)
            {
                _airspeedInputSignal.SignalChanged += _airspeedInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_machInputSignalChangedEventHandler != null && _machInputSignal != null)
            {
                try
                {
                    _machInputSignal.SignalChanged -= _machInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_airspeedInputSignalChangedEventHandler == null || _airspeedInputSignal == null) return;
            try
            {
                _airspeedInputSignal.SignalChanged -= _airspeedInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateAirspeedOutputValues()
        {
            if (_airspeedInputSignal == null) return;
            var airspeedInput = _airspeedInputSignal.State;
            double airspeedOutputValue = 0;
            if (_airspeedOutputSignal == null) return;

            // Editor-authored override: when a .config file declared a
            // piecewise table for this channel, use the generic evaluator and
            // per-channel trim. Falls through to the hardcoded if/else below
            // when no config is present.
            if (_airspeedCalibration != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(airspeedInput, _airspeedCalibration.Transform.Breakpoints);
                _airspeedOutputSignal.State = _airspeedCalibration.ApplyTrim(v, _airspeedOutputSignal.MinValue, _airspeedOutputSignal.MaxValue);
                return;
            }

            if (airspeedInput < 0)
            {
                airspeedOutputValue = -10;
            }
            else if (airspeedInput >= 0 && airspeedInput < 80)
            {
                airspeedOutputValue = -10 + airspeedInput / 80.0 * 1.18;
            }
            else if (airspeedInput >= 80 && airspeedInput < 90)
            {
                airspeedOutputValue = -8.82 + (airspeedInput - 80) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 90 && airspeedInput < 100)
            {
                airspeedOutputValue = -8.24 + (airspeedInput - 90) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 100 && airspeedInput < 110)
            {
                airspeedOutputValue = -7.65 + (airspeedInput - 100) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 110 && airspeedInput < 120)
            {
                airspeedOutputValue = -7.06 + (airspeedInput - 110) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 120 && airspeedInput < 130)
            {
                airspeedOutputValue = -6.47 + (airspeedInput - 120) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 130 && airspeedInput < 140)
            {
                airspeedOutputValue = -5.88 + (airspeedInput - 130) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 140 && airspeedInput < 150)
            {
                airspeedOutputValue = -5.29 + (airspeedInput - 140) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 150 && airspeedInput < 160)
            {
                airspeedOutputValue = -4.71 + (airspeedInput - 150) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 160 && airspeedInput < 170)
            {
                airspeedOutputValue = -4.12 + (airspeedInput - 160) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 170 && airspeedInput < 180)
            {
                airspeedOutputValue = -3.53 + (airspeedInput - 170) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 180 && airspeedInput < 190)
            {
                airspeedOutputValue = -2.94 + (airspeedInput - 180) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 190 && airspeedInput < 200)
            {
                airspeedOutputValue = -2.35 + (airspeedInput - 190) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 200 && airspeedInput < 210)
            {
                airspeedOutputValue = -1.77 + (airspeedInput - 200) / 10.0 * 0.3;
            }
            else if (airspeedInput >= 210 && airspeedInput < 220)
            {
                airspeedOutputValue = -1.47 + (airspeedInput - 210) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 220 && airspeedInput < 230)
            {
                airspeedOutputValue = -1.18 + (airspeedInput - 220) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 230 && airspeedInput < 240)
            {
                airspeedOutputValue = -0.88 + (airspeedInput - 230) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 240 && airspeedInput < 250)
            {
                airspeedOutputValue = -0.59 + (airspeedInput - 240) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 250 && airspeedInput < 260)
            {
                airspeedOutputValue = -0.29 + (airspeedInput - 250) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 260 && airspeedInput < 270)
            {
                airspeedOutputValue = 0 + (airspeedInput - 260) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 270 && airspeedInput < 280)
            {
                airspeedOutputValue = 0.29 + (airspeedInput - 270) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 280 && airspeedInput < 290)
            {
                airspeedOutputValue = 0.59 + (airspeedInput - 280) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 290 && airspeedInput < 300)
            {
                airspeedOutputValue = 0.88 + (airspeedInput - 290) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 300 && airspeedInput < 310)
            {
                airspeedOutputValue = 1.18 + (airspeedInput - 300) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 310 && airspeedInput < 320)
            {
                airspeedOutputValue = 1.41 + (airspeedInput - 310) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 320 && airspeedInput < 330)
            {
                airspeedOutputValue = 1.65 + (airspeedInput - 320) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 330 && airspeedInput < 340)
            {
                airspeedOutputValue = 1.88 + (airspeedInput - 330) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 340 && airspeedInput < 350)
            {
                airspeedOutputValue = 2.12 + (airspeedInput - 340) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 350 && airspeedInput < 360)
            {
                airspeedOutputValue = 2.35 + (airspeedInput - 350) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 360 && airspeedInput < 370)
            {
                airspeedOutputValue = 2.58 + (airspeedInput - 360) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 370 && airspeedInput < 380)
            {
                airspeedOutputValue = 2.82 + (airspeedInput - 370) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 380 && airspeedInput < 390)
            {
                airspeedOutputValue = 3.06 + (airspeedInput - 380) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 390 && airspeedInput < 400)
            {
                airspeedOutputValue = 3.29 + (airspeedInput - 390) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 400 && airspeedInput < 450)
            {
                airspeedOutputValue = 3.53 + (airspeedInput - 400) / 50.0 * 0.88;
            }
            else if (airspeedInput >= 450 && airspeedInput < 500)
            {
                airspeedOutputValue = 4.41 + (airspeedInput - 450) / 50.0 * 0.88;
            }
            else if (airspeedInput >= 500 && airspeedInput < 550)
            {
                airspeedOutputValue = 5.29 + (airspeedInput - 500) / 50.0 * 0.77;
            }
            else if (airspeedInput >= 550 && airspeedInput < 600)
            {
                airspeedOutputValue = 6.06 + (airspeedInput - 550) / 50.0 * 0.76;
            }
            else if (airspeedInput >= 600 && airspeedInput < 650)
            {
                airspeedOutputValue = 6.82 + (airspeedInput - 600) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 650 && airspeedInput < 700)
            {
                airspeedOutputValue = 7.53 + (airspeedInput - 650) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 700 && airspeedInput < 750)
            {
                airspeedOutputValue = 8.24 + (airspeedInput - 700) / 50.0 * 0.58;
            }
            else if (airspeedInput >= 750 && airspeedInput < 800)
            {
                airspeedOutputValue = 8.82 + (airspeedInput - 750) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 800 && airspeedInput < 850)
            {
                airspeedOutputValue = 9.53 + (airspeedInput - 800) / 50.0 * 0.47;
            }
            else if (airspeedInput >= 850)
            {
                airspeedOutputValue = 10;
            }

            if (airspeedOutputValue < -10)
            {
                airspeedOutputValue = -10;
            }
            else if (airspeedOutputValue > 10)
            {
                airspeedOutputValue = 10;
            }

            _airspeedOutputSignal.State = airspeedOutputValue;
        }

        private void UpdateMachOutputValues()
        {
            if (_machInputSignal == null) return;
            var machInput = _machInputSignal.State;
            double machReferenceVoltage = 0;
            if (_machOutputSignal == null) return;
            var airspeedVoltage = _airspeedOutputSignal?.State ?? 0.0000;
            var absoluteAirspeedNeedleAngle = (airspeedVoltage + 10.0) / (20.0000 / 340.0000);

            if (machInput < 0.5)
            {
                machReferenceVoltage = -10;
            }
            else if (machInput >= 0.50 && machInput < 0.55)
            {
                machReferenceVoltage = -7.56 + (machInput - 0.50) / 0.05 * 1.00;
            }
            else if (machInput >= 0.55 && machInput < 0.60)
            {
                machReferenceVoltage = -6.56 + (machInput - 0.55) / 0.05 * 0.91;
            }
            else if (machInput >= 0.60 && machInput < 0.65)
            {
                machReferenceVoltage = -5.65 + (machInput - 0.60) / 0.05 * 0.84;
            }
            else if (machInput >= 0.65 && machInput < 0.70)
            {
                machReferenceVoltage = -4.81 + (machInput - 0.65) / 0.05 * 0.80;
            }
            else if (machInput >= 0.70 && machInput < 0.75)
            {
                machReferenceVoltage = -4.01 + (machInput - 0.70) / 0.05 * 0.77;
            }
            else if (machInput >= 0.75 && machInput < 0.80)
            {
                machReferenceVoltage = -3.24 + (machInput - 0.75) / 0.05 * 0.72;
            }
            else if (machInput >= 0.80 && machInput < 0.85)
            {
                machReferenceVoltage = -2.52 + (machInput - 0.80) / 0.05 * 0.69;
            }
            else if (machInput >= 0.85 && machInput < 0.90)
            {
                machReferenceVoltage = -1.83 + (machInput - 0.85) / 0.05 * 0.65;
            }
            else if (machInput >= 0.90 && machInput < 0.95)
            {
                machReferenceVoltage = -1.18 + (machInput - 0.90) / 0.05 * 0.61;
            }
            else if (machInput >= 0.95 && machInput < 1.00)
            {
                machReferenceVoltage = -0.57 + (machInput - 0.95) / 0.05 * 0.57;
            }
            else if (machInput >= 1.00 && machInput < 1.05)
            {
                machReferenceVoltage = 0.00 + (machInput - 1.00) / 0.05 * 0.53;
            }
            else if (machInput >= 1.05 && machInput < 1.10)
            {
                machReferenceVoltage = 0.53 + (machInput - 1.05) / 0.05 * 0.54;
            }
            else if (machInput >= 1.10 && machInput < 1.15)
            {
                machReferenceVoltage = 1.07 + (machInput - 1.10) / 0.05 * 0.49;
            }
            else if (machInput >= 1.15 && machInput < 1.20)
            {
                machReferenceVoltage = 1.56 + (machInput - 1.15) / 0.05 * 0.5;
            }
            else if (machInput >= 1.20 && machInput < 1.25)
            {
                machReferenceVoltage = 2.06 + (machInput - 1.20) / 0.05 * 0.46;
            }
            else if (machInput >= 1.25 && machInput < 1.30)
            {
                machReferenceVoltage = 2.52 + (machInput - 1.25) / 0.05 * 0.46;
            }
            else if (machInput >= 1.30 && machInput < 1.35)
            {
                machReferenceVoltage = 2.98 + (machInput - 1.30) / 0.05 * 0.45;
            }
            else if (machInput >= 1.35 && machInput < 1.40)
            {
                machReferenceVoltage = 3.43 + (machInput - 1.35) / 0.05 * 0.42;
            }
            else if (machInput >= 1.40 && machInput < 1.45)
            {
                machReferenceVoltage = 3.85 + (machInput - 1.40) / 0.05 * 0.42;
            }
            else if (machInput >= 1.45 && machInput < 1.50)
            {
                machReferenceVoltage = 4.27 + (machInput - 1.45) / 0.05 * 0.42;
            }
            else if (machInput >= 1.50 && machInput < 1.55)
            {
                machReferenceVoltage = 4.69 + (machInput - 1.50) / 0.05 * 0.39;
            }
            else if (machInput >= 1.55 && machInput < 1.60)
            {
                machReferenceVoltage = 5.08 + (machInput - 1.55) / 0.05 * 0.38;
            }
            else if (machInput >= 1.60 && machInput < 1.65)
            {
                machReferenceVoltage = 5.46 + (machInput - 1.60) / 0.05 * 0.38;
            }
            else if (machInput >= 1.65 && machInput < 1.70)
            {
                machReferenceVoltage = 5.84 + (machInput - 1.65) / 0.05 * 0.34;
            }
            else if (machInput >= 1.70 && machInput < 1.75)
            {
                machReferenceVoltage = 6.18 + (machInput - 1.70) / 0.05 * 0.35;
            }
            else if (machInput >= 1.75 && machInput < 1.80)
            {
                machReferenceVoltage = 6.53 + (machInput - 1.75) / 0.05 * 0.34;
            }
            else if (machInput >= 1.80 && machInput < 1.85)
            {
                machReferenceVoltage = 6.87 + (machInput - 1.80) / 0.05 * 0.30;
            }
            else if (machInput >= 1.85 && machInput < 1.90)
            {
                machReferenceVoltage = 7.17 + (machInput - 1.85) / 0.05 * 0.31;
            }
            else if (machInput >= 1.90 && machInput < 1.95)
            {
                machReferenceVoltage = 7.48 + (machInput - 1.90) / 0.05 * 0.31;
            }
            else if (machInput >= 1.95 && machInput < 2.00)
            {
                machReferenceVoltage = 7.79 + (machInput - 1.95) / 0.05 * 0.26;
            }
            else if (machInput >= 2.00 && machInput < 2.05)
            {
                machReferenceVoltage = 8.05 + (machInput - 2.00) / 0.05 * 0.27;
            }
            else if (machInput >= 2.05 && machInput < 2.10)
            {
                machReferenceVoltage = 8.32 + (machInput - 2.05) / 0.05 * 0.27;
            }
            else if (machInput >= 2.10 && machInput < 2.15)
            {
                machReferenceVoltage = 8.59 + (machInput - 2.10) / 0.05 * 0.26;
            }
            else if (machInput >= 2.15 && machInput < 2.20)
            {
                machReferenceVoltage = 8.85 + (machInput - 2.15) / 0.05 * 0.23;
            }
            else if (machInput >= 2.20 && machInput < 2.40)
            {
                machReferenceVoltage = 9.08 + (machInput - 2.20) / 0.20 * 0.92;
            }
            else if (machInput >= 2.4)
            {
                machReferenceVoltage = 10;
            }

            // Editor-authored override: when the Mach config carries a
            // piecewise table (kind="piecewise" with a CoupledTo pointer
            // back to the airspeed channel), evaluate it on the Mach input
            // to get the reference voltage. This replaces the hardcoded
            // if/else above. The cross-coupling math below runs unchanged
            // — only the reference value is data-driven now.
            if (_machCalibration != null
                && _machCalibration.Transform != null
                && _machCalibration.Transform.Kind == "piecewise"
                && _machCalibration.Transform.Breakpoints != null
                && _machCalibration.Transform.Breakpoints.Length >= 2)
            {
                machReferenceVoltage = GaugeTransform.EvaluatePiecewise(
                    machInput, _machCalibration.Transform.Breakpoints);
            }

            const int machOneReferenceAngle = 131;
            var machReferenceAngle = machReferenceVoltage / (20.0000 / 262.0000) + machOneReferenceAngle;
            var machAngleOffsetFromMach1RefAngle = machReferenceAngle - machOneReferenceAngle;

            var airspeedNeedleAngleDifferenceFrom260 = absoluteAirspeedNeedleAngle - 170.0000f;
            var howFarToMoveMachWheel = airspeedNeedleAngleDifferenceFrom260 - machAngleOffsetFromMach1RefAngle;
            var machOutputVoltage = -howFarToMoveMachWheel * (20.0000 / 262.0000);

            if (machOutputVoltage < -10)
            {
                machOutputVoltage = -10;
            }
            else if (machOutputVoltage > 10)
            {
                machOutputVoltage = 10;
            }

            // Editor-authored Mach trim: apply ZeroTrim/Gain on top of the
            // cross-coupled computation. ApplyTrim re-clamps to ±10 V so the
            // explicit clamp above is preserved for the no-config case.
            if (_machCalibration != null)
            {
                machOutputVoltage = _machCalibration.ApplyTrim(machOutputVoltage, _machOutputSignal.MinValue, _machOutputSignal.MaxValue);
            }

            _machOutputSignal.State = machOutputVoltage;
        }
    }
}