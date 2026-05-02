using System;
using System.Xml.Serialization;
using Common.HardwareSupport.Calibration;

namespace SimLinkup.HardwareSupport.Henk.FuelFlow
{
    // Unified-schema calibration config for the Henkie F-16 fuel flow
    // indicator, authored by the SimLinkup Profile Editor.
    //
    // This file lives ALONGSIDE the legacy
    // HenkieF16FuelFlowIndicator.config — it does NOT replace it. The
    // legacy file continues to own everything that's not the calibration
    // table (stator base angles, COM port, address, connection type,
    // diagnostic LED mode, DIG_OUT initial values). The unified file
    // owns just the input→DAC breakpoint table, in the editor's standard
    // <Channels>/<Transform kind="piecewise"> shape.
    //
    // Why split?
    //   - The editor's Calibration tab speaks the unified schema for
    //     every gauge (32 gauges + this one). Trying to fold stator
    //     angles + DIG_OUTs into the unified schema would pollute it
    //     with Henk-specific fields the other 32 gauges don't need.
    //   - The legacy file's identity/stator/DIG_OUT fields are settled
    //     once on bench-up and rarely touched — leaving them in the
    //     legacy file means hand-edits made before the editor knew
    //     about this gauge are preserved verbatim.
    //   - Keeping calibration in its own file makes hot-reload a
    //     small, well-bounded operation: when the editor saves, only
    //     the calibration file's mtime changes, so the gauge HSM's
    //     FileSystemWatcher only re-evaluates the breakpoint table —
    //     no need to reconnect serial ports or re-program stators.
    //
    // Filename: HenkieF16FuelFlowHardwareSupportModule.config (NO
    // "Indicator" suffix, distinguishing it from the legacy
    // HenkieF16FuelFlowIndicator.config). Root element name matches.
    //
    // Editor-side counterpart: src/js/gauges/henkie-fuelflow.js +
    // src/js/gauge-config-io.js's GAUGE_CONFIG_FILENAME_OVERRIDES.
    [Serializable]
    [XmlRoot("HenkieF16FuelFlowHardwareSupportModule")]
    public class HenkieF16FuelFlowHardwareSupportModuleConfig : GaugeCalibrationConfig
    {
    }
}
