using System.Runtime.InteropServices;

namespace Henkie.Common
{
    /// <summary>
    ///   USB Debug feature prefix characters
    /// </summary>
    [ComVisible(true)]
    public enum USBDebugCharacter : byte
    {
        /// <summary>
        /// Indicates that the next two characters are hexadecimal command (first byte)
        /// </summary>
        Command=0x2D,                     // '-'
        /// <summary>
        /// Indicates that the next two characters are hexadecimal data (second byte)
        /// </summary>
        Data=0x3D,                        //  '='
        /// <summary>
        /// "Disable watchdog" command received
        /// </summary>
        DisableWatchdog=0x78,             //  'x'
        /// <summary>
        /// Invalid subaddress in command
        /// </summary>
        InvalidSubaddressInCommand=0x23,  //   '#'
        /// <summary>
        /// USB receive data state machine in illegal state (firmware error)
        /// </summary>
        FirmwareError = 0x21              //'!'
    }
}
