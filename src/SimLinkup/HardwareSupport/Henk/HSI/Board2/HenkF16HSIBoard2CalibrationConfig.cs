using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board2
{
    // Unified-schema calibration config for the Henk F-16 HSI Board 2,
    // authored by the SimLinkup Profile Editor.
    //
    // This file lives ALONGSIDE the legacy
    // HenkF16HSIBoard2HardwareSupportModule.config — same split-file
    // contract as Board 1. The legacy file owns identity / stator
    // offsets / hysteresis thresholds / DIG_OUTs; this file owns the
    // single course-deviation breakpoint table.
    //
    // Filename: HenkF16HSIBoard2Calibration.config — distinct from the
    // legacy HenkF16HSIBoard2HardwareSupportModule.config.
    //
    // One overridable channel:
    //   - Henk_F16_HSI_Board2_Course_Deviation_Indicator_Position_To_Instrument
    //     (input is normalized deviation -1..+1 = courseDeviationDegrees /
    //     courseDeviationLimitDegrees; output is DAC counts 0..1023)
    //
    // Editor-side counterpart: src/js/gauges/henk-hsi-board2.js +
    // src/js/gauge-config-io.js's GAUGE_CONFIG_FILENAME_OVERRIDES.
    [Serializable]
    [XmlRoot("HenkF16HSIBoard2Calibration")]
    public class HenkF16HSIBoard2CalibrationConfig : GaugeCalibrationConfig
    {
    }
}
