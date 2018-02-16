using System.Runtime.InteropServices;

namespace Henkie.Common
{
    /// <summary>
    ///  Diagnostic LED operation modes
    /// </summary>
    [ComVisible(true)]
    public enum DiagnosticLEDMode : byte
    {
        /// <summary>
        ///  Off
        /// </summary>
        Off = 0,
        /// <summary>
        ///  On
        /// </summary>
        On = 1,
        /// <summary>
        ///  Heartbeat
        /// </summary>
        Heartbeat = 2,
        /// <summary>
        ///  Toggle ON/OFF state per accepted command
        /// </summary>
        ToggleOnAcceptedCommand = 3,
    }
}