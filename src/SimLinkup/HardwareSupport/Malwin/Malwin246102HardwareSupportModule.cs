using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Malwin
{
    //Malwin 246102 F-16 Cabin Pressure Altimeter
    public class Malwin246102HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator();

        private AnalogSignal _cabinPressureAltitudeCosOutputSignal;
        private AnalogSignal _cabinPressureAltitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressureAltitudeInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressureAltitudeSinOutputSignal;

        private bool _isDisposed;

        private Malwin246102HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_cabinPressureAltitudeInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_cabinPressureAltitudeSinOutputSignal, _cabinPressureAltitudeCosOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 246102 - Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin246102HardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Malwin246102HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float) _cabinPressureAltitudeInputSignal.State;
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

        private AnalogSignal CreateCabinPressureAltitudeCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Cabin Pressure Altitude (COS)",
                Id = "246102_Cabin_Pressure_Altitude_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateCabinPressureAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "246102_Cabin_Pressure_Altitude_From_Sim",
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
                FriendlyName = "Cabin Pressure Altitude (SIN)",
                Id = "246102_Cabin_Pressure_Altitude_SIN_To_Instrument",
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
            _cabinPressureAltitudeSinOutputSignal = CreateCabinPressureAltitudeSinOutputSignal();
            _cabinPressureAltitudeCosOutputSignal = CreateCabinPressureAltitudeCosOutputSignal();
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
            if (_cabinPressureAltitudeInputSignal != null)
            {
                var cabinPressureAltitudeInput = _cabinPressureAltitudeInputSignal.State;


                var degrees = cabinPressureAltitudeInput < 0.0000
                    ? 0.0000
                    : (cabinPressureAltitudeInput >= 0 && cabinPressureAltitudeInput <= 50000.0000
                        ? cabinPressureAltitudeInput / 50000.0000 * 300.0000
                        : 300.0);

                var cabinPressureAltitudeSinOutputValue = 10.0000 * Math.Sin(degrees * Constants.RADIANS_PER_DEGREE);
                var cabinPressureAltitudeCosOutputValue = 10.0000 * Math.Cos(degrees * Constants.RADIANS_PER_DEGREE);


                if (_cabinPressureAltitudeSinOutputSignal != null)
                {
                    if (cabinPressureAltitudeSinOutputValue < -10)
                    {
                        cabinPressureAltitudeSinOutputValue = -10;
                    }
                    else if (cabinPressureAltitudeSinOutputValue > 10)
                    {
                        cabinPressureAltitudeSinOutputValue = 10;
                    }

                    _cabinPressureAltitudeSinOutputSignal.State = cabinPressureAltitudeSinOutputValue;
                }

                if (_cabinPressureAltitudeCosOutputSignal == null) return;
                if (cabinPressureAltitudeCosOutputValue < -10)
                {
                    cabinPressureAltitudeCosOutputValue = -10;
                }
                else if (cabinPressureAltitudeCosOutputValue > 10)
                {
                    cabinPressureAltitudeCosOutputValue = 10;
                }

                _cabinPressureAltitudeCosOutputSignal.State = cabinPressureAltitudeCosOutputValue;
            }
        }
    }
}