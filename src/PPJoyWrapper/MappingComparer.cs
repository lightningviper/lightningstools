using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   Compares two <see cref = "Mapping" />s for the purpose of sorting.
    /// </summary>
    [ComVisible(false)]
    internal sealed class MappingComparer : Comparer<Mapping>
    {
        ///<summary>
        ///  Performs a comparison of two <see cref = "Mapping" />s
        ///  and returns a value indicating whether one <see cref = "Mapping" /> is "less
        ///  than", "equal to", or "greater than" the other for the purpose of
        ///  sorting.
        ///</summary>
        ///<param name = "x">The first <see cref = "Mapping" /> to compare.</param>
        ///<param name = "y">The second <see cref = "Mapping" /> to compare.</param>
        ///<returns>Value Condition:
        ///  Less than zero  -- x is less than y.
        ///  Zero -- x equals y.
        ///  Greater than zero -- x is greater than y.</returns>
        public override int Compare(Mapping x, Mapping y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null && y != null)
            {
                return -1;
            }
            if (x != null && y == null)
            {
                return 1;
            }
            if (x.ControlNumber < y.ControlNumber)
            {
                return -1;
            }
            else if (x.ControlNumber > y.ControlNumber)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}