using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0216 F-16 FTIT Indicator
    public class Simtek100216HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IFanTurbineInletTemperature _renderer = new FanTurbineInletTemperature();

        private AnalogSignal _ftitInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _ftitInputSignalChangedEventHandler;
        private AnalogSignal _ftitOutputSignal;

        private bool _isDisposed;

        private Simtek100216HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_ftitInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_ftitOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0216 - Indicator, Simulated FTIT";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100216HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek100216HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.InletTemperatureDegreesCelcius = (float) _ftitInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _ftitInputSignalChangedEventHandler = null;
        }

        private AnalogSignal CreateFTITInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "FTIT",
                Id = "100216_FTIT_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 1200
            };
            return thisSignal;
        }

        private AnalogSignal CreateFTITOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT",
                Id = "100216_FTIT_To_Instrument",
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
            _ftitInputSignalChangedEventHandler =
                ftit_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _ftitInputSignal = CreateFTITInputSignal();
        }

        private void CreateOutputSignals()
        {
            _ftitOutputSignal = CreateFTITOutputSignal();
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

        private void ftit_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void RegisterForInputEvents()
        {
            if (_ftitInputSignal != null)
            {
                _ftitInputSignal.SignalChanged += _ftitInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_ftitInputSignalChangedEventHandler != null && _ftitInputSignal != null)
            {
                try
                {
                    _ftitInputSignal.SignalChanged -= _ftitInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_ftitInputSignal != null)
            {
                var ftitInput = _ftitInputSignal.State;
                double ftitOutputValue = 0;
                if (_ftitOutputSignal != null)
                {
                    if (ftitInput <= 200)
                    {
                        ftitOutputValue = -10;
                    }
                    else if (ftitInput >= 200 && ftitInput < 700)
                    {
                        ftitOutputValue = -10 + (ftitInput - 200) / 500 * 6.25;
                    }
                    else if (ftitInput >= 700 && ftitInput < 1000)
                    {
                        ftitOutputValue = -3.75 + (ftitInput - 700) / 300.0 * 11.25;
                    }
                    else if (ftitInput >= 1000 && ftitInput < 1200)
                    {
                        ftitOutputValue = 7.5 + (ftitInput - 1000) / 200.0 * 2.5;
                    }
                    else if (ftitInput >= 1200)
                    {
                        ftitOutputValue = 10;
                    }


                    if (ftitOutputValue < -10)
                    {
                        ftitOutputValue = -10;
                    }
                    else if (ftitOutputValue > 10)
                    {
                        ftitOutputValue = 10;
                    }
                    _ftitOutputSignal.State = ftitOutputValue;
                }
            }
        }
    }
}