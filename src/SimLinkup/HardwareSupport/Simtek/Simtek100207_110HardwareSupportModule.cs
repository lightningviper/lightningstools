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
    //Simtek 10-0207_110 F-16 RPM Indicator (v2 — wider input range, looser table)
    public class Simtek100207_110HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100207_110HardwareSupportModule));
        private readonly ITachometer _renderer = new Tachometer();

        // Editor-authored calibration. Same shape and hot-reload contract as
        // Simtek100207HardwareSupportModule — see that class for the full
        // explanation of the pattern. Both gauges emit the same RPM port id
        // ("100207_RPM_To_Instrument") but read from separate .config files
        // (basename keyed by class short name) so they don't conflict.
        private Simtek100207_110HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _rpmCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _rpmInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rpmInputSignalChangedEventHandler;
        private AnalogSignal _rpmOutputSignal;

        public Simtek100207_110HardwareSupportModule(Simtek100207_110HardwareSupportModuleConfig config)
        {
            _config = config;
            _rpmCalibration = ResolvePiecewiseChannel(config, "100207_RPM_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
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
                var reloaded = Simtek100207_110HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] {_rpmInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_rpmOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0207_110 - Indicator, Simulated Tachometer";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100207_110HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100207_110HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100207_110HardwareSupportModule.config");
                hsmConfig = Simtek100207_110HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100207_110HardwareSupportModule(hsmConfig) };
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

                    if (rpmInput < 20)
                    {
                        rpmOutputValue = Math.Max(-10, -10.0 + rpmInput / 20.0 * 1.89);
                    }
                    else if (rpmInput >= 20 && rpmInput < 40)
                    {
                        rpmOutputValue = -8.11 + (rpmInput - 10) / 20.0 * 1.88;
                    }
                    else if (rpmInput >= 40 && rpmInput < 60)
                    {
                        rpmOutputValue = -6.23 + (rpmInput - 40) / 20.0 * 1.89;
                    }
                    else if (rpmInput >= 60 && rpmInput < 70)
                    {
                        rpmOutputValue = -4.35 + (rpmInput - 60) / 10.0 * 2.86;
                    }
                    else if (rpmInput >= 70 && rpmInput < 76)
                    {
                        rpmOutputValue = -1.48 + (rpmInput - 70) / 6.0 * 1.48;
                    }
                    else if (rpmInput >= 76 && rpmInput < 80)
                    {
                        rpmOutputValue = 0.00 + (rpmInput - 76) / 4.0 * 1.39;
                    }
                    else if (rpmInput >= 80 && rpmInput < 90)
                    {
                        rpmOutputValue = 1.39 + (rpmInput - 80) / 10.0 * 2.87;
                    }
                    else if (rpmInput >= 90 && rpmInput < 100)
                    {
                        rpmOutputValue = 4.26 + (rpmInput - 90) / 10.0 * 2.87;
                    }
                    else if (rpmInput >= 100)
                    {
                        rpmOutputValue = 7.13 + (rpmInput - 100) / 10.0 * 2.87;
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