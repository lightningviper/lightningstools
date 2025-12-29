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
    //Malwin 1956-3 F-16 Liquid Oxygen Quantity Indicator 
    public class Malwin19563HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        //There are currently No LOX QTY outputs from BMS, so there is no renderer in LightningGauges for this
        //private readonly ILiquidOxygenQuantity _renderer = new LiquidOxygenQuantity();

        private AnalogSignal _loxQtyInputSignal; //reserved for future use
        private AnalogSignal.AnalogSignalChangedEventHandler _loxQtyInputSignalChangedEventHandler; //reserved for future use
        private AnalogSignal _loxQtySinOutputSignal;
        private AnalogSignal _loxQtyCosOutputSignal;

        private bool _isDisposed;

        private Malwin19563HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
            UpdateOutputValues();
        }

        public override AnalogSignal[] AnalogInputs => new[] { _loxQtyInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _loxQtySinOutputSignal, _loxQtyCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 1956-3  - Indicator, Simulated Liquid Oxygen Quantity";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin19563HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Malwin19563HardwareSupportModule ()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            //_renderer.InstrumentState.InletTemperatureDegreesCelcius = (float)_loxQtyInputSignal.State;
            //_renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _loxQtyInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateLoxQtyInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "LOX QTY",
                Id = "19563_LOX_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = (new Random().NextDouble() * 2.00) + 3.00,
                MinValue = 0,
                MaxValue = 5
            };
            return thisSignal;
        }

        private AnalogSignal CreateLoxQtySinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "LOX QTY (SIN)",
                Id = "19563_LOX_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0, //volts
                IsSine = true,
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateLoxQtyCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "LOX QTY (COS)",
                Id = "19563_LOX_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 10.00, //volts
                IsCosine = true,
                IsVoltage = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _loxQtyInputSignalChangedEventHandler =
                loxQty_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _loxQtyInputSignal = CreateLoxQtyInputSignal();
        }

        private void CreateOutputSignals()
        {
            _loxQtySinOutputSignal = CreateLoxQtySinOutputSignal();
            _loxQtyCosOutputSignal = CreateLoxQtyCosOutputSignal();
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnregisterForInputEvents();
                    AbandonInputEventHandlers();
                    //Common.Util.DisposeObject(_renderer);
                }
            }
            _isDisposed = true;
        }

        private void loxQty_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        public override void Synchronize()
        {
            base.Synchronize();
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_loxQtyInputSignal != null)
            {
                _loxQtyInputSignal.SignalChanged += _loxQtyInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_loxQtyInputSignalChangedEventHandler != null && _loxQtyInputSignal != null)
            {
                try
                {
                    _loxQtyInputSignal.SignalChanged -= _loxQtyInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_loxQtyInputSignal != null)
            {
                var loxQtyInput = _loxQtyInputSignal.State;
                double loxQtyOutputDegrees = 0;
                if (loxQtyInput < 0.00)
                {
                    loxQtyInput = 0;
                }
                else if (loxQtyInput > 5.00)
                {
                    loxQtyInput = 5.00;
                }

                if (_loxQtySinOutputSignal != null && _loxQtyCosOutputSignal != null)
                {
                    loxQtyOutputDegrees = (loxQtyInput / 5.00) * 180.00; // 180 degrees of angle = 5 liters

                    var loxQtyOutputSinVoltage = Math.Sin(loxQtyOutputDegrees * Constants.RADIANS_PER_DEGREE);
                    if (loxQtyOutputSinVoltage < -10)
                    {
                        loxQtyOutputSinVoltage = -10;
                    }
                    else if (loxQtyOutputSinVoltage > 10)
                    {
                        loxQtyOutputSinVoltage = 10;
                    }
                    _loxQtySinOutputSignal.State = loxQtyOutputSinVoltage;

                    var loxQtyOutputCosVoltage = Math.Cos(loxQtyOutputDegrees * Constants.RADIANS_PER_DEGREE);
                    if (loxQtyOutputCosVoltage < -10)
                    {
                        loxQtyOutputCosVoltage = -10;
                    }
                    else if (loxQtyOutputCosVoltage > 10)
                    {
                        loxQtyOutputCosVoltage = 10;
                    }
                    _loxQtyCosOutputSignal.State = loxQtyOutputCosVoltage;
                }
            }
        }
    }
}