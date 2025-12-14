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
    //Malwin 1956-2 F-16 FTIT Indicator 
    public class Malwin19562HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IFanTurbineInletTemperature _renderer = new FanTurbineInletTemperature();

        private AnalogSignal _ftitInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _ftitInputSignalChangedEventHandler;
        private AnalogSignal _ftitSinOutputSignal;
        private AnalogSignal _ftitCosOutputSignal;

        private bool _isDisposed;

        private Malwin19562HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] { _ftitInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _ftitSinOutputSignal, _ftitCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Malwin P/N 1956-2  - Indicator, Simulated FTIT";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Malwin19562HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Malwin19562HardwareSupportModule ()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.InletTemperatureDegreesCelcius = (float)_ftitInputSignal.State;
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
                Id = "19562_FTIT_From_Sim",
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

        private AnalogSignal CreateFTITSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT (SIN)",
                Id = "19562_FTIT_SIN_To_Instrument",
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

        private AnalogSignal CreateFTITCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "FTIT (COS)",
                Id = "19562_FTIT_COS_To_Instrument",
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
            _ftitInputSignalChangedEventHandler =
                ftit_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _ftitInputSignal = CreateFTITInputSignal();
        }

        private void CreateOutputSignals()
        {
            _ftitSinOutputSignal = CreateFTITSinOutputSignal();
            _ftitCosOutputSignal = CreateFTITCosOutputSignal();
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
                double ftitOutputDegrees = 0;
                ftitInput = 1150;
                if (ftitInput < 0.00)
                {
                    ftitInput = 0;
                }
                else if (ftitInput > 1200.00)
                {
                    ftitInput = 1200.00;
                }

                if (_ftitSinOutputSignal != null && _ftitCosOutputSignal != null)
                {
                    if (ftitInput <= 200)
                    {
                        ftitOutputDegrees = 0;
                    }
                    else if (ftitInput >= 200 && ftitInput < 700)
                    {
                        ftitOutputDegrees = 20.00 * ((ftitInput - 200.00) / 100.00); // 20 degrees of angle per 100 Degrees C
                    }
                    else if (ftitInput >= 700 && ftitInput < 1000)
                    {
                        ftitOutputDegrees = (60.00 * ((ftitInput - 700.00) / 100.00)) + 100.00; // 60 degrees of angle per 100 Degrees C, starting at 100 degrees of angle
                    }
                    else if (ftitInput >= 1000 && ftitInput <= 1200)
                    {
                        ftitOutputDegrees = (20.00 * ((ftitInput - 1000.00) / 100.00)) + 280; // 20 degrees of angle per 100 Degrees C, starting at 280 degrees of angle
                    }

                    var ftitOutputSinVoltage = Math.Sin(ftitOutputDegrees * Constants.RADIANS_PER_DEGREE);
                    if (ftitOutputSinVoltage < -10)
                    {
                        ftitOutputSinVoltage = -10;
                    }
                    else if (ftitOutputSinVoltage > 10)
                    {
                        ftitOutputSinVoltage = 10;
                    }
                    _ftitSinOutputSignal.State = ftitOutputSinVoltage;

                    var ftitOutputCosVoltage = Math.Cos(ftitOutputDegrees * Constants.RADIANS_PER_DEGREE);
                    if (ftitOutputCosVoltage < -10)
                    {
                        ftitOutputCosVoltage = -10;
                    }
                    else if (ftitOutputCosVoltage > 10)
                    {
                        ftitOutputCosVoltage = 10;
                    }
                    _ftitCosOutputSignal.State = ftitOutputCosVoltage;
                }
            }
        }
    }
}
