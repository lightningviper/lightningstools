using System;
using System.Runtime.InteropServices;

namespace Henkie.HSI.Board1
{
    /// <summary>
    ///   Range digits
    /// </summary>
    [Flags]
    [ComVisible(true)]
    public enum RangeDigitStatorCoils : byte
    {
        /// <summary>
        ///  Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///  Range "ones" digit (X)
        /// </summary>
        RANGE_ONES_DIGIT_X = 0x01,
        /// <summary>
        ///  Range "ones" digit (Y)
        /// </summary>
        RANGE_ONES_DIGIT_Y = 0x02,
        /// <summary>
        ///  Range "tens" digit (X)
        /// </summary>
        RANGE_TENS_DIGIT_X = 0x04,
        /// <summary>
        ///  Range "tens" digit (Y)
        /// </summary>
        RANGE_TENS_DIGIT_Y = 0x08,
        /// <summary>
        ///  Range "hundreds" digit (X)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_X = 0x10,
        /// <summary>
        ///  Range "hundreds" digit (Y)
        /// </summary>
        RANGE_HUNDREDS_DIGIT_Y = 0x20,
    }
}