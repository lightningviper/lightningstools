using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.HardwareSupport;
using Common.MacroProgramming;
using log4net;
using PoKeysDevice_DLL;

namespace SimLinkup.HardwareSupport.PoKeys
{
    // Output-driver HSM for PoLabs PoKeys boards. v1 covers digital pin
    // outputs (1..55) and PWM channel outputs (1..6, on physical pins
    // 17..22). USB only — Ethernet PoKeys are out of scope for now.
    //
    // Multi-device by serial: GetInstances() returns one HSM per
    // <Device> in PoKeysHardwareSupportModule.config that successfully
    // matches an enumerated USB device. Boards that are listed in the
    // config but not currently plugged in are logged and skipped, so
    // an unrelated missing PoKeys doesn't break the whole driver.
    //
    // Out of scope for v1: matrix LED, LCD, PoExtBus, PoNET, digital
    // counters, encoders, analog inputs, Ethernet/network discovery.
    // The DLL surfaces all of these — adding them is mostly a matter
    // of declaring extra signal arrays and wiring more SignalChanged
    // handlers in the same shape as digital + PWM.
    public class PoKeysHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PoKeysHardwareSupportModule));

        // Static, shared across HSM instances. The DLL is implemented
        // such that one PoKeysDevice instance per process can reach
        // any device, so we use a single helper for enumeration. Per-
        // instance device handles are obtained via separate
        // PoKeysDevice objects, one per HSM (each calls ConnectToDevice
        // and DisconnectDevice on its own).
        private readonly PoKeysDeviceConfig _deviceConfig;
        private PoKeysDevice _device;
        private bool _isConnected;
        private bool _isDisposed;

        // Single-writer lock for SignalChanged handlers. SimLinkup
        // raises events on multiple threads in parallel for different
        // signals; without this lock, two SignalChanged handlers could
        // call ConnectToDevice / write / DisconnectDevice on the
        // shared _device instance simultaneously and tangle the USB
        // state. Even in hold-connection mode the underlying DLL
        // isn't documented as thread-safe, so the lock is good
        // hygiene either way.
        private readonly object _writeLock = new object();

        // Period in clock cycles that we computed at Initialize time
        // from the configured PWMPeriodMicroseconds and the device's
        // GetPWMFrequency() (12 MHz on PoKeys55, 25 MHz on PoKeys56/57).
        // Stored so the HSM can re-emit it on demand and so PWM duty
        // calculations can scale a 0..1 input to [0..period] cycles.
        private uint _pwmPeriodCycles;

        // Live PWM duty cycle cache, indexed by DLL channel slot (0..5).
        // We keep the full 6-slot array because SetPWMOutputsFast takes
        // all 6 at once, even if the user only configured a subset of
        // channels. Unused channels stay at 0 — harmless.
        // Note: NOT readonly — SetPWMOutputs / SetPWMOutputsFast take
        // ref parameters, and C# refuses to pass a readonly field as
        // ref outside a constructor. The IDE will hint "make readonly"
        // because the array reference itself never reassigns; ignore.
        private uint[] _pwmDuty = new uint[6];

        // Mask of which DLL channel slots were enabled in the
        // SetPWMOutputs initialisation call. Used so SignalChanged for
        // an unconfigured channel (defensive — shouldn't happen) can
        // refuse to clobber a slot the device thinks is disabled.
        // Same readonly-vs-ref note as _pwmDuty.
        private bool[] _pwmEnabled = new bool[6];

        // PoExtBus shift-register chain cache. Up to 10 × 8-bit
        // daisy-chained shift registers = 80 bit outputs total. The
        // DLL's AuxilaryBusSetData takes the WHOLE 10-byte array on
        // every call; on SignalChanged we mutate one bit then push the
        // full payload. _extBusActive flips true the first time we
        // call AuxilaryBusSetData with enabled=1, so SignalChanged
        // doesn't try to drive a disabled bus.
        private readonly byte[] _extBusCache = new byte[10];
        private bool _extBusActive;

        private DigitalSignal[] _digitalOutputSignals;
        private AnalogSignal[] _pwmOutputSignals;
        private DigitalSignal[] _extBusOutputSignals;

        private PoKeysHardwareSupportModule(PoKeysDeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
        }

        public override AnalogSignal[] AnalogOutputs => _pwmOutputSignals;

        // Digital outputs span both GPIO pins (1..55) and PoExtBus
        // shift-register bits (1..80). Concat at access time so the
        // runtime sees a single array, but signal-changed handlers
        // route to the right hardware path via the signal's Id format.
        public override DigitalSignal[] DigitalOutputs
        {
            get
            {
                var pins = _digitalOutputSignals ?? new DigitalSignal[0];
                var bus = _extBusOutputSignals ?? new DigitalSignal[0];
                if (bus.Length == 0) return pins;
                if (pins.Length == 0) return bus;
                var combined = new DigitalSignal[pins.Length + bus.Length];
                pins.CopyTo(combined, 0);
                bus.CopyTo(combined, pins.Length);
                return combined;
            }
        }

        public override string FriendlyName
        {
            get
            {
                var label = string.IsNullOrWhiteSpace(_deviceConfig?.Name)
                    ? $"serial {_deviceConfig?.Serial}"
                    : $"\"{_deviceConfig.Name}\" (serial {_deviceConfig.Serial})";
                return $"PoKeys {label}";
            }
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(
                    Util.CurrentMappingProfileDirectory,
                    "PoKeysHardwareSupportModule.config");
                if (!File.Exists(hsmConfigFilePath))
                {
                    _log.InfoFormat("No PoKeys config at {0}; PoKeys driver inactive for this profile.",
                        hsmConfigFilePath);
                    return toReturn.ToArray();
                }
                var hsmConfig = PoKeysHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                if (hsmConfig == null || hsmConfig.Devices == null || hsmConfig.Devices.Length == 0)
                {
                    return toReturn.ToArray();
                }

                foreach (var deviceConfig in hsmConfig.Devices)
                {
                    if (deviceConfig == null) continue;
                    try
                    {
                        var hsm = new PoKeysHardwareSupportModule(deviceConfig);
                        if (hsm.Initialize())
                        {
                            toReturn.Add(hsm);
                        }
                        else
                        {
                            // Initialize logs the specific failure (no enum match,
                            // connection refused, etc.). Don't treat one missing
                            // PoKeys as fatal — other declared boards may still
                            // be plugged in and usable.
                            hsm.Dispose();
                        }
                    }
                    catch (Exception perDeviceException)
                    {
                        _log.Error(perDeviceException.Message, perDeviceException);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return toReturn.ToArray();
        }

        // Connect to the configured PoKeys by serial, configure pin
        // functions for every declared digital output, prime the PWM
        // hardware if any PWM outputs are declared, and create the
        // signal arrays the runtime will publish. Returns false if
        // the device couldn't be reached so GetInstances can skip it
        // cleanly without registering a half-initialized HSM.
        private bool Initialize()
        {
            _device = new PoKeysDevice();
            // Connect by serial number directly. We deliberately do
            // NOT call EnumeratePoKeysDevices here:
            //
            // PoKeysDevice_DLL's EnumeratePoKeysDevices populates an
            // internal PoKeysUSBDeviceObject[] via native USB
            // enumeration. The native marshaller has a bug where one
            // of the array slots gets the literal byte value 0xFF
            // written into a managed reference slot instead of a real
            // object pointer or null. The CLR doesn't validate
            // references at write time, so the bad array sits dormant
            // in our heap. The first time the GC's mark phase walks
            // it (during a Gen 2 compaction triggered by SimLinkup's
            // runtime loop allocation pressure ~1 minute later), it
            // hits 0xFF as if it were an object pointer, dereferences
            // [0xff+0x478] → AV in clr!SVR::gc_heap::mark_object_simple1
            // → CLR panics → process exits with c0000005 / 80131506.
            //
            // The DLL exposes a direct ConnectToDevice(serial, checkEthernet)
            // overload that opens the USB handle by serial WITHOUT
            // populating the bad array. F4ToPokeys and other PoKeys
            // consumers use this pattern in their runtime hot path
            // (enumerate only in their UI / discovery code).
            //
            // checkEthernet=0 → USB only, no Ethernet probe (matches
            // our previous EnumeratePoKeysDevices(_, _, ethernet:false, _)
            // call). Returns true on success, false if the device
            // isn't reachable (unplugged, in use by another process, etc).
            int serial = (int)_deviceConfig.Serial;
            if (!_device.ConnectToDevice(serial, 0))
            {
                _log.WarnFormat("PoKeys serial {0} not reachable on USB (unplugged, in use, or wrong serial). " +
                                "Skipping this device — other PoKeys boards (if any) will still be initialised.",
                    _deviceConfig.Serial);
                return false;
            }
            _isConnected = true;
            // Clear the DLL's internal detectedDevicesList. ConnectToDevice
            // populates it via EnumerateDevices_HID/Bulk, which has a
            // native marshaling bug that writes the literal byte 0xFF
            // into one of the managed reference slots of the list's
            // backing PoKeysUSBDeviceObject[]. The bad reference sits
            // dormant in our heap until the GC's mark phase walks it,
            // hits 0xFF as if it were an object pointer, and AVs in
            // clr!SVR::gc_heap::mark_object_simple1+0x2ff. Clearing
            // the list here unroots the corrupt array; the next GC
            // collects it and the bug never fires.
            ClearDetectedDevicesList(_device);

            // Configure once at startup either way — pins must be
            // flipped to digital-output mode (they default to input
            // after power-up), the PWM block needs its period set,
            // and PoExtBus needs to be enabled. Per-write mode just
            // drops the connection AFTER configuration; subsequent
            // SignalChanged handlers reconnect on demand.
            ConfigureDigitalOutputs();
            ConfigurePWMOutputs();
            ConfigurePoExtBusOutputs();

            _digitalOutputSignals = CreateDigitalOutputSignals();
            _pwmOutputSignals = CreatePWMOutputSignals();
            _extBusOutputSignals = CreatePoExtBusOutputSignals();

            _log.InfoFormat(
                "PoKeys {0} initialised: {1} digital output{2}, {3} PWM channel{4} (period={5} us), {6} PoExtBus bit{7}, mode={8}.",
                FriendlyName,
                _digitalOutputSignals.Length, _digitalOutputSignals.Length == 1 ? "" : "s",
                _pwmOutputSignals.Length, _pwmOutputSignals.Length == 1 ? "" : "s",
                _deviceConfig.PWMPeriodMicroseconds,
                _extBusOutputSignals.Length, _extBusOutputSignals.Length == 1 ? "" : "s",
                _deviceConfig.ConnectPerWrite ? "connect-per-write" : "hold-connection");

            // In ConnectPerWrite mode, drop the handle now that
            // initial config is done. Subsequent writes will
            // reconnect through EnsureConnectedForWrite below.
            if (_deviceConfig.ConnectPerWrite)
            {
                try { _device.DisconnectDevice(); } catch { }
                _isConnected = false;
            }
            return true;
        }

        // Wipe PoKeysDevice.detectedDevicesList via reflection. The
        // DLL's enumeration code (which is invoked internally by
        // ConnectToDevice) populates this private List<> with results
        // from a buggy native marshal — one slot of its backing array
        // contains the literal byte 0xFF instead of a managed object
        // reference. Replacing the list with a fresh empty one makes
        // the corrupt backing array unrooted; the next GC collects it
        // and the bug never fires.
        //
        // Cached fast-path: look up the FieldInfo once per process.
        // null when reflection can't find the field (different DLL
        // version, etc.) — in that case we silently skip; the worst
        // case is the original crash returns.
        private static FieldInfo _detectedDevicesListField =
            typeof(PoKeysDevice).GetField("detectedDevicesList",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static void ClearDetectedDevicesList(PoKeysDevice device)
        {
            if (device == null || _detectedDevicesListField == null) return;
            try
            {
                _detectedDevicesListField.SetValue(device, new List<PoKeysUSBDeviceObject>());
            }
            catch (Exception e)
            {
                _log.WarnFormat("Could not clear PoKeys detectedDevicesList: {0}", e.Message);
            }
        }

        // For ConnectPerWrite mode: open the device, run the write
        // action, close the device. Locked so concurrent SignalChanged
        // events don't tangle the USB state. Errors are caught and
        // logged so a transient hiccup on one output doesn't break
        // every other output. In hold-connection mode this becomes a
        // straight call under the same lock — safe regardless.
        private void RunWriteUnderLock(Action<PoKeysDevice> writeAction)
        {
            if (writeAction == null || _device == null) return;
            lock (_writeLock)
            {
                if (_isDisposed) return;
                if (_deviceConfig.ConnectPerWrite)
                {
                    try
                    {
                        // Direct-by-serial connect; never call
                        // EnumeratePoKeysDevices (see Initialize for why).
                        if (!_device.ConnectToDevice((int)_deviceConfig.Serial, 0))
                        {
                            _log.WarnFormat("PoKeys serial {0}: connect-per-write failed (device in use or unplugged); skipping this update.",
                                _deviceConfig.Serial);
                            return;
                        }
                        _isConnected = true;
                        // Same enumerate-leak workaround as Initialize.
                        ClearDetectedDevicesList(_device);
                        try
                        {
                            writeAction(_device);
                        }
                        finally
                        {
                            try { _device.DisconnectDevice(); } catch { }
                            _isConnected = false;
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
                }
                else
                {
                    if (!_isConnected) return;
                    try { writeAction(_device); }
                    catch (Exception e) { _log.Error(e.Message, e); }
                }
            }
        }

        // For each declared digital output, write a SetPinData call so
        // the pin is in digital-output mode with the configured invert
        // bit. After power-up the device defaults all pins to digital
        // input for safety, so we MUST flip every pin we plan to drive.
        private void ConfigureDigitalOutputs()
        {
            if (_deviceConfig.DigitalOutputs == null) return;
            // Bit 2 = digital output, bit 7 = invert state (per the
            // SetPinData xmldoc). We always force the invert bit per
            // the config field's value rather than reading whatever
            // the device's flash currently holds — that way PoKeys
            // saved with the vendor's tool to a different invert
            // setting still behave deterministically under SimLinkup.
            const byte digitalOutputBit = 0x04;
            const byte invertBit = 0x80;
            foreach (var pinCfg in _deviceConfig.DigitalOutputs)
            {
                if (pinCfg == null) continue;
                if (pinCfg.Pin < 1 || pinCfg.Pin > 55)
                {
                    _log.WarnFormat("PoKeys serial {0}: digital output pin {1} out of range (1..55); skipping.",
                        _deviceConfig.Serial, pinCfg.Pin);
                    continue;
                }
                // Pin 54 is the device's reset pin. The PoKeys vendor
                // tool greys it out for normal output use, but driving
                // it deliberately is a valid way to issue a soft reset
                // command (e.g. to recover a stuck peripheral). We
                // allow it through with no special handling — the
                // editor surfaces it with a warning so users don't
                // wire it accidentally.
                var pinId0 = (byte)(pinCfg.Pin - 1);
                var function = (byte)(digitalOutputBit | (pinCfg.Invert ? invertBit : 0));
                try
                {
                    _device.SetPinData(pinId0, function);
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
        }

        // Initialise the device's PWM block once with the configured
        // period and zero duty across all 6 channels. After this,
        // signal-changed handlers use SetPWMOutputsFast (duty-only) on
        // the hot path so the period byte doesn't get re-sent on every
        // tick. PWMOutputs in config can be empty — that's fine, we
        // skip the whole block to avoid spurious USB traffic.
        private void ConfigurePWMOutputs()
        {
            if (_deviceConfig.PWMOutputs == null || _deviceConfig.PWMOutputs.Length == 0) return;

            // Build the channel-enable mask. The DLL uses reversed
            // indexing (PWM1=pin17 = channel 5; PWM6=pin22 = channel 0),
            // so the config's 1-based PWM channel maps to (6 - n).
            for (var i = 0; i < 6; i++) _pwmEnabled[i] = false;
            foreach (var pwmCfg in _deviceConfig.PWMOutputs)
            {
                if (pwmCfg == null) continue;
                if (pwmCfg.Channel < 1 || pwmCfg.Channel > 6)
                {
                    _log.WarnFormat("PoKeys serial {0}: PWM channel {1} out of range (1..6); skipping.",
                        _deviceConfig.Serial, pwmCfg.Channel);
                    continue;
                }
                _pwmEnabled[6 - pwmCfg.Channel] = true;
            }

            // Convert the configured period from microseconds to
            // device clock cycles. GetPWMFrequency returns 12e6 for
            // PoKeys55 and 25e6 for PoKeys56+; either way one
            // microsecond costs (frequency / 1e6) cycles. We compute
            // in double then narrow to uint, with a defensive clamp
            // against pathological huge configured periods.
            var freqHz = _device.GetPWMFrequency();
            var cyclesDouble = (double)_deviceConfig.PWMPeriodMicroseconds * (freqHz / 1e6);
            if (cyclesDouble < 1) cyclesDouble = 1;
            if (cyclesDouble > uint.MaxValue) cyclesDouble = uint.MaxValue;
            _pwmPeriodCycles = (uint)cyclesDouble;

            for (var i = 0; i < 6; i++) _pwmDuty[i] = 0;

            try
            {
                _device.SetPWMOutputs(ref _pwmEnabled, ref _pwmPeriodCycles, ref _pwmDuty);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        // Enable the PoExtBus shift-register chain and clear all 80 bit
        // outputs. The DLL's AuxilaryBusSetData(enabled, dataBytes)
        // takes the WHOLE 10-byte payload — there's no per-bit API —
        // so we own the cache and push the full array on every change.
        // Skipped entirely when no PoExtBus outputs are declared so we
        // don't enable the bus on PoKeys boards that aren't using it.
        private void ConfigurePoExtBusOutputs()
        {
            if (_deviceConfig.PoExtBusOutputs == null || _deviceConfig.PoExtBusOutputs.Length == 0) return;

            for (var i = 0; i < _extBusCache.Length; i++) _extBusCache[i] = 0;
            try
            {
                _device.AuxilaryBusSetData(1, _extBusCache);
                _extBusActive = true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        private DigitalSignal[] CreateDigitalOutputSignals()
        {
            if (_deviceConfig.DigitalOutputs == null) return new DigitalSignal[0];
            var list = new List<DigitalSignal>();
            foreach (var pinCfg in _deviceConfig.DigitalOutputs)
            {
                if (pinCfg == null) continue;
                if (pinCfg.Pin < 1 || pinCfg.Pin > 55) continue;
                var signal = new DigitalSignal
                {
                    Category = "Outputs",
                    CollectionName = "Digital Pins",
                    FriendlyName = $"Digital pin {pinCfg.Pin}",
                    Id = $"PoKeys[{_deviceConfig.Serial}]__DIGITAL_PIN[{pinCfg.Pin}]",
                    Index = pinCfg.Pin,
                    PublisherObject = this,
                    Source = _device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _deviceConfig.Serial.ToString(),
                    SubSource = pinCfg.Pin,
                    SubSourceFriendlyName = $"Pin {pinCfg.Pin}",
                    SubSourceAddress = null,
                    State = false
                };
                signal.SignalChanged += DigitalOutputSignalChanged;
                list.Add(signal);
            }
            return list.ToArray();
        }

        private AnalogSignal[] CreatePWMOutputSignals()
        {
            if (_deviceConfig.PWMOutputs == null) return new AnalogSignal[0];
            var list = new List<AnalogSignal>();
            foreach (var pwmCfg in _deviceConfig.PWMOutputs)
            {
                if (pwmCfg == null) continue;
                if (pwmCfg.Channel < 1 || pwmCfg.Channel > 6) continue;
                // PWM channels surface as 0..1 fractional duty cycle
                // — this matches how every other "analog-like" output
                // in SimLinkup is normalised (gauges and resolvers
                // work in -10..+10 V; PoKeys PWM is genuinely a duty
                // ratio, so 0..1 is the natural unit). The handler
                // converts to clock cycles at write time using the
                // cached _pwmPeriodCycles.
                var pin = 16 + pwmCfg.Channel; // PWM1 -> 17, PWM6 -> 22
                var signal = new AnalogSignal
                {
                    Category = "Outputs",
                    CollectionName = "PWM Channels",
                    FriendlyName = $"PWM{pwmCfg.Channel} (pin {pin})",
                    Id = $"PoKeys[{_deviceConfig.Serial}]__PWM[{pwmCfg.Channel}]",
                    Index = pwmCfg.Channel,
                    PublisherObject = this,
                    Source = _device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _deviceConfig.Serial.ToString(),
                    SubSource = pwmCfg.Channel,
                    SubSourceFriendlyName = $"PWM{pwmCfg.Channel}",
                    SubSourceAddress = null,
                    State = 0,
                    MinValue = 0,
                    MaxValue = 1,
                    IsPercentage = true
                };
                signal.SignalChanged += PWMOutputSignalChanged;
                list.Add(signal);
            }
            return list.ToArray();
        }

        // Build one DigitalSignal per declared PoExtBus output. Caches
        // the configured invert flag in SubSourceAddress so the
        // SignalChanged handler can apply it without re-walking the
        // config array on every event. Bit number is stored in Index.
        private DigitalSignal[] CreatePoExtBusOutputSignals()
        {
            if (_deviceConfig.PoExtBusOutputs == null) return new DigitalSignal[0];
            var list = new List<DigitalSignal>();
            foreach (var busCfg in _deviceConfig.PoExtBusOutputs)
            {
                if (busCfg == null) continue;
                if (busCfg.Bit < 1 || busCfg.Bit > 80) continue;
                // Friendly Device:Letter form for logging — the user
                // sees this in the PoKeys vendor tool, so logs that
                // mention "Device 3 : E" are easier to correlate
                // against the wiring than a flat bit number.
                var deviceIndex = ((busCfg.Bit - 1) / 8) + 1; // 1..10
                var letter = (char)('A' + ((busCfg.Bit - 1) % 8)); // A..H
                var signal = new DigitalSignal
                {
                    Category = "Outputs",
                    CollectionName = "PoExtBus",
                    FriendlyName = $"PoExtBus bit {busCfg.Bit} (Device {deviceIndex} : {letter})",
                    Id = $"PoKeys[{_deviceConfig.Serial}]__PoExtBus[{busCfg.Bit}]",
                    Index = busCfg.Bit,
                    PublisherObject = this,
                    Source = _device,
                    SourceFriendlyName = FriendlyName,
                    SourceAddress = _deviceConfig.Serial.ToString(),
                    SubSource = busCfg.Bit,
                    SubSourceFriendlyName = $"Device {deviceIndex} : {letter}",
                    // Stash invert as the string "1"/"0" so the handler
                    // doesn't need to look up the config record by bit
                    // every time. Lightweight and avoids a closure.
                    SubSourceAddress = busCfg.Invert ? "1" : "0",
                    State = false
                };
                signal.SignalChanged += PoExtBusOutputSignalChanged;
                list.Add(signal);
            }
            return list.ToArray();
        }

        private void DigitalOutputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            var signal = sender as DigitalSignal;
            if (signal == null || !signal.Index.HasValue) return;
            var pin = signal.Index.Value;
            if (pin < 1 || pin > 55) return;
            var state = args.CurrentState;
            // Lock + connect-if-needed handled inside RunWriteUnderLock.
            // The _isConnected guard from the old handler is gone:
            // in ConnectPerWrite mode that flag is false between
            // events by design.
            RunWriteUnderLock(dev => dev.SetOutput((byte)(pin - 1), state));
        }

        private void PWMOutputSignalChanged(object sender, AnalogSignalChangedEventArgs args)
        {
            var signal = sender as AnalogSignal;
            if (signal == null || !signal.Index.HasValue) return;
            var channel = signal.Index.Value;
            if (channel < 1 || channel > 6) return;
            // Clamp to [0,1] so a stray out-of-range input doesn't
            // produce a wraparound on the uint cast below.
            var fraction = args.CurrentState;
            if (fraction < 0) fraction = 0;
            if (fraction > 1) fraction = 1;
            var dutyCycles = (uint)(fraction * _pwmPeriodCycles);
            // Reverse-index for the DLL: PWM1 (config) -> slot 5 (DLL).
            _pwmDuty[6 - channel] = dutyCycles;
            // In ConnectPerWrite mode the device's PWM block is in
            // configured-then-disconnected state. Reconnecting brings
            // up a fresh handle, but the device keeps its last
            // SetPWMOutputs configuration across the disconnect, so
            // SetPWMOutputsFast (duty-only update) still works.
            RunWriteUnderLock(dev => dev.SetPWMOutputsFast(ref _pwmDuty));
        }

        // Mutate one bit in the 10-byte PoExtBus cache and push the
        // entire array to the device. The DLL has no per-bit API — the
        // shift-register chain only accepts the full payload — so we
        // pay one USB round-trip per bit change. Acceptable since the
        // panel use case (cockpit relay panels, dozens of bits) doesn't
        // change every signal at once.
        //
        // The shift registers HOLD their state across disconnects, so
        // ConnectPerWrite mode works fine here too: cumulative bit
        // changes survive even though each call disconnects.
        private void PoExtBusOutputSignalChanged(object sender, DigitalSignalChangedEventArgs args)
        {
            if (!_extBusActive) return;
            var signal = sender as DigitalSignal;
            if (signal == null || !signal.Index.HasValue) return;
            var bit = signal.Index.Value;
            if (bit < 1 || bit > 80) return;
            // Apply software invert before writing the bit. Cached on
            // the signal's SubSourceAddress as "1"/"0" by
            // CreatePoExtBusOutputSignals.
            var invert = signal.SubSourceAddress == "1";
            var effective = invert ? !args.CurrentState : args.CurrentState;
            var byteIndex = (bit - 1) / 8;
            var bitInByte = (bit - 1) % 8;
            var mask = (byte)(1 << bitInByte);
            if (effective) _extBusCache[byteIndex] |= mask;
            else _extBusCache[byteIndex] &= (byte)~mask;
            RunWriteUnderLock(dev => dev.AuxilaryBusSetData(1, _extBusCache));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PoKeysHardwareSupportModule()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                try
                {
                    if (_device != null && _isConnected)
                    {
                        _device.DisconnectDevice();
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
                _isConnected = false;
                _device = null;
            }
            _isDisposed = true;
        }
    }
}
