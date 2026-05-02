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
    //Simtek 10-5862 F-16 Nozzle Position Indicator (resolver pair: 0..100% → 0..225°)
    //
    // Per the spec sheet (PL20-5862, dwg 10-5862):
    //   Drive type: SINGLE DC SYNCHRO
    //   Calibration: Table 1 — 6 test points, 0..100% open → 0..225°
    //                  linear (45° per 20% increment).
    //   Signal: 10 VDC × sin/cos of reference angle.
    //
    // Modeled on Simtek101088HardwareSupportModule (the existing F-16
    // nozzle position indicator). Renderer reuse: borrows
    // INozzlePositionIndicator from 10-1088.
    public class Simtek105862HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek105862HardwareSupportModule));
        private readonly INozzlePositionIndicator _renderer = new NozzlePositionIndicator();

        private Simtek105862HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;

        private ConfigFileReloadWatcher _configWatcher;

        private bool _isDisposed;
        private AnalogSignal _nozzlePositionCOSOutputSignal;
        private AnalogSignal _nozzlePositionInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _nozzlePositionInputSignalChangedEventHandler;
        private AnalogSignal _nozzlePositionSINOutputSignal;

        public Simtek105862HardwareSupportModule(Simtek105862HardwareSupportModuleConfig config)
        {
            _config = config;
            ResolvePiecewiseResolverPair(config,
                "105862_Nozzle_Position_SIN_To_Instrument",
                "105862_Nozzle_Position_COS_To_Instrument",
                out _resolverTransform, out _sinChannel, out _cosChannel);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
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
                var reloaded = Simtek105862HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolvePiecewiseResolverPair(reloaded,
                    "105862_Nozzle_Position_SIN_To_Instrument",
                    "105862_Nozzle_Position_COS_To_Instrument",
                    out _resolverTransform, out _sinChannel, out _cosChannel);
                // Re-evaluate the outputs with the cached input value so the
                // user sees the new calibration immediately.
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

        public override string FriendlyName => "Simtek P/N 10-5862 - Nozzle Position Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek105862HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek105862HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek105862HardwareSupportModule.config");
                hsmConfig = Simtek105862HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek105862HardwareSupportModule(hsmConfig) };
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
            _nozzlePositionInputSignalChangedEventHandler = nozzlePosition_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _nozzlePositionInputSignal = CreateNozzlePositionInputSignal();
        }

        private AnalogSignal CreateNozzlePositionInputSignal()
        {
            return new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Nozzle Position (0-100%)",
                Id = "105862_Nozzle_Position_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };
        }

        private AnalogSignal CreateNozzlePositionSINOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (SIN)",
                Id = "105862_Nozzle_Position_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00,
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
        }

        private AnalogSignal CreateNozzlePositionCOSOutputSignal()
        {
            return new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Nozzle Position (COS)",
                Id = "105862_Nozzle_Position_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00,
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
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
            try { _nozzlePositionInputSignal.SignalChanged -= _nozzlePositionInputSignalChangedEventHandler; }
            catch (RemotingException) { }
        }

        private void UpdateOutputValues()
        {
            if (_nozzlePositionInputSignal == null) return;
            var nozzlePositionInput = _nozzlePositionInputSignal.State;

            // Editor-authored override.
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

            // Hardcoded fallback: 0..100% → 0..225° linear.
            var clamped = nozzlePositionInput < 0 ? 0 : (nozzlePositionInput > 100 ? 100 : nozzlePositionInput);
            var refAngle = clamped / 100.0 * 225.0;
            var sin = 10.0 * Math.Sin(refAngle * Constants.RADIANS_PER_DEGREE);
            var cos = 10.0 * Math.Cos(refAngle * Constants.RADIANS_PER_DEGREE);
            if (_nozzlePositionSINOutputSignal != null) _nozzlePositionSINOutputSignal.State = Clamp(sin);
            if (_nozzlePositionCOSOutputSignal != null) _nozzlePositionCOSOutputSignal.State = Clamp(cos);
        }

        private static double Clamp(double v)
        {
            if (v < -10) return -10;
            if (v > 10) return 10;
            return v;
        }
    }
}
