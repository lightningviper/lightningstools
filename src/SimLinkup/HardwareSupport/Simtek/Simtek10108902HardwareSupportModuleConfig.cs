using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Simtek
{
    // Per-gauge calibration config for Simtek 10-1089-02 (F-16 Fuel Quantity
    // Indicator v2). Three piecewise channels:
    //
    //   1. 10108902_Counter_To_Instrument — 11-point linear table across
    //      the C# range 0..20100 lbs.
    //   2. 10108902_AL_To_Instrument      — 9 spec test points 0..4200 lbs
    //      (same scale as 10-0294 AL).
    //   3. 10108902_FR_To_Instrument      — 9 spec test points, separate
    //      from AL so the two pointers can be calibrated independently.
    //
    // No legacy bare-property fields on this gauge — the counter
    // denominator (20100) is hardcoded in the original HSM (no
    // LoadConfig path before today), so there's nothing to round-trip
    // for back-compat. Existing user installs that have no on-disk
    // config file simply pick up the default piecewise tables on first
    // save from the editor.
    //
    // Note: at least one shipping sample profile (Kukki) has a hand-edited
    // `<MaxPoundsTotalFuel>22200</MaxPoundsTotalFuel>` on disk. The original
    // C# HSM never read it (the field was inert), so we don't honour it
    // either — that user's value never had an effect, and the new piecewise
    // counter table is the canonical authoring surface going forward. The
    // editor will overwrite the file with the new <Channels> schema on
    // next save.
    [Serializable]
    [XmlRoot(nameof(Simtek10108902HardwareSupportModule))]
    public class Simtek10108902HardwareSupportModuleConfig : GaugeCalibrationConfig
    {
        public static Simtek10108902HardwareSupportModuleConfig Load(string filePath)
        {
            return GaugeCalibrationConfig.Load<Simtek10108902HardwareSupportModuleConfig>(filePath);
        }
    }
}
