using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.AMI
{
    // Per-gauge calibration config for AMI 9001580-01 (F-16 HSI). 11 direct
    // calibratable channels:
    //
    //   - Compass SIN/COS pair         (piecewise_resolver, identity 0..360°)
    //   - Course SIN/COS pair          (piecewise_resolver, identity 0..360°)
    //   - Course Deviation             (piecewise — linear, ±limit × ±10 V)
    //   - DME × 100 SIN/COS pair       (multi_resolver, 10 digit-units/rev)
    //   - DME × 10  SIN/COS pair       (multi_resolver, 10 digit-units/rev)
    //   - DME × 1   SIN/COS pair       (multi_resolver, 10 digit-units/rev)
    //   - 5 digital flags              (digital_invert × 5: pass-through)
    //
    // Two channels are NOT directly calibratable because their output is
    // CROSS-COUPLED to other inputs:
    //   - Bearing SIN/COS — depends on (compass, bearing-to-beacon)
    //   - Heading SIN/COS — depends on (compass, desired-heading)
    // The cross-coupling math stays hardcoded in the HSM. Users calibrate
    // these by tuning the compass channel; the bearing/heading pointers
    // follow geometrically.
    [Serializable]
    [XmlRoot(nameof(AMI900158001HardwareSupportModule))]
    public class AMI900158001HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static AMI900158001HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<AMI900158001HardwareSupportModuleConfig>(filePath);
        }
    }
}
