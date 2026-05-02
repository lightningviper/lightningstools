using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.HardwareSupport.Calibration;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;
using log4net;

namespace SimLinkup.HardwareSupport.Henk.ADI
{
    //Henk F-16 ADI Support Board for ARU-50/A Primary ADI
    public class HenkF16ADISupportBoardHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HenkF16ADISupportBoardHardwareSupportModule));

        private const float GLIDESLOPE_DEVIATION_LIMIT_DEGREES = 1.0F;
        private const float LOCALIZER_DEVIATION_LIMIT_DEGREES = 5.0F;

        // Editor-authored calibration state. _config carries the loaded
        // unified-schema file; the per-channel pointers below are non-null
        // when the corresponding channel has a usable piecewise breakpoint
        // table. Each Update*OutputValues consults its pointer and falls
        // back to the hardcoded math when null. See
        // HenkF16ADISupportBoardHardwareSupportModuleConfig for the schema.
        private HenkF16ADISupportBoardHardwareSupportModuleConfig _config;
        private GaugeChannelConfig _pitchCalibration;
        private GaugeChannelConfig _rollCalibration;
        private GaugeChannelConfig _horizontalCommandBarCalibration;
        private GaugeChannelConfig _verticalCommandBarCalibration;
        private GaugeChannelConfig _rateOfTurnCalibration;
        private ConfigFileReloadWatcher _configWatcher;

        private readonly IADI _renderer = new LightningGauges.Renderers.F16.ADI();
        private DigitalSignal _auxFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _auxFlagInputSignalChangedEventHandler;

        private DigitalSignal _auxFlagOutputSignal;

        private DigitalSignal _commandBarsVisibleInputSignal;
        private DigitalSignal.SignalChangedEventHandler _commandBarsVisibleInputSignalChangedEventHandler;
        private DigitalSignal _glideslopeIndicatorsPowerOnOffInputSignal;
        private DigitalSignal.SignalChangedEventHandler _glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler;
        private DigitalSignal _glideslopeIndicatorsPowerOnOffOutputSignal;
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

        private DigitalSignal _pitchAndRollEnableInputSignal;
        private DigitalSignal.SignalChangedEventHandler _pitchAndRollEnableInputSignalChangedEventHandler;

        private DigitalSignal _pitchAndRollEnableOutputSignal;

        private AnalogSignal _pitchInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _pitchInputSignalChangedEventHandler;


        private AnalogSignal _pitchOutputSignal;
        private DigitalSignal _rateOfTurnAndFlagsPowerOnOffInputSignal;
        private DigitalSignal.SignalChangedEventHandler _rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler;
        private DigitalSignal _rateOfTurnAndFlagsPowerOnOffOutputSignal;
        private AnalogSignal _rateOfTurnInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rateOfTurnInputSignalChangedEventHandler;
        private AnalogSignal _rateOfTurnOutputSignal;
        private AnalogSignal _rollInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rollInputSignalChangedEventHandler;
        private AnalogSignal _rollOutputSignal;
        private AnalogSignal _verticalCommandBarInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _verticalCommandBarInputSignalChangedEventHandler;
        private AnalogSignal _verticalCommandBarOutputSignal;

        // Public ctor takes the (optionally-null) config. Null = no editor
        // override; the existing hardcoded math handles everything.
        public HenkF16ADISupportBoardHardwareSupportModule(HenkF16ADISupportBoardHardwareSupportModuleConfig config)
        {
            _config = config;
            ResolveAllChannels(config);
            CreateInputSignals();
            CreateInputEventHandlers();
            CreateOutputSignals();
            SetInitialOutputValues();
            RegisterForInputEvents();
            StartConfigWatcher();
        }

        // Resolve all five channel pointers in one pass. Each may end up
        // null (no config / channel absent / channel not piecewise) — the
        // per-update branches handle null individually with a fallback to
        // the hardcoded math.
        private void ResolveAllChannels(GaugeCalibrationConfig config)
        {
            _pitchCalibration                = ResolvePiecewiseChannel(config, "HenkF16ADISupportBoard_Pitch_To_SDI");
            _rollCalibration                 = ResolvePiecewiseChannel(config, "HenkF16ADISupportBoard_Roll_To_SDI");
            _horizontalCommandBarCalibration = ResolvePiecewiseChannel(config, "HenkF16ADISupportBoard_Horizontal_GS_Bar_To_SDI");
            _verticalCommandBarCalibration   = ResolvePiecewiseChannel(config, "HenkF16ADISupportBoard_Vertical_GS_Bar_To_SDI");
            _rateOfTurnCalibration           = ResolvePiecewiseChannel(config, "HenkF16ADISupportBoard_Rate_Of_Turn_To_SDI");
        }

        // Pull the named channel out of the config IFF it carries a usable
        // piecewise breakpoint table. Returns null otherwise so the caller's
        // null-check covers both "no config file" and "config present but
        // doesn't override this channel" with the same code path.
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

        // Set up a ConfigFileReloadWatcher on the config file so editor
        // saves take effect within a few seconds on the running SimLinkup.
        // Skipped when no config was loaded (nothing to watch). The shared
        // helper handles the unreliable bits — silent watcher orphaning
        // under antivirus / OneDrive / SMB filter drivers, internal buffer
        // overflow, mtime dedup — so this HSM just supplies the reload
        // callback.
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

        // Reload the config and re-resolve channel pointers. Wrapped in a
        // try/catch so a partial-write race (the watcher can fire mid-save)
        // doesn't escape into the runtime — we just log and wait for the
        // next change event with a complete file. Re-fires every Update*
        // method at the end so the user sees the new calibration
        // immediately, instead of waiting for the next sim tick.
        private void ReloadConfig()
        {
            try
            {
                var configFile = _config != null ? _config.FilePath : null;
                if (string.IsNullOrEmpty(configFile)) return;
                var reloaded = GaugeCalibrationConfig.Load<HenkF16ADISupportBoardHardwareSupportModuleConfig>(configFile);
                if (reloaded == null) return;
                reloaded.FilePath = configFile;
                _config = reloaded;
                ResolveAllChannels(reloaded);
                UpdatePitchOutputValues();
                UpdateRollOutputValues();
                UpdateHorizontalGSBarOutputValues();
                UpdateVerticalGSBarOutputValues();
                UpdateRateOfTurnOutputValues();
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
            _pitchOutputSignal, _rollOutputSignal, _horizontalCommandBarOutputSignal,
            _verticalCommandBarOutputSignal, _rateOfTurnOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => new[]
        {
            _commandBarsVisibleInputSignal, _auxFlagInputSignal, _gsFlagInputSignal, _locFlagInputSignal,
            _offFlagInputSignal,
            _pitchAndRollEnableInputSignal, _glideslopeIndicatorsPowerOnOffInputSignal,
            _rateOfTurnAndFlagsPowerOnOffInputSignal
        };

        public override DigitalSignal[] DigitalOutputs => new[]
        {
            _auxFlagOutputSignal, _gsFlagOutputSignal, _locFlagOutputSignal,
            _pitchAndRollEnableOutputSignal, _glideslopeIndicatorsPowerOnOffOutputSignal,
            _rateOfTurnAndFlagsPowerOnOffOutputSignal
        };

        public override string FriendlyName => "Henk F-16 ADI Support Board for ARU-50/A Primary ADI";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HenkF16ADISupportBoardHardwareSupportModule()
        {
            Dispose(false);
        }

        // Try to load the optional editor-authored config file alongside
        // the registry. Missing or malformed file → null config → HSM
        // falls back to its hardcoded math (= pre-config behaviour).
        public static IHardwareSupportModule[] GetInstances()
        {
            HenkF16ADISupportBoardHardwareSupportModuleConfig hsmConfig = null;
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "HenkF16ADISupportBoardHardwareSupportModule.config");
                hsmConfig = GaugeCalibrationConfig.Load<HenkF16ADISupportBoardHardwareSupportModuleConfig>(hsmConfigFilePath);
                if (hsmConfig != null)
                {
                    // Stash the path on the config so the instance's
                    // ConfigFileReloadWatcher knows which file to watch.
                    hsmConfig.FilePath = hsmConfigFilePath;
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return new IHardwareSupportModule[] { new HenkF16ADISupportBoardHardwareSupportModule(hsmConfig) };
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
            _renderer.InstrumentState.ShowCommandBars = _commandBarsVisibleInputSignal.State;
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
            _commandBarsVisibleInputSignalChangedEventHandler = null;
            _pitchAndRollEnableInputSignalChangedEventHandler = null;
            _glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler = null;
            _rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler = null;
        }


        private void auxFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateAuxFlagOutputValue();
        }

        private void commandBarsVisible_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateHorizontalGSBarOutputValues();
            UpdateVerticalGSBarOutputValues();
        }

        private DigitalSignal CreateAuxFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "AUX Flag Visible (0=Hidden, 1=Visible)",
                Id = "HenkF16ADISupportBoard_AUX_Flag_From_Sim",
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
                FriendlyName = "AUX Flag Hidden (0=Visible, 1=Hidden)",
                Id = "HenkF16ADISupportBoard_AUX_Flag_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }

        private DigitalSignal CreateCommandBarsVisibleInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Command Bars Visible (0=Hidden; 1=Visible)",
                Id = "HenkF16ADISupportBoard_Command_Bars_Visible_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateGlideslopeIndicatorsPowerOnOffInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "GS POWER",
                Id = "HenkF16ADISupportBoard_GS_POWER_Input",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateGlideslopeIndicatorsPowerOnOffOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "Glideslope Indicators POWER",
                Id = "HenkF16ADISupportBoard_GS_POWER_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
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
                Id = "HenkF16ADISupportBoard_GS_Flag_From_Sim",
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
                Id = "HenkF16ADISupportBoard_GS_Flag_To_SDI",
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
                FriendlyName =
                    "Horizontal Command Bar (Degrees, -1.0=100% deflected up, 0.0=centered, +1.0=100% deflected down)",
                Id = "HenkF16ADISupportBoard_Horizontal_Command_Bar_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.0, //centered
                MinValue = -1.0, //percent deflected up
                MaxValue = 1.0 //percent deflected down
            };
            return thisSignal;
        }

        private AnalogSignal CreateHorizontalCommandBarOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName =
                    "Horizontal Glideslope Indicator Position (Percent Deflection, 0.0=100% deflected down, 0.5=centered, 1.0=100% deflected up)",
                Id = "HenkF16ADISupportBoard_Horizontal_GS_Bar_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0.5, //50%
                MinValue = 0.0, //0%
                MaxValue = 1.0 //100%
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
            _commandBarsVisibleInputSignalChangedEventHandler = commandBarsVisible_InputSignalChanged;
            _pitchAndRollEnableInputSignalChangedEventHandler = pitchAndRollEnable_InputSignalChanged;
            _glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler =
                glideslopeIndicatorsPowerOnOff_InputSignalChanged;
            _rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler =
                rateOfTurnAndFlagsPowerOnOff_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _pitchInputSignal = CreatePitchInputSignal();
            _rollInputSignal = CreateRollInputSignal();
            _horizontalCommandBarInputSignal = CreateHorizontalCommandBarInputSignal();
            _verticalCommandBarInputSignal = CreateVerticalCommandBarInputSignal();
            _rateOfTurnInputSignal = CreateRateOfTurnInputSignal();
            _commandBarsVisibleInputSignal = CreateCommandBarsVisibleInputSignal();
            _auxFlagInputSignal = CreateAuxFlagInputSignal();
            _gsFlagInputSignal = CreateGSFlagInputSignal();
            _locFlagInputSignal = CreateLOCFlagInputSignal();
            _offFlagInputSignal = CreateOFFFlagInputSignal();
            _pitchAndRollEnableInputSignal = CreatePitchAndRollEnableInputSignal();
            _glideslopeIndicatorsPowerOnOffInputSignal = CreateGlideslopeIndicatorsPowerOnOffInputSignal();
            _rateOfTurnAndFlagsPowerOnOffInputSignal = CreateRateOfTurnAndFlagsPowerOnOffInputSignal();
        }

        private DigitalSignal CreateLOCFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "LOC Flag Visible (0=Hidden; 1=Visible)",
                Id = "HenkF16ADISupportBoard_LOC_Flag_From_Sim",
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
                FriendlyName = "LOC Flag Hidden (0=Visible; 1=Hidden)",
                Id = "HenkF16ADISupportBoard_LOC_Flag_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };

            return thisSignal;
        }

        private DigitalSignal CreateOFFFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag Visible (0=Hidden; 1=Visible)",
                Id = "HenkF16ADISupportBoard_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private void CreateOutputSignals()
        {
            _pitchOutputSignal = CreatePitchOutputSignal();
            _rollOutputSignal = CreateRollOutputSignal();
            _horizontalCommandBarOutputSignal = CreateHorizontalCommandBarOutputSignal();
            _verticalCommandBarOutputSignal = CreateVerticalCommandBarOutputSignal();
            _rateOfTurnOutputSignal = CreateRateOfTurnOutputSignal();
            _auxFlagOutputSignal = CreateAuxFlagOutputSignal();
            _gsFlagOutputSignal = CreateGSFlagOutputSignal();
            _locFlagOutputSignal = CreateLOCFlagOutputSignal();

            _pitchAndRollEnableOutputSignal = CreatePitchAndRollEnableOutputSignal();
            _glideslopeIndicatorsPowerOnOffOutputSignal = CreateGlideslopeIndicatorsPowerOnOffOutputSignal();
            _rateOfTurnAndFlagsPowerOnOffOutputSignal = CreateRateOfTurnAndFlagsPowerOnOffOutputSignal();
        }

        private DigitalSignal CreatePitchAndRollEnableInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "Pitch/Roll synchros ENABLED",
                Id = "HenkF16ADISupportBoard_ENABLE_PITCH_AND_ROLL_Input",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreatePitchAndRollEnableOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "Pitch/Roll synchros ENABLED",
                Id = "HenkF16ADISupportBoard_ENABLE_PITCH_AND_ROLL_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };

            return thisSignal;
        }

        private AnalogSignal CreatePitchInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Pitch (Degrees, -90.0=nadir, 0.0=level, +90.0=zenith)",
                Id = "HenkF16ADISupportBoard_Pitch_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.0, //degees
                IsAngle = true,
                MinValue = -90.0, //degrees
                MaxValue = 90.0 //degrees
            };
            return thisSignal;
        }

        private AnalogSignal CreatePitchOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch Synchro Position (0-1023)",
                Id = "HenkF16ADISupportBoard_Pitch_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 424,
                MinValue = 140,
                MaxValue = 700,
                Precision = 0
            };

            return thisSignal;
        }

        private DigitalSignal CreateRateOfTurnAndFlagsPowerOnOffInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "RT and Flags POWER",
                Id = "HenkF16ADISupportBoard_RT_AND_FLAGS_POWER_Input",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };
            return thisSignal;
        }

        private DigitalSignal CreateRateOfTurnAndFlagsPowerOnOffOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "RT and Flags POWER",
                Id = "HenkF16ADISupportBoard_RT_AND_FLAGS_POWER_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = true
            };

            return thisSignal;
        }

        private AnalogSignal CreateRateOfTurnInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName =
                    "Rate of Turn Indicator (% Deflection, -1.0=100% deflected left; 0.0=centered, +1.0=100% deflected right)",
                Id = "HenkF16ADISupportBoard_Rate_Of_Turn_Indicator_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = -1.0, //-100% (left deflected)
                MaxValue = 1.0 //+100% (right deflected)
            };
            return thisSignal;
        }

        private AnalogSignal CreateRateOfTurnOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName =
                    "Rate of Turn Indicator (Percent Deflection, 0.0=100% deflected left, 0.5=centered, 1.0=100% deflected right)",
                Id = "HenkF16ADISupportBoard_Rate_Of_Turn_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0.50, //50%
                MinValue = 0.0, //0%
                MaxValue = 1.0 //100%
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName =
                    "Roll (Degrees, -180.0=inverted left bank, -90.0=left bank, 0.0=wings level, +90.0=right bank, +180.0=inverted right bank)",
                Id = "HenkF16ADISupportBoard_Roll_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.0, //degrees
                IsAngle = true,
                MinValue = -180.0, //degrees
                MaxValue = 180.0 //degrees
            };
            return thisSignal;
        }

        private AnalogSignal CreateRollOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll Synchro Position (0-1023)",
                Id = "HenkF16ADISupportBoard_Roll_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 512,
                MinValue = 0,
                MaxValue = 1023,
                Precision = 0
            };
            return thisSignal;
        }

        private AnalogSignal CreateVerticalCommandBarInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName =
                    "Vertical Command Bar (Degrees, -1.0=100% deflected left; 0.0=centered, +1.0=100% deflected right )",
                Id = "HenkF16ADISupportBoard_Vertical_Command_Bar_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.0f, //centered
                MinValue = -1.0, //percent deflected left
                MaxValue = 1.0 //percent deflected right
            };
            return thisSignal;
        }

        private AnalogSignal CreateVerticalCommandBarOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName =
                    "Vertical Glideslope Indicator Position (Percent Deflection, 0.0=100% deflected righ, 0.5=centered, 1.0=100% deflected left)",
                Id = "HenkF16ADISupportBoard_Vertical_GS_Bar_To_SDI",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0.5, //50%
                MinValue = 0.0, //0%
                MaxValue = 1.0 //100%
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

        private void glideslopeIndicatorsPowerOnOff_InputSignalChanged(object sender,
            DigitalSignalChangedEventArgs args)
        {
            UpdateGSPowerOutputValue();
        }

        private void gsFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateGSFlagOutputValue();
        }

        private void horizontalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateHorizontalGSBarOutputValues();
        }

        private void locFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateLOCFlagOutputValue();
        }

        private static void offFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
        }


        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void pitchAndRollEnable_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdatePitchAndRollEnableOutputValue();
        }

        private void rateOfTurn_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRateOfTurnOutputValues();
        }

        private void rateOfTurnAndFlagsPowerOnOff_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateRateOfTurnAndFlagsPowerOnOffOutputValue();
        }

        private void RegisterForInputEvents()
        {
            if (_auxFlagInputSignal != null)
            {
                _auxFlagInputSignal.SignalChanged += _auxFlagInputSignalChangedEventHandler;
            }
            if (_gsFlagInputSignal != null)
            {
                _gsFlagInputSignal.SignalChanged += _gsFlagInputSignalChangedEventHandler;
            }
            if (_locFlagInputSignal != null)
            {
                _locFlagInputSignal.SignalChanged += _locFlagInputSignalChangedEventHandler;
            }
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
            if (_commandBarsVisibleInputSignal != null)
            {
                _commandBarsVisibleInputSignal.SignalChanged += _commandBarsVisibleInputSignalChangedEventHandler;
            }
            if (_pitchAndRollEnableInputSignal != null)
            {
                _pitchAndRollEnableInputSignal.SignalChanged += _pitchAndRollEnableInputSignalChangedEventHandler;
            }
            if (_glideslopeIndicatorsPowerOnOffInputSignal != null)
            {
                _glideslopeIndicatorsPowerOnOffInputSignal.SignalChanged +=
                    _glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler;
            }
            if (_rateOfTurnAndFlagsPowerOnOffInputSignal != null)
            {
                _rateOfTurnAndFlagsPowerOnOffInputSignal.SignalChanged +=
                    _rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler;
            }
            if (_pitchInputSignal != null)
            {
                _pitchInputSignal.SignalChanged += _pitchInputSignalChangedEventHandler;
            }
            if (_rollInputSignal != null)
            {
                _rollInputSignal.SignalChanged += _rollInputSignalChangedEventHandler;
            }
            if (_horizontalCommandBarInputSignal != null)
            {
                _horizontalCommandBarInputSignal.SignalChanged += _horizontalCommandBarInputSignalChangedEventHandler;
            }
            if (_verticalCommandBarInputSignal != null)
            {
                _verticalCommandBarInputSignal.SignalChanged += _verticalCommandBarInputSignalChangedEventHandler;
            }
            if (_rateOfTurnInputSignal != null)
            {
                _rateOfTurnInputSignal.SignalChanged += _rateOfTurnInputSignalChangedEventHandler;
            }
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
        }

        private void SetInitialOutputValues()
        {
            UpdatePitchAndRollEnableOutputValue();
            UpdatePitchOutputValues();
            UpdateRollOutputValues();
            UpdateGSPowerOutputValue();
            UpdateHorizontalGSBarOutputValues();
            UpdateVerticalGSBarOutputValues();
            UpdateRateOfTurnAndFlagsPowerOnOffOutputValue();
            UpdateRateOfTurnOutputValues();
            UpdateAuxFlagOutputValue();
            UpdateGSFlagOutputValue();
            UpdateLOCFlagOutputValue();
        }

        private void UnregisterForInputEvents()
        {
            if (_auxFlagInputSignalChangedEventHandler != null && _auxFlagInputSignal != null)
            {
                try
                {
                    _auxFlagInputSignal.SignalChanged -= _auxFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_gsFlagInputSignalChangedEventHandler != null && _gsFlagInputSignal != null)
            {
                try
                {
                    _gsFlagInputSignal.SignalChanged -= _gsFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_locFlagInputSignalChangedEventHandler != null && _locFlagInputSignal != null)
            {
                try
                {
                    _locFlagInputSignal.SignalChanged -= _locFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_offFlagInputSignalChangedEventHandler != null && _offFlagInputSignal != null)
            {
                try
                {
                    _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_commandBarsVisibleInputSignalChangedEventHandler != null && _commandBarsVisibleInputSignal != null)
            {
                try
                {
                    _commandBarsVisibleInputSignal.SignalChanged -= _commandBarsVisibleInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_pitchAndRollEnableInputSignalChangedEventHandler != null && _pitchAndRollEnableInputSignal != null)
            {
                try
                {
                    _pitchAndRollEnableInputSignal.SignalChanged -= _pitchAndRollEnableInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler != null &&
                _glideslopeIndicatorsPowerOnOffInputSignal != null)
            {
                try
                {
                    _glideslopeIndicatorsPowerOnOffInputSignal.SignalChanged -=
                        _glideslopeIndicatorsPowerOnOffInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler != null &&
                _rateOfTurnAndFlagsPowerOnOffInputSignal != null)
            {
                try
                {
                    _rateOfTurnAndFlagsPowerOnOffInputSignal.SignalChanged -=
                        _rateOfTurnAndFlagsPowerOnOffInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_pitchInputSignalChangedEventHandler != null && _pitchInputSignal != null)
            {
                try
                {
                    _pitchInputSignal.SignalChanged -= _pitchInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_rollInputSignalChangedEventHandler != null && _rollInputSignal != null)
            {
                try
                {
                    _rollInputSignal.SignalChanged -= _rollInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_horizontalCommandBarInputSignalChangedEventHandler != null && _horizontalCommandBarInputSignal != null)
            {
                try
                {
                    _horizontalCommandBarInputSignal.SignalChanged -=
                        _horizontalCommandBarInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_verticalCommandBarInputSignalChangedEventHandler != null && _verticalCommandBarInputSignal != null)
            {
                try
                {
                    _verticalCommandBarInputSignal.SignalChanged -= _verticalCommandBarInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_rateOfTurnInputSignalChangedEventHandler != null && _rateOfTurnInputSignal != null)
            {
                try
                {
                    _rateOfTurnInputSignal.SignalChanged -= _rateOfTurnInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateAuxFlagOutputValue()
        {
            if (_auxFlagInputSignal != null && _auxFlagOutputSignal != null)
            {
                _auxFlagOutputSignal.State = !_auxFlagInputSignal.State;
            }
        }

        private void UpdateGSFlagOutputValue()
        {
            if (_gsFlagInputSignal != null && _gsFlagOutputSignal != null)
            {
                _gsFlagOutputSignal.State = !_gsFlagInputSignal.State;
            }
        }

        private void UpdateGSPowerOutputValue()
        {
            if (_glideslopeIndicatorsPowerOnOffOutputSignal != null &&
                _glideslopeIndicatorsPowerOnOffInputSignal != null)
            {
                _glideslopeIndicatorsPowerOnOffOutputSignal.State = _glideslopeIndicatorsPowerOnOffInputSignal.State;
            }
        }

        private void UpdateHorizontalGSBarOutputValues()
        {
            if (_horizontalCommandBarInputSignal != null && _horizontalCommandBarOutputSignal != null)
            {
                // Hidden state when the visibility flag is low. Editor-
                // authored HiddenOutput overrides the hardcoded 1.0 (the
                // value that physically pushes the bar off-screen on
                // bench-stock ADIs; some installations need a different
                // park position).
                if (!_commandBarsVisibleInputSignal.State)
                {
                    var hidden = _horizontalCommandBarCalibration?.HiddenOutput ?? 1.0;
                    _horizontalCommandBarOutputSignal.State = _horizontalCommandBarCalibration != null
                        ? _horizontalCommandBarCalibration.ApplyTrim(hidden, _horizontalCommandBarOutputSignal.MinValue, _horizontalCommandBarOutputSignal.MaxValue)
                        : hidden;
                    return;
                }
                if (_horizontalCommandBarCalibration != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(_horizontalCommandBarInputSignal.State, _horizontalCommandBarCalibration.Transform.Breakpoints);
                    _horizontalCommandBarOutputSignal.State = _horizontalCommandBarCalibration.ApplyTrim(v, _horizontalCommandBarOutputSignal.MinValue, _horizontalCommandBarOutputSignal.MaxValue);
                    return;
                }
                _horizontalCommandBarOutputSignal.State = 0.5 + 0.5 * _horizontalCommandBarInputSignal.State;
            }
        }

        private void UpdateLOCFlagOutputValue()
        {
            if (_locFlagInputSignal != null && _locFlagOutputSignal != null)
            {
                _locFlagOutputSignal.State = !_locFlagInputSignal.State;
            }
        }

        private void UpdatePitchAndRollEnableOutputValue()
        {
            if (_pitchAndRollEnableOutputSignal != null && _pitchAndRollEnableInputSignal != null)
            {
                _pitchAndRollEnableOutputSignal.State = _pitchAndRollEnableInputSignal.State;
            }
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal != null && _pitchOutputSignal != null)
            {
                // Editor override: piecewise breakpoint table (input°, output DAC counts).
                if (_pitchCalibration != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(_pitchInputSignal.CorrelatedState, _pitchCalibration.Transform.Breakpoints);
                    _pitchOutputSignal.State = _pitchCalibration.ApplyTrim(v, _pitchOutputSignal.MinValue, _pitchOutputSignal.MaxValue);
                    return;
                }
                _pitchOutputSignal.State = 424 + _pitchInputSignal.CorrelatedState / 90.000 * 255.000;
            }
        }

        private void UpdateRateOfTurnAndFlagsPowerOnOffOutputValue()
        {
            if (_rateOfTurnAndFlagsPowerOnOffOutputSignal != null && _rateOfTurnAndFlagsPowerOnOffInputSignal != null)
            {
                _rateOfTurnAndFlagsPowerOnOffOutputSignal.State = _rateOfTurnAndFlagsPowerOnOffInputSignal.State;
            }
        }

        private void UpdateRateOfTurnOutputValues()
        {
            if (_rateOfTurnInputSignal != null && _rateOfTurnOutputSignal != null)
            {
                // Editor override: piecewise breakpoint table (input -1..+1, output 0..1).
                if (_rateOfTurnCalibration != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(_rateOfTurnInputSignal.State, _rateOfTurnCalibration.Transform.Breakpoints);
                    _rateOfTurnOutputSignal.State = _rateOfTurnCalibration.ApplyTrim(v, _rateOfTurnOutputSignal.MinValue, _rateOfTurnOutputSignal.MaxValue);
                    return;
                }
                _rateOfTurnOutputSignal.State = (_rateOfTurnInputSignal.State + 1.000) / 2.000;
            }
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal != null && _rollOutputSignal != null)
            {
                // Editor override: piecewise breakpoint table (input°, output DAC counts).
                if (_rollCalibration != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(_rollInputSignal.CorrelatedState, _rollCalibration.Transform.Breakpoints);
                    _rollOutputSignal.State = _rollCalibration.ApplyTrim(v, _rollOutputSignal.MinValue, _rollOutputSignal.MaxValue);
                    return;
                }
                _rollOutputSignal.State = 512.000 + _rollInputSignal.CorrelatedState / 180.000 * 512.000;
            }
        }

        private void UpdateVerticalGSBarOutputValues()
        {
            if (_verticalCommandBarInputSignal != null && _verticalCommandBarOutputSignal != null)
            {
                // Hidden state when the visibility flag is low. Editor-
                // authored HiddenOutput overrides the hardcoded 0.0.
                if (!_commandBarsVisibleInputSignal.State)
                {
                    var hidden = _verticalCommandBarCalibration?.HiddenOutput ?? 0.0;
                    _verticalCommandBarOutputSignal.State = _verticalCommandBarCalibration != null
                        ? _verticalCommandBarCalibration.ApplyTrim(hidden, _verticalCommandBarOutputSignal.MinValue, _verticalCommandBarOutputSignal.MaxValue)
                        : hidden;
                    return;
                }
                if (_verticalCommandBarCalibration != null)
                {
                    var v = GaugeTransform.EvaluatePiecewise(_verticalCommandBarInputSignal.State, _verticalCommandBarCalibration.Transform.Breakpoints);
                    _verticalCommandBarOutputSignal.State = _verticalCommandBarCalibration.ApplyTrim(v, _verticalCommandBarOutputSignal.MinValue, _verticalCommandBarOutputSignal.MaxValue);
                    return;
                }
                _verticalCommandBarOutputSignal.State = 1.00 - (0.5 + 0.5 * _verticalCommandBarInputSignal.State);
            }
        }

        private void verticalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateVerticalGSBarOutputValues();
        }
    }
}