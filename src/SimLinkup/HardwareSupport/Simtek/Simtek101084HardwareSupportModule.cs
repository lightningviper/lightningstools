using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1084 F-16 Standby ADI
    public class Simtek101084HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IStandbyADI _renderer = new StandbyADI();

        private bool _isDisposed;

        private DigitalSignal _offFlagInputSignal;
        private DigitalSignal.SignalChangedEventHandler _offFlagInputSignalChangedEventHandler;

        private DigitalSignal _offFlagOutputSignal;
        private AnalogSignal _pitchCosOutputSignal;
        private AnalogSignal _pitchInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _pitchInputSignalChangedEventHandler;
        private AnalogSignal _pitchSinOutputSignal;
        private AnalogSignal _rollCosOutputSignal;
        private AnalogSignal _rollInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rollInputSignalChangedEventHandler;
        private AnalogSignal _rollSinOutputSignal;

        private Simtek101084HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_pitchInputSignal, _rollInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_pitchSinOutputSignal, _pitchCosOutputSignal, _rollSinOutputSignal, _rollCosOutputSignal};

        public override DigitalSignal[] DigitalInputs => new[] {_offFlagInputSignal};

        public override DigitalSignal[] DigitalOutputs => new[] {_offFlagOutputSignal};

        public override string FriendlyName => "Simtek P/N 10-1084 - Indicator - Simulated Standby Attitude";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101084HardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101084HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.OffFlag = _offFlagInputSignal.State;
            _renderer.InstrumentState.PitchDegrees = (float) _pitchInputSignal.State;
            _renderer.InstrumentState.RollDegrees = (float) _rollInputSignal.State;

            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler = null;
            _rollInputSignalChangedEventHandler = null;
            _offFlagInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _pitchInputSignalChangedEventHandler =
                pitch_InputSignalChanged;
            _rollInputSignalChangedEventHandler =
                roll_InputSignalChanged;
            _offFlagInputSignalChangedEventHandler =
                offFlag_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _pitchInputSignal = CreatePitchInputSignal();
            _rollInputSignal = CreateRollInputSignal();
            _offFlagInputSignal = CreateOFFFlagInputSignal();
        }

        private DigitalSignal CreateOFFFlagInputSignal()
        {
            var thisSignal = new DigitalSignal
            {
                Category = "Inputs",
                CollectionName = "Digital Inputs",
                FriendlyName = "OFF Flag",
                Id = "101084_OFF_Flag_From_Sim",
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
                Id = "101084_OFF_Flag_To_Instrument",
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
            _pitchSinOutputSignal = CreatePitchSinOutputSignal();
            _pitchCosOutputSignal = CreatePitchCosOutputSignal();
            _rollSinOutputSignal = CreateRollSinOutputSignal();
            _rollCosOutputSignal = CreateRollCosOutputSignal();
            _offFlagOutputSignal = CreateOFFFlagOutputSignal();
        }

        private AnalogSignal CreatePitchCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (COS)",
                Id = "101084_Pitch_COS_To_Instrument",
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

        private AnalogSignal CreatePitchInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Pitch (Degrees)",
                Id = "101084_Pitch_From_Sim",
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

        private AnalogSignal CreatePitchSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Pitch (SIN)",
                Id = "101084_Pitch_SIN_To_Instrument",
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

        private AnalogSignal CreateRollCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Roll (COS)",
                Id = "101084_Roll_COS_To_Instrument",
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

        private AnalogSignal CreateRollInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Roll (Degrees)",
                Id = "101084_Roll_From_Sim",
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
                Id = "101084_Roll_SIN_To_Instrument",
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

        private void offFlag_InputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            UpdateOFFFlagOutputValue();
        }

        private void pitch_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdatePitchOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_pitchInputSignal != null)
            {
                _pitchInputSignal.SignalChanged += _pitchInputSignalChangedEventHandler;
            }
            if (_rollInputSignal != null)
            {
                _rollInputSignal.SignalChanged += _rollInputSignalChangedEventHandler;
            }
            if (_offFlagInputSignal != null)
            {
                _offFlagInputSignal.SignalChanged += _offFlagInputSignalChangedEventHandler;
            }
        }

        private void roll_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateRollOutputValues();
        }

        private void UnregisterForInputEvents()
        {
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
        }

        private void UpdateOFFFlagOutputValue()
        {
            _offFlagOutputSignal.State = _offFlagInputSignal.State;
        }

        private void UpdatePitchOutputValues()
        {
            if (_pitchInputSignal == null) return;
            var pitchInputDegrees = _pitchInputSignal.State;
            double pitchOutputRefAngleDegrees = 0;

            if (pitchInputDegrees >= 0 && pitchInputDegrees < 10)
            {
                pitchOutputRefAngleDegrees = pitchInputDegrees / 10 * 21.176;
            }
            else if (pitchInputDegrees >= 10 && pitchInputDegrees < 20)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees - 10) / 10 * 21.177;
            }
            else if (pitchInputDegrees >= 20 && pitchInputDegrees < 30)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees - 20) / 10 * 21.176;
            }
            else if (pitchInputDegrees >= 30 && pitchInputDegrees < 60)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees - 30) / 30 * 63.53;
            }
            else if (pitchInputDegrees >= 60 && pitchInputDegrees <= 90)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees - 60) / 30 * 67.76;
            }
            else if (pitchInputDegrees <= -60 && pitchInputDegrees >= -90)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees - -60) / 30 * 38.122;
            }
            else if (pitchInputDegrees <= -30 && pitchInputDegrees >= -60)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees + 60) / 30 * 63.53;
            }
            else if (pitchInputDegrees <= -20 && pitchInputDegrees >= -30)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees + 30) / 10 * 21.176;
            }
            else if (pitchInputDegrees <= -10 && pitchInputDegrees >= -20)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees + 20) / 10 * 21.177;
            }
            else if (pitchInputDegrees <= 0 && pitchInputDegrees >= -10)
            {
                pitchOutputRefAngleDegrees = (pitchInputDegrees + 10) / 10 * 21.176;
            }


            var pitchSinOutputValue = 10.0000 * Math.Sin(pitchOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);
            var pitchCosOutputValue = 10.0000 * Math.Cos(pitchOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);

            if (_pitchSinOutputSignal != null)
            {
                if (pitchSinOutputValue < -10)
                {
                    pitchSinOutputValue = -10;
                }
                else if (pitchSinOutputValue > 10)
                {
                    pitchSinOutputValue = 10;
                }

                _pitchSinOutputSignal.State = pitchSinOutputValue;
            }

            if (_pitchCosOutputSignal == null) return;
            if (pitchCosOutputValue < -10)
            {
                pitchCosOutputValue = -10;
            }
            else if (pitchCosOutputValue > 10)
            {
                pitchCosOutputValue = 10;
            }

            _pitchCosOutputSignal.State = pitchCosOutputValue;
        }

        private void UpdateRollOutputValues()
        {
            if (_rollInputSignal == null) return;
            var rollInputDegrees = _rollInputSignal.State;
            var rollOutputRefAngleDegrees = rollInputDegrees;

            var rollSinOutputValue = 10.0000 * Math.Sin(rollOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);
            var rollCosOutputValue = 10.0000 * Math.Cos(rollOutputRefAngleDegrees * Constants.RADIANS_PER_DEGREE);

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

            if (_rollCosOutputSignal == null) return;
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