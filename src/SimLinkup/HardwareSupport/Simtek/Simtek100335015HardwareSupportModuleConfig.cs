using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0335-01 (F-16 Standby
    // Attitude Indicator). Three channels:
    //
    //   1. Pitch sin/cos pair (kind="piecewise_resolver") — identity
    //      mapping by default (input pitch degrees == resolver angle
    //      degrees); the user can edit the breakpoint table to compensate
    //      for synchro drift at specific pitch angles.
    //
    //   2. Roll sin/cos pair (kind="piecewise_resolver") — identity by
    //      default, full ±180° range editable per row.
    //
    //   3. OFF flag (kind="digital_invert") — single boolean inversion,
    //      same as 10-1084.
    //
    // No legacy bare-property fields; this gauge has never had a config
    // file consumed by SimLinkup before today.
    [Serializable]
    [XmlRoot(nameof(Simtek100335015HardwareSupportModule))]
    public class Simtek100335015HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100335015HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100335015HardwareSupportModuleConfig>(filePath);
        }
    }
}
