using System;

namespace AnalogDevices
{
    [Flags]
    internal enum ControlRegisterBits : byte
    {
        F0 = (byte) BasicMasks.BitZero,
        F1 = (byte) BasicMasks.BitOne,
        F2 = (byte) BasicMasks.BitTwo,
        F3 = (byte) BasicMasks.BitThree,
        F4 = (byte) BasicMasks.BitFour,

        SoftPowerDown = F0,
        ThermalShutdownEnabled = F1,
        InputRegisterSelect = F2,
        PacketErrorCheckErrorOccurred = F3,
        OverTemperature = F4,

        WritableBits = F2 | F1 | F0,
        ReadableBits = F4 | F3 | F2 | F1 | F0
    }
}