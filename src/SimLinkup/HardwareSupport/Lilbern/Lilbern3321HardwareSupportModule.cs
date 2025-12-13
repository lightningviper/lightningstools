using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Lilbern
{
    //Lilbern 3321 F-16 RPM Indicator
    public class Lilbern3321HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ITachometer _renderer = new Tachometer();

        private bool _isDisposed;
        private AnalogSignal _rpmInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _rpmInputSignalChangedEventHandler;
        private AnalogSignal _rpmSinOutputSignal;
        private AnalogSignal _rpmCosOutputSignal;

        private Lilbern3321HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] { _rpmInputSignal };

        public override AnalogSignal[] AnalogOutputs => new[] { _rpmSinOutputSignal, _rpmCosOutputSignal };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Lilbern M/N 3321 - Indicator, Simulated Tachometer";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Lilbern3321HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Lilbern3321HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.RPMPercent = (float)_rpmInputSignal.State;
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
            _rpmSinOutputSignal = CreateRPMSinOutputSignal();
            _rpmCosOutputSignal = CreateRPMCosOutputSignal();
        }

        private AnalogSignal CreateRPMInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "RPM",
                Id = "3321_RPM_From_Sim",
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

        private AnalogSignal CreateRPMSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM (SIN)",
                Id = "3321_RPM_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0, //volts
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }
        private AnalogSignal CreateRPMCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "RPM (COS)",
                Id = "3321_RPM_COS_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = +10.00, //volts
                IsVoltage = true,
                IsCosine = true,
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
                /*
                 * This code is donated from another module that computed +/-10V linear output values directly.
                 * Adaptation below converts +/-10V values to percentage of full-scale range, then converts
                 * that to an angle, and then back to the corresponding SIN/COS +/-10V output signal pair.
                 */
                var rpmInput = _rpmInputSignal.State;
                double rpmOutputValueInLinearVoltageTerms = 0;
                if (_rpmSinOutputSignal != null && _rpmCosOutputSignal != null)
                {
                    if (rpmInput < 20)
                    {
                        rpmOutputValueInLinearVoltageTerms = Math.Max(-10, -10.0 + rpmInput / 20.0 * 1.89);
                    }
                    else if (rpmInput >= 20 && rpmInput < 40)
                    {
                        rpmOutputValueInLinearVoltageTerms = -8.11 + (rpmInput - 10) / 20.0 * 1.88;
                    }
                    else if (rpmInput >= 40 && rpmInput < 60)
                    {
                        rpmOutputValueInLinearVoltageTerms = -6.23 + (rpmInput - 40) / 20.0 * 1.89;
                    }
                    else if (rpmInput >= 60 && rpmInput < 70)
                    {
                        rpmOutputValueInLinearVoltageTerms = -4.35 + (rpmInput - 60) / 10.0 * 2.86;
                    }
                    else if (rpmInput >= 70 && rpmInput < 76)
                    {
                        rpmOutputValueInLinearVoltageTerms = -1.48 + (rpmInput - 70) / 6.0 * 1.48;
                    }
                    else if (rpmInput >= 76 && rpmInput < 80)
                    {
                        rpmOutputValueInLinearVoltageTerms = 0.00 + (rpmInput - 76) / 4.0 * 1.39;
                    }
                    else if (rpmInput >= 80 && rpmInput < 90)
                    {
                        rpmOutputValueInLinearVoltageTerms = 1.39 + (rpmInput - 80) / 10.0 * 2.87;
                    }
                    else if (rpmInput >= 90 && rpmInput < 100)
                    {
                        rpmOutputValueInLinearVoltageTerms = 4.26 + (rpmInput - 90) / 10.0 * 2.87;
                    }
                    else if (rpmInput >= 100)
                    {
                        rpmOutputValueInLinearVoltageTerms = 7.13 + (rpmInput - 100) / 10.0 * 2.87;
                    }

                    if (rpmOutputValueInLinearVoltageTerms < -10)
                    {
                        rpmOutputValueInLinearVoltageTerms = -10;
                    }
                    else if (rpmOutputValueInLinearVoltageTerms > 10)
                    {
                        rpmOutputValueInLinearVoltageTerms = 10;
                    }


                    var percentOfScale = (rpmOutputValueInLinearVoltageTerms + 10.00) / 20.00;
                    var degrees = percentOfScale * 360.00;
                    var sinRaw = Math.Sin(degrees * Constants.RADIANS_PER_DEGREE);
                    var cosRaw = Math.Cos(degrees * Constants.RADIANS_PER_DEGREE);
                    var sinVoltage = sinRaw * 10.00;
                    var cosVoltage = cosRaw * 10.00;

                    if (_rpmSinOutputSignal != null)
                    {
                        if (sinVoltage > 10.00)
                        {
                            sinVoltage = 10.00;
                        }
                        else if (sinVoltage < -10.00)
                        {
                            sinVoltage = -10.00;
                        }
                        _rpmSinOutputSignal.State = sinVoltage;
                    }

                    if (_rpmCosOutputSignal != null)
                    {
                        if (cosVoltage > 10.00)
                        {
                            cosVoltage = 10.00;
                        }
                        else if (cosVoltage < -10.00)
                        {
                            cosVoltage = -10.00;
                        }
                        _rpmCosOutputSignal.State = cosVoltage;
                    }
               }
            }
        }
    }
}