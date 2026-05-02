using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-5860 (F-16 Fuel Flow
    // Indicator). Single piecewise channel:
    //
    //   105860_Fuel_Flow_To_Instrument (kind="piecewise")
    //
    // Default breakpoints come from PL20-5860 Table 1 — 9 spec-sheet
    // test points at PPH×100 = 0, 100, 200, …, 800 mapped to -10..+10 V
    // linearly. The dial reads 0..80,000 PPH with the two least
    // significant digits fixed at 00.
    [Serializable]
    [XmlRoot(nameof(Simtek105860HardwareSupportModule))]
    public class Simtek105860HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek105860HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek105860HardwareSupportModuleConfig>(filePath);
        }
    }
}
