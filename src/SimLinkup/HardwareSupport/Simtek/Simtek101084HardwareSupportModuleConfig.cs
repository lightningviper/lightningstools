using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1084 (F-16 Standby ADI).
    // Three logical channels carried in the GaugeCalibrationConfig.Channels
    // array:
    //
    //   1. Pitch resolver pair (kind="piecewise_resolver"):
    //      101084_Pitch_SIN_To_Instrument (carries the breakpoint table)
    //      101084_Pitch_COS_To_Instrument (role="cos", points back to SIN)
    //   2. Roll resolver pair (kind="resolver"):
    //      101084_Roll_SIN_To_Instrument (carries the linear angle range)
    //      101084_Roll_COS_To_Instrument (role="cos", points back to SIN)
    //   3. OFF flag (kind="digital_invert"):
    //      101084_OFF_Flag_To_Instrument (single Invert bool)
    //
    // The HSM looks up each channel by id and dispatches on kind.
    [Serializable]
    [XmlRoot(nameof(Simtek101084HardwareSupportModule))]
    public class Simtek101084HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101084HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101084HardwareSupportModuleConfig>(filePath);
        }
    }
}
