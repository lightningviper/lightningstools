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
    //Simtek 10-0295 F-16 Simulated Fuel Flow Indicator (piecewise: 0..9900 PPH → ±10 V)
    public class Simtek100295HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100295HardwareSupportModule));
        private readonly IFuelFlow _renderer = new FuelFlow();

        // Editor-authored calibration. The default ships the 7 spec-sheet
        // calibration test points from the gauge's drawing (Table 1 on
        // sheet 3); the user can edit any row to correct local hardware
        // drift. The hardcoded fallback below has known-buggy "high-flow"
        // branches above 10000 PPH that don't match the gauge spec — once
        // a config file is present, those bugs are bypassed.
        private Simtek100295HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _fuelFlowCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _fuelFlowInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _fuelFlowInputSignalChangedEventHandler;
        private AnalogSignal _fuelFlowOutputSignal;

        private bool _isDisposed;

        public Simtek100295HardwareSupportModule(Simtek100295HardwareSupportModuleConfig config)
        {
            _config = config;
            _fuelFlowCalibration = ResolvePiecewiseChannel(config, "100295_Fuel_Flow_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Same helper as the other piecewise gauges.
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
                var reloaded = Simtek100295HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _fuelFlowCalibration = ResolvePiecewiseChannel(reloaded, "100295_Fuel_Flow_To_Instrument");
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

        public override AnalogSignal[] AnalogInputs => new[] {_fuelFlowInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_fuelFlowOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0295 - Simulated Fuel Flow Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100295HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100295HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100295HardwareSupportModule.config");
                hsmConfig = Simtek100295HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100295HardwareSupportModule(hsmConfig) };
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
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Fuel Flow (pounds per hour)",
                Id = "100295_Fuel_Flow_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 99900
            };
            return thisSignal;
        }

        private AnalogSignal CreateFuelFlowOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Fuel Flow",
                Id = "100295_Fuel_Flow_To_Instrument",
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
            _fuelFlowInputSignalChangedEventHandler =
                fuelFlow_InputSignalChanged;
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
            try
            {
                _fuelFlowInputSignal.SignalChanged -= _fuelFlowInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_fuelFlowOutputSignal == null) return;
            var fuelFlow = _fuelFlowInputSignal.State;

            // Editor-authored override: when a .config file declared a
            // piecewise table for this channel, evaluate via the generic
            // helper + per-channel trim. Falls through to the hardcoded
            // if/else below when no config is present. Note: the hardcoded
            // path has known-buggy "high-flow" branches above 10000 PPH
            // that don't match the gauge's spec sheet (max range is 9900
            // PPH per the drawing); editor-authored configs bypass them.
            if (_fuelFlowCalibration != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(fuelFlow, _fuelFlowCalibration.Transform.Breakpoints);
                _fuelFlowOutputSignal.State = _fuelFlowCalibration.ApplyTrim(v, _fuelFlowOutputSignal.MinValue, _fuelFlowOutputSignal.MaxValue);
                return;
            }

            if (fuelFlow <= 10000)
            {
                _fuelFlowOutputSignal.State = Math.Min(_fuelFlowInputSignal.State / 9900.00, 1.00) * 20.00 - 10.00;
            }
            else if (fuelFlow >= 10000 && fuelFlow < 80000)
            {
                _fuelFlowOutputSignal.State = _fuelFlowInputSignal.State / 99000.00 * 20.00 - 10.00;
            }
            else if (fuelFlow >= 80000)
            {
                _fuelFlowOutputSignal.State = 8000 / 9900.00 * 20.00 - 10.00;
            }
        }
    }
}