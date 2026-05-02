using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0194 (F-16 Mach/Airspeed
    // Indicator). Authored by the SimLinkup Profile Editor; consumed by
    // Simtek100194HardwareSupportModule.LoadConfig() to override the hardcoded
    // 43-knot airspeed breakpoint table and to apply per-channel trim to both
    // the airspeed AND the cross-coupled Mach output.
    //
    // The Mach channel is cross-coupled — its voltage is computed from the
    // current airspeed output voltage, not from the Mach input alone. The
    // hardcoded reference voltage table and coupling math stay in
    // UpdateMachOutputValues() unchanged; only ZeroTrim/GainTrim from the
    // config are applied at the end. The editor's Calibration tab surfaces
    // the trim fields on the cross-coupled stub for exactly this purpose.
    //
    // No fields; the schema lives on GaugeCalibrationConfig.
    [Serializable]
    [XmlRoot(nameof(Simtek100194HardwareSupportModule))]
    public class Simtek100194HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100194HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100194HardwareSupportModuleConfig>(filePath);
        }
    }
}
