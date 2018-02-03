using System;

namespace Phcc.DeviceManager.Config
{
    [Serializable]
    public class ServoCalibration : PhccConfigElement
    {
        public Int16 ServoNum { get; set; }
        public byte Gain { get; set; }
        public Int16 CalibrationOffset { get; set; }
    }
}