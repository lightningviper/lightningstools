using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1079 (F-16 Standby Compass).
    // piecewise_resolver pair, continuous 0..360° dial. Input is magnetic
    // heading in degrees; default mapping is identity (input degrees ==
    // reference angle degrees, encoded as 13 breakpoints every 30°). Output
    // is sin/cos × peakVolts (10 V). The piecewise table lets users correct
    // local synchro drift at specific headings.
    [Serializable]
    [XmlRoot(nameof(Simtek101079HardwareSupportModule))]
    public class Simtek101079HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek101079HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek101079HardwareSupportModuleConfig>(filePath);
        }
    }
}
