using System.Runtime.InteropServices;

namespace Henkie.HSI.Board1
{
    /// <summary>
    ///   Update mode for range digits ("jump scroll" or "smooth scroll" behavior for digit motion)
    /// </summary>
    [ComVisible(true)]
    public enum RangeDigitsScrollMode : byte
    {
        /// <summary>
        ///  Unknown
        /// </summary>
        Default = 0,
        /// <summary>
        ///  Jump scroll
        /// </summary>
        Jump = 0,
        /// <summary>
        ///  Smooth scroll
        /// </summary>
        Smooth = 1
    }
}
