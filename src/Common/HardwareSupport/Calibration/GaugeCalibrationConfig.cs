using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Common.HardwareSupport.Calibration
{
    // Shared base for per-gauge calibration configs authored by the SimLinkup
    // Profile Editor. Each gauge HSM that opts in derives a thin subclass with
    // [XmlRoot(nameof(<TheModule>))] so the on-disk root element name matches
    // the gauge HSM class name (matches the existing Simtek100285 / Simtek100294
    // precedent — and what the editor writes).
    //
    // Schema (matches the editor's gauge-config-io.js round-trip):
    //   <YourModuleName>
    //     <Channels>
    //       <Channel id="<HSM-port-id>">
    //         <Transform kind="piecewise|linear|resolver|multi_resolver|cross_coupled">
    //           <Breakpoints>             <!-- piecewise only -->
    //             <Point input="..." volts="..."/>
    //             ...
    //           </Breakpoints>
    //         </Transform>
    //         <ZeroTrimVolts>0</ZeroTrimVolts>
    //         <GainTrim>1</GainTrim>
    //         <CoupledTo>...</CoupledTo>  <!-- cross_coupled only, optional -->
    //       </Channel>
    //     </Channels>
    //   </YourModuleName>
    //
    // Today only the 'piecewise' kind is honoured by SimLinkup's gauge HSMs;
    // the others round-trip safely so files survive future schema growth.
    [Serializable]
    public class GaugeCalibrationConfig
    {
        [XmlArray("Channels")]
        [XmlArrayItem("Channel")]
        public GaugeChannelConfig[] Channels { get; set; }

        // Set by GetInstances after Load() so the gauge HSM's FileSystemWatcher
        // knows which file to watch for hot-reload. [XmlIgnore] keeps it out of
        // the round-trip (matches the ArduinoSeatHardwareSupportModuleConfig
        // precedent).
        [XmlIgnore]
        public string FilePath { get; set; }

        // Find a channel by its HSM port Id. Returns null when not present —
        // gauge HSMs treat that as "no override; use hardcoded behaviour."
        public GaugeChannelConfig FindChannel(string id)
        {
            if (Channels == null || string.IsNullOrEmpty(id)) return null;
            return Channels.FirstOrDefault(c => c != null && c.Id == id);
        }

        // Generic loader. The caller passes T = derived per-gauge subclass and
        // gets a strongly-typed result back. Safe to call when the file is
        // absent — returns null in that case (callers fall back to hardcoded
        // behaviour, matching the 10-0285 / 10-0294 precedent).
        //
        // Hot-reload-safe: this loader is what every editor-authored gauge
        // config flows through, both at SimLinkup startup AND on every
        // FileSystemWatcher Changed event. The watcher path races the
        // editor's writer — the SimLinkup Profile Editor saves via Node's
        // fs.writeFileSync, which on Windows triggers Changed events both
        // when the file is truncated (size 0, XmlSerializer fails because
        // the document is empty) AND when content is fully written. The
        // first event arrives mid-write; without retries the watcher's
        // change handler bails on it and the dedup mtime check skips
        // subsequent events because LastWriteTime didn't change between
        // truncate and write-complete on the same save. We close that
        // race two ways:
        //   1. Open the file with FileShare.ReadWrite so a writer holding
        //      the file with default sharing doesn't lock us out
        //      ("being used by another process" IOException).
        //   2. Retry on transient failures (IOException + the
        //      InvalidOperationException XmlSerializer throws on partial
        //      documents) — 6 attempts × 50ms = ~300ms total, which
        //      comfortably covers Node's writeFileSync window.
        //
        // Startup reads (no concurrent writer) hit the first attempt and
        // return immediately, so this is free for the common case.
        public static T Load<T>(string filePath) where T : GaugeCalibrationConfig
        {
            return Load<T>(filePath, attempts: 6, delayMs: 50);
        }

        public static T Load<T>(string filePath, int attempts, int delayMs) where T : GaugeCalibrationConfig
        {
            if (filePath == null) throw new ArgumentNullException("filePath");
            if (!System.IO.File.Exists(filePath)) return default(T);
            if (attempts < 1) attempts = 1;
            Exception last = null;
            var serializer = new XmlSerializer(typeof(T));
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    using (var fs = new System.IO.FileStream(
                        filePath,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read,
                        System.IO.FileShare.ReadWrite))
                    {
                        return (T)serializer.Deserialize(fs);
                    }
                }
                catch (System.IO.IOException e)
                {
                    last = e;  // editor's writer still holds the file — retry
                }
                catch (System.InvalidOperationException e)
                {
                    last = e;  // XmlSerializer hit a partial / empty file — retry
                }
                if (i + 1 < attempts && delayMs > 0)
                {
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            // All retries exhausted; surface the last error so the caller's
            // catch (every change-handler we've written) logs it.
            if (last != null) throw last;
            return default(T);
        }

        // Mirror of Save in the existing Simtek100285HardwareSupportModuleConfig
        // precedent. Not used by SimLinkup itself today (the editor authors the
        // file); included for symmetry and potential future round-trip tooling.
        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }

    // Per-channel record. Distinguished by `Id` (matches the gauge HSM port
    // signal Id, e.g. "100207_RPM_To_Instrument").
    [Serializable]
    public class GaugeChannelConfig
    {
        // The HSM output port Id this channel calibrates. Required.
        [XmlAttribute("id")]
        public string Id { get; set; }

        // Transform record describing how input → volts is computed.
        // Optional: when absent (or kind is unknown), gauge HSMs fall back to
        // their hardcoded behaviour and still apply ZeroTrim/Gain on top.
        [XmlElement("Transform")]
        public GaugeTransformConfig Transform { get; set; }

        // Per-channel zero-volt offset added to the computed transform output
        // before the ±10 V clamp. 0 by default. Useful for trimming the gauge's
        // physical needle zero point without changing the breakpoint table.
        // Implemented as a nullable so we can distinguish "explicitly 0 in
        // file" from "absent in file"; both behave the same way at runtime
        // (additive offset of 0 is a no-op), but this lets future tooling
        // tell the difference.
        [XmlElement("ZeroTrimVolts")]
        public double? ZeroTrimVolts { get; set; }

        // Per-channel gain multiplier applied to the transform output before
        // the ZeroTrim. 1.0 by default (unity). Useful for trimming full-scale
        // span without re-tabulating breakpoints. Same nullable semantics as
        // ZeroTrimVolts.
        [XmlElement("GainTrim")]
        public double? GainTrim { get; set; }

        // Names another channel this one is computed from (cross_coupled).
        // Round-trip-only today — SimLinkup's gauge HSMs don't yet act on it.
        [XmlElement("CoupledTo")]
        public string CoupledTo { get; set; }

        // For digital_invert channels: whether the digital output is the
        // logical inverse of the input. Defaults to false when absent. The
        // 10-1084 ADI OFF flag uses this — its input is "OFF Flag Visible
        // (1=visible)" and output is "OFF Flag Hidden (1=hidden)".
        [XmlElement("Invert")]
        public bool? Invert { get; set; }

        // Caged-rest behaviour for standby ADI resolver pairs (pitch / roll
        // SIN side carries these on each pair). When the gauge's OFF flag
        // input is TRUE, the gauge's gimbal is mechanically caged — the
        // gyro is at rest at whatever orientation it was in when last spun
        // down, which is effectively random per power-cycle. The HSM picks
        // a random angle in [Min, Max] degrees ONCE at construction time
        // and drives sin/cos × PeakVolts to that angle for as long as the
        // OFF flag stays TRUE. When the OFF flag goes FALSE (gauge spun
        // up + uncaged), the HSM reverts to the configured piecewise
        // transform and follows the input normally.
        //
        // Opt-in via CagedRestEnabled. Defaults to disabled (false / null)
        // so existing profiles keep their pre-feature behaviour: when the
        // OFF flag is visible the HSM continues to follow pitch/roll
        // input regardless. Users who want the random rest behaviour
        // enable it per gauge in the editor's Calibration tab.
        //
        // Only honoured by HSMs that opt in at the C# level (today:
        // 10-0335-01 and 10-1084 standby ADIs, on their pitch and roll
        // SIN channels). Editor defaults: pitch ±20°, roll ±40°. Set
        // both Min and Max to the same value for a fixed (non-random)
        // rest angle.
        [XmlElement("CagedRestEnabled")]
        public bool? CagedRestEnabled { get; set; }

        [XmlElement("CagedRestRangeMinDegrees")]
        public double? CagedRestRangeMinDegrees { get; set; }

        [XmlElement("CagedRestRangeMaxDegrees")]
        public double? CagedRestRangeMaxDegrees { get; set; }

        // Output value driven by the HSM when this channel's "visible"
        // gate is FALSE. Today only used by gauges with a digital
        // visibility flag inhibiting an analog channel — Henk F-16 ADI
        // Support Board's two command bars are the canonical example
        // (when commandBarsVisible is low, the bar parks at a fixed
        // off-screen position). Nullable: absent means "no hidden
        // behaviour" and the channel is driven from its transform
        // unconditionally.
        //
        // The HSM is the source of truth for which channels honour
        // this — opt-in per channel in the HSM's Update*OutputValues
        // method. The schema field is generic so any future
        // flag-gated analog channel can use it without re-revving
        // the file format.
        [XmlElement("HiddenOutput")]
        public double? HiddenOutput { get; set; }

        // Apply ZeroTrim + GainTrim to a computed voltage, then clamp to
        // [outputMin, outputMax] — the gauge's real hardware safe envelope
        // (e.g. Westin EPU is 0.1..2.0 V, not the universal ±10 V). Callers
        // pass the matching output signal's MinValue/MaxValue. The output
        // signal is the canonical source of truth for what voltage is safe
        // to send to the physical gauge mechanism, so trim values that
        // would push the output past those bounds get clipped at the
        // boundary instead of damaging the hardware.
        //
        // Order: gain first, then offset, then per-gauge clamp (matches
        // what users intuitively expect when they say "make full-scale 5%
        // wider then nudge zero up by 0.1 V").
        //
        // If outputMin >= outputMax (degenerate / unset bounds), returns 0
        // so the gauge is visibly dead instead of quietly damaged. An HSM
        // that forgets to declare MinValue/MaxValue on its output signal
        // gets noticed on day one of testing rather than later.
        public double ApplyTrim(double volts, double outputMin, double outputMax)
        {
            if (!(outputMin < outputMax)) return 0;
            var gain   = GainTrim.HasValue      ? GainTrim.Value      : 1.0;
            var offset = ZeroTrimVolts.HasValue ? ZeroTrimVolts.Value : 0.0;
            var v = volts * gain + offset;
            if (v < outputMin) return outputMin;
            if (v > outputMax) return outputMax;
            return v;
        }
    }

    [Serializable]
    public class GaugeTransformConfig
    {
        // 'piecewise' | 'linear' | 'resolver' | 'multi_resolver' | 'cross_coupled'.
        // 'piecewise', 'linear', and 'resolver' are honoured by gauge HSMs
        // today; the others round-trip for forward compatibility.
        [XmlAttribute("kind")]
        public string Kind { get; set; }

        // Resolver pairs: each gauge has TWO output channels (sin + cos) but
        // they share one transform. To avoid duplicating the transform on
        // disk, the SIN channel carries the full transform body (InputMin,
        // InputMax, AngleMin/MaxDegrees, PeakVolts, BelowMinBehavior) and
        // the COS channel carries an empty body with role="cos" plus a
        // PartnerChannel attribute pointing to the SIN channel id. The HSM
        // reads both channels but pulls transform parameters from whichever
        // carries them (always the SIN side as the editor writes it).
        //
        // role: "sin" or "cos" — present only for kind="resolver".
        [XmlAttribute("role")]
        public string Role { get; set; }

        // Channel id of the partner (sin↔cos) — only set when Role is "cos".
        [XmlAttribute("partnerChannel")]
        public string PartnerChannel { get; set; }

        // Piecewise transform: ordered breakpoint table. Linear interpolation
        // between adjacent points; below the first / above the last clamps.
        [XmlArray("Breakpoints")]
        [XmlArrayItem("Point")]
        public GaugeBreakpoint[] Breakpoints { get; set; }

        // Linear and resolver transforms: maps `InputMin → angleMin / -10 V`
        // and `InputMax → angleMax / +10 V`, with hard clamps outside.
        // Nullable so the round-trip distinguishes "absent in file" from
        // "explicitly set to 0".
        [XmlElement("InputMin")]
        public double? InputMin { get; set; }

        [XmlElement("InputMax")]
        public double? InputMax { get; set; }

        // Resolver-specific: angular sweep mapped to [InputMin, InputMax].
        // Default sweep is 0..360° if absent.
        [XmlElement("AngleMinDegrees")]
        public double? AngleMinDegrees { get; set; }

        [XmlElement("AngleMaxDegrees")]
        public double? AngleMaxDegrees { get; set; }

        // Resolver peak voltage: sin/cos × PeakVolts is the output. Defaults
        // to 10 V when absent — matches the Simtek 10-1088 convention.
        [XmlElement("PeakVolts")]
        public double? PeakVolts { get; set; }

        // Resolver: what to do when input falls below InputMin. "zero" sets
        // both outputs to 0 V (rest position; what 10-1088 nozzle does).
        // "clamp" projects at angleMin (continuous behaviour expected by e.g.
        // compass-style gauges). Default "clamp" if absent.
        [XmlElement("BelowMinBehavior")]
        public string BelowMinBehavior { get; set; }

        // Multi-turn resolver: number of input units per full sin/cos
        // revolution. Used by altimeter-style gauges where the synchro
        // wraps many times across the input range (10-1081 altimeter
        // fine = 1000 ft/rev; 10-0285 altimeter fine = 4000 ft/rev,
        // coarse = 100000 ft/rev). The runtime computes
        //   angle = (input / unitsPerRevolution) × 360°
        // then sin/cos × peakVolts. No InputMin/Max needed — the sweep
        // is unbounded; sin/cos handle negative angles cleanly.
        [XmlElement("UnitsPerRevolution")]
        public double? UnitsPerRevolution { get; set; }
    }

    [Serializable]
    public class GaugeBreakpoint
    {
        [XmlAttribute("input")]
        public double Input { get; set; }

        // For piecewise transforms whose output is in volts (32 of the 33
        // calibratable gauges in the editor). Default is 0 V; the C# default
        // of `double` is also 0, so a gauge config that uses `output=` for
        // raw DAC counts won't accidentally pick up the wrong value here.
        [XmlAttribute("volts")]
        public double Volts { get; set; }

        // For piecewise transforms whose output is in raw DAC counts rather
        // than volts (Henkie F-16 fuel flow drives its 12-bit DAC directly
        // over USB/PHCC, bypassing AnalogDevices). Editor writes
        // <Point input="500" output="18"/> for these gauges. HSMs that drive
        // a DAC directly read this; HSMs that drive AD channels read Volts.
        [XmlAttribute("output")]
        public double Output { get; set; }

        // For piecewise_resolver transforms: reference angle (degrees) at
        // this input. Used by ADI-style gauges where the synchro angle is a
        // non-linear function of the sim input (e.g. ADI pitch — input
        // pitch°, output reference angle that drives a sin/cos pair). The
        // angle should be encoded MONOTONICALLY in the breakpoint table —
        // values may exceed 360° to keep linear interpolation working
        // across the 360°→0° wrap; the runtime applies % 360 before
        // computing sin/cos.
        [XmlAttribute("angle")]
        public double Angle { get; set; }
    }

    // Pure helpers. Live on the config namespace so per-gauge HSMs only need
    // one `using Common.HardwareSupport.Calibration;` to evaluate a config.
    public static class GaugeTransform
    {
        // Linearly interpolate `input` across an ordered list of breakpoints.
        // Below the first breakpoint clamps to the first volts; above the last
        // clamps to the last volts (matches the C# if/else fallback shape used
        // by every Simtek HSM today). Returns the clamped voltage in ±10 V
        // (final clamp lives in ApplyTrim, but pre-clamping here protects
        // against breakpoints the editor may have flagged amber but saved).
        //
        // O(n) scan — the largest breakpoint table is 43 entries (10-0194
        // airspeed); per-tick gauge update is well below any real cost.
        public static double EvaluatePiecewise(double input, GaugeBreakpoint[] breakpoints)
        {
            if (breakpoints == null || breakpoints.Length < 2)
            {
                // Defensive: malformed config falls back to 0. Caller should
                // detect this via a separate "config valid?" check before
                // routing to this helper, but if they don't, 0 is the
                // safest neutral output for both volts-based gauges
                // (= 0 V) and DAC-count gauges (= rest position).
                return 0;
            }
            // No clamping here — the per-channel safe envelope lives on the
            // output signal's MinValue/MaxValue and is enforced by ApplyTrim
            // at the HSM boundary. Earlier versions of this helper hardcoded
            // a ±10 V clamp; that was Simtek-specific (Simteks drive AD
            // channels at ±10 V) and silently broke any gauge whose output
            // range wasn't ±10 V — Henk F-16 ADI Support Board's pitch
            // channel runs 140..700 (DAC counts), well outside ±10. Letting
            // the math run and clamping at the per-channel boundary is the
            // correct, gauge-agnostic shape.
            if (input <= breakpoints[0].Input) return breakpoints[0].Volts;
            var last = breakpoints[breakpoints.Length - 1];
            if (input >= last.Input) return last.Volts;
            for (var i = 1; i < breakpoints.Length; i++)
            {
                var hi = breakpoints[i];
                if (input < hi.Input)
                {
                    var lo = breakpoints[i - 1];
                    var span = hi.Input - lo.Input;
                    if (span <= 0) return lo.Volts;  // degenerate; pick the lower
                    var t = (input - lo.Input) / span;
                    return lo.Volts + t * (hi.Volts - lo.Volts);
                }
            }
            return last.Volts;
        }

        // Resolver pair: maps `input` linearly to an angle in
        // [angleMinDegrees, angleMaxDegrees], then projects to (sin, cos) ×
        // peakVolts. `belowMinBehavior` controls the underflow case:
        //   "zero"  — both outputs at 0 V (rest position; nozzle uses this)
        //   "clamp" — project at angleMin (continuous; default)
        // Above InputMax always clamps to angleMax (matches the C# nozzle
        // shape and is the only behaviour that makes sense for a bounded
        // mechanical gauge — full-scale stop).
        //
        // Returns [sinVolts, cosVolts]; caller applies per-channel ApplyTrim
        // independently for each (sin and cos windings calibrate separately).
        public static double[] EvaluateResolver(
            double input,
            double inputMin, double inputMax,
            double angleMinDegrees, double angleMaxDegrees,
            double peakVolts,
            string belowMinBehavior)
        {
            var span = inputMax - inputMin;
            if (span <= 0) return new double[] { 0, 0 };
            double angleDeg;
            if (input < inputMin)
            {
                if (string.Equals(belowMinBehavior, "zero", StringComparison.OrdinalIgnoreCase))
                {
                    return new double[] { 0, 0 };
                }
                angleDeg = angleMinDegrees;
            }
            else if (input > inputMax)
            {
                angleDeg = angleMaxDegrees;
            }
            else
            {
                var t = (input - inputMin) / span;
                angleDeg = angleMinDegrees + t * (angleMaxDegrees - angleMinDegrees);
            }
            // Fully qualify System.Math — there's a Common.Math namespace in
            // sibling projects that shadows the unqualified `Math` here even
            // with `using System;` in scope (because Common.HardwareSupport.
            // Calibration is in the same parent namespace tree).
            var rad = angleDeg * System.Math.PI / 180.0;
            // No clamping here — see EvaluatePiecewise comment. ApplyTrim
            // at the HSM boundary owns the per-channel safe envelope.
            var sin = peakVolts * System.Math.Sin(rad);
            var cos = peakVolts * System.Math.Cos(rad);
            return new double[] { sin, cos };
        }

        // Multi-turn resolver: input → angle = (input / unitsPerRevolution)
        // × 360°, then sin/cos × peakVolts. The synchro wraps cleanly across
        // many revolutions (negative angles included) — sin/cos handle it
        // mathematically without any modulo logic. Used by altimeter-style
        // gauges (10-1081 fine = 1000 ft/rev; 10-0285 fine = 4000 ft/rev).
        //
        // Returns [sinVolts, cosVolts]; caller applies per-channel ApplyTrim
        // independently for each.
        public static double[] EvaluateMultiTurnResolver(
            double input,
            double unitsPerRevolution,
            double peakVolts)
        {
            if (unitsPerRevolution == 0) return new double[] { 0, 0 };
            var revolutions = input / unitsPerRevolution;
            var angleDeg = revolutions * 360.0;
            var rad = angleDeg * System.Math.PI / 180.0;
            // No clamping here — see EvaluatePiecewise comment. ApplyTrim
            // at the HSM boundary owns the per-channel safe envelope.
            var sin = peakVolts * System.Math.Sin(rad);
            var cos = peakVolts * System.Math.Cos(rad);
            return new double[] { sin, cos };
        }

        // Piecewise-then-resolver: linearly interpolate `input` across the
        // breakpoint table (where each Point's `Angle` is the reference
        // angle in degrees), then project to (sin, cos) × peakVolts. Used
        // by ADI-style gauges where the synchro angle is a non-linear
        // function of the sim input (10-1084 pitch is the founding case).
        //
        // The angle table should be MONOTONICALLY increasing — values may
        // exceed 360° to keep linear interpolation working across the
        // 360°→0° wrap; this helper applies % 360 before sin/cos so the
        // wrap is invisible to the synchro hardware.
        //
        // Returns [sinVolts, cosVolts]; caller applies per-channel ApplyTrim
        // independently for each. Below the first breakpoint clamps to the
        // first angle; above the last clamps to the last (matches the
        // EvaluatePiecewise voltage-clamp shape).
        public static double[] EvaluatePiecewiseResolver(
            double input,
            GaugeBreakpoint[] breakpoints,
            double peakVolts)
        {
            if (breakpoints == null || breakpoints.Length < 2)
            {
                return new double[] { 0, 0 };
            }
            double angleDeg;
            if (input <= breakpoints[0].Input)
            {
                angleDeg = breakpoints[0].Angle;
            }
            else
            {
                var last = breakpoints[breakpoints.Length - 1];
                if (input >= last.Input)
                {
                    angleDeg = last.Angle;
                }
                else
                {
                    angleDeg = last.Angle;  // overwritten in the loop unless degenerate
                    for (var i = 1; i < breakpoints.Length; i++)
                    {
                        var hi = breakpoints[i];
                        if (input < hi.Input)
                        {
                            var lo = breakpoints[i - 1];
                            var span = hi.Input - lo.Input;
                            if (span <= 0) { angleDeg = lo.Angle; break; }
                            var t = (input - lo.Input) / span;
                            angleDeg = lo.Angle + t * (hi.Angle - lo.Angle);
                            break;
                        }
                    }
                }
            }
            var rad = (angleDeg % 360.0) * System.Math.PI / 180.0;
            // No clamping here — see EvaluatePiecewise comment. ApplyTrim
            // at the HSM boundary owns the per-channel safe envelope.
            var sin = peakVolts * System.Math.Sin(rad);
            var cos = peakVolts * System.Math.Cos(rad);
            return new double[] { sin, cos };
        }
    }
}
