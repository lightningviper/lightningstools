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
    //Simtek 10-5868 F-16 EPU FUEL QTY IND (piecewise: 0..100% → ±10 V)
    //
    // Per the spec sheet (PL20-5868, dwg 10-5868):
    //   Drive type: SINGLE METER
    //   Calibration: Table 1 — 11 test points, 0..100% remain → -10..+10 V
    //                  linear (2 V per 10% increment).
    //
    // Modeled on Simtek101090HardwareSupportModule (the existing F-16 EPU
    // fuel quantity indicator). Renderer reuse: borrows IEPUFuelGauge
    // from 10-1090.
    public class Simtek105868HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek105868HardwareSupportModule));
        private readonly IEPUFuelGauge _renderer = new EPUFuelGauge();

        private Simtek105868HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _epuCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _epuFuelPercentageInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _epuFuelPercentageInputSignalChangedEventHandler;
        private AnalogSignal _epuFuelPercentageOutputSignal;

        private bool _isDisposed;

        public Simtek105868HardwareSupportModule(Simtek105868HardwareSupportModuleConfig config)
        {
            _config = config;
            _epuCalibration = ResolvePiecewiseChannel(config, "105868_EPU_To_Instrument");
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
                var reloaded = Simtek105868HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _epuCalibration = ResolvePiecewiseChannel(reloaded, "105868_EPU_To_Instrument");
                // Re-evaluate the output with the cached input value so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven loop won't fire until the
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

        public override string FriendlyName => "Simtek P/N 10-5868 - EPU Fuel Quantity Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek105868HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek105868HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek105868HardwareSupportModule.config");
                hsmConfig = Simtek105868HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek105868HardwareSupportModule(hsmConfig) };
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
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "105868_EPU_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };
        }

        private AnalogSignal CreateEPUOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "105868_EPU_To_Instrument",
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
            _epuFuelPercentageInputSignalChangedEventHandler = epu_InputSignalChanged;
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
                try { _epuFuelPercentageInputSignal.SignalChanged -= _epuFuelPercentageInputSignalChangedEventHandler; }
                catch (RemotingException) { }
            }
        }

        private void UpdateOutputValues()
        {
            if (_epuFuelPercentageInputSignal == null) return;
            var epuInput = _epuFuelPercentageInputSignal.State;
            if (_epuFuelPercentageOutputSignal == null) return;

            if (_epuCalibration != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(epuInput, _epuCalibration.Transform.Breakpoints);
                _epuFuelPercentageOutputSignal.State = _epuCalibration.ApplyTrim(v, _epuFuelPercentageOutputSignal.MinValue, _epuFuelPercentageOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback: 0..100% → -10..+10 V linear.
            var clamped = epuInput < 0 ? 0 : (epuInput > 100 ? 100 : epuInput);
            var v2 = clamped / 100.0 * 20.0 - 10.0;
            _epuFuelPercentageOutputSignal.State = v2;
        }
    }
}
