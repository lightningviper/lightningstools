using System;
using System.Runtime.InteropServices;

namespace F4SharedMem.Headers
{
    [ComVisible(true)]
    [Serializable]
    public struct RadioClientStatus
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte ClientFlags;
    }

    [ComVisible(true)]
    [Flags]
    [Serializable]
    public enum ClientFlags
    {
        AllClear = 0x00,            // no status indications present
        ClientActive = 0x01,        // voice client program running
        Connected = 0x02,           // connection established with voice server
        ConnectionFail = 0x8000000, // client failed to connect to voice server
        HostUnknown = 0x10000000,   // bad ip address supplied for voice server
        BadPassword = 0x20000000,   // password rejected by voice server
        NoMicrophone = 0x40000000,  // no input device detected by voice client
        NoSpeakers = -2147483648,   //0x80000000,  // no output device detected by voice client
        ErrorMask = -134217728      //0xF8000000,  // mask including all the error bits
    };

}
