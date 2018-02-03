using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.AMI
{
    //AMI 9002780-02 Primary ADI
    public class AMI900278002HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
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

        private AMI900278002HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
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
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new AMI900278002HardwareSupportModule()
            };
            return toReturn.ToArray();
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
                FriendlyName = "AUX Flag",
                Id = "900278002_AUX_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateAuxFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "AUX Flag",
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
                FriendlyName = "GS Flag",
                Id = "900278002_GS_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateGSFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "GS Flag",
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
            _pitchInputSignalChangedEventHandler =
                pitch_InputSignalChanged;
            _rollInputSignalChangedEventHandler =
                roll_InputSignalChanged;
            _horizontalCommandBarInputSignalChangedEventHandler =
                horizontalCommandBar_InputSignalChanged;
            _verticalCommandBarInputSignalChangedEventHandler =
                verticalCommandBar_InputSignalChanged;
            _rateOfTurnInputSignalChangedEventHandler =
                rateOfTurn_InputSignalChanged;
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
                FriendlyName = "LOC Flag",
                Id = "900278002_LOC_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateLOCFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "LOC Flag",
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
                FriendlyName = "OFF Flag",
                Id = "900278002_OFF_Flag_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = false
            };
            return thisSignal;
        }

        private DigitalSignal CreateOFFFlagOutputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Outputs",
                CollectionName = "Digital Outputs",
                FriendlyName = "OFF Flag",
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
                FriendlyName = "Command Bars Visible Flag",
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

        private void UnregisterForInputEvents()
        {
            if (_auxFlagInputSignal != null)
            {
                try
                {
                    _auxFlagInputSignal.SignalChanged -= _auxFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_gsFlagInputSignal != null)
            {
                try
                {
                    _gsFlagInputSignal.SignalChanged -= _gsFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_locFlagInputSignal != null)
            {
                try
                {
                    _locFlagInputSignal.SignalChanged -= _locFlagInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_offFlagInputSignal != null)
            {
                try
                {
                    _offFlagInputSignal.SignalChanged -= _offFlagInputSignalChangedEventHandler;
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
            _auxFlagOutputSignal.State = _auxFlagInputSignal.State;
        }

        private void UpdateGSFlagOutputValue()
        {
            _gsFlagOutputSignal.State = _gsFlagInputSignal.State;
        }

        private void UpdateHorizontalCommandBarOutputValues()
        {
            if (_horizontalCommandBarInputSignal != null)
            {
                var percentDeflection = _horizontalCommandBarInputSignal.State;

                var outputValue = _showCommandBarsInputSignal.State ? 4 * percentDeflection : 10;

                if (_horizontalCommandBarOutputSignal != null)
                {
                    if (outputValue < -10)
                    {
                        outputValue = -10;
                    }
                    else if (outputValue > 10)
                    {
                        outputValue = 10;
                    }

                    _horizontalCommandBarOutputSignal.State = outputValue;
                }
            }
        }

        private void UpdateLOCFlagOutputValue()
        {
            _locFlagOutputSignal.State = _locFlagInputSignal.State;
        }

        private void UpdateOFFFlagOutputValue()
        {
            _offFlagOutputSignal.State = _offFlagInputSignal.State;
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal != null)
            {
                var pitchInputDegrees = _pitchInputSignal.State;

                var pitchOutputValue = 10.0000 * (pitchInputDegrees / 90.0);

                if (_pitchOutputSignal != null)
                {
                    if (pitchOutputValue < -10)
                    {
                        pitchOutputValue = -10;
                    }
                    else if (pitchOutputValue > 10)
                    {
                        pitchOutputValue = 10;
                    }

                    _pitchOutputSignal.State = pitchOutputValue;
                }
            }
        }

        private void UpdateRateOfTurnOutputValues()
        {
            if (_rateOfTurnInputSignal != null)
            {
                var percentDeflection = _rateOfTurnInputSignal.State;

                var outputValue = 10.0000 * percentDeflection;

                if (_rateOfTurnOutputSignal == null) return;
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
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal != null)
            {
                var rollInputDegrees = _rollInputSignal.State - 15;
                //HACK: compensating for lack of ability to calibrate devices in software right now, so hard-coding this offset for Dave R. instrument


                var rollSinOutputValue = 10.0000 * Math.Sin(rollInputDegrees * Constants.RADIANS_PER_DEGREE);
                var rollCosOutputValue = 10.0000 * Math.Cos(rollInputDegrees * Constants.RADIANS_PER_DEGREE);

                if (_rollSinOutputSignal != null)
                {
                    if (rollSinOutputValue < -10)
                    {
                        rollSinOutputValue = -10;
                    }
                    else if (rollSinOutputValue > 10)
                    {
                        rollSinOutputValue = 10;
                    }

                    _rollSinOutputSignal.State = rollSinOutputValue;
                }

                if (_rollCosOutputSignal != null)
                {
                    if (rollCosOutputValue < -10)
                    {
                        rollCosOutputValue = -10;
                    }
                    else if (rollCosOutputValue > 10)
                    {
                        rollCosOutputValue = 10;
                    }

                    _rollCosOutputSignal.State = rollCosOutputValue;
                }
            }
        }

        private void UpdateVerticalCommandBarOutputValues()
        {
            if (_verticalCommandBarInputSignal != null)
            {
                var percentDeflection = _verticalCommandBarInputSignal.State;

                var outputValue = _showCommandBarsInputSignal.State ? 4 * percentDeflection : 10;

                if (_verticalCommandBarOutputSignal != null)
                {
                    if (outputValue < -10)
                    {
                        outputValue = -10;
                    }
                    else if (outputValue > 10)
                    {
                        outputValue = 10;
                    }

                    _verticalCommandBarOutputSignal.State = outputValue;
                }
            }
        }

        private void verticalCommandBar_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateVerticalCommandBarOutputValues();
        }
    }
}