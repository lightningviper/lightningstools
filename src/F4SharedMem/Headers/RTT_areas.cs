using System;

namespace F4SharedMem.Headers
{
    [Serializable]
    // RTT area indices
    public enum RTT_areas : byte
    {
        RTT_HUD = 0,
        RTT_PFL,
        RTT_DED,
        RTT_RWR,
        RTT_MFDLEFT,
        RTT_MFDRIGHT,
        RTT_HMS,
#if EWMU_AND_EWPI_PATCH_APPLIED
        RTT_EWMU,
		RTT_EWPI_CHAFF_FLARE,
		RTT_EWPI_JAMMER_WINDOW,
#endif
        RTT_noOfAreas
    };
}
