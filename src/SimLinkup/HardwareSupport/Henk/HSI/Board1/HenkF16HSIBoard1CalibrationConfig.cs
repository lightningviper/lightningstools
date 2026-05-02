using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board1
{
    // Unified-schema calibration config for the Henk F-16 HSI Board 1,
    // authored by the SimLinkup Profile Editor.
    //
    // This file lives ALONGSIDE the legacy
    // HenkF16HSIBoard1HardwareSupportModule.config — it does NOT replace
    // it. The legacy file continues to own everything that's not the
    // calibration tables (Address, COMPort, ConnectionType, stator
    // offsets, DIG_OUT initial values). The unified file owns just the
    // five input→DAC breakpoint tables, in the editor's standard
    // <Channels>/<Transform kind="piecewise"> shape.
    //
    // Why split?
    //   - The editor's Calibration tab speaks the unified schema for
    //     every gauge. Folding stator offsets / DIG_OUTs into the
    //     unified schema would pollute it with Henk-specific fields
    //     the other gauges don't need.
    //   - The legacy file's identity / stator / DIG_OUT fields are
    //     settled once on bench-up and rarely touched — leaving them
    //     in the legacy file means hand-edits made before the editor
    //     knew about this gauge are preserved verbatim.
    //
    // Filename: HenkF16HSIBoard1Calibration.config — distinct from the
    // legacy HenkF16HSIBoard1HardwareSupportModule.config to avoid
    // the editor's filename auto-derivation overwriting the legacy file.
    // The matching editor-side filename override lives in
    // src/js/gauge-config-io.js's GAUGE_CONFIG_FILENAME_OVERRIDES.
    //
    // Five overridable channels (all piecewise tables driving DAC counts
    // to synchro stators on the indicator):
    //   - Henk_F16_HSI_Board1_Magnetic_Heading_To_Instrument  (0..360°  → 0..1023)
    //   - Henk_F16_HSI_Board1_Bearing_To_Instrument           (0..360°  → 0..1023)
    //   - Henk_F16_HSI_Board1_Range_x100_To_Instrument        (0..10    → 0..1023)
    //   - Henk_F16_HSI_Board1_Range_x10_To_Instrument         (0..10    → 0..1023)
    //   - Henk_F16_HSI_Board1_Range_x1_To_Instrument          (0..10    → 0..1023)
    //
    // Editor-side counterpart: src/js/gauges/henk-hsi-board1.js +
    // src/js/gauge-config-io.js's GAUGE_CONFIG_FILENAME_OVERRIDES.
    [Serializable]
    [XmlRoot("HenkF16HSIBoard1Calibration")]
    public class HenkF16HSIBoard1CalibrationConfig : GaugeCalibrationConfig
    {
    }
}
