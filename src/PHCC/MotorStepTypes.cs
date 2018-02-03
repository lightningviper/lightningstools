using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    ///   Enumeration of possible stepper motor step types.
    /// </summary>
    public enum MotorStepTypes : byte
    {
        /// <summary>
        ///   Indicates that the step count refers to the number of full steps that the stepper motor should move.
        /// </summary>
        FullStep = 0x00,
        /// <summary>
        ///   Indicates that the step count refers to the number of half steps that the stepper motor should move.
        /// </summary>
        HalfStep = 0x01
    }
}
