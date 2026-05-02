using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1081 (F-16 Altimeter v2).
    // Three channels, two patterns:
    //   1. Fine altitude pair (kind="multi_resolver"):
    //      101081_Altitude_Fine_SIN_To_Instrument (carries UnitsPerRevolution + PeakVolts)
    //      101081_Altitude_Fine_COS_To_Instrument (role="cos", points back to SIN)
    //   2. Coarse altitude (kind="piecewise"):
    //      101081_Altitude_Coarse_To_Instrument (3-breakpoint table)
    //
    // First gauge to use the multi_resolver pattern (synchro that wraps
    // many revolutions across the input range, 1000 ft per revolution).
    [Serializable]
    [XmlRoot(nameof(Simtek101081HardwareSupportModule))]
    public class Simtek101081HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101081HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101081HardwareSupportModuleConfig>(filePath);
        }
    }
}
