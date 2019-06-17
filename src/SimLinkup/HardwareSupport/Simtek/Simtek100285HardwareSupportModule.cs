using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting;
using Common.HardwareSupport;
using Common.MacroProgramming;
using Common.Math;
using LightningGauges.Renderers.F16;
using System.IO;
using log4net;

namespace SimLinkup.HardwareSupport.Simtek
{
    //Simtek 10-0285 F-16 Altimeter
    public class Simtek100285HardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Simtek100285HardwareSupportModule));
        private readonly IAltimeter _renderer = new Altimeter();

        private AnalogSignal _altitudeCoarseCosOutputSignal;
        private AnalogSignal _altitudeCoarseSinOutputSignal;
        private AnalogSignal _altitudeFineCosOutputSignal;
        private AnalogSignal _altitudeFineSinOutputSignal;
        private AnalogSignal _altitudeInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _altitudeInputSignalChangedEventHandler;
        private AnalogSignal _barometricPressureInputSignal;
        private AnalogSignal.AnalogSignalChangedEventHandler _barometricPressureInputSignalChangedEventHandler;
        private bool _isDisposed;
        private const double DEFAULT_MIN_BARO_PRESSURE= 28.09;
        private const double DEFAULT_MAX_BARO_PRESSURE = 31.025;
        private const double DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET= 2800;
        private double _altitudeZeroOffsetInFeet = 0;
        private double _minBaroPressure = DEFAULT_MIN_BARO_PRESSURE;
        private double _maxBaroPressure = DEFAULT_MAX_BARO_PRESSURE;
        private double _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet = DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET;

        private Simtek100285HardwareSupportModule()
        {
            LoadConfig();
            CreateInputSignals();
            CreateOutputSignals();
            CreateInputEventHandlers();
            RegisterForInputEvents();
        }

        public override AnalogSignal[] AnalogInputs => new[] {_altitudeInputSignal, _barometricPressureInputSignal};

        public override AnalogSignal[] AnalogOutputs => new[]
        {
            _altitudeFineSinOutputSignal, _altitudeFineCosOutputSignal,
            _altitudeCoarseSinOutputSignal,
            _altitudeCoarseCosOutputSignal
        };

        public override DigitalSignal[] DigitalInputs => null;

        public override DigitalSignal[] DigitalOutputs => null;

        public override string FriendlyName => "Simtek P/N 10-0285 - Simulated Altimeter";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Simtek100285HardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>
            {
                new Simtek100285HardwareSupportModule()
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

        private void barometricPressure_InputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            UpdateAltitudeOutputValues();
        }
        private void LoadConfig()
        {
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "Simtek100285HardwareSupportModule.config");
                var hsmConfig = Simtek100285HardwareSupportModuleConfig.Load(hsmConfigFilePath);
                _minBaroPressure= hsmConfig.MinBaroPressureInHg.HasValue ? hsmConfig.MinBaroPressureInHg.Value : 28.10;
                _maxBaroPressure= hsmConfig.MaxBaroPressureInHg.HasValue ? hsmConfig.MaxBaroPressureInHg.Value : 31.00;
                _differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet = hsmConfig.IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro.HasValue ? hsmConfig.IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro.Value : DEFAULT_DIFFERENCE_IN_INDICATED_ALTITUDE_FROM_MIN_BARO_TO_MAX_BARO_IN_FEET;
                _altitudeZeroOffsetInFeet = hsmConfig.AltitudeZeroOffsetInFeet.HasValue ? hsmConfig.AltitudeZeroOffsetInFeet.Value : 0;
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

        }
        private AnalogSignal CreateAltitudeCoarseCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse (COS)",
                Id = "100285_Altitude_Coarse_COS_To_Instrument",
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

        private AnalogSignal CreateAltitudeCoarseSinOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Coarse (SIN)",
                Id = "100285_Altitude_Coarse_SIN_To_Instrument",
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

        private AnalogSignal CreateAltitudeFineCosOutputSignal()
        {
            var thisSignal = new AnalogSignal
            {
                Category = "Outputs",
                CollectionName = "Analog Outputs",
                FriendlyName = "Altitude - Fine (COS)",
                Id = "100285_Altitude_Fine_COS_To_Instrument",
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
                Id = "100285_Altitude_Fine_SIN_To_Instrument",
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
                Id = "100285_Altitude_From_Sim",
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
                Id = "100285_Barometric_Pressure_From_Sim",
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

            _barometricPressureInputSignalChangedEventHandler = barometricPressure_InputSignalChanged;
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
            _altitudeCoarseSinOutputSignal = CreateAltitudeCoarseSinOutputSignal();
            _altitudeCoarseCosOutputSignal = CreateAltitudeCoarseCosOutputSignal();
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
                var baroInput = _barometricPressureInputSignal.State;
                if (baroInput == 0.00f) baroInput = 29.92f;
                var baroDeltaFromStandard = baroInput - 29.92f;
                var altToAddForBaroComp = -(_differenceInIndicatedAltitudeFromMinBaroToMaxBaroInFeet / (_maxBaroPressure-_minBaroPressure)) * baroDeltaFromStandard;
                var altitudeOutput = altitudeInput + altToAddForBaroComp + _altitudeZeroOffsetInFeet;

                var numRevolutionsOfFineResolver = altitudeOutput / 4000;
                var numRevolutionsOfCoarseResolver = altitudeOutput / 100000;

                var fineResolverDegrees = numRevolutionsOfFineResolver * 360;
                var coarseResolverDegrees = numRevolutionsOfCoarseResolver * 360;

                var altitudeFineSinOutputValue = 10.0000 * Math.Sin(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);
                var altitudeFineCosOutputValue = 10.0000 * Math.Cos(fineResolverDegrees * Constants.RADIANS_PER_DEGREE);

                var altitudeCoarseSinOutputValue =
                    10.0000 * Math.Sin(coarseResolverDegrees * Constants.RADIANS_PER_DEGREE);
                var altitudeCoarseCosOutputValue =
                    10.0000 * Math.Cos(coarseResolverDegrees * Constants.RADIANS_PER_DEGREE);


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

                if (_altitudeCoarseSinOutputSignal != null)
                {
                    if (altitudeCoarseSinOutputValue < -10)
                    {
                        altitudeCoarseSinOutputValue = -10;
                    }
                    else if (altitudeCoarseSinOutputValue > 10)
                    {
                        altitudeCoarseSinOutputValue = 10;
                    }

                    _altitudeCoarseSinOutputSignal.State = altitudeCoarseSinOutputValue;
                }

                if (_altitudeCoarseCosOutputSignal != null)
                {
                    if (altitudeCoarseCosOutputValue < -10)
                    {
                        altitudeCoarseCosOutputValue = -10;
                    }
                    else if (altitudeCoarseCosOutputValue > 10)
                    {
                        altitudeCoarseCosOutputValue = 10;
                    }

                    _altitudeCoarseCosOutputSignal.State = altitudeCoarseCosOutputValue;
                }
            }
        }
    }
}