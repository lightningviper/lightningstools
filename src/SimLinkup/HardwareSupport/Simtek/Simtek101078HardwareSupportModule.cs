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
    //Simtek 10-1078 F-16 Cabin Pressure Altimeter (piecewise: 0..50000 ft → ±10 V)
    public class Simtek101078HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101078HardwareSupportModule));
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator()
        {

        };

        // Editor-authored calibration. The default mapping is a straight
        // line (the C# fallback below is 10 segments with identical slopes),
        // but the editor ships piecewise breakpoints so users can correct
        // for non-linear hardware drift. See sim-101078-cabin-altimeter.js
        // for the rationale.
        private Simtek101078HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _cabinAltCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _cabinPressInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressOutputSignal;

        public Simtek101078HardwareSupportModule(Simtek101078HardwareSupportModuleConfig config)
        {
            _config = config;
            _cabinAltCalibration = ResolvePiecewiseChannel(config, "101078_CabinAlt_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Same piecewise-channel resolver as the other piecewise gauges:
        // only return the channel when it carries a usable breakpoint
        // table; null otherwise.
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
                var reloaded = Simtek101078HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _cabinAltCalibration = ResolvePiecewiseChannel(reloaded, "101078_CabinAlt_To_Instrument");
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

        public override AnalogSignal[] AnalogInputs => new[] {_cabinPressInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_cabinPressOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1078 - Indicator, Simulated Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101078HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101078HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101078HardwareSupportModule.config");
                hsmConfig = Simtek101078HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101078HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float) _cabinPressInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _cabinPressInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _cabinPressInputSignalChangedEventHandler = cabinPressureAlt_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _cabinPressInputSignal = CreateCabinPressureAltInputSignal();
        }

        private void CreateOutputSignals()
        {
            _cabinPressOutputSignal = CreateCabinPressureAltOutputSignal();
        }

        private AnalogSignal CreateCabinPressureAltInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "101078_CabinAlt_From_Sim",
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

        private AnalogSignal CreateCabinPressureAltOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Cabin Alt",
                Id = "101078_CabinAlt_To_Instrument",
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
            if (_cabinPressInputSignal != null)
            {
                _cabinPressInputSignal.SignalChanged += _cabinPressInputSignalChangedEventHandler;
            }
        }

        private void cabinPressureAlt_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_cabinPressInputSignalChangedEventHandler != null && _cabinPressInputSignal != null)
            {
                try
                {
                    _cabinPressInputSignal.SignalChanged -= _cabinPressInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_cabinPressInputSignal != null)
            {
                var cabinPressInput = _cabinPressInputSignal.State;
                double cabinPressOutputValue = 0;
                if (_cabinPressOutputSignal != null)
                {
                    // Editor-authored override: when a .config file declared
                    // a piecewise transform for this channel, evaluate via
                    // the generic helper + per-channel trim. Falls through
                    // to the hardcoded 10-segment if/else below when no
                    // config is present.
                    if (_cabinAltCalibration != null)
                    {
                        var v = GaugeTransform.EvaluatePiecewise(cabinPressInput, _cabinAltCalibration.Transform.Breakpoints);
                        _cabinPressOutputSignal.State = _cabinAltCalibration.ApplyTrim(v, _cabinPressOutputSignal.MinValue, _cabinPressOutputSignal.MaxValue);
                        return;
                    }

                    if (cabinPressInput < 5000)
                    {
                        cabinPressOutputValue = Math.Max(-10, -10.0 + cabinPressInput / 5000.0 * 2.00);
                    }
                    else if (cabinPressInput >= 5000 && cabinPressInput < 10000)
                    {
                        cabinPressOutputValue = -8.00 + (cabinPressInput - 5000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 10000 && cabinPressInput < 15000)
                    {
                        cabinPressOutputValue = -6.00 + (cabinPressInput - 10000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 15000 && cabinPressInput < 20000)
                    {
                        cabinPressOutputValue = -4.00 + (cabinPressInput - 15000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 20000 && cabinPressInput < 25000)
                    {
                        cabinPressOutputValue = -2.00 + (cabinPressInput - 20000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 25000 && cabinPressInput < 30000)
                    {
                        cabinPressOutputValue = 0.00 + (cabinPressInput - 25000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 30000 && cabinPressInput < 35000)
                    {
                        cabinPressOutputValue = 2.00 + (cabinPressInput - 30000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 35000 && cabinPressInput < 40000)
                    {
                        cabinPressOutputValue = 4.00 + (cabinPressInput - 35000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 40000 && cabinPressInput < 45000)
                    {
                        cabinPressOutputValue = 6.00 + (cabinPressInput - 40000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 45000 && cabinPressInput < 50000)
                    {
                        cabinPressOutputValue = Math.Min(10, 8.00 + Math.Min(1, (cabinPressInput - 45000) / 5000.0) * 2.00);
                    }

                    if (cabinPressOutputValue < -10)
                    {
                        cabinPressOutputValue = -10;
                    }
                    else if (cabinPressOutputValue > 10)
                    {
                        cabinPressOutputValue = 10;
                    }
                    _cabinPressOutputSignal.State = cabinPressOutputValue;
                }
            }
        }
    }
}