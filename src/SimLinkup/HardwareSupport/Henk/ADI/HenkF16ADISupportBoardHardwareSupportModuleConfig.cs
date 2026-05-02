using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Henk.ADI
{
    // Unified-schema calibration config for the Henk F-16 ADI Support
    // Board (ARU-50/A primary ADI), authored by the SimLinkup Profile
    // Editor.
    //
    // Unlike most other gauge HSMs that ship with a legacy config file
    // alongside the unified one, this board had NO config file before
    // editor support — its calibration was entirely hardcoded constants
    // in the HSM. This config is therefore the sole source of editor-
    // authored overrides; absent file = HSM uses hardcoded defaults
    // (no behaviour change for users who haven't adopted the editor).
    //
    // Five overridable channels, all piecewise-table:
    //   - HenkF16ADISupportBoard_Pitch_To_SDI            (input ±90°,  output 169..679)
    //   - HenkF16ADISupportBoard_Roll_To_SDI             (input ±180°, output 0..1024)
    //   - HenkF16ADISupportBoard_Horizontal_GS_Bar_To_SDI (input ±1,    output 0..1)
    //   - HenkF16ADISupportBoard_Vertical_GS_Bar_To_SDI   (input ±1,    output 0..1)
    //   - HenkF16ADISupportBoard_Rate_Of_Turn_To_SDI     (input ±1,    output 0..1)
    //
    // The two command-bar channels also honour the unified schema's
    // HiddenOutput field — when the board's commandBarsVisible digital
    // input is FALSE, the bar is parked at HiddenOutput rather than
    // being driven from its breakpoint table. Defaults: 1.0 horizontal,
    // 0.0 vertical (matches the hardcoded values).
    //
    // Filename: HenkF16ADISupportBoardHardwareSupportModule.config.
    // Root element name matches.
    //
    // Editor-side counterpart: src/js/gauges/henk-adi.js +
    // src/js/gauge-config-io.js's GAUGE_CONFIG_FILENAME_OVERRIDES.
    [Serializable]
    [XmlRoot("HenkF16ADISupportBoardHardwareSupportModule")]
    public class HenkF16ADISupportBoardHardwareSupportModuleConfig : GaugeCalibrationConfig
    {
    }
}
