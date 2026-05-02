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
    //Simtek 10-1082 F-16 Mach/Airspeed Indicator (v2)
    public class Simtek101082HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Simtek101082HardwareSupportModule));
        private readonly IAirspeedIndicator _renderer = new AirspeedIndicator();

        private AnalogSignal _airspeedInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _airspeedInputSignalChangedEventHandler;
        private AnalogSignal _airspeedOutputSignal;
        private bool _isDisposed;
        private AnalogSignal _machInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _machInputSignalChangedEventHandler;
        private AnalogSignal _machOutputSignal;

        // Editor-authored calibration. Two override surfaces:
        //   - airspeed (piecewise): _airspeedChannel
        //   - mach (piecewise reference voltage + cross-coupling math):
        //         _machChannel — the channel's piecewise table is the
        //         reference voltage lookup; the cross-coupling math against
        //         the airspeed output voltage stays hardcoded below.
        // Either may be null independently — Update*OutputValues falls
        // through to the hardcoded path for any unconfigured channel.
        private Simtek101082HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _airspeedChannel;
        private GaugeChannelConfig _machChannel;

        private ConfigFileReloadWatcher _configWatcher;

        public Simtek101082HardwareSupportModule(Simtek101082HardwareSupportModuleConfig config)
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
            _airspeedChannel = ResolvePiecewiseChannel(config, "101082_Airspeed_To_Instrument");
            _machChannel     = ResolvePiecewiseChannel(config, "101082_Mach_To_Instrument");
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
                var reloaded = Simtek101082HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateAirspeedOutputValues();
                UpdateMachOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[] {_machInputSignal, _airspeedInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_machOutputSignal, _airspeedOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1082 - Airspeed/Mach Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101082HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            Simtek101082HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "Simtek101082HardwareSupportModule.config");
                hsmConfig = Simtek101082HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new Simtek101082HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.MachNumber = (float) _machInputSignal.State;
            _renderer.InstrumentState.AirspeedKnots = (float) _airspeedInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _machInputSignalChangedEventHandler = null;
            _airspeedInputSignalChangedEventHandler = null;
        }

        private void airspeed_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAirspeedOutputValues();
            // Mach output depends on the airspeed output, so re-derive it
            // whenever airspeed changes too.
            UpdateMachOutputValues();
        }

        private AnalogSignal CreateAirspeedInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Airspeed",
                Id = "101082_Airspeed_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 850
            };
            return thisSignal;
        }

        private AnalogSignal CreateAirspeedOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Airspeed",
                Id = "101082_Airspeed_To_Instrument",
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

        private void CreateInputEventHandlers()
        {
            _machInputSignalChangedEventHandler =
                mach_InputSignalChanged;
            _airspeedInputSignalChangedEventHandler =
                airspeed_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _machInputSignal = CreateMachInputSignal();
            _airspeedInputSignal = CreateAirspeedInputSignal();
        }

        private AnalogSignal CreateMachInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Mach",
                Id = "101082_Mach_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 2.4
            };
            return thisSignal;
        }

        private AnalogSignal CreateMachOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Mach",
                Id = "101082_Mach_To_Instrument",
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

        private void CreateOutputSignals()
        {
            _machOutputSignal = CreateMachOutputSignal();
            _airspeedOutputSignal = CreateAirspeedOutputSignal();
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

        private void mach_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateMachOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_machInputSignal != null)
            {
                _machInputSignal.SignalChanged += _machInputSignalChangedEventHandler;
            }
            if (_airspeedInputSignal != null)
            {
                _airspeedInputSignal.SignalChanged += _airspeedInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_machInputSignalChangedEventHandler != null && _machInputSignal != null)
            {
                try
                {
                    _machInputSignal.SignalChanged -= _machInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_airspeedInputSignalChangedEventHandler != null && _airspeedInputSignal != null)
            {
                try
                {
                    _airspeedInputSignal.SignalChanged -= _airspeedInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateAirspeedOutputValues()
        {
            if (_airspeedInputSignal == null) return;
            if (_airspeedOutputSignal == null) return;
            var airspeedInput = _airspeedInputSignal.State;

            // Editor-authored override: when a piecewise config is set,
            // evaluate via the generic helper + per-channel trim.
            if (_airspeedChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(airspeedInput, _airspeedChannel.Transform.Breakpoints);
                _airspeedOutputSignal.State = _airspeedChannel.ApplyTrim(v, _airspeedOutputSignal.MinValue, _airspeedOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback (the spec table encoded as 43 segments).
            double airspeedOutputValue = 0;
            if (airspeedInput < 0)
            {
                airspeedOutputValue = -10;
            }
            else if (airspeedInput >= 0 && airspeedInput < 80)
            {
                airspeedOutputValue = -10 + airspeedInput / 80.0 * 1.18;
            }
            else if (airspeedInput >= 80 && airspeedInput < 90)
            {
                airspeedOutputValue = -8.82 + (airspeedInput - 80) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 90 && airspeedInput < 100)
            {
                airspeedOutputValue = -8.24 + (airspeedInput - 90) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 100 && airspeedInput < 110)
            {
                airspeedOutputValue = -7.65 + (airspeedInput - 100) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 110 && airspeedInput < 120)
            {
                airspeedOutputValue = -7.06 + (airspeedInput - 110) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 120 && airspeedInput < 130)
            {
                airspeedOutputValue = -6.47 + (airspeedInput - 120) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 130 && airspeedInput < 140)
            {
                airspeedOutputValue = -5.88 + (airspeedInput - 130) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 140 && airspeedInput < 150)
            {
                airspeedOutputValue = -5.29 + (airspeedInput - 140) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 150 && airspeedInput < 160)
            {
                airspeedOutputValue = -4.71 + (airspeedInput - 150) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 160 && airspeedInput < 170)
            {
                airspeedOutputValue = -4.12 + (airspeedInput - 160) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 170 && airspeedInput < 180)
            {
                airspeedOutputValue = -3.53 + (airspeedInput - 170) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 180 && airspeedInput < 190)
            {
                airspeedOutputValue = -2.94 + (airspeedInput - 180) / 10.0 * 0.59;
            }
            else if (airspeedInput >= 190 && airspeedInput < 200)
            {
                airspeedOutputValue = -2.35 + (airspeedInput - 190) / 10.0 * 0.58;
            }
            else if (airspeedInput >= 200 && airspeedInput < 210)
            {
                airspeedOutputValue = -1.77 + (airspeedInput - 200) / 10.0 * 0.3;
            }
            else if (airspeedInput >= 210 && airspeedInput < 220)
            {
                airspeedOutputValue = -1.47 + (airspeedInput - 210) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 220 && airspeedInput < 230)
            {
                airspeedOutputValue = -1.18 + (airspeedInput - 220) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 230 && airspeedInput < 240)
            {
                airspeedOutputValue = -0.88 + (airspeedInput - 230) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 240 && airspeedInput < 250)
            {
                airspeedOutputValue = -0.59 + (airspeedInput - 240) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 250 && airspeedInput < 260)
            {
                airspeedOutputValue = -0.29 + (airspeedInput - 250) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 260 && airspeedInput < 270)
            {
                airspeedOutputValue = 0 + (airspeedInput - 260) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 270 && airspeedInput < 280)
            {
                airspeedOutputValue = 0.29 + (airspeedInput - 270) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 280 && airspeedInput < 290)
            {
                airspeedOutputValue = 0.59 + (airspeedInput - 280) / 10.0 * 0.29;
            }
            else if (airspeedInput >= 290 && airspeedInput < 300)
            {
                airspeedOutputValue = 0.88 + (airspeedInput - 290) / 10.0 * 0.30;
            }
            else if (airspeedInput >= 300 && airspeedInput < 310)
            {
                airspeedOutputValue = 1.18 + (airspeedInput - 300) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 310 && airspeedInput < 320)
            {
                airspeedOutputValue = 1.41 + (airspeedInput - 310) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 320 && airspeedInput < 330)
            {
                airspeedOutputValue = 1.65 + (airspeedInput - 320) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 330 && airspeedInput < 340)
            {
                airspeedOutputValue = 1.88 + (airspeedInput - 330) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 340 && airspeedInput < 350)
            {
                airspeedOutputValue = 2.12 + (airspeedInput - 340) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 350 && airspeedInput < 360)
            {
                airspeedOutputValue = 2.35 + (airspeedInput - 350) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 360 && airspeedInput < 370)
            {
                airspeedOutputValue = 2.58 + (airspeedInput - 360) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 370 && airspeedInput < 380)
            {
                airspeedOutputValue = 2.82 + (airspeedInput - 370) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 380 && airspeedInput < 390)
            {
                airspeedOutputValue = 3.06 + (airspeedInput - 380) / 10.0 * 0.23;
            }
            else if (airspeedInput >= 390 && airspeedInput < 400)
            {
                airspeedOutputValue = 3.29 + (airspeedInput - 390) / 10.0 * 0.24;
            }
            else if (airspeedInput >= 400 && airspeedInput < 450)
            {
                airspeedOutputValue = 3.53 + (airspeedInput - 400) / 50.0 * 0.88;
            }
            else if (airspeedInput >= 450 && airspeedInput < 500)
            {
                airspeedOutputValue = 4.41 + (airspeedInput - 450) / 50.0 * 0.88;
            }
            else if (airspeedInput >= 500 && airspeedInput < 550)
            {
                airspeedOutputValue = 5.29 + (airspeedInput - 500) / 50.0 * 0.77;
            }
            else if (airspeedInput >= 550 && airspeedInput < 600)
            {
                airspeedOutputValue = 6.06 + (airspeedInput - 550) / 50.0 * 0.76;
            }
            else if (airspeedInput >= 600 && airspeedInput < 650)
            {
                airspeedOutputValue = 6.82 + (airspeedInput - 600) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 650 && airspeedInput < 700)
            {
                airspeedOutputValue = 7.53 + (airspeedInput - 650) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 700 && airspeedInput < 750)
            {
                airspeedOutputValue = 8.24 + (airspeedInput - 700) / 50.0 * 0.58;
            }
            else if (airspeedInput >= 750 && airspeedInput < 800)
            {
                airspeedOutputValue = 8.82 + (airspeedInput - 750) / 50.0 * 0.71;
            }
            else if (airspeedInput >= 800 && airspeedInput < 850)
            {
                airspeedOutputValue = 9.53 + (airspeedInput - 800) / 50.0 * 0.47;
            }
            else if (airspeedInput >= 850)
            {
                airspeedOutputValue = 10;
            }

            if (airspeedOutputValue < -10)
            {
                airspeedOutputValue = -10;
            }
            else if (airspeedOutputValue > 10)
            {
                airspeedOutputValue = 10;
            }

            _airspeedOutputSignal.State = airspeedOutputValue;
        }

        private void UpdateMachOutputValues()
        {
            if (_machInputSignal == null) return;
            if (_machOutputSignal == null) return;
            var machInput = _machInputSignal.State;

            // Step 1: derive the Mach reference voltage. Editor override
            // pulls it from the piecewise table; otherwise the hardcoded
            // 5-segment fallback runs.
            double machReferenceVoltage;
            if (_machChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(machInput, _machChannel.Transform.Breakpoints);
                machReferenceVoltage = _machChannel.ApplyTrim(v, _machOutputSignal.MinValue, _machOutputSignal.MaxValue);
            }
            else if (machInput <= 0)
            {
                machReferenceVoltage = -10;
            }
            else if (machInput > 0 && machInput <= 0.50)
            {
                machReferenceVoltage = -10.0 + ((machInput / 0.50) * 3.44);
            }
            else if (machInput > 0.50 && machInput <= 1.00)
            {
                machReferenceVoltage = ((machInput - 1) / 0.50) * 7.56;
            }
            else if (machInput > 1.00 && machInput <= 1.50)
            {
                machReferenceVoltage = 4.69 - ((1.50 - machInput) / 0.50) * 4.69;
            }
            else if (machInput > 1.50 && machInput <= 2.00)
            {
                machReferenceVoltage = 8.05 - (2 - machInput) / 0.50 * 3.36;
            }
            else if (machInput > 2.00 && machInput <= 2.50)
            {
                machReferenceVoltage = 10 - (2.50 - machInput) / 0.50 * 1.95;
            }
            else
            {
                machReferenceVoltage = 10;
            }

            // Step 2: cross-coupling math. Gauge geometry constants stay
            // hardcoded — they describe the instrument's mechanical layout
            // (Mach 1 sits at 131° on the dial; the angular range is 262°).
            // The Mach pointer is positioned RELATIVE to the airspeed
            // needle so Mach 1 aligns with the airspeed needle's current
            // angle minus the airspeed-at-260-knots reference (170°).
            var airspeedVoltage = _airspeedOutputSignal != null ? _airspeedOutputSignal.State : 0.0;
            var absoluteAirspeedNeedleAngle = (airspeedVoltage + 10.0) / (20.0000 / 340.0000);

            const int machOneReferenceAngle = 131;
            var machReferenceAngle = machReferenceVoltage / (20.0000 / 262.0000) + machOneReferenceAngle;
            var machAngleOffsetFromMach1RefAngle = machReferenceAngle - machOneReferenceAngle;

            var airspeedNeedleAngleDifferenceFrom260 = absoluteAirspeedNeedleAngle - 170.0000;
            var howFarToMoveMachWheel = airspeedNeedleAngleDifferenceFrom260 - machAngleOffsetFromMach1RefAngle;
            var machOutputVoltage = -howFarToMoveMachWheel * (20.0000 / 262.0000);

            if (machOutputVoltage < -10)
            {
                machOutputVoltage = -10;
            }
            else if (machOutputVoltage > 10)
            {
                machOutputVoltage = 10;
            }

            _machOutputSignal.State = machOutputVoltage;
        }
    }
}
