using System;

namespace F4SharedMem.Headers
{
    [Serializable]
    [Flags]
    public enum EcmOperStates : byte
    {
        ECM_OPER_NO_LIT = 0,
        ECM_OPER_STDBY = 1,
        ECM_OPER_ACTIVE = 2,
        ECM_OPER_ALL_LIT = 3,
    };
}
