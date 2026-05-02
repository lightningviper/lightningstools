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

namespace SimLinkup.HardwareSupport.Lilbern
{
    //Lilbern 3321 F-16 RPM Indicator
    public class Lilbern3321HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Lilbern3321HardwareSupportModule));
        private readonly ITachometer _renderer = new Tachometer();

        private bool _isDisposed;
        private AnalogSignal _rpmInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rpmInputSignalChangedEventHandler;
        private AnalogSignal _rpmSinOutputSignal;
        private AnalogSignal _rpmCosOutputSignal;

        private Lilbern3321HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Lilbern3321HardwareSupportModule(Lilbern3321HardwareSupportModuleConfig config)
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
            ResolvePiecewiseResolverPair(config,
                "3321_RPM_SIN_To_Instrument",
                "3321_RPM_COS_To_Instrument",
                out _resolverTransform, out _sinChannel, out _cosChannel);
        }

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
                var reloaded = Lilbern3321HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] { _rpmInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _rpmSinOutputSignal, _rpmCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Lilbern M/N 3321 - Indicator, Simulated Tachometer";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Lilbern3321HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Lilbern3321HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Lilbern3321HardwareSupportModule.config");
                hsmConfig = Lilbern3321HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Lilbern3321HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.RPMPercent = (float)_rpmInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = rpm_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _rpmInputSignal = CreateRPMInputSignal();
        }

        private void CreateOutputSignals()
        {
            _rpmSinOutputSignal = CreateRPMSinOutputSignal();
            _rpmCosOutputSignal = CreateRPMCosOutputSignal();
        }

        private AnalogSignal CreateRPMInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "RPM",
                Id = "3321_RPM_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0,
                MinValue = 0,
                MaxValue = 110
            };
            return thisSignal;
        }

        private AnalogSignal CreateRPMSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM (SIN)",
                Id = "3321_RPM_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }
        private AnalogSignal CreateRPMCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM (COS)",
                Id = "3321_RPM_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
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

        private void RegisterForInputEvents()
        {
            if (_rpmInputSignal != null)
            {
                _rpmInputSignal.SignalChanged += _rpmInputSignalChangedEventHandler;
            }
        }

        private void rpm_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_rpmInputSignalChangedEventHandler != null && _rpmInputSignal != null)
            {
                try
                {
                    _rpmInputSignal.SignalChanged -= _rpmInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_rpmInputSignal == null) return;
            var rpmInput = _rpmInputSignal.State;
            if (_rpmSinOutputSignal == null || _rpmCosOutputSignal == null) return;

            // Editor override: piecewise_resolver pair.
            if (_resolverTransform != null && _sinChannel != null && _cosChannel != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    rpmInput, t.Breakpoints, t.PeakVolts.Value);
                _rpmSinOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _rpmSinOutputSignal.MinValue, _rpmSinOutputSignal.MaxValue);
                _rpmCosOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _rpmCosOutputSignal.MinValue, _rpmCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — input → angle via two-segment piecewise,
            // then sin/cos × 10 V.
            var degrees = 0.00;
            var sixtyRpmDegrees = 90;
            var oneHundredTenRpmDegrees = 330;
            if (rpmInput < 60)
            {
                degrees = (rpmInput / 60.00) * sixtyRpmDegrees;
            }
            else
            {
                degrees =
                    (rpmInput - 60.00)
                        /
                    (110 - 60)
                    * (oneHundredTenRpmDegrees - sixtyRpmDegrees)
                    + sixtyRpmDegrees;
            }

            var sinVoltage = Math.Sin(degrees * Constants.RADIANS_PER_DEGREE) * 10.00;
            var cosVoltage = Math.Cos(degrees * Constants.RADIANS_PER_DEGREE) * 10.00;

            if (sinVoltage > 10.00) sinVoltage = 10.00;
            else if (sinVoltage < -10.00) sinVoltage = -10.00;
            _rpmSinOutputSignal.State = sinVoltage;

            if (cosVoltage > 10.00) cosVoltage = 10.00;
            else if (cosVoltage < -10.00) cosVoltage = -10.00;
            _rpmCosOutputSignal.State = cosVoltage;
        }
    }
}
