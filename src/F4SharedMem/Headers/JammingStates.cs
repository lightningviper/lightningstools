﻿using System;

namespace F4SharedMem.Headers
{
    [Serializable]
    // JammingStates
    public enum JammingStates : byte
    {
        JAMMED_NO = 0,
        JAMMED_YES = 1,
        JAMMED_SHOULD = 2,
    };
}
