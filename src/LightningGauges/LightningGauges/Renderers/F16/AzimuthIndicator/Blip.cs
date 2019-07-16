using System;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    [Serializable]
    public class Blip
    {
        private float _bearingDegrees;
        public int SymbolID { get; set; }


        public float BearingDegrees
        {
            get => _bearingDegrees;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                var bearing = value;
                bearing %= 360.0f;
                _bearingDegrees = bearing;
            }
        }


        public uint MissileActivity { get; set; }
        public uint MissileLaunch { get; set; }
        public uint Selected { get; set; }
        private float _lethality;


        public float Lethality
        {
            get => _lethality;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value)) value = 0;
                _lethality = value;
            }
        }


        public uint NewDetection { get; set; }
        public bool Visible { get; set; }
    }
}