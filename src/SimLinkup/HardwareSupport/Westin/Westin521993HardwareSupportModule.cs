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

namespace SimLinkup.HardwareSupport.Westin
{
    //Westin P/N 521993 F-16 EPU FUEL QTY IND
    public class Westin521993HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Westin521993HardwareSupportModule));
        private readonly IEPUFuelGauge _renderer = new EPUFuelGauge();

        private AnalogSignal _epuFuelPercentageInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _epuFuelPercentageInputSignalChangedEventHandler;
        private AnalogSignal _epuFuelPercentageOutputSignal;
        private bool _isDisposed;

        private Westin521993HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _epuChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Westin521993HardwareSupportModule(Westin521993HardwareSupportModuleConfig config)
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
            _epuChannel = ResolvePiecewiseChannel(config, "521993_EPU_To_Instrument");
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
                var reloaded = Westin521993HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
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

        public override string FriendlyName => "Westin P/N 521993 - EPU Fuel Quantity Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Westin521993HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Westin521993HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Westin521993HardwareSupportModule.config");
                hsmConfig = Westin521993HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Westin521993HardwareSupportModule(hsmConfig) };
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
                Id = "521993_EPU_From_Sim",
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
                Id = "521993_EPU_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.1, //volts
                IsVoltage = true,
                MinValue = 0.1,
                MaxValue = 2
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
            if (_epuFuelPercentageInputSignalChangedEventHandler == null ||
                _epuFuelPercentageInputSignal == null)
            {
                return;
            }
            try
            {
                _epuFuelPercentageInputSignal.SignalChanged -= _epuFuelPercentageInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_epuFuelPercentageInputSignal == null) return;
            if (_epuFuelPercentageOutputSignal == null) return;
            var epuInput = _epuFuelPercentageInputSignal.State;

            // Editor override: piecewise channel.
            if (_epuChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(epuInput, _epuChannel.Transform.Breakpoints);
                _epuFuelPercentageOutputSignal.State = _epuChannel.ApplyTrim(v, _epuFuelPercentageOutputSignal.MinValue, _epuFuelPercentageOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — 0..100% mapped to 0.1..2.0 V.
            var epuOutputValue = epuInput < 0 ? 0.1 : (epuInput > 100 ? 2 : epuInput / 100 * 1.9 + 0.1);
            if (epuOutputValue < 0) epuOutputValue = 0.1;
            else if (epuOutputValue > 2) epuOutputValue = 2;
            _epuFuelPercentageOutputSignal.State = epuOutputValue;
        }
    }
}
