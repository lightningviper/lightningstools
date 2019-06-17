using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0295 F-16 Simulated Fuel Flow Indicator
    public class Simtek100295HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IFuelFlow _renderer = new FuelFlow();
        private AnalogSignal _fuelFlowInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _fuelFlowInputSignalChangedEventHandler;
        private AnalogSignal _fuelFlowOutputSignal;

        private bool _isDisposed;

        private Simtek100295HardwareSupportModule()
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

        public override string FriendlyName => "Simtek P/N 10-0295 - Simulated Fuel Flow Indicator";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100295HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek100295HardwareSupportModule()
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
                Id = "100295_Fuel_Flow_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 99900
            };
            return thisSignal;
        }

        private AnalogSignal CreateFuelFlowOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Fuel Flow",
                Id = "100295_Fuel_Flow_To_Instrument",
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
            _fuelFlowInputSignalChangedEventHandler =
                fuelFlow_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _fuelFlowInputSignal = CreateFuelFlowInputSignal();
        }


        private void CreateOutputSignals()
        {
            _fuelFlowOutputSignal = CreateFuelFlowOutputSignal();
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
            UpdateOutputValues();
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
            if (_fuelFlowInputSignalChangedEventHandler == null || _fuelFlowInputSignal == null) return;
            try
            {
                _fuelFlowInputSignal.SignalChanged -= _fuelFlowInputSignalChangedEventHandler;
            }
            catch (RemotingException)
            {
            }
        }

        private void UpdateOutputValues()
        {
            if (_fuelFlowOutputSignal == null) return;
            var fuelFlow = _fuelFlowInputSignal.State;
            if (fuelFlow <= 10000)
            {
                _fuelFlowOutputSignal.State = Math.Min(_fuelFlowInputSignal.State / 9900.00, 1.00) * 20.00 - 10.00;
            }
            else if (fuelFlow >= 10000 && fuelFlow < 80000)
            {
                _fuelFlowOutputSignal.State = _fuelFlowInputSignal.State / 99000.00 * 20.00 - 10.00;
            }
            else if (fuelFlow >= 80000)
            {
                _fuelFlowOutputSignal.State = 8000 / 9900.00 * 20.00 - 10.00;
            }
        }
    }
}