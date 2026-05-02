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
    //Simtek 10-1088 F-16 NOZZLE POSITION IND (resolver pair: 0..100% → 0..225°)
    public class Simtek101088HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101088HardwareSupportModule));
        private readonly INozzlePositionIndicator _renderer = new NozzlePositionIndicator();

        // Editor-authored calibration. Same hot-reload contract as the
        // piecewise/linear gauges. Resolver-specific: we resolve a
        // (transform, sinTrim, cosTrim) triple — the transform parameters
        // come from the SIN channel record (where the editor writes them);
        // sin and cos trim are per-channel.
        private Simtek101088HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _nozzlePositionCOSOutputSignal;
        private AnalogSignal _nozzlePositionInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _nozzlePositionInputSignalChangedEventHandler;
        private AnalogSignal _nozzlePositionSINOutputSignal;

        public Simtek101088HardwareSupportModule(Simtek101088HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolvePiecewiseResolverPair(config,
                "101088_Nozzle_Position_SIN_To_Instrument",
                "101088_Nozzle_Position_COS_To_Instrument",
                out _resolverTransform, out _sinChannel, out _cosChannel);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Piecewise-resolver-pair resolver: pulls the SIN channel (which
        // carries the shared transform body — breakpoint table + PeakVolts)
        // and the COS channel (which just carries its own per-channel
        // trim). Returns null transform when the config doesn't carry a
        // usable record; HSM falls back to the hardcoded path in that case.
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
                var reloaded = Simtek101088HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolvePiecewiseResolverPair(reloaded,
                    "101088_Nozzle_Position_SIN_To_Instrument",
                    "101088_Nozzle_Position_COS_To_Instrument",
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

        public override AnalogSignal[] AnalogInputs => new[] {_nozzlePositionInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_nozzlePositionSINOutputSignal, _nozzlePositionCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1088 - Nozzle Position Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101088HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101088HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101088HardwareSupportModule.config");
                hsmConfig = Simtek101088HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101088HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.NozzlePositionPercent = (float) _nozzlePositionInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _nozzlePositionInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _nozzlePositionInputSignalChangedEventHandler =
                nozzlePosition_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _nozzlePositionInputSignal = CreateNozzlePositionInputSignal();
        }

        private AnalogSignal CreateNozzlePositionCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (COS)",
                Id = "101088_Nozzle_Position_COS_To_Instrument",
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

        private AnalogSignal CreateNozzlePositionInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Nozzle Position (0-100%)",
                Id = "101088_Nozzle_Position_From_Sim",
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

        private AnalogSignal CreateNozzlePositionSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (SIN)",
                Id = "101088_Nozzle_Position_SIN_To_Instrument",
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
            _nozzlePositionSINOutputSignal = CreateNozzlePositionSINOutputSignal();
            _nozzlePositionCOSOutputSignal = CreateNozzlePositionCOSOutputSignal();
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

        private void nozzlePosition_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_nozzlePositionInputSignal != null)
            {
                _nozzlePositionInputSignal.SignalChanged += _nozzlePositionInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_nozzlePositionInputSignalChangedEventHandler == null || _nozzlePositionInputSignal == null) return;
            try
            {
                _nozzlePositionInputSignal.SignalChanged -= _nozzlePositionInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_nozzlePositionInputSignal == null) return;
            var nozzlePositionInput = _nozzlePositionInputSignal.State;

            // Editor-authored override: when a .config file declared a
            // resolver transform for this gauge, evaluate via the generic
            // helper and per-channel trim (sin and cos calibrate
            // independently). Falls through to the hardcoded sin/cos blocks
            // below when no config is present.
            if (_resolverTransform != null
                && _sinChannel != null
                && _cosChannel != null
                && _nozzlePositionSINOutputSignal != null
                && _nozzlePositionCOSOutputSignal != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    nozzlePositionInput, t.Breakpoints, t.PeakVolts.Value);
                _nozzlePositionSINOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _nozzlePositionSINOutputSignal.MinValue, _nozzlePositionSINOutputSignal.MaxValue);
                _nozzlePositionCOSOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _nozzlePositionCOSOutputSignal.MinValue, _nozzlePositionCOSOutputSignal.MaxValue);
                return;
            }

            if (_nozzlePositionSINOutputSignal != null)
            {
                var nozzlePositionSINOutputValue = nozzlePositionInput < 0
                    ? 0
                    : (nozzlePositionInput > 100
                        ? 10.0000 * Math.Sin(225.0000 * Constants.RADIANS_PER_DEGREE)
                        : 10.0000 *
                          Math.Sin(nozzlePositionInput / 100.0000 * 225.0000 *
                                   Constants.RADIANS_PER_DEGREE));

                if (nozzlePositionSINOutputValue < -10)
                {
                    nozzlePositionSINOutputValue = -10;
                }
                else if (nozzlePositionSINOutputValue > 10)
                {
                    nozzlePositionSINOutputValue = 10;
                }

                _nozzlePositionSINOutputSignal.State = nozzlePositionSINOutputValue;
            }

            if (_nozzlePositionCOSOutputSignal == null) return;
            var nozzlePositionCOSOutputValue = nozzlePositionInput < 0
                ? 0
                : (nozzlePositionInput > 100
                    ? 10.0000 * Math.Cos(225.0000 * Constants.RADIANS_PER_DEGREE)
                    : 10.0000 *
                      Math.Cos(nozzlePositionInput / 100.0000 * 225.0000 *
                               Constants.RADIANS_PER_DEGREE));

            if (nozzlePositionCOSOutputValue < -10)
            {
                nozzlePositionCOSOutputValue = -10;
            }
            else if (nozzlePositionCOSOutputValue > 10)
            {
                nozzlePositionCOSOutputValue = 10;
            }

            _nozzlePositionCOSOutputSignal.State = nozzlePositionCOSOutputValue;
        }
    }
}