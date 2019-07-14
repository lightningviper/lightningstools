using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    // RTT area indices
    public enum RTT_areas : int
    {
        RTT_HUD = 0,
        RTT_PFL,
        RTT_DED,
        RTT_RWR,
        RTT_MFDLEFT,
        RTT_MFDRIGHT,
        RTT_HMS,
        RTT_noOfAreas
    };
}
