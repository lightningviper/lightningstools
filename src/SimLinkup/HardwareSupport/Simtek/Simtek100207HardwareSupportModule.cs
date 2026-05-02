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
    //Simtek 10-0207 F-16 RPM Indicator
    public class Simtek100207HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100207HardwareSupportModule));
        private readonly ITachometer _renderer = new Tachometer();

        // Editor-authored calibration. Null when no .config file is present
        // (or it failed to parse) — UpdateOutputValues falls back to the
        // hardcoded if/else in that case so behaviour pre-config is preserved.
        // Pattern matches ArduinoSeatHardwareSupportModule: load + inject in
        // GetInstances, store the whole config object on the instance, watch
        // the file for hot reload via ConfigFileReloadWatcher.
        //
        // Not `readonly` because the hot-reload handler reassigns these on
        // file change. Reads are atomic reference assignments — Update-
        // OutputValues sees either the old or new pointer, never a torn
        // value, so no locking is needed.
        private Simtek100207HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _rpmCalibration;

        // Hot-reload plumbing. ConfigFileReloadWatcher invokes ReloadConfig
        // when the config file changes on disk; mtime dedup of the watcher's
        // chatty multi-fire-per-save behaviour is centralised in the helper.
        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _rpmInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rpmInputSignalChangedEventHandler;
        private AnalogSignal _rpmOutputSignal;

        // Public ctor takes the (optionally-null) config. Null = no editor
        // override; the existing hardcoded transform handles everything.
        public Simtek100207HardwareSupportModule(Simtek100207HardwareSupportModuleConfig config)
        {
            _config = config;
            _rpmCalibration = ResolvePiecewiseChannel(config, "100207_RPM_To_Instrument");
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

        // Reload the config and re-resolve the channel pointer. Wrapped in a
        // try/catch so a partial-write race (the watcher can fire mid-save)
        // doesn't escape into the runtime — we just log and wait for the
        // next change event with a complete file.
        private void ReloadConfig()
        {
            try
            {
                var configFile = _config != null ? _config.FilePath : null;
                if (string.IsNullOrEmpty(configFile)) return;
                var reloaded = Simtek100207HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _rpmCalibration = ResolvePiecewiseChannel(reloaded, "100207_RPM_To_Instrument");
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        // Pull the named channel out of the config IFF it carries a usable
        // piecewise breakpoint table. Returns null otherwise so the caller's
        // null-check covers both "no config file" and "config present but
        // doesn't override this channel" with the same code path.
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

        public override AnalogSignal[] AnalogInputs => new[] {_rpmInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_rpmOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0207 - Indicator, Simulated Tachometer";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100207HardwareSupportModule()
        {
            Dispose(false);
        }

        // Match the ArduinoSeat / Simtek100285 pattern: build the file path,
        // try to deserialize, swallow exceptions to the log, and inject (the
        // possibly-null) config into the instance. A missing or malformed
        // file passes null and the gauge falls back to its hardcoded
        // transform — pre-config behaviour preserved.
        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100207HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100207HardwareSupportModule.config");
                hsmConfig = Simtek100207HardwareSupportModuleConfig.Load(hsmConfigFilePath);
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
            return new IHardwareSupportModule[] { new Simtek100207HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.RPMPercent = (float) _rpmInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = rpm_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _rpmInputSignal = CreateRPMInputSignal();
        }

        private void CreateOutputSignals()
        {
            _rpmOutputSignal = CreateRPMOutputSignal();
        }

        private AnalogSignal CreateRPMInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "RPM",
                Id = "100207_RPM_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0,
                MinValue = 0,
                MaxValue = 110
            };
            return thisSignal;
        }

        private AnalogSignal CreateRPMOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM",
                Id = "100207_RPM_To_Instrument",
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

        private void RegisterForInputEvents()
        {
            if (_rpmInputSignal != null)
            {
                _rpmInputSignal.SignalChanged += _rpmInputSignalChangedEventHandler;
            }
        }

        private void rpm_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_rpmInputSignalChangedEventHandler != null && _rpmInputSignal != null)
            {
                try
                {
                    _rpmInputSignal.SignalChanged -= _rpmInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_rpmInputSignal != null)
            {
                var rpmInput = _rpmInputSignal.State;
                double rpmOutputValue = 0;
                if (_rpmOutputSignal != null)
                {
                    // Editor-authored override: when a .config file declared a
                    // piecewise table for this channel, use the generic
                    // evaluator and per-channel trim. Falls through to the
                    // hardcoded if/else below when no config is present.
                    if (_rpmCalibration != null)
                    {
                        var v = GaugeTransform.EvaluatePiecewise(rpmInput, _rpmCalibration.Transform.Breakpoints);
                        _rpmOutputSignal.State = _rpmCalibration.ApplyTrim(v, _rpmOutputSignal.MinValue, _rpmOutputSignal.MaxValue);
                        return;
                    }

                    if (rpmInput < 10)
                    {
                        rpmOutputValue = Math.Max(-10, -10.0 + rpmInput / 10.0 * 1.25);
                    }
                    else if (rpmInput >= 10 && rpmInput < 20)
                    {
                        rpmOutputValue = -8.75 + (rpmInput - 10) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 20 && rpmInput < 30)
                    {
                        rpmOutputValue = -7.50 + (rpmInput - 20) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 30 && rpmInput < 40)
                    {
                        rpmOutputValue = -6.25 + (rpmInput - 30) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 40 && rpmInput < 50)
                    {
                        rpmOutputValue = -5.00 + (rpmInput - 40) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 50 && rpmInput < 60)
                    {
                        rpmOutputValue = -3.75 + (rpmInput - 50) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 60 && rpmInput < 65)
                    {
                        rpmOutputValue = -2.50 + (rpmInput - 60) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 65 && rpmInput < 68)
                    {
                        rpmOutputValue = -0.938 + (rpmInput - 65) / 3.0 * 0.938;
                    }
                    else if (rpmInput >= 68 && rpmInput < 70)
                    {
                        rpmOutputValue = 0.00 + (rpmInput - 68) / 2.0 * 0.625;
                    }
                    else if (rpmInput >= 70 && rpmInput < 75)
                    {
                        rpmOutputValue = 0.625 + (rpmInput - 70) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 75 && rpmInput < 80)
                    {
                        rpmOutputValue = 2.188 + (rpmInput - 75) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 80 && rpmInput < 85)
                    {
                        rpmOutputValue = 3.750 + (rpmInput - 80) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 85 && rpmInput < 90)
                    {
                        rpmOutputValue = 5.313 + (rpmInput - 85) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 90 && rpmInput < 95)
                    {
                        rpmOutputValue = 6.875 + (rpmInput - 90) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 95)
                    {
                        rpmOutputValue = Math.Min(10, 8.438 + Math.Min(1, (rpmInput - 95) / 5.0) * 1.562);
                    }

                    if (rpmOutputValue < -10)
                    {
                        rpmOutputValue = -10;
                    }
                    else if (rpmOutputValue > 10)
                    {
                        rpmOutputValue = 10;
                    }
                    _rpmOutputSignal.State = rpmOutputValue;
                }
            }
        }
    }
}