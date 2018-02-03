using System.Runtime.InteropServices;

namespace SDI
{
    /// <summary>
    ///   Polarity
    /// </summary>
    [ComVisible(true)]
    public enum Polarity : byte
    {
        /// <summary>
        ///  Negative
        /// </summary>
        Negative = 0,
        /// <summary>
        ///  Positive
        /// </summary>
        Positive = 1,
    }
}