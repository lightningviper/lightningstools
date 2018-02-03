using System;
using System.Runtime.InteropServices;

namespace PPJoy
{

    #region Internal enums

    /// <summary>
    ///   Enumeration of map prefixes for specific device capabilities/control types.
    ///   These byte values are used as signals within raw mapping data 
    ///   sent to/received from PPJoy.  They declare the presence of a control
    ///   of the specified type and are followed by a second byte that describes
    ///   further details of the control declaration.
    /// </summary>
    internal enum DeviceCapabilitiesPrefixes : byte
    {
        OrdinaryAxisOrPOV = 1,
        WheelOrThrottleAxis = 2,
        Button = 9
    }

    /// <summary>
    ///   Enumeration of file device types (subset of NativeMethods.EFileDevice) that
    ///   can be referenced by native IOCTL calls.
    /// </summary>
    internal enum FileDevices : uint
    {
        VirtualDevice = NativeMethods.EFileDevice.Unknown, //identifies the PPJoy device type
        DeviceBus = NativeMethods.EFileDevice.BusExtender,
    }

    /// <summary>
    ///   Enumeration of message version signatures used by PPJoy in various message types
    /// </summary>
    internal enum MessageVersions : uint
    {
        JoystickStateV1 = 0x53544143, //identifies the version of the JOYSTATE struct that gets passed to PPJoy
        JoystickMapV1 = 0x454E4F47
    }

    #endregion

    #region Internal Struct Declarations

    /// <summary>
    ///   Structure that gets passed over IOCTL interface to PPJOY to update the state
    ///   of analog and digital data sources that PPJoy virtual devices get their
    ///   data from.  Can also be returned by PPJoy in response to a GetState command.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct JoystickState
    {
        internal UInt32 Signature; // Signature to identify packet to PPJoy IOCTL
        internal Byte NumAnalog; // Num of analog values we pass 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VirtualJoystick.MaxAnalogDataSources)] internal Int32[] Analog;
        // Analog input values

        internal Byte NumDigital; // Number of digital values we pass

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VirtualJoystick.MaxDigitalDataSources)] internal Byte[] Digital;
        // Digital input values
    } ;

    /// <summary>
    ///   PPJoy IOCTL header that describes a specific PPJoy device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DeviceInfo
    {
        internal UInt32 Size; // Number of bytes in this structure 
        internal UInt32 PortAddress; // Base address for LPT port specified in LPTNumber 
        internal Byte JoyType; // Index into joystick type table. 0 is no joystick 
        internal Byte JoySubType; // Sub-model of joystick type, 0 if not applicable 
        internal Byte UnitNumber; // Index of joystick on interface. First unit is 0 
        internal Byte LPTNumber; // Number of LPT port, 0 is virtual interface 
        internal UInt16 VendorID; // PnP vendor ID for this device 
        internal UInt16 ProductID; // PnP product ID for this device 
    }

    /// <summary>
    ///   PPJoy IOCTL Header that is used to add a device to the system. 
    ///   Callers should fill all the fields of AddDeviceMessage 
    ///   except the PortAddress field.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AddDeviceMessage
    {
        internal DeviceInfo JoyData; //Data for joystick to add 
        internal UInt32 Persistent; // 1= automatically add joystick after reboot 
    }

    /// <summary>
    ///   PPJoy IOCTL header that specifies a joystick to delete. 
    ///   Callers should fill in the Size, JoyType, 
    ///   UnitNumber and LPTNumber fields in the JoyData DeviceInfo structure. 
    ///   Set all other fields to 0.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RemoveDeviceMessage
    {
        internal DeviceInfo JoyData; // Data for joystick to delete
    }

    /// <summary>
    ///   PPJoy IOCTL header that will enumerate all the joysticks currently defined
    ///   within PPJoy.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct EnumerateDevicesMessage
    {
        internal UInt32 Count; // Number of joystick currently defined 
        internal UInt32 Size; // Size needed to enumerate all joysticks 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VirtualJoystick.MaxVirtualDevices)] internal DeviceInfo[]
            Joysticks; // Array of joystick records (holds return values)
    }

    /// <summary>
    ///   PPJoy IOCTL structure that contains a payload for a map definition message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct JoystickMapPayload
    {
        internal Byte NumAxes; //the number of axes declared in the map data
        internal Byte NumButtons; //the number of buttons declared in the map data
        internal Byte NumHats; //the number of Point-of-View hats declared in the map data
        internal Byte NumMaps; //the number of maps declared in the map data (always =1)

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int) Headers.MaxMappingPayloadLength)] internal Byte[] Data;
        //the raw map data
    }

    /// <summary>
    ///   PPJoy IOCTL header that indicates a virtual joystick map data packet.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct JoystickMapHeader
    {
        internal UInt32 Version; //a signature describing the version of this header being used
        internal Byte MapScope; //indicates the scope to which the map contained in the payload applies
        internal Byte JoyType; //describes the type of joystick to which the map applies
        internal Byte Pad1; //extra space
        internal Byte Pad2; //extra space
        internal UInt32 MapSize; //number of bytes in the map payload structure
        internal JoystickMapPayload MapData; //map payload structure
    }

    #endregion

    /// <summary>
    ///   Contains enums, structs, and methods for communicating with PPJoy's device drivers at a low level via P/Invoke.
    /// </summary>
    internal class Headers
    {
        private Headers()
        {
        }

        #region Dynamic Declarations

        internal const UInt32 MaxMappingPayloadLength = 250;
        //(uint)(4 * VirtualJoystick.MaxVisibleAxes) + (3 * VirtualJoystick.MaxVisibleButtons) + (6 * VirtualJoystick.MaxVisiblePovs);

        internal static UInt32 IoCtlCreatePPJoyDevice = GetPPJoyBusIoCtlCode(0x3);
        internal static UInt32 IoCtlDeletePPJoyDevice = GetPPJoyBusIoCtlCode(0x4);
        internal static UInt32 IoCtlEnumeratePPJoyDevices = GetPPJoyBusIoCtlCode(0x5);
        internal static UInt32 IoCtlSetPPJoyDeviceState = GetPPJoyDeviceIoCtlCode(0x0);
        internal static UInt32 IoCtlSetPPJoyDeviceMappings = GetPPJoyDeviceIoCtlCode(0x2);
        internal static UInt32 IoCtlGetPPJoyDeviceMappings = GetPPJoyDeviceIoCtlCode(0x3);
        internal static UInt32 IoCtlDeletePPJoyDeviceMappings = GetPPJoyDeviceIoCtlCode(0x4);

        /// <summary>
        ///   Looks up the PPJOY IOCTL code for a given opcode (e.g. write)
        /// </summary>
        /// <param name = "_index_">PPJOY opcode</param>
        /// <returns>IOCTL code for PPJOY device type, using the buffered write method and ANY file access</returns>
        internal static UInt32 GetPPJoyDeviceIoCtlCode(UInt32 _index_)
        {
            return NativeMethods.GetIoCtlCode((uint) FileDevices.VirtualDevice, _index_, NativeMethods.EMethod.Buffered,
                                              NativeMethods.ECTL_CODEFileAccess.FileAnyAccess);
        }

        /// <summary>
        ///   Looks up the PPJOY BUS IOCTL code for a given opcode (e.g. write)
        /// </summary>
        /// <param name = "_index_">PPJOY bus opcode</param>
        /// <returns>IOCTL code for PPJOY bus device type, using the buffered write method and ANY file access</returns>
        internal static UInt32 GetPPJoyBusIoCtlCode(UInt32 _index_)
        {
            return NativeMethods.GetIoCtlCode((uint) FileDevices.DeviceBus, _index_, NativeMethods.EMethod.Buffered,
                                              NativeMethods.ECTL_CODEFileAccess.FileAnyAccess);
        }

        #endregion
    }
}