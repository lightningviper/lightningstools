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
    //Simtek 10-1089-02 F-16 Simulated Fuel Quantity Indicator (v2)
    public class Simtek10108902HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek10108902HardwareSupportModule));
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
        // below runs for that channel (counter denominator stays hardcoded
        // at 20100 since this gauge has no MaxPoundsTotalFuel-style field).
        private Simtek10108902HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _counterChannel;
        private GaugeChannelConfig _aftLeftChannel;
        private GaugeChannelConfig _foreRightChannel;

        private ConfigFileReloadWatcher _configWatcher;

        public Simtek10108902HardwareSupportModule(Simtek10108902HardwareSupportModuleConfig config)
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
            _counterChannel  = ResolvePiecewiseChannel(config, "10108902_Counter_To_Instrument");
            _aftLeftChannel  = ResolvePiecewiseChannel(config, "10108902_AL_To_Instrument");
            _foreRightChannel = ResolvePiecewiseChannel(config, "10108902_FR_To_Instrument");
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
                var reloaded = Simtek10108902HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[]
            {_totalFuelInputSignal, _foreRightFuelInputSignal, _aftLeftFuelInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_foreRightOutputSignal, _aftLeftOutputSignal, _counterOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1089-02 - Indicator - Simulated Fuel Qty";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek10108902HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek10108902HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek10108902HardwareSupportModule.config");
                hsmConfig = Simtek10108902HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek10108902HardwareSupportModule(hsmConfig) };
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
                Id = "10108902_AftAndLeft_Fuel_From_Sim",
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
                Id = "10108902_AL_To_Instrument",
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
                Id = "10108902_Counter_To_Instrument",
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
                Id = "10108902_ForeAndRight_Fuel_From_Sim",
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
                Id = "10108902_FR_To_Instrument",
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
                Id = "10108902_Total_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 20100
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

            // Counter: piecewise override or hardcoded linear (20100 max,
            // unlike 10-0294 which has an editable MaxPoundsTotalFuel).
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
                    _counterOutputSignal.State = _totalFuelInputSignal.State / 20100 * 20.00 - 10.00;
                }
            }
        }
    }
}
