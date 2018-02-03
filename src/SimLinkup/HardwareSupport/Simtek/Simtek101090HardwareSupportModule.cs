using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1090 F-16 EPU FUEL QTY IND
    public class Simtek101090HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IEPUFuelGauge _renderer = new EPUFuelGauge();

        private AnalogSignal _epuFuelPercentageInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _epuFuelPercentageInputSignalChangedEventHandler;
        private AnalogSignal _epuFuelPercentageOutputSignal;

        private bool _isDisposed;

        private Simtek101090HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_epuFuelPercentageInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_epuFuelPercentageOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1090 - EPU Fuel Quantity Ind";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101090HardwareSupportModule()
        {
            Dispose();
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101090HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.FuelRemainingPercent = (float) _epuFuelPercentageInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _epuFuelPercentageInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateEPUInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "101090_EPU_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };

            return thisSignal;
        }

        private AnalogSignal CreateEPUOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "EPU Fuel Quantity %",
                Id = "101090_EPU_To_Instrument",
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
            _epuFuelPercentageInputSignalChangedEventHandler =
                epu_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _epuFuelPercentageInputSignal = CreateEPUInputSignal();
        }

        private void CreateOutputSignals()
        {
            _epuFuelPercentageOutputSignal = CreateEPUOutputSignal();
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

        private void epu_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_epuFuelPercentageInputSignal != null)
            {
                _epuFuelPercentageInputSignal.SignalChanged += _epuFuelPercentageInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_epuFuelPercentageInputSignalChangedEventHandler != null && _epuFuelPercentageInputSignal != null)
            {
                try
                {
                    _epuFuelPercentageInputSignal.SignalChanged -= _epuFuelPercentageInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_epuFuelPercentageInputSignal == null) return;
            var epuInput = _epuFuelPercentageInputSignal.State;
            if (_epuFuelPercentageOutputSignal == null) return;
            var epuOutputValue = epuInput < 0 ? -10 : (epuInput > 100 ? 10 : epuInput / 100 * 20 - 10);

            if (epuOutputValue < -10)
            {
                epuOutputValue = -10;
            }
            else if (epuOutputValue > 10)
            {
                epuOutputValue = 10;
            }

            _epuFuelPercentageOutputSignal.State = epuOutputValue;
        }
    }
}