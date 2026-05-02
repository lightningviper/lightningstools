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
    //Simtek 10-0294 F-16 Simulated Fuel Quantity Indicator
    public class Simtek100294HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100294HardwareSupportModule));
        private readonly IFuelQuantityIndicator _renderer = new FuelQuantityIndicator();

        private AnalogSignal _aftLeftFuelInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _aftLeftFuelInputSignalChangedEventHandler;
        private AnalogSignal _aftLeftOutputSignal;
        private AnalogSignal _counterOutputSignal;
        private AnalogSignal _foreRightFuelInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _foreRightFuelInputSignalChangedEventHandler;
        private AnalogSignal _foreRightOutputSignal;
        private bool _isDisposed;
        private AnalogSignal _totalFuelInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _totalFuelInputSignalChangedEventHandler;

        // Editor-authored calibration. Each output channel can be overridden
        // independently — when an override is present, that channel uses
        // EvaluatePiecewise + ApplyTrim. When absent, the legacy linear math
        // below runs for that channel using _maxPoundsTotalFuel as the
        // counter denominator.
        private Simtek100294HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _counterChannel;
        private GaugeChannelConfig _aftLeftChannel;
        private GaugeChannelConfig _foreRightChannel;

        private ConfigFileReloadWatcher _configWatcher;

        // Legacy linear-rescale denominator for the counter output. Loaded
        // from the bare <MaxPoundsTotalFuel> field for back-compat. Only
        // consulted when the counter has no piecewise override.
        private const uint DEFAULT_MAX_POUNDS_TOTAL_FUEL = 9900;
        private uint _maxPoundsTotalFuel = DEFAULT_MAX_POUNDS_TOTAL_FUEL;

        public Simtek100294HardwareSupportModule(Simtek100294HardwareSupportModuleConfig config)
        {
            _config = config;
            ApplyLegacyFields(config);
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        private void ApplyLegacyFields(Simtek100294HardwareSupportModuleConfig config)
        {
            if (config == null) return;
            if (config.MaxPoundsTotalFuel.HasValue) _maxPoundsTotalFuel = config.MaxPoundsTotalFuel.Value;
        }

        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            _counterChannel  = ResolvePiecewiseChannel(config, "100294_Counter_To_Instrument");
            _aftLeftChannel  = ResolvePiecewiseChannel(config, "100294_AL_To_Instrument");
            _foreRightChannel = ResolvePiecewiseChannel(config, "100294_FR_To_Instrument");
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
                var reloaded = Simtek100294HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ApplyLegacyFields(reloaded);
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

        public override AnalogSignal[] AnalogInputs => new[]
            {_totalFuelInputSignal, _foreRightFuelInputSignal, _aftLeftFuelInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_foreRightOutputSignal, _aftLeftOutputSignal, _counterOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0294 - Indicator - Simulated Fuel Qty";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100294HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100294HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100294HardwareSupportModule.config");
                hsmConfig = Simtek100294HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100294HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.AftLeftFuelQuantityPounds = (float) _aftLeftFuelInputSignal.State;
            _renderer.InstrumentState.ForeRightFuelQuantityPounds = (float) _foreRightFuelInputSignal.State;
            _renderer.InstrumentState.TotalFuelQuantityPounds = (float) _totalFuelInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _totalFuelInputSignalChangedEventHandler = null;
            _foreRightFuelInputSignalChangedEventHandler = null;
            _aftLeftFuelInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateAftLeftFuelInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "A/L Fuel",
                Id = "100294_AftAndLeft_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 42000
            };
            return thisSignal;
        }

        private AnalogSignal CreateAftLeftOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "A/L",
                Id = "100294_AL_To_Instrument",
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

        private AnalogSignal CreateCounterOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = nameof(Counter),
                Id = "100294_Counter_To_Instrument",
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

        private AnalogSignal CreateForeRightFuelInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "F/R Fuel",
                Id = "100294_ForeAndRight_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 42000
            };
            return thisSignal;
        }

        private AnalogSignal CreateForeRightOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "F/R",
                Id = "100294_FR_To_Instrument",
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
            _totalFuelInputSignalChangedEventHandler =
                fuel_InputSignalChanged;
            _aftLeftFuelInputSignalChangedEventHandler =
                fuel_InputSignalChanged;
            _foreRightFuelInputSignalChangedEventHandler =
                fuel_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _totalFuelInputSignal = CreateTotalFuelInputSignal();
            _aftLeftFuelInputSignal = CreateAftLeftFuelInputSignal();
            _foreRightFuelInputSignal = CreateForeRightFuelInputSignal();
        }


        private void CreateOutputSignals()
        {
            _foreRightOutputSignal = CreateForeRightOutputSignal();
            _aftLeftOutputSignal = CreateAftLeftOutputSignal();
            _counterOutputSignal = CreateCounterOutputSignal();
        }

        private AnalogSignal CreateTotalFuelInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Total Fuel (Pounds)",
                Id = "100294_Total_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = _maxPoundsTotalFuel
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

        private void fuel_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_totalFuelInputSignal != null)
            {
                _totalFuelInputSignal.SignalChanged += _totalFuelInputSignalChangedEventHandler;
            }
            if (_aftLeftFuelInputSignal != null)
            {
                _aftLeftFuelInputSignal.SignalChanged += _aftLeftFuelInputSignalChangedEventHandler;
            }
            if (_foreRightFuelInputSignal != null)
            {
                _foreRightFuelInputSignal.SignalChanged += _foreRightFuelInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_totalFuelInputSignalChangedEventHandler != null && _totalFuelInputSignal != null)
            {
                try
                {
                    _totalFuelInputSignal.SignalChanged -= _totalFuelInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }


            if (_aftLeftFuelInputSignalChangedEventHandler != null && _aftLeftFuelInputSignal != null)
            {
                try
                {
                    _aftLeftFuelInputSignal.SignalChanged -= _aftLeftFuelInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_foreRightFuelInputSignalChangedEventHandler == null || _foreRightFuelInputSignal == null) return;
            try
            {
                _foreRightFuelInputSignal.SignalChanged -= _foreRightFuelInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            // F/R pointer: piecewise override or legacy linear.
            if (_foreRightOutputSignal != null)
            {
                if (_foreRightChannel != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(
                        _foreRightFuelInputSignal.State,
                        _foreRightChannel.Transform.Breakpoints);
                    _foreRightOutputSignal.State = _foreRightChannel.ApplyTrim(v, _foreRightOutputSignal.MinValue, _foreRightOutputSignal.MaxValue);
                }
                else
                {
                    _foreRightOutputSignal.State = _foreRightFuelInputSignal.State / 100.00 / 42.00 * 20.00 - 10.00;
                }
            }

            // A/L pointer: piecewise override or legacy linear.
            if (_aftLeftOutputSignal != null)
            {
                if (_aftLeftChannel != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(
                        _aftLeftFuelInputSignal.State,
                        _aftLeftChannel.Transform.Breakpoints);
                    _aftLeftOutputSignal.State = _aftLeftChannel.ApplyTrim(v, _aftLeftOutputSignal.MinValue, _aftLeftOutputSignal.MaxValue);
                }
                else
                {
                    _aftLeftOutputSignal.State = _aftLeftFuelInputSignal.State / 100.00 / 42.00 * 20.00 - 10.00;
                }
            }

            // Counter: piecewise override or legacy linear (with the bare
            // MaxPoundsTotalFuel as denominator).
            if (_counterOutputSignal != null)
            {
                if (_counterChannel != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(
                        _totalFuelInputSignal.State,
                        _counterChannel.Transform.Breakpoints);
                    _counterOutputSignal.State = _counterChannel.ApplyTrim(v, _counterOutputSignal.MinValue, _counterOutputSignal.MaxValue);
                }
                else
                {
                    _counterOutputSignal.State = (_totalFuelInputSignal.State / _maxPoundsTotalFuel) * 20.00 - 10.00;
                }
            }
        }
    }
}
