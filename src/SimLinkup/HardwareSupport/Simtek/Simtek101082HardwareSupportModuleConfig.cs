using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1082 (F-16 Airspeed/Mach
    // Indicator v2). Two channels:
    //
    //   1. 101082_Airspeed_To_Instrument (kind="piecewise") —
    //      43-breakpoint piecewise table converting input airspeed in
    //      knots to a -10..+10 V output that drives the airspeed needle
    //      directly via the gauge's internal servo.
    //
    //   2. 101082_Mach_To_Instrument (kind="piecewise", coupledTo=...) —
    //      Cross-coupled. The piecewise table here produces a "reference
    //      voltage" representing the user-requested Mach number; the HSM
    //      combines that reference voltage with the current airspeed
    //      output voltage AND the gauge's geometry (Mach 1 ref angle 131°,
    //      262° angular range) to compute the final DAC voltage that
    //      positions the Mach pointer relative to the airspeed needle.
    //
    // Same family as 10-0194; the spec sheet has a coarser Mach reference
    // table (6 points vs 36 in 10-0194). The cross-coupling math itself is
    // gauge geometry and stays hardcoded in the HSM — only the reference
    // voltage table is editable.
    [Serializable]
    [XmlRoot(nameof(Simtek101082HardwareSupportModule))]
    public class Simtek101082HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101082HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101082HardwareSupportModuleConfig>(filePath);
        }
    }
}
