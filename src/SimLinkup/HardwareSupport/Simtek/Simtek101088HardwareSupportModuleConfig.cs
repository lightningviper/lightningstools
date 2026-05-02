using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1088 (F-16 Nozzle Position
    // Indicator). Resolver pair: input 0..100% maps to angle 0..225° (gauge
    // sweep), sin/cos × peakVolts (default 10) drive the resolver windings.
    //
    // The schema lives on the GaugeCalibrationConfig base. The two output
    // channels (sin and cos) are flat <Channel> records; the SIN channel
    // carries the full transform body, the COS channel just points back to
    // its sin partner via Transform.PartnerChannel — see the schema docs in
    // GaugeCalibrationConfig.cs for the round-trip contract.
    [Serializable]
    [XmlRoot(nameof(Simtek101088HardwareSupportModule))]
    public class Simtek101088HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101088HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101088HardwareSupportModuleConfig>(filePath);
        }
    }
}
