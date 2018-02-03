using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Flags]
    [Serializable]
    public enum CmdsModes : int
    {
        CmdsOFF = 0,
        CmdsSTBY = 1,
        CmdsMAN = 2,
        CmdsSEMI = 3,
        CmdsAUTO = 4,
        CmdsBYP = 5,
    };
}
