using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0295 (F-16 Fuel Flow
    // Indicator). Single piecewise channel — dual-scale fuel flow gauge
    // (low-flow + high-flow pointers riding the same DAC channel via an
    // intentional discontinuity at 10000 lb/hr).
    [Serializable]
    [XmlRoot(nameof(Simtek100295HardwareSupportModule))]
    public class Simtek100295HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100295HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100295HardwareSupportModuleConfig>(filePath);
        }
    }
}
