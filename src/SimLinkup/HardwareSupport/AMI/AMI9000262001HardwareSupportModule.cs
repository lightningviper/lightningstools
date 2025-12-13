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
    //AMI 90002620-01 F-16 Cabin Pressure Altimeter
    public class AMI9000262001HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator();

        private AnalogSignal _cabinPressureAltitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressureAltitudeInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressureAltitudeOutputSignal;

        private bool _isDisposed;

        private AMI9000262001HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] { _cabinPressureAltitudeInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[]
            { _cabinPressureAltitudeOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "AMI P/N 90002620-01 - Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AMI9000262001HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new AMI9000262001HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float)_cabinPressureAltitudeInputSignal.State;
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

        private AnalogSignal CreateCabinPressureAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "9000262001_Cabin_Pressure_Altitude_From_Sim",
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
                FriendlyName = "Cabin Pressure Altitude",
                Id = "9000262001_Cabin_Pressure_Altitude_To_Instrument",
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
            _cabinPressureAltitudeOutputSignal = CreateCabinPressureAltitudeSinOutputSignal();
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

                var cabinPressureAltitudeOutputValue = ((degrees / 360.00) * 20.00) - 10.00;

                if (_cabinPressureAltitudeOutputSignal != null)
                {
                    if (cabinPressureAltitudeOutputValue < -10)
                    {
                        cabinPressureAltitudeOutputValue = -10;
                    }
                    else if (cabinPressureAltitudeOutputValue > 10)
                    {
                        cabinPressureAltitudeOutputValue = 10;
                    }

                    _cabinPressureAltitudeOutputSignal.State = cabinPressureAltitudeOutputValue;
                }
            }
        }
    }
}