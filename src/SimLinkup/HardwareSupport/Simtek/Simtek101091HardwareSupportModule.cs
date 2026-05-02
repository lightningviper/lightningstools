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

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1091 F-16 ENGINE OIL PRESSURE IND (resolver pair: 0..100% → 0..320°)
    public class Simtek101091HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101091HardwareSupportModule));
        private readonly IOilPressureGauge _renderer = new OilPressureGauge();

        // Editor-authored calibration. Same hot-reload contract as the
        // Simtek 10-1088 nozzle gauge; see that class for the full
        // explanation of the resolver-pair pattern.
        private Simtek101091HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _oilPressureCOSOutputSignal;
        private AnalogSignal _oilPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _oilPressureInputSignalChangedEventHandler;
        private AnalogSignal _oilPressureSINOutputSignal;

        public Simtek101091HardwareSupportModule(Simtek101091HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolvePiecewiseResolverPair(config,
                "101091_Oil_Pressure_SIN_To_Instrument",
                "101091_Oil_Pressure_COS_To_Instrument",
                out _resolverTransform, out _sinChannel, out _cosChannel);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Piecewise-resolver-pair resolver: pulls the SIN channel (which
        // carries the breakpoint table + PeakVolts) and the COS channel
        // (which just carries its own per-channel trim). Returns null
        // transform when the config doesn't carry a usable record; HSM
        // falls back to the hardcoded path in that case.
        private static void ResolvePiecewiseResolverPair(
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
                || t.Kind != "piecewise_resolver"
                || t.Breakpoints == null
                || t.Breakpoints.Length < 2
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
                var reloaded = Simtek101091HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolvePiecewiseResolverPair(reloaded,
                    "101091_Oil_Pressure_SIN_To_Instrument",
                    "101091_Oil_Pressure_COS_To_Instrument",
                    out _resolverTransform, out _sinChannel, out _cosChannel);
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

        public override AnalogSignal[] AnalogInputs => new[] {_oilPressureInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_oilPressureSINOutputSignal, _oilPressureCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1091 - Engine Oil Pressure Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101091HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101091HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101091HardwareSupportModule.config");
                hsmConfig = Simtek101091HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101091HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.OilPressurePercent = (float) _oilPressureInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _oilPressureInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _oilPressureInputSignalChangedEventHandler =
                oil_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _oilPressureInputSignal = CreateOilInputSignal();
        }

        private AnalogSignal CreateOilCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Oil  Pressure (COS)",
                Id = "101091_Oil_Pressure_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateOilInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Oil Pressure (0-100%)",
                Id = "101091_Oil_Pressure_From_Sim",
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

        private AnalogSignal CreateOilSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Oil  Pressure (SIN)",
                Id = "101091_Oil_Pressure_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _oilPressureSINOutputSignal = CreateOilSINOutputSignal();
            _oilPressureCOSOutputSignal = CreateOilCOSOutputSignal();
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

        private void oil_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_oilPressureInputSignal != null)
            {
                _oilPressureInputSignal.SignalChanged += _oilPressureInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_oilPressureInputSignalChangedEventHandler == null || _oilPressureInputSignal == null) return;
            try
            {
                _oilPressureInputSignal.SignalChanged -= _oilPressureInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_oilPressureInputSignal == null) return;
            var oilPressureInput = _oilPressureInputSignal.State;

            // Editor-authored override: when a .config file declared a
            // resolver transform for this gauge, evaluate via the generic
            // helper and per-channel trim. Falls through to the hardcoded
            // sin/cos blocks below when no config is present.
            if (_resolverTransform != null
                && _sinChannel != null
                && _cosChannel != null
                && _oilPressureSINOutputSignal != null
                && _oilPressureCOSOutputSignal != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    oilPressureInput, t.Breakpoints, t.PeakVolts.Value);
                _oilPressureSINOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _oilPressureSINOutputSignal.MinValue, _oilPressureSINOutputSignal.MaxValue);
                _oilPressureCOSOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _oilPressureCOSOutputSignal.MinValue, _oilPressureCOSOutputSignal.MaxValue);
                return;
            }

            if (_oilPressureSINOutputSignal != null)
            {
                var oilPressureSINOutputValue = oilPressureInput < 0
                    ? 0
                    : (oilPressureInput > 100
                        ? 10.0000 * Math.Sin(320.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(oilPressureInput / 100.0000 * 320.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (oilPressureSINOutputValue < -10)
                {
                    oilPressureSINOutputValue = -10;
                }
                else if (oilPressureSINOutputValue > 10)
                {
                    oilPressureSINOutputValue = 10;
                }

                _oilPressureSINOutputSignal.State = oilPressureSINOutputValue;
            }

            if (_oilPressureCOSOutputSignal == null) return;
            var oilPressureCOSOutputValue = oilPressureInput < 0
                ? 0
                : (oilPressureInput > 100
                    ? 10.0000 * Math.Cos(320.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(oilPressureInput / 100.0000 * 320.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (oilPressureCOSOutputValue < -10)
            {
                oilPressureCOSOutputValue = -10;
            }
            else if (oilPressureCOSOutputValue > 10)
            {
                oilPressureCOSOutputValue = 10;
            }

            _oilPressureCOSOutputSignal.State = oilPressureCOSOutputValue;
        }
    }
}