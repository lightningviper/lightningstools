using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    public enum TacanSources : int
    {
        UFC = 0,
        AUX = 1,
        NUMBER_OF_SOURCES = 2,
    };
}
