using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0672-02 (F-16 Standby
    // Attitude Indicator). Same shape as Simtek 10-0335-015 — three
    // channels:
    //
    //   1. Pitch sin/cos pair (kind="piecewise_resolver") — identity
    //      mapping by default (input pitch degrees == resolver angle
    //      degrees); the user can edit the breakpoint table to compensate
    //      for synchro drift at specific pitch angles.
    //
    //   2. Roll sin/cos pair (kind="piecewise_resolver") — identity by
    //      default, full ±180° range editable per row.
    //
    //   3. OFF flag (kind="digital_invert") — single boolean inversion.
    //
    // No legacy bare-property fields; this gauge has never had a config
    // file consumed by SimLinkup before today.
    [Serializable]
    [XmlRoot(nameof(Simtek10067202HardwareSupportModule))]
    public class Simtek10067202HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek10067202HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek10067202HardwareSupportModuleConfig>(filePath);
        }
    }
}
