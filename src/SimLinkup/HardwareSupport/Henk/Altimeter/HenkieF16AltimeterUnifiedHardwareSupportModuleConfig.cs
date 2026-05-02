using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Henk.Altimeter
{
    // Unified-schema calibration config for the Henkie F-16 altimeter
    // drive board, authored by the SimLinkup Profile Editor.
    //
    // This file lives ALONGSIDE the legacy HenkieF16Altimeter.config
    // (distinct filename, no collision). The legacy file continues to
    // own everything that's not the calibration table:
    //   - Address, COMPort, ConnectionType, DiagnosticLEDMode
    //   - StatorBaseAnglesConfig (S1/S2/S3 base angles)
    //   - OutputChannelsConfig (DIG_OUT_1/2/3 initial values)
    //   - MinBaroPressureInHg / MaxBaroPressureInHg /
    //     IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro (the
    //     baro-compensation triangle that biases altitude before the
    //     calibration table is consulted)
    //
    // The unified file owns just the input→DAC breakpoint table, in the
    // editor's standard <Channels>/<Transform kind="piecewise"> shape.
    //
    // Filename: HenkieF16AltimeterHardwareSupportModule.config — matches
    // the editor's filename auto-derivation from the gauge's `cls`
    // (SimLinkup.HardwareSupport.Henk.Altimeter.HenkieF16Altimeter
    // HardwareSupportModule). Distinct from the legacy
    // HenkieF16Altimeter.config so no override is needed in the editor's
    // GAUGE_CONFIG_FILENAME_OVERRIDES.
    //
    // One overridable channel:
    //   - HenkieF16Altimeter_Indicator_Position_To_Instrument
    //     (input is altitude mod 10000 ft after baro compensation;
    //     output is DAC counts 0..4095 driving the synchro stator)
    //
    // Editor-side counterpart: src/js/gauges/henkie-altimeter.js.
    [Serializable]
    [XmlRoot("HenkieF16AltimeterHardwareSupportModule")]
    public class HenkieF16AltimeterUnifiedHardwareSupportModuleConfig : GaugeCalibrationConfig
    {
    }
}
