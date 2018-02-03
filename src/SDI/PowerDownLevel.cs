using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Stator power down level
    /// </summary>
    [ComVisible(true)]
    public enum PowerDownLevel : byte
    {
        /// <summary>
        ///  Half power down
        /// </summary>
        Half = 0x00,
        /// <summary>
        ///  Full power down
        /// </summary>
        Full = 1 <<6       
    }
}
