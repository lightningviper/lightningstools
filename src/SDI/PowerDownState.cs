using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Power down state
    /// </summary>
    [ComVisible(true)]
    public enum PowerDownState: byte
    {
        /// <summary>
        ///  Disabled
        /// </summary>
        Disabled = 0x00,
        /// <summary>
        ///  Enabled
        /// </summary>
        Enabled = 1 << 7
    }
}
