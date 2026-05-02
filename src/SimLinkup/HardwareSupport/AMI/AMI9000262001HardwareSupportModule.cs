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

namespace SimLinkup.HardwareSupport.AMI
{
    //AMI 90002620-01 F-16 Cabin Pressure Altimeter
    public class AMI9000262001HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AMI9000262001HardwareSupportModule));
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator();

        private AnalogSignal _cabinPressureAltitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressureAltitudeInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressureAltitudeOutputSignal;

        private bool _isDisposed;

        private AMI9000262001HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _cabinAltChannel;
        private ConfigFileReloadWatcher _configWatcher;

        public AMI9000262001HardwareSupportModule(AMI9000262001HardwareSupportModuleConfig config)
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
            _cabinAltChannel = ResolvePiecewiseChannel(config, "9000262001_Cabin_Pressure_Altitude_To_Instrument");
        }

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
                var reloaded = AMI9000262001HardwareSupportModuleConfig.Load(configFile);
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

        public override AnalogSignal[] AnalogInputs => new[] { _cabinPressureAltitudeInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[]
            { _cabinPressureAltitudeOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "AMI P/N 90002620-01 - Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AMI9000262001HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            AMI9000262001HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "AMI9000262001HardwareSupportModule.config");
                hsmConfig = AMI9000262001HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new AMI9000262001HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float)_cabinPressureAltitudeInputSignal.State;
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

        private AnalogSignal CreateCabinPressureAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "9000262001_Cabin_Pressure_Altitude_From_Sim",
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
                FriendlyName = "Cabin Pressure Altitude",
                Id = "9000262001_Cabin_Pressure_Altitude_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts;
                IsVoltage = true,
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
            _cabinPressureAltitudeOutputSignal = CreateCabinPressureAltitudeSinOutputSignal();
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
            if (_cabinPressureAltitudeOutputSignal == null) return;
            var cabinPressureAltitudeInput = _cabinPressureAltitudeInputSignal.State;

            // Editor override: piecewise channel.
            if (_cabinAltChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(
                    cabinPressureAltitudeInput,
                    _cabinAltChannel.Transform.Breakpoints);
                _cabinPressureAltitudeOutputSignal.State = _cabinAltChannel.ApplyTrim(
                    v,
                    _cabinPressureAltitudeOutputSignal.MinValue,
                    _cabinPressureAltitudeOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — 0..50000 ft → -10..+10 V linear.
            var degrees = cabinPressureAltitudeInput < 0.0000
                ? 0.0000
                : (cabinPressureAltitudeInput >= 0 && cabinPressureAltitudeInput <= 50000.0000
                    ? cabinPressureAltitudeInput / 50000.0000 * 300.0000
                    : 300.0);
            var cabinPressureAltitudeOutputValue = ((degrees / 300.00) * 20.00) - 10.00;
            if (cabinPressureAltitudeOutputValue < -10) cabinPressureAltitudeOutputValue = -10;
            else if (cabinPressureAltitudeOutputValue > 10) cabinPressureAltitudeOutputValue = 10;
            _cabinPressureAltitudeOutputSignal.State = cabinPressureAltitudeOutputValue;
        }
    }
}
