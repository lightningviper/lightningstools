using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Malwin
{
    // Per-gauge calibration config for Malwin 1956-3 (F-16 LOX Quantity
    // Indicator). Single multi_resolver pair (sin/cos) — input liters maps
    // linearly to dial pointer angle as `(input/5) × 180°`, equivalent to
    // 10 liters per full revolution. Note: original C# was missing the ×10
    // multiplier on the sin/cos output, so legacy installs had ±1 V output
    // instead of ±10 V; the rewritten fallback path now applies it.
    [Serializable]
    [XmlRoot(nameof(Malwin19563HardwareSupportModule))]
    public class Malwin19563HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Malwin19563HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Malwin19563HardwareSupportModuleConfig>(filePath);
        }
    }
}
