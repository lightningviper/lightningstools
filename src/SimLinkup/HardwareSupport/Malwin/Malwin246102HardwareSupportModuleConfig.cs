using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Malwin
{
    // Per-gauge calibration config for Malwin 246102 (F-16 Cabin Pressure
    // Altimeter). Single multi_resolver pair (sin/cos) — input ft maps
    // linearly to dial pointer angle as `(input/50000) × 300°`, equivalent
    // to 60000 ft per full revolution.
    [Serializable]
    [XmlRoot(nameof(Malwin246102HardwareSupportModule))]
    public class Malwin246102HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Malwin246102HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Malwin246102HardwareSupportModuleConfig>(filePath);
        }
    }
}
