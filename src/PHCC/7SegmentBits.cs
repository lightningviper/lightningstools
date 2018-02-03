using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    /// Seven-segment display bitmasks
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum SevenSegmentBits : byte
    {
        /// <summary>
        ///   Bitmask for all segments off
        /// </summary>
        None = 0x00,
        /// <summary>
        ///   Bitmask for segment A
        /// </summary>
        SegmentA = 0x80,
        /// <summary>
        ///   Bitmask for segment B
        /// </summary>
        SegmentB = 0x40,
        /// <summary>
        ///   Bitmask for segment C
        /// </summary>
        SegmentC = 0x20,
        /// <summary>
        ///   Bitmask for segment D
        /// </summary>
        SegmentD = 0x08,
        /// <summary>
        ///   Bitmask for segment E
        /// </summary>
        SegmentE = 0x04,
        /// <summary>
        ///   Bitmask for segment F
        /// </summary>
        SegmentF = 0x02,
        /// <summary>
        ///   Bitmask for segment G
        /// </summary>
        SegmentG = 0x01,
        /// <summary>
        ///   Bitmask for decimal point segment
        /// </summary>
        SegmentDP = 0x10,
        /// <summary>
        ///   Bitmask for all segments on
        /// </summary>
        All = 0xFF,
    }

}
