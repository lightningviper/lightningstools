using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    public enum TriStateSwitchStates : byte
    {
        Down = 0x00,
        Middle = 0x1,
        Up = 0x02,
    };

    [ComVisible(true)]
    [Serializable]
    public enum BiStateSwitchStates : byte
    {
        Off = 0x00,
        On = 0x01,
    };
}
