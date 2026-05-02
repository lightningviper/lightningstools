using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-5862 (F-16 Nozzle Position
    // Indicator). Single resolver pair:
    //
    //   105862_Nozzle_Position_SIN_To_Instrument (carries the breakpoint table)
    //   105862_Nozzle_Position_COS_To_Instrument (role="cos", points back)
    //
    // Default breakpoints come from PL20-5862 Table 1 — 6 spec-sheet test
    // points: 0..100% open mapped to 0..225° reference angle linearly
    // (45° per 20% increment). Drive type: single DC synchro.
    [Serializable]
    [XmlRoot(nameof(Simtek105862HardwareSupportModule))]
    public class Simtek105862HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek105862HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek105862HardwareSupportModuleConfig>(filePath);
        }
    }
}
