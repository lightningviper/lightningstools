using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1091 (F-16 Engine Oil
    // Pressure Indicator). Resolver pair: input 0..100% drives a 320°
    // sweep on the dial; sin/cos × peakVolts drive the synchro windings.
    //
    // Same shape as the Simtek 10-1088 nozzle config — the GaugeCalibrationConfig
    // base carries the schema; this subclass only pins the on-disk root
    // element name to the gauge HSM class.
    [Serializable]
    [XmlRoot(nameof(Simtek101091HardwareSupportModule))]
    public class Simtek101091HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101091HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101091HardwareSupportModuleConfig>(filePath);
        }
    }
}
