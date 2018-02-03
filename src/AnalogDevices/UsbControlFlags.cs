using System;

namespace AnalogDevices
{
    [Flags]
    public enum UsbCtrlFlags : byte
    {
        Direction_Out = 0,
        Recipient_Device = 0,
        RequestType_Standard = 0,
        Recipient_Interface = 1,
        Recipient_Endpoint = 2,
        Recipient_Other = 3,
        RequestType_Class = 32,
        RequestType_Vendor = 64,
        RequestType_Reserved = 96,
        Direction_In = 128
    }
}