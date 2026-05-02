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

namespace SimLinkup.HardwareSupport.Gould
{
    //GOULD F-16 COMPASS
    public class GouldHS070D51341HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GouldHS070D51341HardwareSupportModule));
        private readonly ICompass _compass = new Compass();
        private AnalogSignal _compassCOSOutputSignal;
        private AnalogSignal _compassInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _compassInputSignalChangedEventHandler;
        private AnalogSignal _compassSINOutputSignal;

        private bool _isDisposed;

        private GouldHS070D51341HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public GouldHS070D51341HardwareSupportModule(GouldHS070D51341HardwareSupportModuleConfig config)
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
                "HS070D51341_Compass__SIN_To_Instrument",
                "HS070D51341_Compass__COS_To_Instrument",
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
                var reloaded = GouldHS070D51341HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] {_compassInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_compassSINOutputSignal, _compassCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Gould P/N HS070D5134-1 - Standby Compass";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GouldHS070D51341HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            GouldHS070D51341HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "GouldHS070D51341HardwareSupportModule.config");
                hsmConfig = GouldHS070D51341HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new GouldHS070D51341HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _compass.InstrumentState.MagneticHeadingDegrees = (float) _compassInputSignal.CorrelatedState;
            _compass.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _compassInputSignalChangedEventHandler = null;
        }

        private void compass_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private AnalogSignal CreateCompassCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (COS)",
                Id = "HS070D51341_Compass__COS_To_Instrument",
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

        private AnalogSignal CreateCompassInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Magnetic Heading (Degrees)",
                Id = "HS070D51341_Compass__Magnetic_Heading_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };

            return thisSignal;
        }

        private AnalogSignal CreateCompassSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (SIN)",
                Id = "HS070D51341_Compass__SIN_To_Instrument",
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

        private void CreateInputEventHandlers()
        {
            _compassInputSignalChangedEventHandler =
                compass_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _compassInputSignal = CreateCompassInputSignal();
        }

        private void CreateOutputSignals()
        {
            _compassSINOutputSignal = CreateCompassSINOutputSignal();
            _compassCOSOutputSignal = CreateCompassCOSOutputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_compass);
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
            if (_compassInputSignal != null)
            {
                _compassInputSignal.SignalChanged += _compassInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_compassInputSignalChangedEventHandler != null && _compassInputSignal != null)
            {
                try
                {
                    _compassInputSignal.SignalChanged -= _compassInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_compassInputSignal == null) return;
            var compassInput = Math.Abs(_compassInputSignal.CorrelatedState % 360.000);

            // Editor override: piecewise_resolver pair.
            if (_resolverTransform != null
                && _sinChannel != null
                && _cosChannel != null
                && _compassSINOutputSignal != null
                && _compassCOSOutputSignal != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    compassInput, t.Breakpoints, t.PeakVolts.Value);
                _compassSINOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _compassSINOutputSignal.MinValue, _compassSINOutputSignal.MaxValue);
                _compassCOSOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _compassCOSOutputSignal.MinValue, _compassCOSOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — straight 10·sin/cos(input°).
            if (_compassSINOutputSignal != null)
            {
                var compassSINOutputValue = 10.0000 * Math.Sin(compassInput * Constants.RADIANS_PER_DEGREE);
                if (compassSINOutputValue < -10) compassSINOutputValue = -10;
                else if (compassSINOutputValue > 10) compassSINOutputValue = 10;
                _compassSINOutputSignal.State = compassSINOutputValue;
            }

            if (_compassCOSOutputSignal == null) return;
            var compassCOSOutputValue = 10.0000 * Math.Cos(compassInput * Constants.RADIANS_PER_DEGREE);
            if (compassCOSOutputValue < -10) compassCOSOutputValue = -10;
            else if (compassCOSOutputValue > 10) compassCOSOutputValue = 10;
            _compassCOSOutputSignal.State = compassCOSOutputValue;
        }
    }
}
