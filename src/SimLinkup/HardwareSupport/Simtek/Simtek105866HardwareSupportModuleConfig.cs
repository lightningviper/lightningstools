using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-5866 (F-16 Fuel Quantity
    // Indicator, multi-pointer with remote electronics 50-4363-01).
    //
    // Three logical channels carried in the GaugeCalibrationConfig.Channels
    // array (all kind="piecewise"):
    //
    //   105866_AL_To_Instrument        — A/L pointer (Table 1, 0..4200 LBS)
    //   105866_FR_To_Instrument        — F/R pointer (Table 1, 0..4200 LBS)
    //   105866_Counter_To_Instrument   — TOTAL counter (Table 2, 0..20000 LBS)
    //
    // The A/L and F/R pointers share the same calibration table per the
    // spec sheet (PL20-5866 Table 1 — "SEL LBS X 100"); they're driven by
    // the cockpit selector switch through separate signal pins (J, K) but
    // both terminate at the same servo via the remote electronics box.
    [Serializable]
    [XmlRoot(nameof(Simtek105866HardwareSupportModule))]
    public class Simtek105866HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek105866HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek105866HardwareSupportModuleConfig>(filePath);
        }
    }
}
