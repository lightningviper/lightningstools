using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.AMI
{
    // Per-gauge calibration config for AMI 90002620-01 (F-16 Cabin Pressure
    // Altimeter). Single piecewise channel — 0..50000 ft → -10..+10 V linear.
    [Serializable]
    [XmlRoot(nameof(AMI9000262001HardwareSupportModule))]
    public class AMI9000262001HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static AMI9000262001HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<AMI9000262001HardwareSupportModuleConfig>(filePath);
        }
    }
}
