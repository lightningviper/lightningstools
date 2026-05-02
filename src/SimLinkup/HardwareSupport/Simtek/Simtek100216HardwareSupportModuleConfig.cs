using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0216 (F-16 FTIT Indicator).
    // Single piecewise channel — 4 breakpoints from 200°C to 1200°C mapping
    // to ±10 V. Schema lives on GaugeCalibrationConfig; this subclass pins
    // the on-disk root element name to the gauge HSM class.
    [Serializable]
    [XmlRoot(nameof(Simtek100216HardwareSupportModule))]
    public class Simtek100216HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100216HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100216HardwareSupportModuleConfig>(filePath);
        }
    }
}
