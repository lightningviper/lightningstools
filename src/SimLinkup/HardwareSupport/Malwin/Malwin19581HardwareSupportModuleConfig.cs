using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Malwin
{
    // Per-gauge calibration config for Malwin 19581 (F-16 Hydraulic Pressure
    // Indicator). Two independent piecewise_resolver pairs (A and B systems),
    // each mapping 0..4000 PSI to a sin/cos resolver pair via the dial's
    // 320° mechanical sweep.
    [Serializable]
    [XmlRoot(nameof(Malwin19581HardwareSupportModule))]
    public class Malwin19581HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Malwin19581HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Malwin19581HardwareSupportModuleConfig>(filePath);
        }
    }
}
