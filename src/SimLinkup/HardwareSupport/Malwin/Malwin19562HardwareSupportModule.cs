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
    //Malwin 1956-2 F-16 FTIT Indicator
    public class Malwin19562HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Malwin19562HardwareSupportModule));
        private readonly IFanTurbineInletTemperature _renderer = new FanTurbineInletTemperature();

        private AnalogSignal _ftitInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _ftitInputSignalChangedEventHandler;
        private AnalogSignal _ftitSinOutputSignal;
        private AnalogSignal _ftitCosOutputSignal;

        private bool _isDisposed;

        private Malwin19562HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Malwin19562HardwareSupportModule(Malwin19562HardwareSupportModuleConfig config)
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
                "19562_FTIT_SIN_To_Instrument",
                "19562_FTIT_COS_To_Instrument",
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
                var reloaded = Malwin19562HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] { _ftitInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _ftitSinOutputSignal, _ftitCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 1956-2  - Indicator, Simulated FTIT";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin19562HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Malwin19562HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Malwin19562HardwareSupportModule.config");
                hsmConfig = Malwin19562HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Malwin19562HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.InletTemperatureDegreesCelcius = (float)_ftitInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _ftitInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateFTITInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "FTIT",
                Id = "19562_FTIT_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 1200
            };
            return thisSignal;
        }

        private AnalogSignal CreateFTITSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT (SIN)",
                Id = "19562_FTIT_SIN_To_Instrument",
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

        private AnalogSignal CreateFTITCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT (COS)",
                Id = "19562_FTIT_COS_To_Instrument",
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
            _ftitInputSignalChangedEventHandler =
                ftit_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _ftitInputSignal = CreateFTITInputSignal();
        }

        private void CreateOutputSignals()
        {
            _ftitSinOutputSignal = CreateFTITSinOutputSignal();
            _ftitCosOutputSignal = CreateFTITCosOutputSignal();
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

        private void ftit_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_ftitInputSignal != null)
            {
                _ftitInputSignal.SignalChanged += _ftitInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_ftitInputSignalChangedEventHandler != null && _ftitInputSignal != null)
            {
                try
                {
                    _ftitInputSignal.SignalChanged -= _ftitInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_ftitInputSignal == null) return;
            if (_ftitSinOutputSignal == null || _ftitCosOutputSignal == null) return;
            var ftitInput = _ftitInputSignal.State;
            if (ftitInput < 0.00) ftitInput = 0;
            else if (ftitInput > 1200.00) ftitInput = 1200.00;

            // Editor override: piecewise_resolver pair.
            if (_resolverTransform != null && _sinChannel != null && _cosChannel != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    ftitInput, t.Breakpoints, t.PeakVolts.Value);
                _ftitSinOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _ftitSinOutputSignal.MinValue, _ftitSinOutputSignal.MaxValue);
                _ftitCosOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _ftitCosOutputSignal.MinValue, _ftitCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — input → angle via 4-segment piecewise,
            // then sin/cos × 10 V. NOTE: the original C# was missing the
            // ×10 multiplier so legacy installs got ±1 V output. The
            // multiplier is now applied here to fix the fallback path.
            double ftitOutputDegrees = 0;
            if (ftitInput <= 200)
            {
                ftitOutputDegrees = 0;
            }
            else if (ftitInput >= 200 && ftitInput < 700)
            {
                ftitOutputDegrees = 20.00 * ((ftitInput - 200.00) / 100.00);
            }
            else if (ftitInput >= 700 && ftitInput < 1000)
            {
                ftitOutputDegrees = (60.00 * ((ftitInput - 700.00) / 100.00)) + 100.00;
            }
            else if (ftitInput >= 1000 && ftitInput <= 1200)
            {
                ftitOutputDegrees = (20.00 * ((ftitInput - 1000.00) / 100.00)) + 280;
            }

            var ftitOutputSinVoltage = 10.00 * Math.Sin(ftitOutputDegrees * Constants.RADIANS_PER_DEGREE);
            if (ftitOutputSinVoltage < -10) ftitOutputSinVoltage = -10;
            else if (ftitOutputSinVoltage > 10) ftitOutputSinVoltage = 10;
            _ftitSinOutputSignal.State = ftitOutputSinVoltage;

            var ftitOutputCosVoltage = 10.00 * Math.Cos(ftitOutputDegrees * Constants.RADIANS_PER_DEGREE);
            if (ftitOutputCosVoltage < -10) ftitOutputCosVoltage = -10;
            else if (ftitOutputCosVoltage > 10) ftitOutputCosVoltage = 10;
            _ftitCosOutputSignal.State = ftitOutputCosVoltage;
        }
    }
}
