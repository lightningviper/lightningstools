using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-0294 (F-16 Fuel Quantity
    // Indicator).
    //
    // Two phases of file format coexist in this single class:
    //
    // 1. Legacy bare-property schema (predates the editor):
    //      <MaxPoundsTotalFuel>            counter linear-rescale denominator
    //    Older SimLinkup builds rescale the counter output as
    //    `(input / MaxPoundsTotalFuel) * 20 - 10`. Default 9900 matches the
    //    spec sheet's full-scale; users with non-9900 aircraft can dial it
    //    down here.
    //
    // 2. Editor-authored <Channels> schema (inherited from GaugeCalibrationConfig):
    //    Three independent piecewise channels — Counter, AL pointer, FR
    //    pointer — each with its own breakpoint table and per-channel trim.
    //    The piecewise counter table fully subsumes MaxPoundsTotalFuel: to
    //    change the input that maps to +10 V, edit the last counter
    //    breakpoint's input.
    //
    // When `Channels` is non-empty the HSM treats it as authoritative for
    // any channel that has an override; the bare MaxPoundsTotalFuel field
    // only drives channels that lack an override (matches the 10-0285
    // pattern). Existing user installs that have a hand-edited 10-0294
    // config but no editor-authored <Channels> block keep behaving exactly
    // as they do today.
    [Serializable]
    [XmlRoot(nameof(Simtek100294HardwareSupportModule))]
    public class Simtek100294HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public uint? MaxPoundsTotalFuel { get; set; } = 9200;

        public static Simtek100294HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek100294HardwareSupportModuleConfig>(filePath);
        }
    }
}
