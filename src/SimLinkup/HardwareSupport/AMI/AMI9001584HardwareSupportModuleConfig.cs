using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.AMI
{
    // Per-gauge calibration config for AMI 9001584 (F-16 Fuel Quantity
    // Indicator). Three piecewise channels — counter (0..18000 lbs) +
    // AL/FR pointers (0..4200 lbs each, ±7 V swing for the Nigel-modded
    // 3-turn pot variant). No legacy bare-property fields.
    [Serializable]
    [XmlRoot(nameof(AMI9001584HardwareSupportModule))]
    public class AMI9001584HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static AMI9001584HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<AMI9001584HardwareSupportModuleConfig>(filePath);
        }
    }
}
