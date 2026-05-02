using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0581-02 (F-16 Vertical
    // Velocity Indicator). Single piecewise channel for the VVI → DAC
    // mapping (8 spec-sheet test points covering -6000..+6000 FPM). The
    // gauge also has a digital POWER-OFF input that overrides the analog
    // output to -10 V — that override stays hardcoded in the HSM (not a
    // user-calibratable property). Same pattern as Simtek10058201 AoA.
    [Serializable]
    [XmlRoot(nameof(Simtek10058102HardwareSupportModule))]
    public class Simtek10058102HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek10058102HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek10058102HardwareSupportModuleConfig>(filePath);
        }
    }
}
