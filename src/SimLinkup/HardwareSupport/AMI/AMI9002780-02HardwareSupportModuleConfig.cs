using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.AMI
{
    // Per-gauge calibration config for AMI 9002780-02 (F-16 Primary ADI,
    // smaller variant with single-channel pitch). 9 calibratable channels:
    //
    //   - Pitch                     (piecewise — single channel, linear)
    //   - Roll SIN/COS pair         (piecewise_resolver, identity ±180°)
    //   - 4 digital flags           (digital_invert × 4)
    //   - Horizontal command bar    (piecewise; show/hide gating stays in C#)
    //   - Vertical command bar      (piecewise; show/hide gating stays in C#)
    //   - Rate of turn              (piecewise, ±100% × 10 V)
    [Serializable]
    [XmlRoot(nameof(AMI900278002HardwareSupportModule))]
    public class AMI900278002HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static AMI900278002HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<AMI900278002HardwareSupportModuleConfig>(filePath);
        }
    }
}
