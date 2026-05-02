using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Malwin
{
    // Per-gauge calibration config for Malwin 1956-2 (F-16 FTIT Indicator).
    // Single piecewise_resolver pair (sin/cos) — input °C maps to dial
    // pointer angle via four linear segments (dead-band 0..200°C, then
    // 0..100° / 100..280° / 280..320° at progressively different slopes),
    // then sin/cos × 10 V (note: the original C# was missing the × 10
    // multiplier; see editor file header for details).
    [Serializable]
    [XmlRoot(nameof(Malwin19562HardwareSupportModule))]
    public class Malwin19562HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Malwin19562HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Malwin19562HardwareSupportModuleConfig>(filePath);
        }
    }
}
