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
    //Simtek 10-5866 F-16 Fuel Quantity Indicator (multi-pointer, with remote electronics)
    //
    // Per the spec sheet (PL20-5866, dwg 10-5866):
    //   Drive type: MULTIPLE DC SERVO
    //   Connectors: 3 signal inputs (A/L, F/R, TOTAL)
    //   Calibration:
    //     Table 1 — SEL LBS×100, 0..42 (0..4200 LBS) → -10..+10 V linear
    //               (used for both A/L and F/R pointers; cockpit switch
    //                selects which signal drives SEL pointer).
    //     Table 2 — TOTAL LBS, 0..20000 → -10..+10 V linear (5-digit
    //                counter at the bottom of the dial).
    //   Remote electronics: 50-4363-01 (separate enclosure, see PL20-5866 fig 2).
    //
    // Modeled on Simtek10108902HardwareSupportModule (the existing F-16
    // fuel quantity indicator v2). Renderer reuse: borrows
    // IFuelQuantityIndicator from 10-1089-02.
    public class Simtek105866HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek105866HardwareSupportModule));
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

        private Simtek105866HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _counterChannel;
        private GaugeChannelConfig _aftLeftChannel;
        private GaugeChannelConfig _foreRightChannel;

        private ConfigFileReloadWatcher _configWatcher;

        public Simtek105866HardwareSupportModule(Simtek105866HardwareSupportModuleConfig config)
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
            _counterChannel  = ResolvePiecewiseChannel(config, "105866_Counter_To_Instrument");
            _aftLeftChannel  = ResolvePiecewiseChannel(config, "105866_AL_To_Instrument");
            _foreRightChannel = ResolvePiecewiseChannel(config, "105866_FR_To_Instrument");
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
                var reloaded = Simtek105866HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate the outputs with the cached input values so the
                // user sees the new calibration immediately.
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

        public override string FriendlyName => "Simtek P/N 10-5866 - Indicator - Fuel Qty (Multi-Pointer)";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek105866HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek105866HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek105866HardwareSupportModule.config");
                hsmConfig = Simtek105866HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek105866HardwareSupportModule(hsmConfig) };
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
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "A/L Fuel",
                Id = "105866_AftAndLeft_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 4200
            };
        }

        private AnalogSignal CreateAftLeftOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "A/L",
                Id = "105866_AL_To_Instrument",
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

        private AnalogSignal CreateCounterOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Counter",
                Id = "105866_Counter_To_Instrument",
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

        private AnalogSignal CreateForeRightFuelInputSignal()
        {
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "F/R Fuel",
                Id = "105866_ForeAndRight_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 4200
            };
        }

        private AnalogSignal CreateForeRightOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "F/R",
                Id = "105866_FR_To_Instrument",
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
            _totalFuelInputSignalChangedEventHandler = fuel_InputSignalChanged;
            _aftLeftFuelInputSignalChangedEventHandler = fuel_InputSignalChanged;
            _foreRightFuelInputSignalChangedEventHandler = fuel_InputSignalChanged;
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
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Total Fuel (Pounds)",
                Id = "105866_Total_Fuel_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 20000
            };
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
                _totalFuelInputSignal.SignalChanged += _totalFuelInputSignalChangedEventHandler;
            if (_aftLeftFuelInputSignal != null)
                _aftLeftFuelInputSignal.SignalChanged += _aftLeftFuelInputSignalChangedEventHandler;
            if (_foreRightFuelInputSignal != null)
                _foreRightFuelInputSignal.SignalChanged += _foreRightFuelInputSignalChangedEventHandler;
        }

        private void UnregisterForInputEvents()
        {
            if (_totalFuelInputSignalChangedEventHandler != null && _totalFuelInputSignal != null)
            {
                try { _totalFuelInputSignal.SignalChanged -= _totalFuelInputSignalChangedEventHandler; }
                catch (RemotingException) { }
            }
            if (_aftLeftFuelInputSignalChangedEventHandler != null && _aftLeftFuelInputSignal != null)
            {
                try { _aftLeftFuelInputSignal.SignalChanged -= _aftLeftFuelInputSignalChangedEventHandler; }
                catch (RemotingException) { }
            }
            if (_foreRightFuelInputSignalChangedEventHandler == null || _foreRightFuelInputSignal == null) return;
            try { _foreRightFuelInputSignal.SignalChanged -= _foreRightFuelInputSignalChangedEventHandler; }
            catch (RemotingException) { }
        }

        private void UpdateOutputValues()
        {
            // F/R pointer: 0..4200 lbs → -10..+10 V linear (Table 1).
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
                    _foreRightOutputSignal.State = _foreRightFuelInputSignal.State / 4200.0 * 20.0 - 10.0;
                }
            }

            // A/L pointer: 0..4200 lbs → -10..+10 V linear (Table 1).
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
                    _aftLeftOutputSignal.State = _aftLeftFuelInputSignal.State / 4200.0 * 20.0 - 10.0;
                }
            }

            // Counter: 0..20000 lbs → -10..+10 V linear (Table 2).
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
                    _counterOutputSignal.State = _totalFuelInputSignal.State / 20000.0 * 20.0 - 10.0;
                }
            }
        }
    }
}
