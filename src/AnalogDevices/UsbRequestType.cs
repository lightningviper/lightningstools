using System;

namespace AnalogDevices
{
    [Flags]
    public enum UsbRequestType : byte
    {
        TypeStandard = 0,
        TypeClass = 32,
        TypeVendor = 64,
        TypeReserved = 96
    }
}