using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-5868 (F-16 EPU Fuel
    // Quantity Indicator). Single piecewise channel:
    //
    //   105868_EPU_To_Instrument (kind="piecewise")
    //
    // Default breakpoints come from PL20-5868 Table 1 — 11 spec-sheet
    // test points 0..100% remain → -10..+10 V linear. Drive type:
    // single meter.
    [Serializable]
    [XmlRoot(nameof(Simtek105868HardwareSupportModule))]
    public class Simtek105868HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek105868HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek105868HardwareSupportModuleConfig>(filePath);
        }
    }
}
