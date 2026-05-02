using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Westin
{
    // Per-gauge calibration config for Westin 521993 (F-16 EPU Fuel Quantity
    // Indicator). Single piecewise channel — output is an unusual 0.1..2.0 V
    // range driving a low-voltage hot-wire EPU hydrazine gauge directly.
    [Serializable]
    [XmlRoot(nameof(Westin521993HardwareSupportModule))]
    public class Westin521993HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Westin521993HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Westin521993HardwareSupportModuleConfig>(filePath);
        }
    }
}
