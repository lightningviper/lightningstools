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

namespace SimLinkup.HardwareSupport.AMI
{
    //AMI 9002780-02 Primary ADI
    public class AMI900278002HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AMI900278002HardwareSupportModule));
        private const float GLIDESLOPE_DEVIATION_LIMIT_DEGREES = 1.0F;
        private const float LOCALIZER_DEVIATION_LIMIT_DEGREES = 5.0F;

        private readonly IADI _renderer = new ADI();
        private DigitalSignal _auxFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _auxFlagInputSignalChangedEventHandler;
        private DigitalSignal _auxFlagOutputSignal;
        private DigitalSignal _gsFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _gsFlagInputSignalChangedEventHandler;
        private DigitalSignal _gsFlagOutputSignal;
        private AnalogSignal _horizontalCommandBarInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _horizontalCommandBarInputSignalChangedEventHandler;
        private AnalogSignal _horizontalCommandBarOutputSignal;
        private bool _isDisposed;
        private DigitalSignal _locFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _locFlagInputSignalChangedEventHandler;
        private DigitalSignal _locFlagOutputSignal;
        private DigitalSignal _offFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _offFlagInputSignalChangedEventHandler;
        private DigitalSignal _offFlagOutputSignal;
        private AnalogSignal _pitchInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _pitchInputSignalChangedEventHandler;
        private AnalogSignal _pitchOutputSignal;
        private AnalogSignal _rateOfTurnInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rateOfTurnInputSignalChangedEventHandler;
        private AnalogSignal _rateOfTurnOutputSignal;
        private AnalogSignal _rollCosOutputSignal;
        private AnalogSignal _rollInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rollInputSignalChangedEventHandler;
        private AnalogSignal _rollSinOutputSignal;
        private DigitalSignal _showCommandBarsInputSignal;
        private AnalogSignal _verticalCommandBarInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _verticalCommandBarInputSignalChangedEventHandler;
        private AnalogSignal _verticalCommandBarOutputSignal;

        private AMI900278002HardwareSupportModuleConfig _config;
        private GaugeChannelConfig _pitchChannel;
        private GaugeTransformConfig _rollTransform;
        private GaugeChannelConfig _rollSinChannel;
        private GaugeChannelConfig _rollCosChannel;
        private GaugeChannelConfig _offFlagChannel;
        private GaugeChannelConfig _gsFlagChannel;
        private GaugeChannelConfig _locFlagChannel;
        private GaugeChannelConfig _auxFlagChannel;
        private GaugeChannelConfig _horizontalCommandBarChannel;
        private GaugeChannelConfig _verticalCommandBarChannel;
        private GaugeChannelConfig _rateOfTurnChannel;

        private ConfigFileReloadWatcher _configWatcher;

        public AMI900278002HardwareSupportModule(AMI900278002HardwareSupportModuleConfig config)
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
            _pitchChannel = ResolvePiecewiseChannel(config, "900278002_Pitch_To_Instrument");
            ResolvePiecewiseResolverPair(config,
                "900278002_Roll_SIN_To_Instrument",
                "900278002_Roll_COS_To_Instrument",
                out _rollTransform, out _rollSinChannel, out _rollCosChannel);
            _offFlagChannel = ResolveDigitalInvertChannel(config, "900278002_OFF_Flag_To_Instrument");
            _gsFlagChannel  = ResolveDigitalInvertChannel(config, "900278002_GS_Flag_To_Instrument");
            _locFlagChannel = ResolveDigitalInvertChannel(config, "900278002_LOC_Flag_To_Instrument");
            _auxFlagChannel = ResolveDigitalInvertChannel(config, "900278002_AUX_Flag_To_Instrument");
            _horizontalCommandBarChannel = ResolvePiecewiseChannel(config, "900278002_Horizontal_Command_Bar_To_Instrument");
            _verticalCommandBarChannel   = ResolvePiecewiseChannel(config, "900278002_Vertical_Command_Bar_To_Instrument");
            _rateOfTurnChannel = ResolvePiecewiseChannel(config, "900278002_Rate_Of_Turn_To_Instrument");
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

        private static GaugeChannelConfig ResolveDigitalInvertChannel(GaugeCalibrationConfig config, string channelId)
        {
            if (config == null) return null;
            var ch = config.FindChannel(channelId);
            if (ch != null
                && ch.Transform != null
                && ch.Transform.Kind == "digital_invert"
                && ch.Invert.HasValue)
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
                var reloaded = AMI900278002HardwareSupportModuleConfig.Load(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                // Re-evaluate every output with the cached input values so the
                // user sees the new calibration immediately. Without this,
                // SimLinkup's event-driven update loop won't fire until the
                // simulator next pushes a new input value.
                UpdateAuxFlagOutputValue();
                UpdateGSFlagOutputValue();
                UpdateLOCFlagOutputValue();
                UpdateOFFFlagOutputValue();
                UpdateHorizontalCommandBarOutputValues();
                UpdatePitchOutputValues();
                UpdateRateOfTurnOutputValues();
                UpdateRollOutputValues();
                UpdateVerticalCommandBarOutputValues();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override AnalogSignal[] AnalogInputs => new[]
        {
            _pitchInputSignal, _rollInputSignal, _horizontalCommandBarInputSignal, _verticalCommandBarInputSignal,
            _rateOfTurnInputSignal
        };

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _pitchOutputSignal, _rollSinOutputSignal, _rollCosOutputSignal, _horizontalCommandBarOutputSignal,
            _verticalCommandBarOutputSignal, _rateOfTurnOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => new[]
        {
            _showCommandBarsInputSignal, _auxFlagInputSignal, _gsFlagInputSignal, _locFlagInputSignal,
            _offFlagInputSignal
        };

        public override DigitalSignal[] DigitalOutputs => new[]
            {_auxFlagOutputSignal, _gsFlagOutputSignal, _locFlagOutputSignal, _offFlagOutputSignal};

        public override string FriendlyName => "AMI P/N 9002780-02 - Indicator - Simulated Attitude Director Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AMI900278002HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            AMI900278002HardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "AMI900278002HardwareSupportModule.config");
                hsmConfig = AMI900278002HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new AMI900278002HardwareSupportModule(hsmConfig) };
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.AuxFlag = _auxFlagInputSignal.State;
            _renderer.InstrumentState.GlideslopeDeviationDegrees = (float) _horizontalCommandBarInputSignal.State;
            _renderer.InstrumentState.GlideslopeDeviationLimitDegrees = GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
            _renderer.InstrumentState.GlideslopeFlag = _gsFlagInputSignal.State;
            _renderer.InstrumentState.LocalizerDeviationDegrees = (float) _verticalCommandBarInputSignal.State;
            _renderer.InstrumentState.LocalizerDeviationLimitDegrees = LOCALIZER_DEVIATION_LIMIT_DEGREES;
            _renderer.InstrumentState.LocalizerFlag = _locFlagInputSignal.State;
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.PitchDegrees = (float) _pitchInputSignal.State;
            _renderer.InstrumentState.RollDegrees = (float) _rollInputSignal.State;
            _renderer.InstrumentState.ShowCommandBars = _showCommandBarsInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = null;
            _rollInputSignalChangedEventHandler = null;
            _horizontalCommandBarInputSignalChangedEventHandler = null;
            _verticalCommandBarInputSignalChangedEventHandler = null;
            _rateOfTurnInputSignalChangedEventHandler = null;
            _auxFlagInputSignalChangedEventHandler = null;
            _gsFlagInputSignalChangedEventHandler = null;
            _locFlagInputSignalChangedEventHandler = null;
            _offFlagInputSignalChangedEventHandler = null;
        }

        private void auxFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateAuxFlagOutputValue();
        }

        private DigitalSignal CreateAuxFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "AUX Flag Visible (0=Hidden; 1=Visible)",
                Id = "900278002_AUX_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateAuxFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "AUX Flag Hidden (0=Visible; 1=Hidden)",
                Id = "900278002_AUX_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }

        private DigitalSignal CreateGSFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "GS Flag Visible (0=Hidden; 1=Visible)",
                Id = "900278002_GS_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateGSFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "GS Flag Hidden (0=Visible; 1=Hidden)",
                Id = "900278002_GS_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }

        private AnalogSignal CreateHorizontalCommandBarInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Horizontal Command Bar (Degrees)",
                Id = "900278002_Horizontal_Command_Bar_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateHorizontalCommandBarOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Horizontal Command Bar",
                Id = "900278002_Horizontal_Command_Bar_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = pitch_InputSignalChanged;
            _rollInputSignalChangedEventHandler = roll_InputSignalChanged;
            _horizontalCommandBarInputSignalChangedEventHandler = horizontalCommandBar_InputSignalChanged;
            _verticalCommandBarInputSignalChangedEventHandler = verticalCommandBar_InputSignalChanged;
            _rateOfTurnInputSignalChangedEventHandler = rateOfTurn_InputSignalChanged;
            _auxFlagInputSignalChangedEventHandler = auxFlag_InputSignalChanged;
            _gsFlagInputSignalChangedEventHandler = gsFlag_InputSignalChanged;
            _locFlagInputSignalChangedEventHandler = locFlag_InputSignalChanged;
            _offFlagInputSignalChangedEventHandler = offFlag_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _pitchInputSignal = CreatePitchInputSignal();
            _rollInputSignal = CreateRollInputSignal();
            _horizontalCommandBarInputSignal = CreateHorizontalCommandBarInputSignal();
            _verticalCommandBarInputSignal = CreateVerticalCommandBarInputSignal();
            _rateOfTurnInputSignal = CreateRateOfTurnInputSignal();
            _showCommandBarsInputSignal = CreateShowCommandBarsInputSignal();
            _auxFlagInputSignal = CreateAuxFlagInputSignal();
            _gsFlagInputSignal = CreateGSFlagInputSignal();
            _locFlagInputSignal = CreateLOCFlagInputSignal();
            _offFlagInputSignal = CreateOffFlagInputSignal();
        }

        private DigitalSignal CreateLOCFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "LOC Flag Visible (0=Hidden; 1=Visible)",
                Id = "900278002_LOC_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateLOCFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "LOC Flag Hidden(0=Visible; 1=Hidden)",
                Id = "900278002_LOC_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }


        private DigitalSignal CreateOffFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag Visible (0=Hidden; 1=Visible)",
                Id = "900278002_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateOFFFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "OFF Flag Hidden (0=Visible; 1=Hidden)",
                Id = "900278002_OFF_Flag_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _pitchOutputSignal = CreatePitchOutputSignal();
            _rollSinOutputSignal = CreateRollSinOutputSignal();
            _rollCosOutputSignal = CreateRollCosOutputSignal();
            _horizontalCommandBarOutputSignal = CreateHorizontalCommandBarOutputSignal();
            _verticalCommandBarOutputSignal = CreateVerticalCommandBarOutputSignal();
            _rateOfTurnOutputSignal = CreateRateOfTurnOutputSignal();
            _auxFlagOutputSignal = CreateAuxFlagOutputSignal();
            _gsFlagOutputSignal = CreateGSFlagOutputSignal();
            _locFlagOutputSignal = CreateLOCFlagOutputSignal();
            _offFlagOutputSignal = CreateOFFFlagOutputSignal();
        }

        private AnalogSignal CreatePitchInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Pitch (Degrees)",
                Id = "900278002_Pitch_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -90,
                MaxValue = 90
            };
            return thisSignal;
        }

        private AnalogSignal CreatePitchOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch",
                Id = "900278002_Pitch_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };

            return thisSignal;
        }

        private AnalogSignal CreateRateOfTurnInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Rate of Turn Indicator (% Deflection)",
                Id = "900278002_Rate_Of_Turn_Indicator_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = -1,
                MaxValue = 1
            };
            return thisSignal;
        }

        private AnalogSignal CreateRateOfTurnOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Rate of Turn Indicator",
                Id = "900278002_Rate_Of_Turn_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (COS)",
                Id = "900278002_Roll_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                //            thisSignal.State = +10.00; //volts
                State =
                    Math.Cos(-15.0 * Constants.RADIANS_PER_DEGREE) *
                    10.00, //volts; //HACK - for Dave R. alt that needs to be cal'd at the hardware level still
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Roll (Degrees)",
                Id = "900278002_Roll_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = -180,
                MaxValue = 180
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (SIN)",
                Id = "900278002_Roll_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                //thisSignal.State = 0.00; //volts;
                State =
                    Math.Sin(-15.0 * Constants.RADIANS_PER_DEGREE) *
                    10.00, //volts; //HACK - for Dave R. alt that needs to be cal'd at the hardware level still
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private DigitalSignal CreateShowCommandBarsInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Command Bars Visible Flag (0=Hidden; 1=Visible)",
                Id = "900278002_Show_Command_Bars_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private AnalogSignal CreateVerticalCommandBarInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Vertical Command Bar (Degrees)",
                Id = "900278002_Vertical_Command_Bar_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateVerticalCommandBarOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Vertical Command Bar",
                Id = "900278002_Vertical_Command_Bar_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
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

        private void gsFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateGSFlagOutputValue();
        }

        private void horizontalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateHorizontalCommandBarOutputValues();
        }

        private void locFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateLOCFlagOutputValue();
        }

        private void offFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOFFFlagOutputValue();
        }

        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void rateOfTurn_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRateOfTurnOutputValues();
        }

        private void RegisterForInputEvents()
        {
            // NOTE: original C# never registered _showCommandBarsInputSignal —
            // changes to that flag don't trigger a re-render. Preserved to
            // avoid behavior drift; users who need bars to hide/show
            // dynamically can update one of the bar inputs to force an
            // update.
            if (_auxFlagInputSignal != null) { _auxFlagInputSignal.SignalChanged += _auxFlagInputSignalChangedEventHandler; }
            if (_gsFlagInputSignal != null) { _gsFlagInputSignal.SignalChanged += _gsFlagInputSignalChangedEventHandler; }
            if (_locFlagInputSignal != null) { _locFlagInputSignal.SignalChanged += _locFlagInputSignalChangedEventHandler; }
            if (_offFlagInputSignal != null) { _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler; }
            if (_pitchInputSignal != null) { _pitchInputSignal.SignalChanged += _pitchInputSignalChangedEventHandler; }
            if (_rollInputSignal != null) { _rollInputSignal.SignalChanged += _rollInputSignalChangedEventHandler; }
            if (_horizontalCommandBarInputSignal != null) { _horizontalCommandBarInputSignal.SignalChanged += _horizontalCommandBarInputSignalChangedEventHandler; }
            if (_verticalCommandBarInputSignal != null) { _verticalCommandBarInputSignal.SignalChanged += _verticalCommandBarInputSignalChangedEventHandler; }
            if (_rateOfTurnInputSignal != null) { _rateOfTurnInputSignal.SignalChanged += _rateOfTurnInputSignalChangedEventHandler; }
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
        }

        private void verticalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateVerticalCommandBarOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            try { if (_auxFlagInputSignal != null) _auxFlagInputSignal.SignalChanged -= _auxFlagInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_gsFlagInputSignal != null) _gsFlagInputSignal.SignalChanged -= _gsFlagInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_locFlagInputSignal != null) _locFlagInputSignal.SignalChanged -= _locFlagInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_offFlagInputSignal != null) _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_pitchInputSignal != null) _pitchInputSignal.SignalChanged -= _pitchInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_rollInputSignal != null) _rollInputSignal.SignalChanged -= _rollInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_horizontalCommandBarInputSignal != null) _horizontalCommandBarInputSignal.SignalChanged -= _horizontalCommandBarInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_verticalCommandBarInputSignal != null) _verticalCommandBarInputSignal.SignalChanged -= _verticalCommandBarInputSignalChangedEventHandler; } catch (RemotingException) { }
            try { if (_rateOfTurnInputSignal != null) _rateOfTurnInputSignal.SignalChanged -= _rateOfTurnInputSignalChangedEventHandler; } catch (RemotingException) { }
        }

        private static bool ApplyDigitalInvert(GaugeChannelConfig ch, bool input)
        {
            return ch.Invert.Value ? !input : input;
        }

        private void UpdateAuxFlagOutputValue()
        {
            if (_auxFlagOutputSignal == null) return;
            if (_auxFlagChannel != null) { _auxFlagOutputSignal.State = ApplyDigitalInvert(_auxFlagChannel, _auxFlagInputSignal.State); return; }
            _auxFlagOutputSignal.State = !_auxFlagInputSignal.State;
        }

        private void UpdateGSFlagOutputValue()
        {
            if (_gsFlagOutputSignal == null) return;
            if (_gsFlagChannel != null) { _gsFlagOutputSignal.State = ApplyDigitalInvert(_gsFlagChannel, _gsFlagInputSignal.State); return; }
            _gsFlagOutputSignal.State = !_gsFlagInputSignal.State;
        }

        private void UpdateLOCFlagOutputValue()
        {
            if (_locFlagOutputSignal == null) return;
            if (_locFlagChannel != null) { _locFlagOutputSignal.State = ApplyDigitalInvert(_locFlagChannel, _locFlagInputSignal.State); return; }
            _locFlagOutputSignal.State = !_locFlagInputSignal.State;
        }

        private void UpdateOFFFlagOutputValue()
        {
            if (_offFlagOutputSignal == null) return;
            if (_offFlagChannel != null) { _offFlagOutputSignal.State = ApplyDigitalInvert(_offFlagChannel, _offFlagInputSignal.State); return; }
            _offFlagOutputSignal.State = !_offFlagInputSignal.State;
        }

        private void UpdateHorizontalCommandBarOutputValues()
        {
            if (_horizontalCommandBarInputSignal == null) return;
            if (_horizontalCommandBarOutputSignal == null) return;

            // Show/hide gating preserved verbatim.
            if (!_showCommandBarsInputSignal.State)
            {
                _horizontalCommandBarOutputSignal.State = 10;
                return;
            }

            var percentDeflection = _horizontalCommandBarInputSignal.State;

            if (_horizontalCommandBarChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(percentDeflection, _horizontalCommandBarChannel.Transform.Breakpoints);
                _horizontalCommandBarOutputSignal.State = _horizontalCommandBarChannel.ApplyTrim(v, _horizontalCommandBarOutputSignal.MinValue, _horizontalCommandBarOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — input × 4.
            var outputValue = 4 * percentDeflection;
            if (outputValue < -10) outputValue = -10;
            else if (outputValue > 10) outputValue = 10;
            _horizontalCommandBarOutputSignal.State = outputValue;
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal == null) return;
            if (_pitchOutputSignal == null) return;
            var pitchInputDegrees = _pitchInputSignal.State;

            if (_pitchChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(pitchInputDegrees, _pitchChannel.Transform.Breakpoints);
                _pitchOutputSignal.State = _pitchChannel.ApplyTrim(v, _pitchOutputSignal.MinValue, _pitchOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — linear ±90° → ±10 V.
            var pitchOutputValue = 10.0000 * (pitchInputDegrees / 90.0);
            if (pitchOutputValue < -10) pitchOutputValue = -10;
            else if (pitchOutputValue > 10) pitchOutputValue = 10;
            _pitchOutputSignal.State = pitchOutputValue;
        }

        private void UpdateRateOfTurnOutputValues()
        {
            if (_rateOfTurnInputSignal == null) return;
            if (_rateOfTurnOutputSignal == null) return;
            var percentDeflection = _rateOfTurnInputSignal.State;

            if (_rateOfTurnChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(percentDeflection, _rateOfTurnChannel.Transform.Breakpoints);
                _rateOfTurnOutputSignal.State = _rateOfTurnChannel.ApplyTrim(v, _rateOfTurnOutputSignal.MinValue, _rateOfTurnOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — preserves original `else if (out > 10) out = 0`
            // behavior verbatim.
            var outputValue = 10.0000 * percentDeflection;
            if (outputValue < -10)
            {
                outputValue = -10;
            }
            else if (outputValue > 10)
            {
                outputValue = 0;
            }
            _rateOfTurnOutputSignal.State = outputValue;
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal == null) return;

            // Editor override: piecewise_resolver pair. Override path uses the
            // user's input (no -15° hack); per-channel ZeroTrim is the
            // editor-supported way to compensate for the gauge's natural
            // zero offset.
            if (_rollTransform != null
                && _rollSinChannel != null
                && _rollCosChannel != null
                && _rollSinOutputSignal != null
                && _rollCosOutputSignal != null)
            {
                var t = _rollTransform;
                var sinCos = GaugeTransform.EvaluatePiecewiseResolver(
                    _rollInputSignal.State, t.Breakpoints, t.PeakVolts.Value);
                _rollSinOutputSignal.State = _rollSinChannel.ApplyTrim(sinCos[0], _rollSinOutputSignal.MinValue, _rollSinOutputSignal.MaxValue);
                _rollCosOutputSignal.State = _rollCosChannel.ApplyTrim(sinCos[1], _rollCosOutputSignal.MinValue, _rollCosOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — preserves the -15° offset for Dave R.
            var rollInputDegrees = _rollInputSignal.State - 15;
            //HACK: compensating for lack of ability to calibrate devices in software right now, so hard-coding this offset for Dave R. instrument

            var rollSinOutputValue = 10.0000 * Math.Sin(rollInputDegrees * Constants.RADIANS_PER_DEGREE);
            var rollCosOutputValue = 10.0000 * Math.Cos(rollInputDegrees * Constants.RADIANS_PER_DEGREE);

            if (_rollSinOutputSignal != null)
            {
                if (rollSinOutputValue < -10) rollSinOutputValue = -10;
                else if (rollSinOutputValue > 10) rollSinOutputValue = 10;
                _rollSinOutputSignal.State = rollSinOutputValue;
            }

            if (_rollCosOutputSignal != null)
            {
                if (rollCosOutputValue < -10) rollCosOutputValue = -10;
                else if (rollCosOutputValue > 10) rollCosOutputValue = 10;
                _rollCosOutputSignal.State = rollCosOutputValue;
            }
        }

        private void UpdateVerticalCommandBarOutputValues()
        {
            if (_verticalCommandBarInputSignal == null) return;
            if (_verticalCommandBarOutputSignal == null) return;

            // Show/hide gating preserved verbatim.
            if (!_showCommandBarsInputSignal.State)
            {
                _verticalCommandBarOutputSignal.State = 10;
                return;
            }

            var percentDeflection = _verticalCommandBarInputSignal.State;

            if (_verticalCommandBarChannel != null)
            {
                var v = GaugeTransform.EvaluatePiecewise(percentDeflection, _verticalCommandBarChannel.Transform.Breakpoints);
                _verticalCommandBarOutputSignal.State = _verticalCommandBarChannel.ApplyTrim(v, _verticalCommandBarOutputSignal.MinValue, _verticalCommandBarOutputSignal.MaxValue);
                return;
            }

            // Hardcoded fallback — input × 4.
            var outputValue = 4 * percentDeflection;
            if (outputValue < -10) outputValue = -10;
            else if (outputValue > 10) outputValue = 10;
            _verticalCommandBarOutputSignal.State = outputValue;
        }
    }
}
