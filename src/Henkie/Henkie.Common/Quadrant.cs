using System.Runtime.InteropServices;

namespace Henkie.Common
{
    /// <summary>
    ///   Quadrants
    /// </summary>
    [ComVisible(true)]
    public enum Quadrant:byte
    {
        /// <summary>
        ///   Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///   Quadrant 1
        /// </summary>
        One = 1,
        /// <summary>
        ///   Quadrant 2
        /// </summary>
        Two = 2,
        /// <summary>
        ///   Quadrant 3
        /// </summary>
        Three = 3,
        /// <summary>
        ///   Quadrant 4
        /// </summary>
        Four = 4,
    }
}
