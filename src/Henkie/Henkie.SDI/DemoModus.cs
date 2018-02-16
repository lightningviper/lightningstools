using System.Runtime.InteropServices;

namespace Henkie.SDI
{
    /// <summary>
    ///   DEMO feature modus
    /// </summary>
    [ComVisible(true)]    
    public enum DemoModus : byte
    {
        /// <summary>
        ///  Sweep "up" from start position to end position, then sweep "down" from end position to start
        /// </summary>
        UpFromStartToEndThenDown = 0,
        /// <summary>
        ///  Sweep "up" from start position to end position, then start over back at start position again
        /// </summary>
        UpFromStartToEndThenRestart = 1
    }
}
