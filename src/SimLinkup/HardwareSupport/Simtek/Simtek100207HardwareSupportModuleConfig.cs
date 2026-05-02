using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0207 (F-16 RPM Tachometer).
    // Authored by the SimLinkup Profile Editor; consumed by
    // Simtek100207HardwareSupportModule.LoadConfig() to override the hardcoded
    // piecewise breakpoint table when present.
    //
    // The schema lives on the GaugeCalibrationConfig base. This subclass exists
    // only so XmlSerializer uses the right root element name ("Simtek100207HardwareSupportModule")
    // — matching what the editor writes and the existing Simtek100285 / Simtek100294
    // precedent.
    //
    // No fields; the class is intentionally empty. The .Load(filePath) helper
    // round-trips the inherited Channels[] array.
    [Serializable]
    [XmlRoot(nameof(Simtek100207HardwareSupportModule))]
    public class Simtek100207HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100207HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100207HardwareSupportModuleConfig>(filePath);
        }
    }
}
