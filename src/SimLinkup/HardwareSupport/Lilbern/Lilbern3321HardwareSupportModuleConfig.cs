using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Lilbern
{
    // Per-gauge calibration config for Lilbern 3321 (F-16 Tachometer/RPM
    // Indicator). Single piecewise_resolver pair (sin/cos) — input %RPM
    // maps to dial pointer angle via two linear segments (0..60% → 0..90°,
    // 60..110% → 90..330°), then sin/cos × 10 V.
    [Serializable]
    [XmlRoot(nameof(Lilbern3321HardwareSupportModule))]
    public class Lilbern3321HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Lilbern3321HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Lilbern3321HardwareSupportModuleConfig>(filePath);
        }
    }
}
