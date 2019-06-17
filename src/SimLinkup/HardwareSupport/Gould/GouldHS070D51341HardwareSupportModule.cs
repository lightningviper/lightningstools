using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Gould
{
    //GOULD F-16 COMPASS
    public class GouldHS070D51341HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ICompass _compass = new Compass();
        private AnalogSignal _compassCOSOutputSignal;
        private AnalogSignal _compassInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _compassInputSignalChangedEventHandler;
        private AnalogSignal _compassSINOutputSignal;

        private bool _isDisposed;

        private GouldHS070D51341HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_compassInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_compassSINOutputSignal, _compassCOSOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Gould P/N HS070D5134-1 - Standby Compass";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GouldHS070D51341HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new GouldHS070D51341HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _compass.InstrumentState.MagneticHeadingDegrees = (float) _compassInputSignal.State;
            _compass.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _compassInputSignalChangedEventHandler = null;
        }

        private void compass_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private AnalogSignal CreateCompassCOSOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (COS)",
                Id = "HS070D51341_Compass__COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00, //volts
                IsVoltage = true,
                IsCosine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCompassInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Magnetic Heading (Degrees)",
                Id = "HS070D51341_Compass__Magnetic_Heading_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsAngle = true,
                MinValue = 0,
                MaxValue = 360
            };

            return thisSignal;
        }

        private AnalogSignal CreateCompassSINOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Compass (SIN)",
                Id = "HS070D51341_Compass__SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _compassInputSignalChangedEventHandler =
                compass_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _compassInputSignal = CreateCompassInputSignal();
        }

        private void CreateOutputSignals()
        {
            _compassSINOutputSignal = CreateCompassSINOutputSignal();
            _compassCOSOutputSignal = CreateCompassCOSOutputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    Common.Util.DisposeObject(_compass);
                }
            }
            _isDisposed = true;
        }

        private void RegisterForInputEvents()
        {
            if (_compassInputSignal != null)
            {
                _compassInputSignal.SignalChanged += _compassInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_compassInputSignalChangedEventHandler != null && _compassInputSignal != null)
            {
                try
                {
                    _compassInputSignal.SignalChanged -= _compassInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_compassInputSignal == null) return;
            var compassInput = Math.Abs(_compassInputSignal.State % 360.000);
            if (_compassSINOutputSignal != null)
            {
                var compassSINOutputValue = 10.0000 * Math.Sin(compassInput * Constants.RADIANS_PER_DEGREE);

                if (compassSINOutputValue < -10)
                {
                    compassSINOutputValue = -10;
                }
                else if (compassSINOutputValue > 10)
                {
                    compassSINOutputValue = 10;
                }

                _compassSINOutputSignal.State = compassSINOutputValue;
            }

            if (_compassCOSOutputSignal == null) return;
            var compassCOSOutputValue = 10.0000 * Math.Cos(compassInput * Constants.RADIANS_PER_DEGREE);

            if (compassCOSOutputValue < -10)
            {
                compassCOSOutputValue = -10;
            }
            else if (compassCOSOutputValue > 10)
            {
                compassCOSOutputValue = 10;
            }

            _compassCOSOutputSignal.State = compassCOSOutputValue;
        }
    }
}