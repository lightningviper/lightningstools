using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-5859 (F-16 Standby Attitude
    // Indicator). Two channel pairs:
    //
    //   1. Pitch (piecewise_resolver):
    //      105859_Pitch_SIN_To_Instrument (carries the breakpoint table)
    //      105859_Pitch_COS_To_Instrument (role="cos", points back to SIN)
    //   2. Roll (piecewise_resolver):
    //      105859_Roll_SIN_To_Instrument (carries the breakpoint table)
    //      105859_Roll_COS_To_Instrument (role="cos", points back to SIN)
    //
    // Drive type per the gauge spec sheet: dual DC servo. Calibration tables
    // come from PL20-5859 Tables 1 & 2.
    [Serializable]
    [XmlRoot(nameof(Simtek105859HardwareSupportModule))]
    public class Simtek105859HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek105859HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek105859HardwareSupportModuleConfig>(filePath);
        }
    }
}
