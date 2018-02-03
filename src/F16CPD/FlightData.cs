using System;
using F16CPD.FlightInstruments;
using F16CPD.FlightInstruments.Pfd;

namespace F16CPD
{
    [Serializable]
    public sealed class FlightData
    {
        private float _adiIlsGlideslopeDeviationDecimalDegrees;
        private float _adiIlsLocalizerDeviationDecimalDegrees;
        private float _altitudeAboveGroundLevelInFeet;
        private float _angleOfAttackInDegrees;
        private int _automaticLowAltitudeWarningInFeet;
        private float _barometricPressure = 29.92f;
        private float _betaAngleInDegrees;
        private float _gammaAngleInDegrees;
        private float _hsiBearingToBeaconInDegrees;
        private float _hsiCourseDeviationInDegrees;
        private float _hsiCourseDeviationLimitInDecimalDegrees;
        private int _hsiDesiredCourseInDegrees;
        private int _hsiDesiredHeadingInDegrees;
        private float _hsiDistanceToBeaconInNauticalMiles;
        private float _hsiLocalizerCourseInDecimalDegrees;
        private float _indicatedAirspeedFeetPerSecond;
        private float _indicatedAltitudeAboveMeanSeaLevelInFeet;
        private float _magneticHeadingInDecimalDegrees;
        private float _pitchAngleInDegrees;
        private float _rateOfTurnInDegreesPerSecond;
        private float _rollAngleInDegrees;
        private string _tacanChannel = "106X";
        private int _transitionAltitudeInFeet = 18000;
        private float _trueAltitudeAboveMeanSeaLevelInFeet;
        private float _windOffsetToFlightPathMarkerDegrees;

        private float _mapCoordinateFeetNorth;
        public float MapCoordinateFeetNorth
        {
            get { return _mapCoordinateFeetNorth; }
            set
            {
                if (!float.IsInfinity(value) && !float.IsNaN(value))
                {
                    _mapCoordinateFeetNorth = value;
                }
                else
                {
                    _mapCoordinateFeetNorth = 0.0f;
                }
            }
        }

        private float _mapCoordinateFeetEast;
        public float MapCoordinateFeetEast
        {
            get { return _mapCoordinateFeetEast; }
            set
            {
                if (!float.IsInfinity(value) && !float.IsNaN(value))
                {
                    _mapCoordinateFeetEast = value;
                }
                else
                {
                    _mapCoordinateFeetEast = 0.0f;
                }
            }
        }

        public bool HsiDisplayToFromFlag { get; set; }

        public string TacanChannel
        {
            get { return _tacanChannel; }
            set { _tacanChannel = value; }
        }

        private float _latitudeInDecimalDegrees;
        public float LatitudeInDecimalDegrees
        {
            get { return _latitudeInDecimalDegrees; }
            set
            {
                if (!float.IsInfinity(value) && !float.IsNaN(value))
                {
                    _latitudeInDecimalDegrees = value;
                }
                else
                {
                    _latitudeInDecimalDegrees = 0.0f;
                }
            }
        }

        private float _longitudeInDecimalDegrees;
        public float LongitudeInDecimalDegrees
        {
            get { return _longitudeInDecimalDegrees; }
            set
            {
                if (!float.IsInfinity(value) && !float.IsNaN(value))
                {
                    _longitudeInDecimalDegrees = value;
                }
                else
                {
                    _longitudeInDecimalDegrees = 0.0f;
                }
            }
        
        }

        public int TransitionAltitudeInFeet
        {
            get { return _transitionAltitudeInFeet; }
            set
            {
                if (value < 1000) value = -2500;
                if (value > 20000) value = 20000;

                _transitionAltitudeInFeet = value;
            }
        }

        public float RateOfTurnInDecimalDegreesPerSecond
        {
            get { return _rateOfTurnInDegreesPerSecond; }
            set
            {
                if (!float.IsInfinity(value) && !float.IsNaN(value))
                {
                    _rateOfTurnInDegreesPerSecond = value;
                }
                else
                {
                    _rateOfTurnInDegreesPerSecond = 0.0f;
                }
            }
        }

        public float IndicatedAltitudeAboveMeanSeaLevelInDecimalFeet
        {
            get { return _indicatedAltitudeAboveMeanSeaLevelInFeet; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                if (value < -4500) value = -4500;
                if (value > 120000) value = 120000;
                _indicatedAltitudeAboveMeanSeaLevelInFeet = value;
            }
        }

        public float TrueAltitudeAboveMeanSeaLevelInDecimalFeet
        {
            get { return _trueAltitudeAboveMeanSeaLevelInFeet; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                if (value < -2500) value = -2500;
                if (value > 120000) value = 120000;
                _trueAltitudeAboveMeanSeaLevelInFeet = value;
            }
        }

        public float AltitudeAboveGroundLevelInDecimalFeet
        {
            get { return _altitudeAboveGroundLevelInFeet; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                if (value < 0) value = 0;
                if (value > 180000) value = 180000;
                _altitudeAboveGroundLevelInFeet = value;
            }
        }

        private float _machNumber;
        public float MachNumber
        {
            get { return _machNumber; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _machNumber = value;
            }
        }

        public float IndicatedAirspeedInDecimalFeetPerSecond
        {
            get { return _indicatedAirspeedFeetPerSecond; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _indicatedAirspeedFeetPerSecond = (float)(Math.Round(value, 1));
            }
        }

        private float _trueAirspeedInDecimalFeetPerSecond;
        public float TrueAirspeedInDecimalFeetPerSecond
        {
            get { return _trueAirspeedInDecimalFeetPerSecond; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _trueAirspeedInDecimalFeetPerSecond = value;
            }
        }

        private float _groundSpeedInDecimalFeetPerSecond;
        public float GroundSpeedInDecimalFeetPerSecond
        {
            get { return _groundSpeedInDecimalFeetPerSecond; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _groundSpeedInDecimalFeetPerSecond = value;
            }
        }

        public int AutomaticLowAltitudeWarningInFeet
        {
            get { return _automaticLowAltitudeWarningInFeet; }
            set
            {
                if (value < 0) value = 0;
                if (value > 99999) value = 99999;
                _automaticLowAltitudeWarningInFeet = value;
            }
        }

        public float BarometricPressure
        {
            get { return _barometricPressure; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _barometricPressure = value;
            }
        }
        public AltimeterUnits AltimeterUnits { get; set; }
        public float AngleOfAttackInDegrees
        {
            get { return _angleOfAttackInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _angleOfAttackInDegrees = value % 360;
            }
        }

        public float VerticalVelocityInDecimalFeetPerSecond { get; set; }

        public float PitchAngleInDecimalDegrees
        {
            get { return _pitchAngleInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _pitchAngleInDegrees = value % 360;
                if (_pitchAngleInDegrees > 90) _pitchAngleInDegrees = 90;
                if (_pitchAngleInDegrees < -90) _pitchAngleInDegrees = -90;
            }
        }

        public float RollAngleInDecimalDegrees
        {
            get { return _rollAngleInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _rollAngleInDegrees = value % 360;
            }
        }

        public float BetaAngleInDecimalDegrees
        {
            get { return _betaAngleInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _betaAngleInDegrees = value % 360;
            }
        }

        public float GammaAngleInDecimalDegrees
        {
            get { return _gammaAngleInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _gammaAngleInDegrees = value % 360;
            }
        }

        public float WindOffsetToFlightPathMarkerInDecimalDegrees
        {
            get { return _windOffsetToFlightPathMarkerDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _windOffsetToFlightPathMarkerDegrees = value % 360;
            }
        }

        public int HsiDesiredCourseInDegrees
        {
            get { return _hsiDesiredCourseInDegrees; }
            set
            {
                _hsiDesiredCourseInDegrees = value;
                if (_hsiDesiredCourseInDegrees < 0)
                    _hsiDesiredCourseInDegrees = 360 - Math.Abs(_hsiDesiredCourseInDegrees);
            }
        }

        public int HsiDesiredHeadingInDegrees
        {
            get { return _hsiDesiredHeadingInDegrees; }
            set
            {
                _hsiDesiredHeadingInDegrees = value;
                if (_hsiDesiredHeadingInDegrees < 0)
                    _hsiDesiredHeadingInDegrees = 360 - Math.Abs(_hsiDesiredHeadingInDegrees);
            }
        }

        public float AdiIlsLocalizerDeviationInDecimalDegrees
        {
            get { return _adiIlsLocalizerDeviationDecimalDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _adiIlsLocalizerDeviationDecimalDegrees = value % 360;
                if (Math.Abs(_adiIlsLocalizerDeviationDecimalDegrees) >
                    Pfd.ADI_ILS_LOCALIZER_DEVIATION_LIMIT_DECIMAL_DEGREES)
                {
                    _adiIlsLocalizerDeviationDecimalDegrees = (Math.Sign(_adiIlsLocalizerDeviationDecimalDegrees)*
                                                               Pfd.ADI_ILS_LOCALIZER_DEVIATION_LIMIT_DECIMAL_DEGREES);
                }
            }
        }

        public float AdiIlsGlideslopeDeviationInDecimalDegrees
        {
            get { return _adiIlsGlideslopeDeviationDecimalDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _adiIlsGlideslopeDeviationDecimalDegrees = value % 360;
                if (Math.Abs(_adiIlsGlideslopeDeviationDecimalDegrees) >
                    Pfd.ADI_ILS_GLIDESLOPE_DEVIATION_LIMIT_DECIMAL_DEGREES)
                {
                    _adiIlsGlideslopeDeviationDecimalDegrees = (Math.Sign(_adiIlsGlideslopeDeviationDecimalDegrees)*
                                                                Pfd.ADI_ILS_GLIDESLOPE_DEVIATION_LIMIT_DECIMAL_DEGREES);
                }
            }
        }

        public float HsiCourseDeviationInDecimalDegrees
        {
            get { return _hsiCourseDeviationInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _hsiCourseDeviationInDegrees = value % 360;
            }
        }

        public float HsiDistanceToBeaconInNauticalMiles
        {
            get { return _hsiDistanceToBeaconInNauticalMiles; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                if (value < 0) value = 0;
                if (value > 3500) value = 3500;
                _hsiDistanceToBeaconInNauticalMiles = value;
            }
        }

        public float HsiBearingToBeaconInDecimalDegrees
        {
            get { return _hsiBearingToBeaconInDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _hsiBearingToBeaconInDegrees = value % 360;
                if (_hsiBearingToBeaconInDegrees < 0)
                    _hsiBearingToBeaconInDegrees = 360 - Math.Abs(_hsiBearingToBeaconInDegrees);
            }
        }

        public float HsiCourseDeviationLimitInDecimalDegrees
        {
            get { return _hsiCourseDeviationLimitInDecimalDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _hsiCourseDeviationLimitInDecimalDegrees = value % 360;
                if (_hsiCourseDeviationLimitInDecimalDegrees < 0)
                    _hsiCourseDeviationLimitInDecimalDegrees = Math.Abs(_hsiCourseDeviationLimitInDecimalDegrees);
            }
        }

        public float HsiLocalizerDeviationInDecimalDegrees
        {
            get { return _hsiLocalizerCourseInDecimalDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _hsiLocalizerCourseInDecimalDegrees = value % 360;
                if (_hsiLocalizerCourseInDecimalDegrees < 0)
                    _hsiLocalizerCourseInDecimalDegrees = 360 - Math.Abs(_hsiLocalizerCourseInDecimalDegrees);
            }
        }

        public float MagneticHeadingInDecimalDegrees
        {
            get { return _magneticHeadingInDecimalDegrees; }
            set
            {
                if (float.IsInfinity(value) || float.IsNaN(value)) value = 0.0f;
                _magneticHeadingInDecimalDegrees = value % 360;
                if (_magneticHeadingInDecimalDegrees < 0)
                    _magneticHeadingInDecimalDegrees = 360 - Math.Abs(_magneticHeadingInDecimalDegrees);
            }
        }

        public bool HsiDeviationInvalidFlag { get; set; }
        public bool HsiDistanceInvalidFlag { get; set; }
        public bool AdiAuxFlag { get; set; }
        public bool AdiGlideslopeInvalidFlag { get; set; }
        public bool AdiLocalizerInvalidFlag { get; set; }
        public bool AdiEnableCommandBars { get; set; }
        public AltimeterMode AltimeterMode { get; set; }
        public NavModes NavMode { get; set; }
        public bool HsiOffFlag { get; set; }
        public bool AdiOffFlag { get; set; }
        public bool RadarAltimeterOffFlag { get; set; }
        public bool AoaOffFlag { get; set; }
        public bool VviOffFlag { get; set; }
        public bool CpdPowerOnFlag { get; set; }
        public bool PfdOffFlag { get; set; }
        public bool SplitMapDisplay { get; set; }
        public string ActiveMFD { get; set; }
        public bool MarkerBeaconOuterMarkerFlag { get; set; }
        public bool MarkerBeaconMiddleMarkerFlag { get; set; }
        public byte[] LMFDImage { get; set; }
        public byte[] RMFDImage { get; set; }
    }
}