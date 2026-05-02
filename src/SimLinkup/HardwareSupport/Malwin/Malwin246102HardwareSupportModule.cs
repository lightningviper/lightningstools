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
    //Malwin 246102 F-16 Cabin Pressure Altimeter
    public class Malwin246102HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Malwin246102HardwareSupportModule));
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator();

        private AnalogSignal _cabinPressureAltitudeCosOutputSignal;
        private AnalogSignal _cabinPressureAltitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressureAltitudeInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressureAltitudeSinOutputSignal;

        private bool _isDisposed;

        private Malwin246102HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _resolverTransform;
        private GaugeChannelConfig _sinChannel;
        private GaugeChannelConfig _cosChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public Malwin246102HardwareSupportModule(Malwin246102HardwareSupportModuleConfig config)
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
                "246102_Cabin_Pressure_Altitude_SIN_To_Instrument",
                "246102_Cabin_Pressure_Altitude_COS_To_Instrument",
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
                var reloaded = Malwin246102HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateCabinPressureAltitudeOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_cabinPressureAltitudeInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_cabinPressureAltitudeSinOutputSignal, _cabinPressureAltitudeCosOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 246102 - Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin246102HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Malwin246102HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Malwin246102HardwareSupportModule.config");
                hsmConfig = Malwin246102HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Malwin246102HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float) _cabinPressureAltitudeInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _cabinPressureAltitudeInputSignalChangedEventHandler = null;
        }

        private void cabinPressureAltitude_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateCabinPressureAltitudeOutputValues();
        }

        private AnalogSignal CreateCabinPressureAltitudeCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Cabin Pressure Altitude (COS)",
                Id = "246102_Cabin_Pressure_Altitude_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCabinPressureAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "246102_Cabin_Pressure_Altitude_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 50000
            };
            return thisSignal;
        }

        private AnalogSignal CreateCabinPressureAltitudeSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Cabin Pressure Altitude (SIN)",
                Id = "246102_Cabin_Pressure_Altitude_SIN_To_Instrument",
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

        private void CreateInputEventHandlers()
        {
            _cabinPressureAltitudeInputSignalChangedEventHandler =
                cabinPressureAltitude_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _cabinPressureAltitudeInputSignal = CreateCabinPressureAltitudeInputSignal();
        }

        private void CreateOutputSignals()
        {
            _cabinPressureAltitudeSinOutputSignal = CreateCabinPressureAltitudeSinOutputSignal();
            _cabinPressureAltitudeCosOutputSignal = CreateCabinPressureAltitudeCosOutputSignal();
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
            if (_cabinPressureAltitudeInputSignal != null)
            {
                _cabinPressureAltitudeInputSignal.SignalChanged += _cabinPressureAltitudeInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_cabinPressureAltitudeInputSignalChangedEventHandler != null &&
                _cabinPressureAltitudeInputSignal != null)
            {
                try
                {
                    _cabinPressureAltitudeInputSignal.SignalChanged -=
                        _cabinPressureAltitudeInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateCabinPressureAltitudeOutputValues()
        {
            if (_cabinPressureAltitudeInputSignal == null) return;
            if (_cabinPressureAltitudeSinOutputSignal == null || _cabinPressureAltitudeCosOutputSignal == null) return;
            var cabinPressureAltitudeInput = _cabinPressureAltitudeInputSignal.State;

            // Editor override: multi_resolver pair.
            if (_resolverTransform != null && _sinChannel != null && _cosChannel != null)
            {
                var t = _resolverTransform;
                var sinCos = GaugeTransform.EvaluateMultiTurnResolver(
                    cabinPressureAltitudeInput, t.UnitsPerRevolution.Value, t.PeakVolts.Value);
                _cabinPressureAltitudeSinOutputSignal.State = _sinChannel.ApplyTrim(sinCos[0], _cabinPressureAltitudeSinOutputSignal.MinValue, _cabinPressureAltitudeSinOutputSignal.MaxValue);
                _cabinPressureAltitudeCosOutputSignal.State = _cosChannel.ApplyTrim(sinCos[1], _cabinPressureAltitudeCosOutputSignal.MinValue, _cabinPressureAltitudeCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — `(input/50000) × 300°` then sin/cos × 10 V.
            var degrees = cabinPressureAltitudeInput < 0.0000
                ? 0.0000
                : (cabinPressureAltitudeInput >= 0 && cabinPressureAltitudeInput <= 50000.0000
                    ? cabinPressureAltitudeInput / 50000.0000 * 300.0000
                    : 300.0);

            var cabinPressureAltitudeSinOutputValue = 10.0000 * Math.Sin(degrees * Constants.RADIANS_PER_DEGREE);
            if (cabinPressureAltitudeSinOutputValue < -10) cabinPressureAltitudeSinOutputValue = -10;
            else if (cabinPressureAltitudeSinOutputValue > 10) cabinPressureAltitudeSinOutputValue = 10;
            _cabinPressureAltitudeSinOutputSignal.State = cabinPressureAltitudeSinOutputValue;

            var cabinPressureAltitudeCosOutputValue = 10.0000 * Math.Cos(degrees * Constants.RADIANS_PER_DEGREE);
            if (cabinPressureAltitudeCosOutputValue < -10) cabinPressureAltitudeCosOutputValue = -10;
            else if (cabinPressureAltitudeCosOutputValue > 10) cabinPressureAltitudeCosOutputValue = 10;
            _cabinPressureAltitudeCosOutputSignal.State = cabinPressureAltitudeCosOutputValue;
        }
    }
}
