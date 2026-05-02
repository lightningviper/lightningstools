using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Astronautics
{
    // Per-gauge calibration config for Astronautics 12871 (F-16 Primary
    // ADI). 10 calibratable channels:
    //
    //   - Pitch SIN/COS pair        (piecewise_resolver, identity ±90°)
    //   - Roll SIN/COS pair         (piecewise_resolver, identity ±180°)
    //   - 4 digital flags           (digital_invert × 4: OFF, GS, LOC, AUX)
    //   - Horizontal command bar    (piecewise; show/hide gating stays in C#)
    //   - Vertical command bar      (piecewise; show/hide gating stays in C#)
    //   - Inclinometer              (piecewise, ±100% × 10 V)
    //   - Rate of turn              (piecewise, ±100% × 10 V)
    [Serializable]
    [XmlRoot(nameof(Astronautics12871HardwareSupportModule))]
    public class Astronautics12871HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Astronautics12871HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Astronautics12871HardwareSupportModuleConfig>(filePath);
        }
    }
}
