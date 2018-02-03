using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Lilbern
{
    //Lilbern 3239 F-16A Fuel Flow Indicator
    public class Lilbern3239HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IFuelFlow _renderer = new FuelFlow();

        private AnalogSignal _fuelFlowInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _fuelFlowInputSignalChangedEventHandler;
        private AnalogSignal _fuelFlowOutputSignal;
        private bool _isDisposed;

        private Lilbern3239HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_fuelFlowInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_fuelFlowOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Lilbern M/N 3239 - F-16A Fuel Flow Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Lilbern3239HardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Lilbern3239HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.FuelFlowPoundsPerHour = (float) _fuelFlowInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _fuelFlowInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateFuelFlowInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Fuel Flow (pounds per hour)",
                Id = "3239_Fuel_Flow_Pounds_Per_Hour_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 99999
            };
            return thisSignal;
        }

        private AnalogSignal CreateFuelFlowPoundsPerHourOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Fuel Flow (pounds per hour)",
                Id = "3239_Fuel_Flow_Pounds_Per_Hour_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = -10.00, //volts
                IsVoltage = true,
                MinValue = 0,
                MaxValue = 99999
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _fuelFlowInputSignalChangedEventHandler =
                fuelFlow_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _fuelFlowInputSignal = CreateFuelFlowInputSignal();
        }

        private void CreateOutputSignals()
        {
            _fuelFlowOutputSignal = CreateFuelFlowPoundsPerHourOutputSignal();
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

        private void fuelFlow_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateFuelFlowOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_fuelFlowInputSignal != null)
            {
                _fuelFlowInputSignal.SignalChanged += _fuelFlowInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_fuelFlowInputSignalChangedEventHandler != null && _fuelFlowInputSignal != null)
            {
                try
                {
                    _fuelFlowInputSignal.SignalChanged -= _fuelFlowInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateFuelFlowOutputValues()
        {
            if (_fuelFlowInputSignal != null)
            {
                var fuelFlowInput = _fuelFlowInputSignal.State;

                var fuelFlowOutputValue = fuelFlowInput <= 0 ? -10.00 : -10.00 + fuelFlowInput / 80000.0000 * 20.0000;


                if (_fuelFlowOutputSignal == null) return;
                if (fuelFlowOutputValue < -10)
                {
                    fuelFlowOutputValue = -10;
                }
                else if (fuelFlowOutputValue > 10)
                {
                    fuelFlowOutputValue = 10;
                }

                _fuelFlowOutputSignal.State = fuelFlowOutputValue;
            }
        }
    }
}