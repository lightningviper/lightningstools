using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.PoKeys
{
    // Multi-device PoKeys output driver config. One <Device> per physical
    // PoKeys board the user has plugged in; addressed by serial number.
    //
    // Schema:
    //   <PoKeys>
    //     <Devices>
    //       <Device>
    //         <Serial>12345</Serial>            (uint, matches PoKeysDeviceInfo.SerialNumber)
    //         <Name>cockpit-left</Name>          (optional; for friendly names in logs)
    //         <PWMPeriodMicroseconds>20000</PWMPeriodMicroseconds>
    //         <DigitalOutputs>
    //           <Output><Pin>5</Pin><Invert>true</Invert></Output>
    //           ...
    //         </DigitalOutputs>
    //         <PWMOutputs>
    //           <Output><Channel>1</Channel></Output>  (PWM1=pin17 .. PWM6=pin22)
    //           ...
    //         </PWMOutputs>
    //       </Device>
    //     </Devices>
    //   </PoKeys>
    //
    // The PWM period is per-device because the hardware shares one period
    // across all 6 PWM channels on a given board. Stored in microseconds
    // so the file is human-portable across PoKeys55 (12 MHz clock) and
    // PoKeys56/57 (25 MHz clock); the HSM converts to clock cycles at
    // apply time using PoKeysDevice.GetPWMFrequency().
    [Serializable]
    [XmlRoot("PoKeys")]
    public class PoKeysHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem("Device")]
        public PoKeysDeviceConfig[] Devices { get; set; }

        public static PoKeysHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<PoKeysHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }

    [Serializable]
    public class PoKeysDeviceConfig
    {
        public uint Serial { get; set; }
        public string Name { get; set; }

        // PWM period in microseconds applied to all 6 PWM channels on
        // this device. 20000 us = 20 ms = a typical RC-servo period and
        // a reasonable default for LED dimming. The hardware accepts
        // anything that fits in 32 bits when converted to clock cycles
        // at the device's PWM base frequency.
        public uint PWMPeriodMicroseconds { get; set; } = 20000;

        // When true, the HSM connects to the device for each signal
        // change and disconnects afterwards instead of holding a
        // persistent USB handle. Lets other apps (PoKeys vendor tool,
        // a separately running test session, the editor's per-row
        // Test buttons, etc.) share the board while SimLinkup is
        // running, but adds ~30-80 ms of latency per write.
        //
        // Primary use case: the editor's PoKeys test affordance —
        // when SimLinkup is running with HoldConnection, the bridge's
        // setOutput call can't connect because SimLinkup owns the
        // device. With ConnectPerWrite enabled, both sides can take
        // turns. Useful for testing wiring without stopping SimLinkup.
        //
        // Useful for boards that only drive event-driven digital
        // outputs (lamps, relays); not recommended for boards with
        // continuously-updating PWM or analog channels because
        // updates throttle to ~12 Hz which makes smooth transitions
        // stutter.
        //
        // Default false (hold-connection model) preserves the
        // existing performance for users who don't need to share the
        // device.
        public bool ConnectPerWrite { get; set; } = false;

        [XmlArray("DigitalOutputs")]
        [XmlArrayItem("Output")]
        public PoKeysDigitalOutputConfig[] DigitalOutputs { get; set; }

        [XmlArray("PWMOutputs")]
        [XmlArrayItem("Output")]
        public PoKeysPWMOutputConfig[] PWMOutputs { get; set; }

        // PoExtBus is a daisy-chain of up to 10 × 8-bit shift registers
        // (80 total bit outputs) driven via the dedicated PoExtBus
        // connector. Bit numbering is 1..80, where bit 1 = Device 1
        // output A (the first bit shifted out), bit 8 = Device 1 output
        // H, bit 9 = Device 2 output A, etc. The HSM owns a 10-byte
        // cache; SignalChanged updates one bit and pushes the whole
        // array via AuxilaryBusSetData.
        [XmlArray("PoExtBusOutputs")]
        [XmlArrayItem("Output")]
        public PoKeysPoExtBusOutputConfig[] PoExtBusOutputs { get; set; }
    }

    [Serializable]
    public class PoKeysDigitalOutputConfig
    {
        // 1-based pin number, 1..55. Stored 1-based to match the editor
        // UI and the silkscreen labels on the board; the HSM converts
        // to the 0-based pin ID the API expects.
        public byte Pin { get; set; }

        // Free-form human-readable name for this output (e.g. "Gear
        // down lamp", "Master caution"). Editor-only metadata; surfaced
        // in the Mappings dropdown so users wire by purpose rather than
        // pin number. Empty string is fine — the dropdown falls back
        // to "DIGITAL_PIN[N]" when no name is set.
        public string Name { get; set; } = "";

        // When true, SetPinData is called with bit 7 of the function
        // byte set so the device inverts the pin's logical level.
        // Default true matches the convention every other SimLinkup
        // output uses (state=true => pin sourcing 3.3V), and
        // counteracts the hardware's documented "uninverted output:
        // 0 -> 3.3V, 1 -> 0V" behavior.
        public bool Invert { get; set; } = true;
    }

    [Serializable]
    public class PoKeysPoExtBusOutputConfig
    {
        // 1-based PoExtBus bit number, 1..80. Maps to byte (n-1)/8 and
        // bit (n-1)%8 within the 10-byte AuxilaryBusSetData payload.
        // The PoKeys vendor tool labels these as "Device N : A..H"; the
        // editor UI surfaces both forms (Device + letter, plus the flat
        // bit number) for orientation, but the on-disk and runtime
        // representation is the flat 1..80 form.
        public int Bit { get; set; }

        // Free-form human-readable name (see PoKeysDigitalOutputConfig
        // for the rationale). Surfaced in the Mappings dropdown.
        public string Name { get; set; } = "";

        // Software-side invert applied before the bit is written to the
        // shift register. Default FALSE — opposite of GPIO digital
        // pins. PoExtBus drives shift-register outputs that go to
        // relay coils directly: bit=1 energises the coil, bit=0
        // de-energises. At startup the cache is all zeros (relays
        // OFF), and a sim signal transitioning to state=true turns
        // the relay ON, which is no inversion. (GPIO pins default to
        // Invert=true because their uninverted output is documented
        // as 0=3.3V — that quirk doesn't apply to the shift-register
        // chain.) Users wiring active-low relays can flip per output.
        public bool Invert { get; set; } = false;
    }

    [Serializable]
    public class PoKeysPWMOutputConfig
    {
        // 1-based channel number where:
        //   PWM1 = physical pin 17  (DLL channel index 5)
        //   PWM2 = physical pin 18  (DLL channel index 4)
        //   PWM3 = physical pin 19  (DLL channel index 3)
        //   PWM4 = physical pin 20  (DLL channel index 2)
        //   PWM5 = physical pin 21  (DLL channel index 1)
        //   PWM6 = physical pin 22  (DLL channel index 0)
        // The DLL uses reversed indexing (per its xmldoc); the HSM
        // applies the reversal at write time so the config and the
        // editor surface stay in human-readable order.
        public byte Channel { get; set; }

        // Free-form human-readable name (see PoKeysDigitalOutputConfig
        // for the rationale). Surfaced in the Mappings dropdown.
        public string Name { get; set; } = "";
    }
}
