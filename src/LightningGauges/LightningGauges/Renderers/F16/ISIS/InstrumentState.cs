using System;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16.ISIS
{
    [Serializable]
    public class InstrumentState : InstrumentStateBase
    {
        private const int MAX_BRIGHTNESS = 255;
        private const float MAX_VELOCITY = 100000;
        private const float MIN_VELOCITY = -100000;
        private const float MIN_PITCH = -90;
        private const float MAX_PITCH = 90;
        private const float MIN_ROLL = -180;
        private const float MAX_ROLL = 180;
        private const float MAX_MACH = 3.0F;
        private const float MAX_AIRSPEED = 3000.0F;
        private const float DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES = 1.0F;
        private const float DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES = 5.0F;
        private float _airspeedKnots;
        private float _barometricPressure = 2992f;
        private int _brightness = MAX_BRIGHTNESS;
        private float _glideslopeDeviationDegrees;
        private float _glideslopeDeviationLimitDegrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
        private float _localizerDeviationDegrees;
        private float _localizerDeviationLimitDegrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
        private float _machNumber;
        private float _magneticHeadingDegrees;
        private float _neverExceedSpeedKnots;
        private float _pitchDegrees;
        private float _rollDegrees;
        private float _verticalVelocityFeetPerMinute;
        private float _radarAltitudeAGL;
        private bool _radarAltitudeValid;
        private float _indicatedAltitudeFeetMSL;

        public InstrumentState()
        {
            PitchDegrees = 0;
            RollDegrees = 0;
            AirspeedKnots = 0;
            MachNumber = 0;
            NeverExceedSpeedKnots = 0;
            IndicatedAltitudeFeetMSL = 0;
            RadarAltitudeAGL = 0;
            RadarAltitudeValid = false;
            VerticalVelocityFeetPerMinute = 0;
            GlideslopeDeviationLimitDegrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
            GlideslopeDeviationDegrees = 0;
            LocalizerDeviationLimitDegrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
            LocalizerDeviationDegrees = 0;
            GlideslopeFlag = false;
            LocalizerFlag = false;
            ShowCommandBars = false;
            AuxFlag = false;
            OffFlag = false;
        }

        public bool OffFlag { get; set; }
        public bool AuxFlag { get; set; }
        public bool GlideslopeFlag { get; set; }
        public bool LocalizerFlag { get; set; }
        public bool ShowCommandBars { get; set; }


        public float BarometricPressure
        {
            get => _barometricPressure;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                _barometricPressure = value;
            }
        }


        public float VerticalVelocityFeetPerMinute
        {
            get => _verticalVelocityFeetPerMinute;
            set
            {
                var vv = value;
                if (float.IsNaN(vv) || float.IsInfinity(vv)) vv = MIN_VELOCITY;
                if (vv < MIN_VELOCITY) vv = MIN_VELOCITY;
                if (vv > MAX_VELOCITY) vv = MAX_VELOCITY;
                _verticalVelocityFeetPerMinute = vv;
            }
        }


        public float MagneticHeadingDegrees
        {
            get => _magneticHeadingDegrees;
            set
            {
                var heading = value;
                if (float.IsNaN(heading) || float.IsInfinity(heading)) heading = 0;
                heading %= 360.0f;
                _magneticHeadingDegrees = heading;
            }
        }


        public float IndicatedAltitudeFeetMSL
        {
            get => _indicatedAltitudeFeetMSL;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                _indicatedAltitudeFeetMSL = value;
            }
        }


        public float RadarAltitudeAGL
        {
            get => _radarAltitudeAGL;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                _radarAltitudeAGL = value;
            }
        }
        public bool RadarAltitudeValid
        {
            get => _radarAltitudeValid;
            set => _radarAltitudeValid = value;
        }


        public float AirspeedKnots
        {
            get => _airspeedKnots;
            set
            {
                var knots = value;
                if (float.IsNaN(knots) || float.IsInfinity(knots)) knots = 0;
                if (knots < 0) knots = 0;
                if (knots > MAX_AIRSPEED) knots = MAX_AIRSPEED;
                _airspeedKnots = knots;
            }
        }


        public float MachNumber
        {
            get => _machNumber;
            set
            {
                var mach = value;
                if (float.IsNaN(mach) || float.IsInfinity(mach)) mach = 0;
                if (mach < 0) mach = 0;
                if (mach > MAX_MACH) mach = MAX_MACH;
                _machNumber = mach;
            }
        }


        public float NeverExceedSpeedKnots
        {
            get => _neverExceedSpeedKnots;
            set
            {
                var vne = value;
                if (float.IsNaN(vne) || float.IsInfinity(vne)) vne = 0;
                if (vne < 0) vne = 0;
                if (vne > MAX_AIRSPEED) vne = MAX_AIRSPEED;
                _neverExceedSpeedKnots = vne;
            }
        }


        public float PitchDegrees
        {
            get => _pitchDegrees;
            set
            {
                var pitch = value;
                if (float.IsNaN(pitch) || float.IsInfinity(pitch)) pitch = 0;
                if (pitch < MIN_PITCH) pitch = MIN_PITCH;
                if (pitch > MAX_PITCH) pitch = MAX_PITCH;
                _pitchDegrees = pitch;
            }
        }


        public float RollDegrees
        {
            get => _rollDegrees;
            set
            {
                var roll = value;
                if (float.IsInfinity(roll) || float.IsNaN(roll)) roll = 0;
                if (roll < MIN_ROLL) roll = MIN_ROLL;
                if (roll > MAX_ROLL) roll = MAX_ROLL;
                _rollDegrees = roll;
            }
        }


        public int Brightness
        {
            get => _brightness;
            set
            {
                var brightness = value;
                if (float.IsNaN(brightness) || float.IsInfinity(brightness)) brightness = MAX_BRIGHTNESS;
                if (brightness < 0) brightness = 0;
                if (brightness > MAX_BRIGHTNESS) brightness = MAX_BRIGHTNESS;
                _brightness = brightness;
            }
        }


        public int MaxBrightness => MAX_BRIGHTNESS;


        public float LocalizerDeviationDegrees
        {
            get => _localizerDeviationDegrees;
            set
            {
                var degrees = value;
                degrees %= 360.0f;
                if (float.IsNaN(degrees) || float.IsInfinity(degrees)) degrees = 0;
                _localizerDeviationDegrees = degrees;
            }
        }


        public float LocalizerDeviationLimitDegrees
        {
            get => _localizerDeviationLimitDegrees;
            set
            {
                var degrees = value;
                degrees %= 360.0f;
                if (float.IsInfinity(degrees) || float.IsNaN(degrees) || degrees == 0) degrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
                _localizerDeviationLimitDegrees = degrees;
            }
        }


        public float GlideslopeDeviationLimitDegrees
        {
            get => _glideslopeDeviationLimitDegrees;
            set
            {
                var degrees = value;
                degrees %= 360.0f;
                if (float.IsNaN(degrees) || float.IsInfinity(degrees) || degrees == 0) degrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
                _glideslopeDeviationLimitDegrees = degrees;
            }
        }


        public float GlideslopeDeviationDegrees
        {
            get => _glideslopeDeviationDegrees;
            set
            {
                var degrees = value;
                degrees %= 360.0f;
                if (float.IsInfinity(degrees) || float.IsNaN(degrees)) degrees = 0;
                _glideslopeDeviationDegrees = degrees;
            }
        }
    }
}