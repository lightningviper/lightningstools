using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1078 (F-16 Cabin Pressure
    // Altimeter). Single piecewise channel — input cabin altitude in feet
    // (0..50000) maps to ±10 V. Defaults are an 11-point straight line
    // matching the C# fallback exactly; the user can edit individual
    // breakpoints to correct for non-linear hardware drift.
    [Serializable]
    [XmlRoot(nameof(Simtek101078HardwareSupportModule))]
    public class Simtek101078HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101078HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101078HardwareSupportModuleConfig>(filePath);
        }
    }
}
