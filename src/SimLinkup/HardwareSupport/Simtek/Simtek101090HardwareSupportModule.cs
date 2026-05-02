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
    //Simtek 10-1090 F-16 EPU FUEL QTY IND (piecewise: 0..100% → ±10 V)
    public class Simtek101090HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101090HardwareSupportModule));
        private readonly IEPUFuelGauge _renderer = new EPUFuelGauge();

        // Editor-authored calibration. The default mapping is a straight
        // line (the C# fallback below is `epuInput / 100 * 20 - 10`), but
        // the editor ships 5 piecewise breakpoints so users can correct
        // for non-linear hardware drift. See sim-101090-epu-fuel.js for
        // the rationale.
        private Simtek101090HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _epuCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _epuFuelPercentageInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _epuFuelPercentageInputSignalChangedEventHandler;
        private AnalogSignal _epuFuelPercentageOutputSignal;

        private bool _isDisposed;

        public Simtek101090HardwareSupportModule(Simtek101090HardwareSupportModuleConfig config)
        {
            _config = config;
            _epuCalibration = ResolvePiecewiseChannel(config, "101090_EPU_To_Instrument");
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
                var reloaded = Simtek101090HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _epuCalibration = ResolvePiecewiseChannel(reloaded, "101090_EPU_To_Instrument");
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

        public override AnalogSignal[] AnalogInputs => new[] {_epuFuelPercentageInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_epuFuelPercentageOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1090 - EPU Fuel Quantity Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101090HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101090HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101090HardwareSupportModule.config");
                hsmConfig = Simtek101090HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101090HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.FuelRemainingPercent = (float) _epuFuelPercentageInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _epuFuelPercentageInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateEPUInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "101090_EPU_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };

            return thisSignal;
        }

        private AnalogSignal CreateEPUOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "101090_EPU_To_Instrument",
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
            _epuFuelPercentageInputSignalChangedEventHandler =
                epu_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _epuFuelPercentageInputSignal = CreateEPUInputSignal();
        }

        private void CreateOutputSignals()
        {
            _epuFuelPercentageOutputSignal = CreateEPUOutputSignal();
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

        private void epu_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_epuFuelPercentageInputSignal != null)
            {
                _epuFuelPercentageInputSignal.SignalChanged += _epuFuelPercentageInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_epuFuelPercentageInputSignalChangedEventHandler != null && _epuFuelPercentageInputSignal != null)
            {
                try
                {
                    _epuFuelPercentageInputSignal.SignalChanged -= _epuFuelPercentageInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_epuFuelPercentageInputSignal == null) return;
            var epuInput = _epuFuelPercentageInputSignal.State;
            if (_epuFuelPercentageOutputSignal == null) return;

            // Editor-authored override: when a .config file declared a
            // piecewise transform for this channel, evaluate via the generic
            // helper + per-channel trim. Falls through to the hardcoded
            // formula below when no config is present.
            if (_epuCalibration != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(epuInput, _epuCalibration.Transform.Breakpoints);
                _epuFuelPercentageOutputSignal.State = _epuCalibration.ApplyTrim(v, _epuFuelPercentageOutputSignal.MinValue, _epuFuelPercentageOutputSignal.MaxValue);
                return;
            }

            var epuOutputValue = epuInput < 0 ? -10 : (epuInput > 100 ? 10 : epuInput / 100 * 20 - 10);

            if (epuOutputValue < -10)
            {
                epuOutputValue = -10;
            }
            else if (epuOutputValue > 10)
            {
                epuOutputValue = 10;
            }

            _epuFuelPercentageOutputSignal.State = epuOutputValue;
        }
    }
}