using System;

namespace AnalogDevices
{
    [Flags]
    internal enum BasicMasks : ushort
    {
        OneBit = 0x01,
        TwoBits = 0x03,
        ThreeBits = 0x07,
        FourBits = 0x0F,
        FiveBits = 0x1F,
        SixBits = 0x3F,
        SevenBits = 0x7F,
        EightBits = 0xFF,
        FourteenBits = 0x3FFF,
        HighFourteenBits = 0xFFFC,
        SixteenBits = 0xFFFF,
        AllBitsZero = 0x00,
        BitZero = 1,
        BitOne = 1 << 1,
        BitTwo = 1 << 2,
        BitThree = 1 << 3,
        BitFour = 1 << 4,
        BitFive = 1 << 5,
        BitSix = 1 << 6,
        BitSeven = 1 << 7,
        BitEight = 1 << 8,
        BitNine = 1 << 9,
        BitTen = 1 << 10,
        BitEleven = 1 << 11,
        BitTwelve = 1 << 12,
        BitThirteen = 1 << 13,
        BitFourteen = 1 << 14,
        BitFifteen = 1 << 15
    }
}