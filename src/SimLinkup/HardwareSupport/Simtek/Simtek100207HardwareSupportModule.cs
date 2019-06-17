using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0207 F-16 RPM Indicator
    public class Simtek100207HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ITachometer _renderer = new Tachometer();

        private bool _isDisposed;
        private AnalogSignal _rpmInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rpmInputSignalChangedEventHandler;
        private AnalogSignal _rpmOutputSignal;

        private Simtek100207HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_rpmInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[] {_rpmOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0207 - Indicator, Simulated Tachometer";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100207HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek100207HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.RPMPercent = (float) _rpmInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = null;
        }

        private void CreateInputEventHandlers()
        {
            _rpmInputSignalChangedEventHandler = rpm_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _rpmInputSignal = CreateRPMInputSignal();
        }

        private void CreateOutputSignals()
        {
            _rpmOutputSignal = CreateRPMOutputSignal();
        }

        private AnalogSignal CreateRPMInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "RPM",
                Id = "100207_RPM_From_Sim",
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

        private AnalogSignal CreateRPMOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM",
                Id = "100207_RPM_To_Instrument",
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
            if (_rpmInputSignal != null)
            {
                _rpmInputSignal.SignalChanged += _rpmInputSignalChangedEventHandler;
            }
        }

        private void rpm_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateOutputValues();
        }

        private void UnregisterForInputEvents()
        {
            if (_rpmInputSignalChangedEventHandler != null && _rpmInputSignal != null)
            {
                try
                {
                    _rpmInputSignal.SignalChanged -= _rpmInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateOutputValues()
        {
            if (_rpmInputSignal != null)
            {
                var rpmInput = _rpmInputSignal.State;
                double rpmOutputValue = 0;
                if (_rpmOutputSignal != null)
                {
                    if (rpmInput < 10)
                    {
                        rpmOutputValue = Math.Max(-10, -10.0 + rpmInput / 10.0 * 1.25);
                    }
                    else if (rpmInput >= 10 && rpmInput < 20)
                    {
                        rpmOutputValue = -8.75 + (rpmInput - 10) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 20 && rpmInput < 30)
                    {
                        rpmOutputValue = -7.50 + (rpmInput - 20) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 30 && rpmInput < 40)
                    {
                        rpmOutputValue = -6.25 + (rpmInput - 30) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 40 && rpmInput < 50)
                    {
                        rpmOutputValue = -5.00 + (rpmInput - 40) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 50 && rpmInput < 60)
                    {
                        rpmOutputValue = -3.75 + (rpmInput - 50) / 10.0 * 1.25;
                    }
                    else if (rpmInput >= 60 && rpmInput < 65)
                    {
                        rpmOutputValue = -2.50 + (rpmInput - 60) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 65 && rpmInput < 68)
                    {
                        rpmOutputValue = -0.938 + (rpmInput - 65) / 3.0 * 0.938;
                    }
                    else if (rpmInput >= 68 && rpmInput < 70)
                    {
                        rpmOutputValue = 0.00 + (rpmInput - 68) / 2.0 * 0.625;
                    }
                    else if (rpmInput >= 70 && rpmInput < 75)
                    {
                        rpmOutputValue = 0.625 + (rpmInput - 70) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 75 && rpmInput < 80)
                    {
                        rpmOutputValue = 2.188 + (rpmInput - 75) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 80 && rpmInput < 85)
                    {
                        rpmOutputValue = 3.750 + (rpmInput - 80) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 85 && rpmInput < 90)
                    {
                        rpmOutputValue = 5.313 + (rpmInput - 85) / 5.0 * 1.562;
                    }
                    else if (rpmInput >= 90 && rpmInput < 95)
                    {
                        rpmOutputValue = 6.875 + (rpmInput - 90) / 5.0 * 1.563;
                    }
                    else if (rpmInput >= 95)
                    {
                        rpmOutputValue = Math.Min(10, 8.438 + Math.Min(1, (rpmInput - 95) / 5.0) * 1.562);
                    }

                    if (rpmOutputValue < -10)
                    {
                        rpmOutputValue = -10;
                    }
                    else if (rpmOutputValue > 10)
                    {
                        rpmOutputValue = 10;
                    }
                    _rpmOutputSignal.State = rpmOutputValue;
                }
            }
        }
    }
}