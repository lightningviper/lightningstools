using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Flags]
    [Serializable]
    public enum NavModes : int
    {
        ILS_TACAN = 0,
        TACAN = 1,
        NAV = 2,
        ILS_NAV = 3,
    };
}
