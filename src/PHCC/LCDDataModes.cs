using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phcc
{
    /// <summary>
    ///   Enumeration of LCD data modes.
    /// </summary>
    public enum LcdDataModes : byte
    {
        /// <summary>
        ///   Specifies that the data being sent to the LCD is to be considered as Display Data
        /// </summary>
        DisplayData = 0x07,
        /// <summary>
        ///   Specifies that the data being sent to the LCD is to be considered as Control Data
        /// </summary>
        ControlData = 0x0F
    }
}
