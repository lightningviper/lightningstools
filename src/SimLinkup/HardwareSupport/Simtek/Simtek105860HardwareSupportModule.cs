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
    //Simtek 10-5860 F-16 Fuel Flow Indicator (piecewise: 0..80,000 PPH → ±10 V)
    //
    // Per the spec sheet (PL20-5860, dwg 10-5860):
    //   Drive type: SINGLE DC SERVO
    //   Calibration: Table 1 — 9 test points, PPH×100 0..800 → -10..+10 V
    //                  linear (2.5 V per 100 PPH×100, i.e. 1 V per 4000 PPH).
    //   Display: 5-digit counter, two least-significant digits fixed at "00".
    //
    // Modeled on Simtek100295HardwareSupportModule (the existing F-16 fuel
    // flow indicator). Renderer reuse: borrows IFuelFlow from 10-0295.
    public class Simtek105860HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek105860HardwareSupportModule));
        private readonly IFuelFlow _renderer = new FuelFlow();

        // Editor-authored calibration. Default ships the 9 spec-sheet
        // test points from PL20-5860 Table 1; users can edit any row to
        // correct local hardware drift.
        private Simtek105860HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _fuelFlowCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _fuelFlowInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _fuelFlowInputSignalChangedEventHandler;
        private AnalogSignal _fuelFlowOutputSignal;

        private bool _isDisposed;

        public Simtek105860HardwareSupportModule(Simtek105860HardwareSupportModuleConfig config)
        {
            _config = config;
            _fuelFlowCalibration = ResolvePiecewiseChannel(config, "105860_Fuel_Flow_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

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
                var reloaded = Simtek105860HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _fuelFlowCalibration = ResolvePiecewiseChannel(reloaded, "105860_Fuel_Flow_To_Instrument");
                // Re-evaluate the output with the cached input value so the
                // user sees the new calibration immediately.
                UpdateOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_fuelFlowInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_fuelFlowOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-5860 - Fuel Flow Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek105860HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek105860HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek105860HardwareSupportModule.config");
                hsmConfig = Simtek105860HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek105860HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.FuelFlowPoundsPerHour = (float) _fuelFlowInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _fuelFlowInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateFuelFlowInputSignal()
        {
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Fuel Flow (pounds per hour)",
                Id = "105860_Fuel_Flow_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 80000
            };
        }

        private AnalogSignal CreateFuelFlowOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Fuel Flow",
                Id = "105860_Fuel_Flow_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00,
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
        }

        private void CreateInputEventHandlers()
        {
            _fuelFlowInputSignalChangedEventHandler = fuelFlow_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _fuelFlowInputSignal = CreateFuelFlowInputSignal();
        }

        private void CreateOutputSignals()
        {
            _fuelFlowOutputSignal = CreateFuelFlowOutputSignal();
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

        private void fuelFlow_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_fuelFlowInputSignal != null)
            {
                _fuelFlowInputSignal.SignalChanged += _fuelFlowInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_fuelFlowInputSignalChangedEventHandler == null || _fuelFlowInputSignal == null) return;
            try { _fuelFlowInputSignal.SignalChanged -= _fuelFlowInputSignalChangedEventHandler; }
            catch (RemotingException) { }
        }

        private void UpdateOutputValues()
        {
            if (_fuelFlowOutputSignal == null) return;
            var fuelFlow = _fuelFlowInputSignal.State;

            // Editor-authored override: when a .config file declared a
            // piecewise table, evaluate via the generic helper + per-channel
            // trim. Falls through to the linear formula below when no
            // config is present.
            if (_fuelFlowCalibration != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(fuelFlow, _fuelFlowCalibration.Transform.Breakpoints);
                _fuelFlowOutputSignal.State = _fuelFlowCalibration.ApplyTrim(v, _fuelFlowOutputSignal.MinValue, _fuelFlowOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback: PL20-5860 Table 1 is exactly linear from
            // 0 PPH → -10 V to 80,000 PPH → +10 V. (PPH×100 of 0..800 in
            // 100 steps maps to 9 evenly-spaced 2.5 V increments.)
            var clamped = fuelFlow < 0 ? 0 : (fuelFlow > 80000 ? 80000 : fuelFlow);
            _fuelFlowOutputSignal.State = (clamped / 80000.0) * 20.0 - 10.0;
        }
    }
}
