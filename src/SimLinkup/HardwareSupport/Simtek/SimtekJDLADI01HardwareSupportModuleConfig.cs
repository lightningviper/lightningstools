using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek JDL-ADI01 (F-16 Primary
    // ADI). 10 calibratable channels — same shape as Astronautics 12871:
    //
    //   - Pitch SIN/COS pair        (piecewise_resolver, identity ±90°)
    //   - Roll SIN/COS pair         (piecewise_resolver, identity ±180°)
    //   - 4 digital flags           (digital_invert × 4: OFF, GS, LOC, AUX)
    //   - Horizontal command bar    (piecewise; show/hide gating stays in C#)
    //   - Vertical command bar      (piecewise; show/hide gating stays in C#)
    //   - Inclinometer              (piecewise, ±100% × 10 V)
    //   - Rate of turn              (piecewise, ±100% × 10 V)
    //
    // Connector pinout calls the command bars "horizontal pointer" and
    // "vertical pointer"; we keep the Command_Bar port naming for
    // consistency with the rest of the F-16 ADI HSMs in the catalog.
    [Serializable]
    [XmlRoot(nameof(SimtekJDLADI01HardwareSupportModule))]
    public class SimtekJDLADI01HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static SimtekJDLADI01HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<SimtekJDLADI01HardwareSupportModuleConfig>(filePath);
        }
    }
}
