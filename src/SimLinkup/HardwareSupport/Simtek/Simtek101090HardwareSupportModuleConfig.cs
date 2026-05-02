using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1090 (F-16 EPU Fuel Quantity
    // Indicator). Single piecewise channel — input EPU fuel % (0..100)
    // maps to ±10 V. Defaults are a 5-point straight line matching the C#
    // fallback exactly; the user can edit individual breakpoints to correct
    // for non-linear hardware drift.
    [Serializable]
    [XmlRoot(nameof(Simtek101090HardwareSupportModule))]
    public class Simtek101090HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101090HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101090HardwareSupportModuleConfig>(filePath);
        }
    }
}
