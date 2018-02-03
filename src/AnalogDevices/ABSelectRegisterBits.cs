using System;

namespace AnalogDevices
{
    [Flags]
    internal enum ABSelectRegisterBits : byte
    {
        F0 = (byte) BasicMasks.BitZero,
        F1 = (byte) BasicMasks.BitOne,
        F2 = (byte) BasicMasks.BitTwo,
        F3 = (byte) BasicMasks.BitThree,
        F4 = (byte) BasicMasks.BitFour,
        F5 = (byte) BasicMasks.BitFive,
        F6 = (byte) BasicMasks.BitSix,
        F7 = (byte) BasicMasks.BitSeven,

        Channel0 = F0,
        Channel1 = F1,
        Channel2 = F2,
        Channel3 = F3,
        Channel4 = F4,
        Channel5 = F5,
        Channel6 = F6,
        Channel7 = F7,

        AllChannelsA = (byte) BasicMasks.AllBitsZero,
        AllChannelsB = Channel0 | Channel1 | Channel2 | Channel3 | Channel4 | Channel5 | Channel6 | Channel7
    }
}