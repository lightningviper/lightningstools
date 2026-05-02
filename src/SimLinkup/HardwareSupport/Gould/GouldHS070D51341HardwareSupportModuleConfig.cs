using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Gould
{
    // Per-gauge calibration config for Gould HS070D5134-1 (F-16 Standby
    // Compass). Single piecewise_resolver pair (sin/cos) mapping 0..360°
    // magnetic heading to a continuous resolver angle. Same gauge family as
    // Simtek 10-1079.
    [Serializable]
    [XmlRoot(nameof(GouldHS070D51341HardwareSupportModule))]
    public class GouldHS070D51341HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static GouldHS070D51341HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<GouldHS070D51341HardwareSupportModuleConfig>(filePath);
        }
    }
}
