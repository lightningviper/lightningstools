using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Lilbern
{
    // Per-gauge calibration config for Lilbern 3239 (F-16A Fuel Flow
    // Indicator). Single piecewise channel — linear mapping 0..80000 lbs/hr
    // to -10..+10 V.
    [Serializable]
    [XmlRoot(nameof(Lilbern3239HardwareSupportModule))]
    public class Lilbern3239HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Lilbern3239HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Lilbern3239HardwareSupportModuleConfig>(filePath);
        }
    }
}
