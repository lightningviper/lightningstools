using System;

namespace AnalogDevices
{
    [Flags]
    internal enum SerialInterfaceModeBits : uint
    {
        M1 = 1 << 23, //bit I23 in serial word bit assignment
        M0 = 1 << 22, //bit I22 in serial word bit assignment

        SpecialFunction = 0, //bits M1=0 and bit M2=0
        WriteToDACGainRegisterM = M0, //bit M1=0 and bit M0=1
        WriteToDACOffsetRegisterC = M1, //bit M1=1 and bit M0=0
        WriteToDACInputDataRegisterX = M1 | M0 //bit M1=1 and bit M1=1
    }
}