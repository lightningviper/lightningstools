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
    //Simtek 10-0216 F-16 FTIT Indicator (piecewise: 200..1200°C → ±10 V)
    public class Simtek100216HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100216HardwareSupportModule));
        private readonly IFanTurbineInletTemperature _renderer = new FanTurbineInletTemperature();

        // Editor-authored calibration. Same hot-reload contract as the other
        // piecewise gauges (Simtek 10-0207, 10-0207_110, 10-0194 airspeed).
        private Simtek100216HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _ftitCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _ftitInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _ftitInputSignalChangedEventHandler;
        private AnalogSignal _ftitOutputSignal;

        private bool _isDisposed;

        public Simtek100216HardwareSupportModule(Simtek100216HardwareSupportModuleConfig config)
        {
            _config = config;
            _ftitCalibration = ResolvePiecewiseChannel(config, "100216_FTIT_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Same helper as the other piecewise gauges: only return the channel
        // when it carries a usable piecewise table; null otherwise.
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
                var reloaded = Simtek100216HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _ftitCalibration = ResolvePiecewiseChannel(reloaded, "100216_FTIT_To_Instrument");
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

        public override AnalogSignal[] AnalogInputs => new[] {_ftitInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_ftitOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0216 - Indicator, Simulated FTIT";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100216HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100216HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100216HardwareSupportModule.config");
                hsmConfig = Simtek100216HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100216HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.InletTemperatureDegreesCelcius = (float) _ftitInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _ftitInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateFTITInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "FTIT",
                Id = "100216_FTIT_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 1200
            };
            return thisSignal;
        }

        private AnalogSignal CreateFTITOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT",
                Id = "100216_FTIT_To_Instrument",
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
            _ftitInputSignalChangedEventHandler =
                ftit_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _ftitInputSignal = CreateFTITInputSignal();
        }

        private void CreateOutputSignals()
        {
            _ftitOutputSignal = CreateFTITOutputSignal();
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

        private void ftit_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_ftitInputSignal != null)
            {
                _ftitInputSignal.SignalChanged += _ftitInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_ftitInputSignalChangedEventHandler != null && _ftitInputSignal != null)
            {
                try
                {
                    _ftitInputSignal.SignalChanged -= _ftitInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_ftitInputSignal != null)
            {
                var ftitInput = _ftitInputSignal.State;
                double ftitOutputValue = 0;
                if (_ftitOutputSignal != null)
                {
                    // Editor-authored override: when a .config file declared
                    // a piecewise table for this channel, use the generic
                    // evaluator and per-channel trim. Falls through to the
                    // hardcoded if/else below when no config is present.
                    if (_ftitCalibration != null)
                    {
                        var v = GaugeTransform.EvaluatePiecewise(ftitInput, _ftitCalibration.Transform.Breakpoints);
                        _ftitOutputSignal.State = _ftitCalibration.ApplyTrim(v, _ftitOutputSignal.MinValue, _ftitOutputSignal.MaxValue);
                        return;
                    }

                    if (ftitInput <= 200)
                    {
                        ftitOutputValue = -10;
                    }
                    else if (ftitInput >= 200 && ftitInput < 700)
                    {
                        ftitOutputValue = -10 + (ftitInput - 200) / 500 * 6.25;
                    }
                    else if (ftitInput >= 700 && ftitInput < 1000)
                    {
                        ftitOutputValue = -3.75 + (ftitInput - 700) / 300.0 * 11.25;
                    }
                    else if (ftitInput >= 1000 && ftitInput < 1200)
                    {
                        ftitOutputValue = 7.5 + (ftitInput - 1000) / 200.0 * 2.5;
                    }
                    else if (ftitInput >= 1200)
                    {
                        ftitOutputValue = 10;
                    }


                    if (ftitOutputValue < -10)
                    {
                        ftitOutputValue = -10;
                    }
                    else if (ftitOutputValue > 10)
                    {
                        ftitOutputValue = 10;
                    }
                    _ftitOutputSignal.State = ftitOutputValue;
                }
            }
        }
    }
}