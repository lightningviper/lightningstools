using System;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    [Serializable]
    public class VerticalVelocityIndicatorInstrumentState : InstrumentStateBase
    {
        private const float MAX_VELOCITY = 6000;
        private const float MIN_VELOCITY = -6000;
        private float _verticalVelocityFeet;

        public VerticalVelocityIndicatorInstrumentState()
        {
            OffFlag = false;
            VerticalVelocityFeet = 0.0f;
        }


        public float VerticalVelocityFeet
        {
            get => _verticalVelocityFeet;
            set
            {
                var vv = value;
                if (float.IsInfinity(vv) || float.IsNaN(vv)) vv = 0;
                if (vv < MIN_VELOCITY) vv = MIN_VELOCITY;
                if (vv > MAX_VELOCITY) vv = MAX_VELOCITY;
                _verticalVelocityFeet = vv;
            }
        }


        public bool OffFlag { get; set; }
    }
}