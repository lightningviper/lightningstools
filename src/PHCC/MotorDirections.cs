using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    ///   Enumeration of stepper motor directions.
    /// </summary>
    public enum MotorDirections : byte
    {
        /// <summary>
        ///   Specifies clockwise stepper motor movement.
        /// </summary>
        Clockwise = 0x00,
        /// <summary>
        ///   Specifies counterclockwise stepper motor movement.
        /// </summary>
        Counterclockwise = 0x80
    }
}
