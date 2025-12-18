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
                if (_rpmSinOutputSignal != null && _rpmCosOutputSignal != null)
                {
                    var degrees = 0.00;
                    var sixtyRpmDegrees = 90;
                    var oneHundredTenRpmDegrees = 330;
                    if (rpmInput < 60)
                    {
                        degrees = (rpmInput / 60.00) * sixtyRpmDegrees;
                    }
                    else
                    {
                        degrees =
                            (rpmInput - 60.00) //how far past 60 degrees are we
                                /  //as a fraction of
                            (110 - 60) //the width of the range in RPM from 60-110 RPM on the dial face
                            * (oneHundredTenRpmDegrees - sixtyRpmDegrees) //times the distance in degrees represented by that span
                            + sixtyRpmDegrees; // plus the offset for the sixty-RPM mark in degrees
                    }

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