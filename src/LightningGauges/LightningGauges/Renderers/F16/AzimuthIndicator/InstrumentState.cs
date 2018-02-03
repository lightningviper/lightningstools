using System;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    [Serializable]
    public class InstrumentState : InstrumentStateBase
    {
        private const int MAX_BRIGHTNESS = 255;
        private const int MAX_CHAFF = 99;
        private const int MAX_FLARE = 99;
        private const int MAX_OTHER1 = 99;
        private const int MAX_OTHER2 = 99;
        private int _brightness = MAX_BRIGHTNESS;

        private int _chaffCount;
        private int _flareCount;
        private float _magneticHeadingDegrees;
        private int _other1Count;
        private int _other2Count;
        private float _rollDegrees;

        public InstrumentState()
        {
            ChaffCount = 0;
            FlareCount = 0;
            Other1Count = 0;
            Other2Count = 0;
            ChaffLow = false;
            FlareLow = false;
            Other1Low = false;
            Other2Low = false;
            Blips = null;
            RWRPowerOn = true;
            SeparateMode = false;
            Handoff = false;
            Launch = false;
            PriorityMode = false;
            NavalMode = false;
            UnknownThreatScanMode = false;
            SearchMode = false;
            Activity = false;
            LowAltitudeMode = false;
            MagneticHeadingDegrees = 0;
            RollDegrees = 0;
            Inverted = false;
            EWMSMode = EWMSMode.Manual;
        }

        internal bool Inverted { get; set; }


        public float RollDegrees
        {
            get => _rollDegrees;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                if (Inverted) { if (Math.Abs(value) < 90) Inverted = false; }
                else { if (Math.Abs(value) > 120) Inverted = true; }
                _rollDegrees = value;
            }
        }


        public EWMSMode EWMSMode { get; set; }
        public int cmdsMode { get; set; } //Added Falcas 10-11-2012
        public bool EWSGo { get; set; }
        public bool EWSNoGo { get; set; }
        public bool EWSDegraded { get; set; }
        public bool EWSDispenseReady { get; set; }


        public int ChaffCount
        {
            get => _chaffCount;
            set
            {
                var chaff = value;
                if (chaff < 0) chaff = 0;
                if (chaff > MAX_CHAFF) chaff = MAX_CHAFF;
                _chaffCount = chaff;
            }
        }


        public int FlareCount
        {
            get => _flareCount;
            set
            {
                var flare = value;
                if (flare < 0) flare = 0;
                if (flare > MAX_FLARE) flare = MAX_FLARE;
                _flareCount = flare;
            }
        }


        public int Other1Count
        {
            get => _other1Count;
            set
            {
                var other1 = value;
                if (other1 < 0) other1 = 0;
                if (other1 > MAX_OTHER1) other1 = MAX_OTHER1;
                _other1Count = other1;
            }
        }


        public int Other2Count
        {
            get => _other2Count;
            set
            {
                var other2 = value;
                if (other2 < 0) other2 = 0;
                if (other2 > MAX_OTHER2) other2 = MAX_OTHER2;
                _other2Count = other2;
            }
        }


        public bool ChaffLow { get; set; }
        public bool FlareLow { get; set; }
        public bool Other1Low { get; set; }
        public bool Other2Low { get; set; }
        public bool SearchMode { get; set; }
        public bool Activity { get; set; }
        public bool LowAltitudeMode { get; set; }
        public bool Handoff { get; set; }
        public bool Launch { get; set; }
        public bool PriorityMode { get; set; }
        public bool NavalMode { get; set; }
        public bool UnknownThreatScanMode { get; set; }
        public Blip[] Blips { get; set; }
        public bool SeparateMode { get; set; }
        public bool RWRPowerOn { get; set; }
        public bool RWRTest1 { get; set; } //Added Falcas 07-11-2012
        public bool RWRTest2 { get; set; } //Added Falcas 07-11-2012
        public DateTime TestStartTime { get; set; }


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
    }
}