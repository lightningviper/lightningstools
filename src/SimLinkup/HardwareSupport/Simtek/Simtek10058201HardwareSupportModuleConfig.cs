using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0582-01 (F-16 Angle of
    // Attack Indicator). Single piecewise channel for the AoA → DAC
    // mapping. The gauge also has a digital POWER-OFF input that, when
    // true, overrides the analog output to -10 V — that override stays
    // hardcoded in the HSM (not a user-calibratable property).
    [Serializable]
    [XmlRoot(nameof(Simtek10058201HardwareSupportModule))]
    public class Simtek10058201HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek10058201HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek10058201HardwareSupportModuleConfig>(filePath);
        }
    }
}
