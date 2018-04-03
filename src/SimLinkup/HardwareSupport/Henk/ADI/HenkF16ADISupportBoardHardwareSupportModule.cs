using System;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Henk.ADI
{
    //Henk F-16 ADI Support Board for ARU-50/A Primary ADI
    public class HenkF16ADISupportBoardHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const float GLIDESLOPE_DEVIATION_LIMIT_DEGREES = 1.0F;
        private const float LOCALIZER_DEVIATION_LIMIT_DEGREES = 5.0F;

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

        private HenkF16ADISupportBoardHardwareSupportModule()
        {
            CreateInputSignals();
            CreateInputEventHandlers();
            CreateOutputSignals();
            SetInitialOutputValues();
            RegisterForInputEvents();
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
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            return new IHardwareSupportModule[] {new HenkF16ADISupportBoardHardwareSupportModule()};
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
                MaxValue = 700
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
                MaxValue = 1023
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
                _horizontalCommandBarOutputSignal.State = _commandBarsVisibleInputSignal.State
                    ? 0.5 + 0.5 * _horizontalCommandBarInputSignal.State
                    : 1.0f;
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
                _pitchOutputSignal.State = 424 + _pitchInputSignal.State / 90.000 * 255.000;
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
                _rateOfTurnOutputSignal.State = (_rateOfTurnInputSignal.State + 1.000) / 2.000;
            }
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal != null && _rollOutputSignal != null)
            {
                _rollOutputSignal.State = 512.000 + _rollInputSignal.State / 180.000 * 512.000;
            }
        }

        private void UpdateVerticalGSBarOutputValues()
        {
            if (_verticalCommandBarInputSignal != null && _verticalCommandBarOutputSignal != null)
            {
                _verticalCommandBarOutputSignal.State = _commandBarsVisibleInputSignal.State
                    ? 1.00 - (0.5 + 0.5 * _verticalCommandBarInputSignal.State)
                    : 0.0f;
            }
        }

        private void verticalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateVerticalGSBarOutputValues();
        }
    }
}