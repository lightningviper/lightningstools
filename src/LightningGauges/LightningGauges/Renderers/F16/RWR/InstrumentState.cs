using System;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16.RWR
{
    [Serializable]
    public class InstrumentState : InstrumentStateBase
    {
        public InstrumentState(){}
        public float yaw { get; set; } = 0.0f;
        public int RwrObjectCount { get; set; } = 0;
        public int[] RWRsymbol { get; set; } = Array.Empty<int>();
        public float[] bearing { get; set; } = Array.Empty<float>();
        public uint[] missileActivity { get; set; } = Array.Empty<uint>();
        public uint[] missileLaunch { get; set; } = Array.Empty<uint>();
        public uint[] selected { get; set; } = Array.Empty<uint>();
        public float[] lethality { get; set; } = Array.Empty<float>();
        public uint[] newDetection { get; set; } = Array.Empty<uint>();
        public byte[] RwrInfo { get; set; } = Array.Empty<byte>();
        public float ChaffCount { get; set; } = 0.0f;
        public float FlareCount { get; set; } = 0.0f;
        public bool PowerOn { get; set; }
    }

}