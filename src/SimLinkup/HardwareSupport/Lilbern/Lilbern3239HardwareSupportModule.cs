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

namespace SimLinkup.HardwareSupport.Lilbern
{
    //Lilbern 3239 F-16A Fuel Flow Indicator
    public class Lilbern3239HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Lilbern3239HardwareSupportModule));
        private readonly IFuelFlow _renderer = new FuelFlow();

        private AnalogSignal _fuelFlowInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _fuelFlowInputSignalChangedEventHandler;
        private AnalogSignal _fuelFlowOutputSignal;
        private bool _isDisposed;

        private Lilbern3239HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _fuelFlowChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Lilbern3239HardwareSupportModule(Lilbern3239HardwareSupportModuleConfig config)
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
            _fuelFlowChannel = ResolvePiecewiseChannel(config, "3239_Fuel_Flow_Pounds_Per_Hour_To_Instrument");
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
                var reloaded = Lilbern3239HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateFuelFlowOutputValues();
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

        public override string FriendlyName => "Lilbern M/N 3239 - F-16A Fuel Flow Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Lilbern3239HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Lilbern3239HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Lilbern3239HardwareSupportModule.config");
                hsmConfig = Lilbern3239HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Lilbern3239HardwareSupportModule(hsmConfig) };
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
                Id = "3239_Fuel_Flow_Pounds_Per_Hour_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 99999
            };
            return thisSignal;
        }

        private AnalogSignal CreateFuelFlowPoundsPerHourOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Fuel Flow (pounds per hour)",
                Id = "3239_Fuel_Flow_Pounds_Per_Hour_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts
                IsVoltage = true,
                MinValue = 0,
                MaxValue = 99999
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
            _fuelFlowOutputSignal = CreateFuelFlowPoundsPerHourOutputSignal();
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
            UpdateFuelFlowOutputValues();
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
            if (_fuelFlowInputSignalChangedEventHandler != null && _fuelFlowInputSignal != null)
            {
                try
                {
                    _fuelFlowInputSignal.SignalChanged -= _fuelFlowInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateFuelFlowOutputValues()
        {
            if (_fuelFlowInputSignal == null) return;
            if (_fuelFlowOutputSignal == null) return;
            var fuelFlowInput = _fuelFlowInputSignal.State;

            // Editor override: piecewise channel.
            if (_fuelFlowChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(fuelFlowInput, _fuelFlowChannel.Transform.Breakpoints);
                _fuelFlowOutputSignal.State = _fuelFlowChannel.ApplyTrim(v, _fuelFlowOutputSignal.MinValue, _fuelFlowOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — 0..80000 lbs/hr → -10..+10 V linear.
            var fuelFlowOutputValue = fuelFlowInput <= 0 ? -10.00 : -10.00 + fuelFlowInput / 80000.0000 * 20.0000;
            if (fuelFlowOutputValue < -10) fuelFlowOutputValue = -10;
            else if (fuelFlowOutputValue > 10) fuelFlowOutputValue = 10;
            _fuelFlowOutputSignal.State = fuelFlowOutputValue;
        }
    }
}
