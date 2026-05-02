using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;
using log4net;

namespace SimLinkup.HardwareSupport.Malwin
{
    //Malwin 1956-3 F-16 Liquid Oxygen Quantity Indicator
    public class Malwin19563HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Malwin19563HardwareSupportModule));
        //There are currently No LOX QTY outputs from BMS, so there is no renderer in LightningGauges for this
        //private readonly ILiquidOxygenQuantity _renderer = new LiquidOxygenQuantity();

        private AnalogSignal _loxQtyInputSignal; //reserved for future use
        private AnalogSignal.AnalogSignalChangedEventHandler _loxQtyInputSignalChangedEventHandler; //reserved for future use
        private AnalogSignal _loxQtySinOutputSignal;
        private AnalogSignal _loxQtyCosOutputSignal;

        private bool _isDisposed;

        private Malwin19563HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Malwin19563HardwareSupportModule(Malwin19563HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
            UpdateOutputValues();
        }

        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            ResolveMultiResolverPair(config,
                "19563_LOX_SIN_To_Instrument",
                "19563_LOX_COS_To_Instrument",
                out _resolverTransform, out _sinChannel, out _cosChannel);
        }

        private static void ResolveMultiResolverPair(
            GaugeCalibrationConfig config,
            string sinChannelId,
            string cosChannelId,
            out GaugeTransformConfig transform,
            out GaugeChannelConfig sinCh,
            out GaugeChannelConfig cosCh)
        {
            transform = null;
            sinCh = null;
            cosCh = null;
            if (config == null) return;
            var s = config.FindChannel(sinChannelId);
            var c = config.FindChannel(cosChannelId);
            if (s == null || c == null) return;
            var t = s.Transform;
            if (t == null
                || t.Kind != "multi_resolver"
                || !t.UnitsPerRevolution.HasValue
                || t.UnitsPerRevolution.Value == 0
                || !t.PeakVolts.HasValue)
            {
                return;
            }
            transform = t;
            sinCh = s;
            cosCh = c;
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
                var reloaded = Malwin19563HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] { _loxQtyInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _loxQtySinOutputSignal, _loxQtyCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 1956-3  - Indicator, Simulated Liquid Oxygen Quantity";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin19563HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Malwin19563HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Malwin19563HardwareSupportModule.config");
                hsmConfig = Malwin19563HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Malwin19563HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            //_renderer.InstrumentState.InletTemperatureDegreesCelcius = (float)_loxQtyInputSignal.State;
            //_renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _loxQtyInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateLoxQtyInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "LOX QTY",
                Id = "19563_LOX_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = (new Random().NextDouble() * 2.00) + 3.00,
                MinValue = 0,
                MaxValue = 5
            };
            return thisSignal;
        }

        private AnalogSignal CreateLoxQtySinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "LOX QTY (SIN)",
                Id = "19563_LOX_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0, //volts
                IsSine = true,
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateLoxQtyCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "LOX QTY (COS)",
                Id = "19563_LOX_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00, //volts
                IsCosine = true,
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _loxQtyInputSignalChangedEventHandler =
                loxQty_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _loxQtyInputSignal = CreateLoxQtyInputSignal();
        }

        private void CreateOutputSignals()
        {
            _loxQtySinOutputSignal = CreateLoxQtySinOutputSignal();
            _loxQtyCosOutputSignal = CreateLoxQtyCosOutputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    //Common.Util.DisposeObject(_renderer);
                    if (_configWatcher != null)
                    {
                        try { _configWatcher.Dispose(); } catch { }
                        _configWatcher = null;
                    }
                }
            }
            _isDisposed = true;
        }

        private void loxQty_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        public override void Synchronize()
        {
            base.Synchronize();
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_loxQtyInputSignal != null)
            {
                _loxQtyInputSignal.SignalChanged += _loxQtyInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_loxQtyInputSignalChangedEventHandler != null && _loxQtyInputSignal != null)
            {
                try
                {
                    _loxQtyInputSignal.SignalChanged -= _loxQtyInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_loxQtyInputSignal == null) return;
            if (_loxQtySinOutputSignal == null || _loxQtyCosOutputSignal == null) return;
            var loxQtyInput = _loxQtyInputSignal.State;
            if (loxQtyInput < 0.00) loxQtyInput = 0;
            else if (loxQtyInput > 5.00) loxQtyInput = 5.00;

            // Editor override: multi_resolver pair.
            if (_resolverTransform != null && _sinChannel != null && _cosChannel != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluateMultiTurnResolver(
                    loxQtyInput, t.UnitsPerRevolution.Value, t.PeakVolts.Value);
                _loxQtySinOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _loxQtySinOutputSignal.MinValue, _loxQtySinOutputSignal.MaxValue);
                _loxQtyCosOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _loxQtyCosOutputSignal.MinValue, _loxQtyCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — `(input/5) × 180°` then sin/cos × 10 V.
            // NOTE: original C# was missing the ×10 multiplier; fixed here.
            var loxQtyOutputDegrees = (loxQtyInput / 5.00) * 180.00;

            var loxQtyOutputSinVoltage = 10.00 * Math.Sin(loxQtyOutputDegrees * Constants.RADIANS_PER_DEGREE);
            if (loxQtyOutputSinVoltage < -10) loxQtyOutputSinVoltage = -10;
            else if (loxQtyOutputSinVoltage > 10) loxQtyOutputSinVoltage = 10;
            _loxQtySinOutputSignal.State = loxQtyOutputSinVoltage;

            var loxQtyOutputCosVoltage = 10.00 * Math.Cos(loxQtyOutputDegrees * Constants.RADIANS_PER_DEGREE);
            if (loxQtyOutputCosVoltage < -10) loxQtyOutputCosVoltage = -10;
            else if (loxQtyOutputCosVoltage > 10) loxQtyOutputCosVoltage = 10;
            _loxQtyCosOutputSignal.State = loxQtyOutputCosVoltage;
        }
    }
}
