using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1082 F-16 Mach/Airspeed Indicator
    public class Simtek101082HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IAirspeedIndicator _renderer = new AirspeedIndicator();

        private AnalogSignal _airspeedInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _airspeedInputSignalChangedEventHandler;
        private AnalogSignal _airspeedOutputSignal;
        private bool _isDisposed;
        private AnalogSignal _machInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _machInputSignalChangedEventHandler;
        private AnalogSignal _machOutputSignal;

        private Simtek101082HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
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
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101082HardwareSupportModule()
            };

            return toReturn.ToArray();
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
            var airspeedInput = _airspeedInputSignal.State;
            double airspeedOutputValue = 0;
            if (_airspeedOutputSignal == null) return;
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
            var machInput = _machInputSignal.State;
            double machOutputValue = 0;
            if (_machOutputSignal == null) return;
            if (machInput < 0)
            {
                machOutputValue = -10;
            }
            else if (machInput >= 0 && machInput < 0.50)
            {
                machOutputValue = -10.0 + machInput / 0.50 * 2.44;
            }
            else if (machInput >= 0.50 && machInput < 0.55)
            {
                machOutputValue = -7.56 + (machInput - 0.55) / 0.05 * 1.00;
            }
            else if (machInput >= 0.55 && machInput < 0.60)
            {
                machOutputValue = -6.56 + (machInput - 0.55) / 0.05 * 0.91;
            }
            else if (machInput >= 0.60 && machInput < 0.65)
            {
                machOutputValue = -5.65 + (machInput - 0.60) / 0.05 * 0.84;
            }
            else if (machInput >= 0.65 && machInput < 0.70)
            {
                machOutputValue = -4.81 + (machInput - 0.65) / 0.05 * 0.80;
            }
            else if (machInput >= 0.70 && machInput < 0.75)
            {
                machOutputValue = -4.01 + (machInput - 0.70) / 0.05 * 0.77;
            }
            else if (machInput >= 0.75 && machInput < 0.80)
            {
                machOutputValue = -3.24 + (machInput - 0.75) / 0.05 * 0.72;
            }
            else if (machInput >= 0.80 && machInput < 0.85)
            {
                machOutputValue = -2.52 + (machInput - 0.80) / 0.05 * 0.69;
            }
            else if (machInput >= 0.85 && machInput < 0.90)
            {
                machOutputValue = -1.83 + (machInput - 0.85) / 0.05 * 0.65;
            }
            else if (machInput >= 0.90 && machInput < 0.95)
            {
                machOutputValue = -1.18 + (machInput - 0.90) / 0.05 * 0.61;
            }
            else if (machInput >= 0.95 && machInput < 1.00)
            {
                machOutputValue = -0.57 + (machInput - 0.95) / 0.05 * 0.57;
            }
            else if (machInput >= 1.00 && machInput < 1.05)
            {
                machOutputValue = 0.00 + (machInput - 1.00) / 0.05 * 0.53;
            }
            else if (machInput >= 1.05 && machInput < 1.10)
            {
                machOutputValue = 0.53 + (machInput - 1.05) / 0.05 * 0.54;
            }
            else if (machInput >= 1.10 && machInput < 1.15)
            {
                machOutputValue = 1.07 + (machInput - 1.10) / 0.05 * 0.49;
            }
            else if (machInput >= 1.15 && machInput < 1.20)
            {
                machOutputValue = 1.56 + (machInput - 1.15) / 0.05 * 0.5;
            }
            else if (machInput >= 1.20 && machInput < 1.25)
            {
                machOutputValue = 2.06 + (machInput - 1.20) / 0.05 * 0.46;
            }
            else if (machInput >= 1.25 && machInput < 1.30)
            {
                machOutputValue = 2.52 + (machInput - 1.25) / 0.05 * 0.46;
            }
            else if (machInput >= 1.30 && machInput < 1.35)
            {
                machOutputValue = 2.98 + (machInput - 1.30) / 0.05 * 0.45;
            }
            else if (machInput >= 1.35 && machInput < 1.40)
            {
                machOutputValue = 3.43 + (machInput - 1.35) / 0.05 * 0.42;
            }
            else if (machInput >= 1.40 && machInput < 1.45)
            {
                machOutputValue = 3.85 + (machInput - 1.40) / 0.05 * 0.42;
            }
            else if (machInput >= 1.45 && machInput < 1.50)
            {
                machOutputValue = 4.27 + (machInput - 1.45) / 0.05 * 0.42;
            }
            else if (machInput >= 1.50 && machInput < 1.55)
            {
                machOutputValue = 4.69 + (machInput - 1.50) / 0.05 * 0.39;
            }
            else if (machInput >= 1.55 && machInput < 1.60)
            {
                machOutputValue = 5.08 + (machInput - 1.55) / 0.05 * 0.38;
            }
            else if (machInput >= 1.60 && machInput < 1.65)
            {
                machOutputValue = 5.46 + (machInput - 1.60) / 0.05 * 0.38;
            }
            else if (machInput >= 1.65 && machInput < 1.70)
            {
                machOutputValue = 5.84 + (machInput - 1.65) / 0.05 * 0.34;
            }
            else if (machInput >= 1.70 && machInput < 1.75)
            {
                machOutputValue = 6.18 + (machInput - 1.70) / 0.05 * 0.35;
            }
            else if (machInput >= 1.75 && machInput < 1.80)
            {
                machOutputValue = 6.53 + (machInput - 1.75) / 0.05 * 0.34;
            }
            else if (machInput >= 1.80 && machInput < 1.85)
            {
                machOutputValue = 6.87 + (machInput - 1.80) / 0.05 * 0.30;
            }
            else if (machInput >= 1.85 && machInput < 1.90)
            {
                machOutputValue = 7.17 + (machInput - 1.85) / 0.05 * 0.31;
            }
            else if (machInput >= 1.90 && machInput < 1.95)
            {
                machOutputValue = 7.48 + (machInput - 1.90) / 0.05 * 0.31;
            }
            else if (machInput >= 1.95 && machInput < 2.00)
            {
                machOutputValue = 7.79 + (machInput - 1.95) / 0.05 * 0.26;
            }
            else if (machInput >= 2.00 && machInput < 2.05)
            {
                machOutputValue = 8.05 + (machInput - 2.00) / 0.05 * 0.27;
            }
            else if (machInput >= 2.05 && machInput < 2.10)
            {
                machOutputValue = 8.32 + (machInput - 2.05) / 0.05 * 0.27;
            }
            else if (machInput >= 2.10 && machInput < 2.15)
            {
                machOutputValue = 8.59 + (machInput - 2.10) / 0.05 * 0.26;
            }
            else if (machInput >= 2.15 && machInput < 2.20)
            {
                machOutputValue = 8.85 + (machInput - 2.15) / 0.05 * 0.23;
            }
            else if (machInput >= 2.20 && machInput < 2.40)
            {
                machOutputValue = 9.08 + (machInput - 2.20) / 0.20 * 0.92;
            }
            else if (machInput >= 2.4)
            {
                machOutputValue = 10;
            }

            if (machOutputValue < -10)
            {
                machOutputValue = -10;
            }
            else if (machOutputValue > 10)
            {
                machOutputValue = 10;
            }

            _machOutputSignal.State = machOutputValue;
        }
    }
}