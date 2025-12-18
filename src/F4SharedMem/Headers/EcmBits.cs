using System;

namespace F4SharedMem.Headers
{
    [Serializable]
    [Flags]
    public enum EcmBits : uint
    {
        ECM_UNPRESSED_NO_LIT = 0x01,
        ECM_UNPRESSED_ALL_LIT = 0x02,
        ECM_PRESSED_NO_LIT = 0x04,
        ECM_PRESSED_STANDBY = 0x08,
        ECM_PRESSED_ACTIVE = 0x10,
        ECM_PRESSED_TRANSMIT = 0x20,
        ECM_PRESSED_FAIL = 0x40,
        ECM_PRESSED_ALL_LIT = 0x80,
    };
}

