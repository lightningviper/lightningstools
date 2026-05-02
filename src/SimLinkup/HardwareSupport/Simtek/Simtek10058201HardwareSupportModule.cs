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
    //Simtek 10-0582-01 F-16 AOA Indicator (piecewise: -5..40° → -6.37..+10 V; +13° = 0 V)
    public class Simtek10058201HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek10058201HardwareSupportModule));
        private readonly IAngleOfAttackIndicator _renderer = new AngleOfAttackIndicator();

        // Editor-authored calibration. Default ships the 3 spec-sheet
        // calibration test points (-5° lower stop, +13° on-speed, +40°
        // upper stop) so users can verify their hardware against the
        // manufacturer's checkpoints. The digital POWER-OFF input still
        // overrides to -10 V regardless of the AoA value when it's true
        // — that's gauge mechanism, not user-calibratable, so it stays in
        // the override branch below before evaluating the table.
        private Simtek10058201HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _aoaCalibration;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _aoaInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _aoaInputSignalChangedEventHandler;

        private AnalogSignal _aoaOutputSignal;
        private DigitalSignal.SignalChangedEventHandler _aoaPowerInputSignalChangedEventHandler;
        private DigitalSignal _aoaPowerOffInputSignal;

        private bool _isDisposed;

        public Simtek10058201HardwareSupportModule(Simtek10058201HardwareSupportModuleConfig config)
        {
            _config = config;
            _aoaCalibration = ResolvePiecewiseChannel(config, "10058201_AOA_To_Instrument");
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Same helper as the other piecewise gauges.
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
                var reloaded = Simtek10058201HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                _aoaCalibration = ResolvePiecewiseChannel(reloaded, "10058201_AOA_To_Instrument");
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

        public override AnalogSignal[] AnalogInputs => new[] {_aoaInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_aoaOutputSignal};

        public override DigitalSignal[] DigitalInputs => new[] {_aoaPowerOffInputSignal};

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName =>
            "Simtek P/N 10-0582-01 - Indicator - Simulated Angle Of Attack Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek10058201HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek10058201HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek10058201HardwareSupportModule.config");
                hsmConfig = Simtek10058201HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek10058201HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            g.Clear(Color.Black);
            _renderer.InstrumentState.OffFlag = _aoaPowerOffInputSignal.State;
            _renderer.InstrumentState.AngleOfAttackDegrees = (float) _aoaInputSignal.State;

            var aoaWidth = (int) (destinationRectangle.Height * (102f / 227f));
            var aoaHeight = destinationRectangle.Height;

            using (var aoaBmp = new Bitmap(aoaWidth, aoaHeight))
            using (var aoaBmpGraphics = Graphics.FromImage(aoaBmp))
            {
                _renderer.Render(aoaBmpGraphics, new Rectangle(0, 0, aoaWidth, aoaHeight));
                var targetRectangle = new Rectangle(
                    destinationRectangle.X + (int) ((destinationRectangle.Width - aoaWidth) / 2.0),
                    destinationRectangle.Y, aoaWidth, destinationRectangle.Height);
                g.DrawImage(aoaBmp, targetRectangle);
            }
        }

        private void AbandonInputEventHandlers()
        {
            _aoaInputSignalChangedEventHandler = null;
            _aoaPowerInputSignalChangedEventHandler = null;
        }

        private void AOA_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void AOAPower_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private AnalogSignal CreateAOAInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Angle of Attack [alpha] (degrees)",
                Id = "10058201_AOA_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -5,
                MaxValue = 40
            };
            return thisSignal;
        }

        private AnalogSignal CreateAOAOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "AOA",
                Id = "10058201_AOA_To_Instrument",
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

        private DigitalSignal CreateAOAPowerInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag",
                Id = "10058201_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _aoaInputSignalChangedEventHandler = AOA_InputSignalChanged;
            _aoaPowerInputSignalChangedEventHandler =
                AOAPower_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _aoaInputSignal = CreateAOAInputSignal();
            _aoaPowerOffInputSignal = CreateAOAPowerInputSignal();
        }

        private void CreateOutputSignals()
        {
            _aoaOutputSignal = CreateAOAOutputSignal();
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
            if (_aoaInputSignal != null)
            {
                _aoaInputSignal.SignalChanged += _aoaInputSignalChangedEventHandler;
            }
            if (_aoaPowerOffInputSignal != null)
            {
                _aoaPowerOffInputSignal.SignalChanged += _aoaPowerInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_aoaInputSignalChangedEventHandler != null && _aoaInputSignal != null)
            {
                try
                {
                    _aoaInputSignal.SignalChanged -= _aoaInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_aoaPowerInputSignalChangedEventHandler == null || _aoaPowerOffInputSignal == null) return;
            try
            {
                _aoaPowerOffInputSignal.SignalChanged -= _aoaPowerInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            var aoaPowerOff = false;
            if (_aoaPowerOffInputSignal != null)
            {
                aoaPowerOff = _aoaPowerOffInputSignal.State;
            }

            if (_aoaInputSignal != null)
            {
                var aoaInput = _aoaInputSignal.State;

                if (_aoaOutputSignal == null) return;

                // Editor-authored override: when a .config file declared a
                // piecewise table for this channel, evaluate via the
                // generic helper and per-channel trim. Power-off override
                // is checked first regardless of the config — it's gauge
                // mechanism, not a calibratable property. Falls through
                // to the hardcoded formula below when no config is present.
                if (_aoaCalibration != null)
                {
                    double v;
                    if (aoaPowerOff)
                    {
                        v = -10;
                    }
                    else
                    {
                        v = GaugeTransform.EvaluatePiecewise(aoaInput, _aoaCalibration.Transform.Breakpoints);
                    }
                    _aoaOutputSignal.State = _aoaCalibration.ApplyTrim(v, _aoaOutputSignal.MinValue, _aoaOutputSignal.MaxValue);
                    return;
                }

                var aoaOutputValue = aoaPowerOff
                    ? -10
                    : (aoaInput < -5
                        ? -6.37
                        : (aoaInput >= -5 && aoaInput <= 40 ? -6.37 + (aoaInput + 5) / 45 * 16.37 : 10));

                if (aoaOutputValue < -10)
                {
                    aoaOutputValue = -10;
                }
                else if (aoaOutputValue > 10)
                {
                    aoaOutputValue = 10;
                }

                _aoaOutputSignal.State = aoaOutputValue;
            }
        }
    }
}