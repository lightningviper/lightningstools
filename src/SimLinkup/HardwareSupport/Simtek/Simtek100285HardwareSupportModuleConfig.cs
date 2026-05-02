using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0285 (F-16 Altimeter).
    //
    // Two phases of file format coexist in this single class:
    //
    // 1. Legacy bare-property schema (predates the editor):
    //      <MinBaroPressureInHg>             pre-transform baro adjust min
    //      <MaxBaroPressureInHg>             pre-transform baro adjust max
    //      <IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro>
    //      <AltitudeZeroOffsetInFeet>
    //    These are read by older SimLinkup builds to bias the displayed altitude
    //    based on the Kollsman knob setting. Newer SimLinkup builds use BMS's
    //    already-baro-compensated altitude (`aauz`) directly and bypass these
    //    fields when phase 2 is present (see UpdateAltitudeOutputValues).
    //
    // 2. Editor-authored <Channels> schema (inherited from GaugeCalibrationConfig):
    //    Two `multi_resolver` pairs — fine (4000 ft/rev) and coarse
    //    (100000 ft/rev) — each carrying UnitsPerRevolution + PeakVolts on
    //    the SIN side and per-channel ZeroTrim/GainTrim.
    //
    // When `Channels` is non-empty, the HSM treats it as authoritative and
    // ignores the bare baro fields. When it's null/empty, the HSM falls back
    // to the legacy baro math so existing user installs that have a hand-
    // edited 10-0285 config but no editor-authored <Channels> block keep
    // behaving exactly as they do today.
    [Serializable]
    [XmlRoot(nameof(Simtek100285HardwareSupportModule))]
    public class Simtek100285HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public double? MinBaroPressureInHg { get; set; } = 28.09;
        public double? MaxBaroPressureInHg { get; set; } = 31.025;
        public double? IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro { get; set; } = 2800;
        public double? AltitudeZeroOffsetInFeet { get; set; } = 0;

        public static Simtek100285HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100285HardwareSupportModuleConfig>(filePath);
        }
    }
}
