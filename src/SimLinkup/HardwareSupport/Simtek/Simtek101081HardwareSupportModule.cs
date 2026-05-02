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
    //Simtek 10-1081 F-16 Altimeter v2 (fine multi_resolver + coarse piecewise)
    public class Simtek101081HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101081HardwareSupportModule));
        private readonly IAltimeter _renderer = new Altimeter();

        // Editor-authored calibration. Two override surfaces:
        //   - fine (multi_resolver pair): _fineTransform + _fineSinChannel + _fineCosChannel
        //   - coarse (piecewise):         _coarseChannel
        // Either may be null independently — Update*OutputValues falls
        // through to the hardcoded path for any unconfigured channel.
        private Simtek101081HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _fineTransform;
        private GaugeChannelConfig _fineSinChannel;
        private GaugeChannelConfig _fineCosChannel;
        private GaugeChannelConfig _coarseChannel;

        private ConfigFileReloadWatcher _configWatcher;

        private AnalogSignal _altitudeCoarseOutputSignal;
        private AnalogSignal _altitudeFineCosOutputSignal;
        private AnalogSignal _altitudeFineSinOutputSignal;
        private AnalogSignal _altitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _altitudeInputSignalChangedEventHandler;

        private AnalogSignal _barometricPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _barometricPressureInputSignalChangedEventHandler;
        private bool _isDisposed;

        public Simtek101081HardwareSupportModule(Simtek101081HardwareSupportModuleConfig config)
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
            ResolveMultiResolverPair(config,
                "101081_Altitude_Fine_SIN_To_Instrument",
                "101081_Altitude_Fine_COS_To_Instrument",
                out _fineTransform, out _fineSinChannel, out _fineCosChannel);
            _coarseChannel = ResolvePiecewiseChannel(config, "101081_Altitude_Coarse_To_Instrument");
        }

        // Multi-turn-resolver-pair resolver: SIN side carries
        // UnitsPerRevolution + PeakVolts; COS side just points back via
        // partnerChannel. Returns null transform when the config doesn't
        // carry a usable record.
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
                var reloaded = Simtek101081HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateAltitudeOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_altitudeInputSignal, _barometricPressureInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_altitudeFineSinOutputSignal, _altitudeFineCosOutputSignal, _altitudeCoarseOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1081 - Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101081HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101081HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101081HardwareSupportModule.config");
                hsmConfig = Simtek101081HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101081HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BarometricPressure = (float) _barometricPressureInputSignal.State * 100;
            _renderer.InstrumentState.IndicatedAltitudeFeetMSL = (float) _altitudeInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _altitudeInputSignalChangedEventHandler = null;
            _barometricPressureInputSignalChangedEventHandler = null;
        }

        private void altitude_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }

        private static void barometricPressure_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateBarometricPressureOutputValues();
        }

        private AnalogSignal CreateAltitudeCoarseOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse",
                Id = "101081_Altitude_Coarse_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (COS)",
                Id = "101081_Altitude_Fine_COS_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (SIN)",
                Id = "101081_Altitude_Fine_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Altitude (Indicated)",
                Id = "101081_Altitude_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -1000,
                MaxValue = 80000
            };
            return thisSignal;
        }

        private AnalogSignal CreateBarometricPressureInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Barometric Pressure (Indicated), In. Hg.",
                Id = "101081_Barometric_Pressure_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 29.92,
                MinValue = 28.10,
                MaxValue = 31.00
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _altitudeInputSignalChangedEventHandler =
                altitude_InputSignalChanged;
            _barometricPressureInputSignalChangedEventHandler =
                barometricPressure_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _altitudeInputSignal = CreateAltitudeInputSignal();
            _barometricPressureInputSignal = CreateBarometricPressureInputSignal();
        }

        private void CreateOutputSignals()
        {
            _altitudeFineSinOutputSignal = CreateAltitudeFineSinOutputSignal();
            _altitudeFineCosOutputSignal = CreateAltitudeFineCosOutputSignal();
            _altitudeCoarseOutputSignal = CreateAltitudeCoarseOutputSignal();
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
            if (_altitudeInputSignal != null)
            {
                _altitudeInputSignal.SignalChanged += _altitudeInputSignalChangedEventHandler;
            }
            if (_barometricPressureInputSignal != null)
            {
                _barometricPressureInputSignal.SignalChanged += _barometricPressureInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_altitudeInputSignalChangedEventHandler != null && _altitudeInputSignal != null)
            {
                try
                {
                    _altitudeInputSignal.SignalChanged -= _altitudeInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_barometricPressureInputSignalChangedEventHandler != null && _barometricPressureInputSignal != null)
            {
                try
                {
                    _barometricPressureInputSignal.SignalChanged -= _barometricPressureInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateAltitudeOutputValues()
        {
            if (_altitudeInputSignal != null)
            {
                var altitudeInput = _altitudeInputSignal.State;

                // Editor-authored override: when a multi_resolver config is
                // set, evaluate the fine sin/cos pair via the generic helper
                // and per-channel trim. Falls through to the hardcoded math
                // below when no config is present (sets the same Math.Sin/Cos
                // outputs after the conditional).
                bool fineHandled = false;
                if (_fineTransform != null
                    && _fineSinChannel != null
                    && _fineCosChannel != null
                    && _altitudeFineSinOutputSignal != null
                    && _altitudeFineCosOutputSignal != null)
                {
                    var t = _fineTransform;
                    var sinCos = GaugeTransform.EvaluateMultiTurnResolver(
                        altitudeInput, t.UnitsPerRevolution.Value, t.PeakVolts.Value);
                    _altitudeFineSinOutputSignal.State = _fineSinChannel.ApplyTrim(sinCos[0], _altitudeFineSinOutputSignal.MinValue, _altitudeFineSinOutputSignal.MaxValue);
                    _altitudeFineCosOutputSignal.State = _fineCosChannel.ApplyTrim(sinCos[1], _altitudeFineCosOutputSignal.MinValue, _altitudeFineCosOutputSignal.MaxValue);
                    fineHandled = true;
                }

                // Editor-authored override: when a piecewise config is set,
                // evaluate the coarse output via the generic helper. Falls
                // through to the hardcoded if/else below otherwise.
                bool coarseHandled = false;
                if (_coarseChannel != null && _altitudeCoarseOutputSignal != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(altitudeInput, _coarseChannel.Transform.Breakpoints);
                    _altitudeCoarseOutputSignal.State = _coarseChannel.ApplyTrim(v, _altitudeCoarseOutputSignal.MinValue, _altitudeCoarseOutputSignal.MaxValue);
                    coarseHandled = true;
                }

                // Both channels handled by configs — skip the hardcoded
                // path entirely.
                if (fineHandled && coarseHandled) return;

                double altitudeCoarseOutputValue = 0;

                var numRevolutionsOfFineResolver = altitudeInput / 1000.0000;
                var fineResolverDegrees = numRevolutionsOfFineResolver * 360.0000;
                var altitudeFineSinOutputValue = 10.0000 * Math.Sin(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);
                var altitudeFineCosOutputValue = 10.0000 * Math.Cos(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);

                if (altitudeInput < -1000)
                {
                    altitudeCoarseOutputValue = -10.0000;
                }
                else if (altitudeInput >= -1000 && altitudeInput < 0)
                {
                    altitudeCoarseOutputValue = -10.0000 + (altitudeInput - -1000.0000) / 1000.0000 * 0.2500;
                }
                else if (altitudeInput >= 0 && altitudeInput < 80000)
                {
                    altitudeCoarseOutputValue = -9.7500 + altitudeInput / 1000.0000 * (19.7500 / 80.0000);
                }
                else if (altitudeInput >= 80000)
                {
                    altitudeCoarseOutputValue = 10;
                }

                if (!fineHandled && _altitudeFineSinOutputSignal != null)
                {
                    if (altitudeFineSinOutputValue < -10)
                    {
                        altitudeFineSinOutputValue = -10;
                    }
                    else if (altitudeFineSinOutputValue > 10)
                    {
                        altitudeFineSinOutputValue = 10;
                    }

                    _altitudeFineSinOutputSignal.State = altitudeFineSinOutputValue;
                }

                if (!fineHandled && _altitudeFineCosOutputSignal != null)
                {
                    if (altitudeFineCosOutputValue < -10)
                    {
                        altitudeFineCosOutputValue = -10;
                    }
                    else if (altitudeFineCosOutputValue > 10)
                    {
                        altitudeFineCosOutputValue = 10;
                    }

                    _altitudeFineCosOutputSignal.State = altitudeFineCosOutputValue;
                }

                if (coarseHandled || _altitudeCoarseOutputSignal == null) return;
                if (altitudeCoarseOutputValue < -10)
                {
                    altitudeCoarseOutputValue = -10;
                }
                else if (altitudeCoarseOutputValue > 10)
                {
                    altitudeCoarseOutputValue = 10;
                }

                _altitudeCoarseOutputSignal.State = altitudeCoarseOutputValue;
            }
        }

        private static void UpdateBarometricPressureOutputValues()
        {
            //do nothing
        }
    }
}