using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0207_110 (F-16 RPM Tachometer
    // v2). Same channel id as 10-0207 ("100207_RPM_To_Instrument" — they share
    // the digit prefix), but a separate .config file because the file basename
    // is keyed by the gauge's class short name (Simtek100207_110... vs
    // Simtek100207...). SimLinkup loads each independently from its own file.
    //
    // The schema lives on the GaugeCalibrationConfig base. This subclass is
    // empty by design; the [XmlRoot] attribute pins the on-disk root element
    // name so it round-trips with what the editor writes.
    [Serializable]
    [XmlRoot(nameof(Simtek100207_110HardwareSupportModule))]
    public class Simtek100207_110HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek100207_110HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100207_110HardwareSupportModuleConfig>(filePath);
        }
    }
}
