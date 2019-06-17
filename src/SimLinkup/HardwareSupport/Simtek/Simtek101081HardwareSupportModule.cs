using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-1081 F-16 Altimeter
    public class Simtek101081HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly IAltimeter _renderer = new Altimeter();

        private AnalogSignal _altitudeCoarseOutputSignal;
        private AnalogSignal _altitudeFineCosOutputSignal;
        private AnalogSignal _altitudeFineSinOutputSignal;
        private AnalogSignal _altitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _altitudeInputSignalChangedEventHandler;

        private AnalogSignal _barometricPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _barometricPressureInputSignalChangedEventHandler;
        private bool _isDisposed;

        private Simtek101081HardwareSupportModule()
        {
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_altitudeInputSignal, _barometricPressureInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
            {_altitudeFineSinOutputSignal, _altitudeFineCosOutputSignal, _altitudeCoarseOutputSignal};

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-1081 - Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek101081HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek101081HardwareSupportModule()
            };

            return toReturn.ToArray();
        }

        public override void Render(Graphics g, Rectangle destinationRectangle)
        {
            _renderer.InstrumentState.BarometricPressure = (float) _barometricPressureInputSignal.State * 100;
            _renderer.InstrumentState.IndicatedAltitudeFeetMSL = (float) _altitudeInputSignal.State;
            _renderer.Render(g, destinationRectangle);
        }

        private void AbandonInputEventHandlers()
        {
            _altitudeInputSignalChangedEventHandler = null;
            _barometricPressureInputSignalChangedEventHandler = null;
        }

        private void altitude_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }

        private static void barometricPressure_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateBarometricPressureOutputValues();
        }

        private AnalogSignal CreateAltitudeCoarseSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse",
                Id = "101081_Altitude_Coarse_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (COS)",
                Id = "101081_Altitude_Fine_COS_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (SIN)",
                Id = "101081_Altitude_Fine_SIN_To_Instrument",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0.00, //volts;
                IsVoltage = true,
                IsSine = true,
                MinValue = -10,
                MaxValue = 10
            };
            return thisSignal;
        }

        private AnalogSignal CreateAltitudeInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Altitude (Indicated)",
                Id = "101081_Altitude_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 0,
                MinValue = -1000,
                MaxValue = 80000
            };
            return thisSignal;
        }

        private AnalogSignal CreateBarometricPressureInputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Inputs",
                CollectionName = "Analog Inputs",
                FriendlyName = "Barometric Pressure (Indicated), In. Hg.",
                Id = "101081_Barometric_Pressure_From_Sim",
                Index = 0,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                State = 29.92,
                MinValue = 28.10,
                MaxValue = 31.00
            };
            return thisSignal;
        }

        private void CreateInputEventHandlers()
        {
            _altitudeInputSignalChangedEventHandler =
                altitude_InputSignalChanged;
            _barometricPressureInputSignalChangedEventHandler =
                barometricPressure_InputSignalChanged;
        }

        private void CreateInputSignals()
        {
            _altitudeInputSignal = CreateAltitudeInputSignal();
            _barometricPressureInputSignal = CreateBarometricPressureInputSignal();
        }

        private void CreateOutputSignals()
        {
            _altitudeFineSinOutputSignal = CreateAltitudeFineSinOutputSignal();
            _altitudeFineCosOutputSignal = CreateAltitudeFineCosOutputSignal();
            _altitudeCoarseOutputSignal = CreateAltitudeCoarseSinOutputSignal();
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
            if (_altitudeInputSignal != null)
            {
                _altitudeInputSignal.SignalChanged += _altitudeInputSignalChangedEventHandler;
            }
            if (_barometricPressureInputSignal != null)
            {
                _barometricPressureInputSignal.SignalChanged += _barometricPressureInputSignalChangedEventHandler;
            }
        }

        private void UnregisterForInputEvents()
        {
            if (_altitudeInputSignalChangedEventHandler != null && _altitudeInputSignal != null)
            {
                try
                {
                    _altitudeInputSignal.SignalChanged -= _altitudeInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
            if (_barometricPressureInputSignalChangedEventHandler != null && _barometricPressureInputSignal != null)
            {
                try
                {
                    _barometricPressureInputSignal.SignalChanged -= _barometricPressureInputSignalChangedEventHandler;
                }
                catch (RemotingException)
                {
                }
            }
        }

        private void UpdateAltitudeOutputValues()
        {
            if (_altitudeInputSignal != null)
            {
                var altitudeInput = _altitudeInputSignal.State;
                double altitudeCoarseOutputValue = 0;

                var numRevolutionsOfFineResolver = altitudeInput / 1000.0000;
                var fineResolverDegrees = numRevolutionsOfFineResolver * 360.0000;
                var altitudeFineSinOutputValue = 10.0000 * Math.Sin(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);
                var altitudeFineCosOutputValue = 10.0000 * Math.Cos(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);

                if (altitudeInput < -1000)
                {
                    altitudeCoarseOutputValue = -10.0000;
                }
                else if (altitudeInput >= -1000 && altitudeInput < 0)
                {
                    altitudeCoarseOutputValue = -10.0000 + (altitudeInput - -1000.0000) / 1000.0000 * 0.2500;
                }
                else if (altitudeInput >= 0 && altitudeInput < 80000)
                {
                    altitudeCoarseOutputValue = -9.7500 + altitudeInput / 1000.0000 * (19.7500 / 80.0000);
                }
                else if (altitudeInput >= 80000)
                {
                    altitudeCoarseOutputValue = 10;
                }

                if (_altitudeFineSinOutputSignal != null)
                {
                    if (altitudeFineSinOutputValue < -10)
                    {
                        altitudeFineSinOutputValue = -10;
                    }
                    else if (altitudeFineSinOutputValue > 10)
                    {
                        altitudeFineSinOutputValue = 10;
                    }

                    _altitudeFineSinOutputSignal.State = altitudeFineSinOutputValue;
                }

                if (_altitudeFineCosOutputSignal != null)
                {
                    if (altitudeFineCosOutputValue < -10)
                    {
                        altitudeFineCosOutputValue = -10;
                    }
                    else if (altitudeFineCosOutputValue > 10)
                    {
                        altitudeFineCosOutputValue = 10;
                    }

                    _altitudeFineCosOutputSignal.State = altitudeFineCosOutputValue;
                }

                if (_altitudeCoarseOutputSignal == null) return;
                if (altitudeCoarseOutputValue < -10)
                {
                    altitudeCoarseOutputValue = -10;
                }
                else if (altitudeCoarseOutputValue > 10)
                {
                    altitudeCoarseOutputValue = 10;
                }

                _altitudeCoarseOutputSignal.State = altitudeCoarseOutputValue;
            }
        }

        private static void UpdateBarometricPressureOutputValues()
        {
            //do nothing
        }
    }
}