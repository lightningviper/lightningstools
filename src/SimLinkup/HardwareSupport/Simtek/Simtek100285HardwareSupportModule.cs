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
    //Simtek 10-0285 F-16 Altimeter
    public class Simtek100285HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek100285HardwareSupportModule));
        private readonly IAltimeter _renderer = new Altimeter();

        private AnalogSignal _altitudeCoarseCosOutputSignal;
        private AnalogSignal _altitudeCoarseSinOutputSignal;
        private AnalogSignal _altitudeFineCosOutputSignal;
        private AnalogSignal _altitudeFineSinOutputSignal;
        private AnalogSignal _altitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _altitudeInputSignalChangedEventHandler;
        private AnalogSignal _barometricPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _barometricPressureInputSignalChangedEventHandler;
        private bool _isDisposed;

        // Editor-authored calibration. Two override surfaces:
        //   - fine   pair (multi_resolver, 4000 ft/rev):
        //         _fineTransform + _fineSinChannel + _fineCosChannel
        //   - coarse pair (multi_resolver, 100000 ft/rev):
        //         _coarseTransform + _coarseSinChannel + _coarseCosChannel
        // When BOTH pairs are populated the HSM bypasses the legacy baro
        // compensation entirely (BMS already publishes baro-compensated
        // altitude). When EITHER pair is missing the legacy baro math runs
        // and drives whichever pair lacks an override via the hardcoded
        // sin/cos × 10 V path.
        private Simtek100285HardwareSupportModuleConfig _config;
        private GaugeTransformConfig _fineTransform;
        private GaugeChannelConfig _fineSinChannel;
        private GaugeChannelConfig _fineCosChannel;
        private GaugeTransformConfig _coarseTransform;
        private GaugeChannelConfig _coarseSinChannel;
        private GaugeChannelConfig _coarseCosChannel;

        private ConfigFileReloadWatcher _configWatcher;

        // Legacy baro math fallback values. These are loaded from the bare-
        // property fields in the config file when the editor-authored
        // <Channels> block is empty/missing, OR when one of the two resolver
        // pairs lacks an override. Defaults match the values that existed
        // before the calibration system landed.
        private const double DEFAULT_MIN_BARO_PRESSURE = 28.09;
        private const double DEFAULT_MAX_BARO_PRESSURE = 31.025;
        private const double DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET = 2800;
        private double _altitudeZeroOffsetInFeet = 0;
        private double _minBaroPressure = DEFAULT_MIN_BARO_PRESSURE;
        private double _maxBaroPressure = DEFAULT_MAX_BARO_PRESSURE;
        private double _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet = DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET;

        public Simtek100285HardwareSupportModule(Simtek100285HardwareSupportModuleConfig config)
        {
            _config = config;
            ApplyLegacyBaroFields(config);
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Pull the four legacy baro doubles out of the config and store them
        // as fields. A null config (no file at all) leaves the defaults from
        // the initialisers above in place.
        private void ApplyLegacyBaroFields(Simtek100285HardwareSupportModuleConfig config)
        {
            if (config == null) return;
            if (config.MinBaroPressureInHg.HasValue) _minBaroPressure = config.MinBaroPressureInHg.Value;
            if (config.MaxBaroPressureInHg.HasValue) _maxBaroPressure = config.MaxBaroPressureInHg.Value;
            if (config.IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro.HasValue)
            {
                _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet =
                    config.IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro.Value;
            }
            if (config.AltitudeZeroOffsetInFeet.HasValue) _altitudeZeroOffsetInFeet = config.AltitudeZeroOffsetInFeet.Value;
        }

        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            ResolveMultiResolverPair(config,
                "100285_Altitude_Fine_SIN_To_Instrument",
                "100285_Altitude_Fine_COS_To_Instrument",
                out _fineTransform, out _fineSinChannel, out _fineCosChannel);
            ResolveMultiResolverPair(config,
                "100285_Altitude_Coarse_SIN_To_Instrument",
                "100285_Altitude_Coarse_COS_To_Instrument",
                out _coarseTransform, out _coarseSinChannel, out _coarseCosChannel);
        }

        // Multi-turn-resolver pair resolver: SIN side carries UnitsPerRevolution
        // + PeakVolts; COS side just points back via partnerChannel. Returns
        // null transform when the config doesn't carry a usable record.
        // Same shape as Simtek101081HardwareSupportModule.ResolveMultiResolverPair.
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
                var reloaded = Simtek100285HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ApplyLegacyBaroFields(reloaded);
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
        {
            _altitudeFineSinOutputSignal, _altitudeFineCosOutputSignal,
            _altitudeCoarseSinOutputSignal,
            _altitudeCoarseCosOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0285 - Simulated Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100285HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek100285HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek100285HardwareSupportModule.config");
                hsmConfig = Simtek100285HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek100285HardwareSupportModule(hsmConfig) };
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

        private void barometricPressure_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }

        private AnalogSignal CreateAltitudeCoarseCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse (COS)",
                Id = "100285_Altitude_Coarse_COS_To_Instrument",
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

        private AnalogSignal CreateAltitudeCoarseSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse (SIN)",
                Id = "100285_Altitude_Coarse_SIN_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (COS)",
                Id = "100285_Altitude_Fine_COS_To_Instrument",
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
                Id = "100285_Altitude_Fine_SIN_To_Instrument",
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
                Id = "100285_Altitude_From_Sim",
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
                Id = "100285_Barometric_Pressure_From_Sim",
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

            _barometricPressureInputSignalChangedEventHandler = barometricPressure_InputSignalChanged;
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
            _altitudeCoarseSinOutputSignal = CreateAltitudeCoarseSinOutputSignal();
            _altitudeCoarseCosOutputSignal = CreateAltitudeCoarseCosOutputSignal();
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
            if (_altitudeInputSignal == null) return;
            var altitudeInput = _altitudeInputSignal.State;

            // Decide which altitude value drives the resolvers. When BOTH
            // resolver pairs are configured by the editor, we trust BMS's
            // already-baro-compensated altitude (`aauz`) directly — no further
            // baro math, no zero offset. When either pair is missing an
            // override, the legacy baro math applies to keep behaviour
            // identical to pre-calibration-system installs for whichever
            // pair falls back to the hardcoded sin/cos × 10 V path.
            var hasFineOverride = _fineTransform != null
                                  && _fineSinChannel != null && _fineCosChannel != null
                                  && _altitudeFineSinOutputSignal != null
                                  && _altitudeFineCosOutputSignal != null;
            var hasCoarseOverride = _coarseTransform != null
                                    && _coarseSinChannel != null && _coarseCosChannel != null
                                    && _altitudeCoarseSinOutputSignal != null
                                    && _altitudeCoarseCosOutputSignal != null;

            double altitudeForResolvers;
            if (hasFineOverride && hasCoarseOverride)
            {
                altitudeForResolvers = altitudeInput;
            }
            else
            {
                var baroInput = _barometricPressureInputSignal.State;
                if (baroInput == 0.00f) baroInput = 29.92f;
                var baroDeltaFromStandard = baroInput - 29.92f;
                var altToAddForBaroComp = -(_differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet
                                            / (_maxBaroPressure - _minBaroPressure)) * baroDeltaFromStandard;
                altitudeForResolvers = altitudeInput + altToAddForBaroComp + _altitudeZeroOffsetInFeet;
            }

            // Fine pair.
            if (hasFineOverride)
            {
                var t = _fineTransform;
                var sinCos = GaugeTransform.EvaluateMultiTurnResolver(
                    altitudeInput, t.UnitsPerRevolution.Value, t.PeakVolts.Value);
                _altitudeFineSinOutputSignal.State = _fineSinChannel.ApplyTrim(sinCos[0], _altitudeFineSinOutputSignal.MinValue, _altitudeFineSinOutputSignal.MaxValue);
                _altitudeFineCosOutputSignal.State = _fineCosChannel.ApplyTrim(sinCos[1], _altitudeFineCosOutputSignal.MinValue, _altitudeFineCosOutputSignal.MaxValue);
            }
            else
            {
                var revolutions = altitudeForResolvers / 4000;
                var degrees = revolutions * 360;
                var sin = 10.0000 * Math.Sin(degrees * Constants.RADIANS_PER_DEGREE);
                var cos = 10.0000 * Math.Cos(degrees * Constants.RADIANS_PER_DEGREE);
                if (_altitudeFineSinOutputSignal != null) _altitudeFineSinOutputSignal.State = ClampPm10(sin);
                if (_altitudeFineCosOutputSignal != null) _altitudeFineCosOutputSignal.State = ClampPm10(cos);
            }

            // Coarse pair.
            if (hasCoarseOverride)
            {
                var t = _coarseTransform;
                var sinCos = GaugeTransform.EvaluateMultiTurnResolver(
                    altitudeInput, t.UnitsPerRevolution.Value, t.PeakVolts.Value);
                _altitudeCoarseSinOutputSignal.State = _coarseSinChannel.ApplyTrim(sinCos[0], _altitudeCoarseSinOutputSignal.MinValue, _altitudeCoarseSinOutputSignal.MaxValue);
                _altitudeCoarseCosOutputSignal.State = _coarseCosChannel.ApplyTrim(sinCos[1], _altitudeCoarseCosOutputSignal.MinValue, _altitudeCoarseCosOutputSignal.MaxValue);
            }
            else
            {
                var revolutions = altitudeForResolvers / 100000;
                var degrees = revolutions * 360;
                var sin = 10.0000 * Math.Sin(degrees * Constants.RADIANS_PER_DEGREE);
                var cos = 10.0000 * Math.Cos(degrees * Constants.RADIANS_PER_DEGREE);
                if (_altitudeCoarseSinOutputSignal != null) _altitudeCoarseSinOutputSignal.State = ClampPm10(sin);
                if (_altitudeCoarseCosOutputSignal != null) _altitudeCoarseCosOutputSignal.State = ClampPm10(cos);
            }
        }

        private static double ClampPm10(double v)
        {
            if (v < -10) return -10;
            if (v >  10) return  10;
            return v;
        }
    }
}
