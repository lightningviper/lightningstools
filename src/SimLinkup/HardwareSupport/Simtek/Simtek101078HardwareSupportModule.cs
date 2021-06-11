using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1078 F-16 Cabin Pressure Altimeter 
    public class Simtek101078HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ICabinPressureAltitudeIndicator _renderer = new CabinPressureAltitudeIndicator()
        {

        };
        private bool _isDisposed;
        private AnalogSignal _cabinPressInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _cabinPressInputSignalChangedEventHandler;
        private AnalogSignal _cabinPressOutputSignal;

        private Simtek101078HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_cabinPressInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_cabinPressOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1078 - Indicator, Simulated Cabin Pressure Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101078HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101078HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.CabinPressureAltitudeFeet = (float) _cabinPressInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _cabinPressInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _cabinPressInputSignalChangedEventHandler = cabinPressureAlt_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _cabinPressInputSignal = CreateCabinPressureAltInputSignal();
        }

        private void CreateOutputSignals()
        {
            _cabinPressOutputSignal = CreateCabinPressureAltOutputSignal();
        }

        private AnalogSignal CreateCabinPressureAltInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Cabin Pressure Altitude",
                Id = "101078_CabinAlt_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                IsPercentage = true,
                State = 0,
                MinValue = 0,
                MaxValue = 110
            };
            return thisSignal;
        }

        private AnalogSignal CreateCabinPressureAltOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Cabin Alt",
                Id = "101078_CabinAlt_To_Instrument",
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
            if (_cabinPressInputSignal != null)
            {
                _cabinPressInputSignal.SignalChanged += _cabinPressInputSignalChangedEventHandler;
            }
        }

        private void cabinPressureAlt_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_cabinPressInputSignalChangedEventHandler != null && _cabinPressInputSignal != null)
            {
                try
                {
                    _cabinPressInputSignal.SignalChanged -= _cabinPressInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_cabinPressInputSignal != null)
            {
                var cabinPressInput = _cabinPressInputSignal.State;
                double cabinPressOutputValue = 0;
                if (_cabinPressOutputSignal != null)
                {
                    if (cabinPressInput < 5000)
                    {
                        cabinPressOutputValue = Math.Max(-10, -10.0 + cabinPressInput / 5000.0 * 2.00);
                    }
                    else if (cabinPressInput >= 5000 && cabinPressInput < 10000)
                    {
                        cabinPressOutputValue = -8.00 + (cabinPressInput - 5000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 10000 && cabinPressInput < 15000)
                    {
                        cabinPressOutputValue = -6.00 + (cabinPressInput - 10000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 15000 && cabinPressInput < 20000)
                    {
                        cabinPressOutputValue = -4.00 + (cabinPressInput - 15000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 20000 && cabinPressInput < 25000)
                    {
                        cabinPressOutputValue = -2.00 + (cabinPressInput - 20000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 25000 && cabinPressInput < 30000)
                    {
                        cabinPressOutputValue = 0.00 + (cabinPressInput - 25000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 30000 && cabinPressInput < 35000)
                    {
                        cabinPressOutputValue = 2.00 + (cabinPressInput - 30000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 35000 && cabinPressInput < 40000)
                    {
                        cabinPressOutputValue = 4.00 + (cabinPressInput - 35000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 40000 && cabinPressInput < 45000)
                    {
                        cabinPressOutputValue = 6.00 + (cabinPressInput - 40000) / 5000.0 * 2.00;
                    }
                    else if (cabinPressInput >= 45000 && cabinPressInput < 50000)
                    {
                        cabinPressOutputValue = Math.Min(10, 8.00 + Math.Min(1, (cabinPressInput - 45000) / 5000.0) * 2.00);
                    }

                    if (cabinPressOutputValue < -10)
                    {
                        cabinPressOutputValue = -10;
                    }
                    else if (cabinPressOutputValue > 10)
                    {
                        cabinPressOutputValue = 10;
                    }
                    _cabinPressOutputSignal.State = cabinPressOutputValue;
                }
            }
        }
    }
}