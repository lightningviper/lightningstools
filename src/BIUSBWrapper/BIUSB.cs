using System;
using System.Runtime.InteropServices;

namespace BIUSBWrapper
{
    /******************************************************************************
     RWR symbol constants (presented as an enumeration) from rwr.h
    ******************************************************************************/

    public enum CRTRWRSymbols : byte
    {
        /// <summary>
        ///   do not display any symbol
        /// </summary>
        RWRSYM_NONE = 0,
        /// <summary>
        ///   U
        /// </summary>
        RWRSYM_UNKNOWN = 1,
        /// <summary>
        ///   not implemented
        /// </summary>
        RWRSYM_ADVANCED_INTERCEPTOR = 2,
        /// <summary>
        ///   not implemented
        /// </summary>
        RWRSYM_BASIC_INTERCEPTOR = 3,
        /// <summary>
        ///   M
        /// </summary>
        RWRSYM_ACTIVE_MISSILE = 4,
        /// <summary>
        ///   H
        /// </summary>
        RWRSYM_HAWK = 5,
        /// <summary>
        ///   P
        /// </summary>
        RWRSYM_PATRIOT = 6,
        /// <summary>
        ///   2
        /// </summary>
        RWRSYM_SA2 = 7,
        /// <summary>
        ///   3
        /// </summary>
        RWRSYM_SA3 = 8,
        /// <summary>
        ///   4
        /// </summary>
        RWRSYM_SA4 = 9,
        /// <summary>
        ///   5
        /// </summary>
        RWRSYM_SA5 = 10,
        /// <summary>
        ///   6
        /// </summary>
        RWRSYM_SA6 = 11,
        /// <summary>
        ///   8
        /// </summary>
        RWRSYM_SA8 = 12,
        /// <summary>
        ///   9
        /// </summary>
        RWRSYM_SA9 = 13,
        /// <summary>
        ///   10
        /// </summary>
        RWRSYM_SA10 = 14,
        /// <summary>
        ///   13
        /// </summary>
        RWRSYM_SA13 = 15,
        /// <summary>
        ///   A with a single dot beneath it
        /// </summary>
        RWRSYM_A1 = 16,
        /// <summary>
        ///   S
        /// </summary>
        RWRSYM_SEARCH = 17,
        /// <summary>
        ///   boat symbol
        /// </summary>
        RWRSYM_NAVAL = 18,
        /// <summary>
        ///   C
        /// </summary>
        RWRSYM_CHAPARAL = 19,
        /// <summary>
        ///   15 or M alternating
        /// </summary>
        RWRSYM_SA15 = 20,
        /// <summary>
        ///   N
        /// </summary>
        RWRSYM_NIKE = 21,
        /// <summary>
        ///   A or S alternating
        /// </summary>
        RWRSYM_AAA = 22,
        /// <summary>
        ///   A with two dots beneath it
        /// </summary>
        RWRSYM_A2 = 23,
        /// <summary>
        ///   not implemented
        /// </summary>
        RWRSYM_A3 = 24,
        /// <summary>
        ///   P with a dot beneath it
        /// </summary>
        RWRSYM_PDOT = 25,
        /// <summary>
        ///   P with a vertical bar on right side
        /// </summary>
        RWRSYM_PSLASH = 26,
        /// <summary>
        ///   U with one dot beneath it
        /// </summary>
        RWRSYM_UNK1 = 27,
        /// <summary>
        ///   U with two dots beneath it
        /// </summary>
        RWRSYM_UNK2 = 28,
        /// <summary>
        ///   U with three dots beneath it
        /// </summary>
        RWRSYM_UNK3 = 29,
        /// <summary>
        ///   C
        /// </summary>
        RWRSYM_KSAM = 30,
        /// <summary>
        ///   4
        /// </summary>
        RWRSYM_V4 = 32,
        /// <summary>
        ///   5
        /// </summary>
        RWRSYM_V5 = 33,
        /// <summary>
        ///   6
        /// </summary>
        RWRSYM_V6 = 34,
        /// <summary>
        ///   14
        /// </summary>
        RWRSYM_V14 = 35,
        /// <summary>
        ///   15
        /// </summary>
        RWRSYM_V15 = 36,
        /// <summary>
        ///   16
        /// </summary>
        RWRSYM_V16 = 37,
        /// <summary>
        ///   18
        /// </summary>
        RWRSYM_V18 = 38,
        /// <summary>
        ///   19
        /// </summary>
        RWRSYM_V19 = 39,
        /// <summary>
        ///   20
        /// </summary>
        RWRSYM_V20 = 40,
        /// <summary>
        ///   21
        /// </summary>
        RWRSYM_V21 = 41,
        /// <summary>
        ///   22
        /// </summary>
        RWRSYM_V22 = 42,
        /// <summary>
        ///   23
        /// </summary>
        RWRSYM_V23 = 43,
        /// <summary>
        ///   25
        /// </summary>
        RWRSYM_V25 = 44,
        /// <summary>
        ///   27
        /// </summary>
        RWRSYM_V27 = 45,
        /// <summary>
        ///   29
        /// </summary>
        RWRSYM_V29 = 46,
        /// <summary>
        ///   30
        /// </summary>
        RWRSYM_V30 = 47,
        /// <summary>
        ///   31
        /// </summary>
        RWRSYM_V31 = 48,
        /// <summary>
        ///   P
        /// </summary>
        RWRSYM_VP = 49,
        /// <summary>
        ///   PD
        /// </summary>
        RWRSYM_VPD = 50,
        /// <summary>
        ///   A
        /// </summary>
        RWRSYM_VA = 51,
        /// <summary>
        ///   B
        /// </summary>
        RWRSYM_VB = 52,
        /// <summary>
        ///   S
        /// </summary>
        RWRSYM_VS = 53,
        /// <summary>
        ///   A with a vertical bar on right
        /// </summary>
        RWRSYM_Aa = 54,
        /// <summary>
        ///   A with vertical bars on left and right side
        /// </summary>
        RWRSYM_Ab = 55,
        /// <summary>
        ///   A with vertical bars on left, center and right side
        /// </summary>
        RWRSYM_Ac = 56,
        /// <summary>
        ///   F or S alternating
        /// </summary>
        RWRSYM_MIB_F_S = 57,
        /// <summary>
        ///   F or A alternating
        /// </summary>
        RWRSYM_MIB_F_A = 58,
        /// <summary>
        ///   F or M alternating
        /// </summary>
        RWRSYM_MIB_F_M = 59,
        /// <summary>
        ///   F or U alternating
        /// </summary>
        RWRSYM_MIB_F_U = 60,
        /// <summary>
        ///   F or basic interceptor shape
        /// </summary>
        RWRSYM_MIB_F_BW = 61,
        /// <summary>
        ///   S or basic interceptor shape
        /// </summary>
        RWRSYM_MIB_BW_S = 62,
        /// <summary>
        ///   A or basic interceptor shape
        /// </summary>
        RWRSYM_MIB_BW_A = 63,
        /// <summary>
        ///   M or basic interceptor shape
        /// </summary>
        RWRSYM_MIB_BW_M = 64,
        /// <summary>
        ///   reserved for future use
        /// </summary>
        RWRSYM_SA0 = 0xFA,
        /// <summary>
        ///   reserved for future use
        /// </summary>
        RWRSYM_SA1 = 0xFB,
        /// <summary>
        ///   reserved for future use
        /// </summary>
        RWRSYM_SA7 = 0xFC,
        /// <summary>
        ///   Diamond symbol
        /// </summary>
        RWRSYM_DIAMOND = 0xFD,
        /// <summary>
        ///   Missile launch symbol
        /// </summary>
        RWRSYM_LAUNCH = 0xFE,
        /// <summary>
        ///   Draw test pattern
        /// </summary>
        RWRSYM_TEST = 0xFF,
    }

    /******************************************************************************
     User-defined types for API calls
    ******************************************************************************/

    /// <summary>
    ///   Structure to hold return values from the <see cref = "RetrieveStatus">RetrieveStatus</see> method.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct DeviceStatus
    {
        /// <summary>
        ///   Inputs is active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_INPUTS)] public byte[] InputActive;

        /// <summary>
        ///   Output is active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_OUTPUTS)] public byte[] OutputActive;

        /// <summary>
        ///   Port is active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_PORTS)] public byte[] PortActive;

        /// <summary>
        ///   Port is set to input mode if 1, output mode if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_PORTS)] public byte[] PortIOMode;

        /// <summary>
        ///   Currently only valid for ElectronFlux class devices. 
        ///   <para />Return values are: 
        ///   <para /><see cref = "MODE_DIRECT">MODE_DIRECT</see> = 0 
        ///   <para /><see cref = "MODE_MUX">MODE_MUX</see> = 1 
        ///   <para /><see cref = "MODE_RWR">MODE_RWR</see> = 2 
        ///   <para /><see cref = "MODE_GLCD">MODE_GLCD</see> = 3
        ///   <para /><see cref = "MODE_DAC">MODE_DAC</see> = 4
        ///   <para /><see cref = "MODE_LATCHED">MODE_LATCHED</see> = 5
        ///   <para /><see cref = "MODE_SPI">MODE_SPI</see> = 6
        ///   <para /><see cref = "MODE_DOTMATRIX">MODE_DOTMATRIX</see> = 7
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_PORTS)] public byte[] PortMode;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_PORTS)] public byte[] OutputMode;

        /// <summary>
        ///   Analog channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_ANALOG)] public byte[] AnalogActive;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_HATS)] public byte[] HATActive;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_ROTARY)] public byte[] RotaryActive;

        /// <summary>
        ///   CRT RWR channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_RWR)] public byte[] CRTRWRActive;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_PWM)] public byte[] PWMActive;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_LCDS)] public byte[] LCDActive;

        /// <summary>
        ///   Graphic LCD channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_GLCDS)] public byte[] GLCDActive;

        /// <summary>
        ///   Multiplexed display channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_MUXDISPLAYS)] public byte[] MuxDisplayActive;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_ALPHANUMERICS)] public byte[] AlphanumericActive;

        /// <summary>
        ///   DAC channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_DACS)] public byte[] DACActive;

        /// <summary>
        ///   SPI channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_SPI)] public byte[] SPIActive;

        /// <summary>
        ///   Latched channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_LATCHED)] public byte[] LatchedActive;

        /// <summary>
        ///   DotMatrix channels active if 1, disabled if 0.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_DOTMATRIX)] public byte[] DotMatrixActive;
    }

    /// <summary>
    ///   Structure to hold return values from the <see cref = "DetectHID">DetectHID</see> method.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct DeviceParam
    {
        /// <summary>
        ///   The read/write handle for the device. This is a unique value 
        ///   and changes with each call to DetectHID.
        /// </summary>
        [MarshalAs(UnmanagedType.SysInt)] public IntPtr DeviceHandle;

        /// <summary>
        ///   The number of unpacked inputs that will be returned from this device.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberInputIndices;

        /// <summary>
        ///   The number of unpacked outputs that will be read by this device.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberOutputIndices;

        /// <summary>
        ///   The number of separate IO ports on a device. These can also refer to JPs found on older devices.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberPortIndices;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberAnalogIndices;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberHATIndices;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberRotaryIndices;

        /// <summary>
        ///   Number of RWR supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberCRTRWRIndices;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberPWMIndices;

        /// <summary>
        ///   Number of character-based LCDs supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberLCDIndices;

        /// <summary>
        ///   Number of graphic based LCDs supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberGLCDIndices;

        /// <summary>
        ///   Number of multiplexed displays supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberMuxDisplayIndices;

        /// <summary>
        ///   Not currently supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberAlphanumericIndices;

        /// <summary>
        ///   Number of DAC (Digital to Analog Converter) supported.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort NumberDACIndices;

        //Number of SPI interface devices supported.
        [MarshalAs(UnmanagedType.U2)] public ushort NumberSPIIndices;
        //Number of latched outputs supported.
        [MarshalAs(UnmanagedType.U2)] public ushort NumberLatchedIndices;
        //Number of OSRAM type dot matrix displays supported.
        [MarshalAs(UnmanagedType.U2)] public ushort NumberDotMatrixIndices;

        /// <summary>
        ///   USB.org assigned unique vendor ID number.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort VendorID;

        /// <summary>
        ///   Vendor assigned unique device ID number.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort ProductID;

        /// <summary>
        ///   Vendor assigned product version number.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort VersionNumber;

        /// <summary>
        ///   Current Firmware revision of device.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort FlashVersion;

        /// <summary>
        ///   For internal use only.
        /// </summary>
        public byte ProgramFlag;

        /// <summary>
        ///   Device path key string as found in the Windows registry.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_CHAR)] public byte[] DevicePath;

        /// <summary>
        ///   Length in bytes of the <see cref = "DevicePath">DevicePath</see> string.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public uint PathLength;

        /// <summary>
        ///   Device name string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_CHAR)] public byte[] DeviceName;

        /// <summary>
        ///   Length in bytes of the <see cref = "DeviceName">DeviceName</see> string.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public uint DeviceNameLength;

        /// <summary>
        ///   Manufacturer name string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BIUSB.MAX_CHAR)] public byte[] ManufName;

        /// <summary>
        ///   Length in bytes of the <see cref = "ManufName">ManufName</see> string.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public uint ManufNameLength;

        /// <summary>
        ///   User assigned device serial number string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] SerialNum;

        /// <summary>
        ///   Length in bytes of the <see cref = "SerialNum">SerialNum</see> string.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public uint SerialNumLength;

        /// <summary>
        ///   For internal use only.
        /// </summary>
        public byte ConfigFlag;

        /// <summary>
        ///   The zero based device index associated with this device.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort DevIndex;

        /// <summary>
        ///   The number of packed bytes that will be returned by the device on each <see cref = "ReadInputData">ReadInputData</see> read request.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort InputReportByteLength;

        /// <summary>
        ///   The number of packed bytes used by the device for output data.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort OutputReportByteLength;

        /// <summary>
        ///   For internal use only.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort Usage;

        /// <summary>
        ///   For internal use only.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public ushort UsagePage;
    }

    /// <summary>
    ///   Data structure for sending output using the 
    ///   <see cref = "WriteDirectOutput">WriteDirectOutput</see> 
    ///   and <see cref = "WriteMuxOutput">WriteMuxOutput</see> commands.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct DIRECT_OUTPUT
    {
        /// <summary>
        ///   The low-order bit (bit 1) - set to 0 for OFF and to 1 for ON.
        ///   The upper 7 bits specify the output level range from 0 to 10.  This value sets the pulse width of the multiplexed output controlling intensity.
        /// </summary>
        public byte state;
    }

    /// <summary>
    ///   Data structure for sending output using the 
    ///   <see cref = "WriteDirectOutput">WriteCRTRWR</see> command.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct CRTRWR_OUTPUT
    {
        /// <summary>
        ///   Symbol ID to display (refer to RWR Symbol Class table entries found in rwr.h)
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public CRTRWRSymbols symbol;

        /// <summary>
        ///   X position on screen to display symbol.  Range 0 (left) to 255 (right).
        /// </summary>
        public byte xPos;

        /// <summary>
        ///   Y position on screen to display symbol.  Range 0 (top) to 255 (bottom).
        /// </summary>
        public byte yPos;

        /// <summary>
        ///   1st (low-order) bit    : missileActivity	       -- if set to 1, display missile activity symbol.
        ///   <para />2nd bit                 : missileLaunch     -- if set to 1, display missile launch symbol
        ///   <para />3rd bit                 : newguy            -- if set to 1, display new contact symbol
        ///   <para />4th bit                 : selected          -- if set to 1, draw contact selected symbol
        ///   <para />5th thru 8th bits       : unused
        /// </summary>
        public byte flags;
    }

    /// <summary>
    ///   Data structure for sending output using the <see cref = "WriteSPIDAC">WriteSPIDAC</see> command.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct DAC_OUTPUTS
    {
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_1;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_2;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_3;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_4;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_5;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_6;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_7;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_8;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_9;
        [MarshalAs(UnmanagedType.U2)] public ushort DAC_10;
    }

    /// <summary>
    ///   TODO: document
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [ComVisible(false)]
    public struct FUSION_OUTPUT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] text_a;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] text_b;
        public byte gear;
        public byte lbg8;
        public byte shiftlight;
        public byte level;
    }


    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(false)]
    public sealed class BIUSB
    {
        public const ushort VENDOR_ID_1 = 0x6666;
        public const ushort VENDOR_ID_2 = 0x12DA;
        public const string MANUF_NAME = "Beta Innovations Inc.";

        /* ----------------------------------------
         * USB PRODUCT IDs : HID DEVICES (DT_HID)
         * ---------------------------------------*/

        /// <summary>
        ///   0X64B
        /// </summary>
        public const ushort PID_0X64B = 0x300;

        /// <summary>
        ///   GammaRay-256
        /// </summary>
        public const ushort PID_GAMMARAY = 0x500;

        /// <summary>
        ///   GammaRay-256 V2 Core
        /// </summary>
        public const ushort PID_GAMMARAY_V2 = 0x510;

        /// <summary>
        ///   GammaRay-256 V3 Core
        /// </summary>
        public const ushort PID_GAMMARAY_V3 = 0x520;

        /// <summary>
        ///   GammaRay-64
        /// </summary>
        public const ushort PID_GAMMARAY64 = 0x700;

        /// <summary>
        ///   GammaTron Core
        /// </summary>
        public const ushort PID_GAMMATRON = 0x710;

        /// <summary>
        ///   ElectronFlux Core
        /// </summary>
        public const ushort PID_ELECTRONFLUX = 0x800;

        /// <summary>
        ///   Nitro-SLG Core
        /// </summary>
        public const ushort PID_NITRO_SLG = 0x1500;

        public const string PIDN_0X64B = "0X64B USB MODULE";
        public const string PIDN_GAMMARAY = "GammaRay-256 USB MODULE";
        public const string PIDN_GAMMARAY64 = "GammaRay-64 USB MODULE";
        public const string PIDN_ELECTRONFLUX = "ElectronFlux";
        public const string PIDN_GAMMARAY_V2 = "GammaRay V2";
        public const string PIDN_GAMMARAY_V3 = "GammaRay V3";
        public const string PIDN_GAMMATRON = "GammaTron";
        public const string PIDN_NITRO_SLG = "Nitro-SLG";

        /* --------------------------------------------------------------------------------
         * USB PRODUCT IDs : STANDARD JOYSTICK DEVICES (DT_DEVICES)
         * -------------------------------------------------------------------------------*/

        /// <summary>
        ///   4X24BH
        /// </summary>
        public const ushort PID_4X24BH = 0x100;

        /// <summary>
        ///   6X16B
        /// </summary>
        public const ushort PID_6X16B = 0x200;

        /// <summary>
        ///   6X13B
        /// </summary>
        public const ushort PID_6X13B = 0x250;

        /// <summary>
        ///   5X18BH
        /// </summary>
        public const ushort PID_5X18BH = 0x400;

        /// <summary>
        ///   Plasma single device
        /// </summary>
        public const ushort PID_PLASMA = 0x600;

        /// <summary>
        ///   Plasma HOTAS single device
        /// </summary>
        public const ushort PID_PLASMA_HOTAS = 0x601;

        /// <summary>
        ///   Plasma DUAL device
        /// </summary>
        public const ushort PID_PLASMA_DUAL = 0x602;

        /// <summary>
        ///   Plasma HOTAS DUAL device
        /// </summary>
        public const ushort PID_PLASMA_HOTAS_DUAL = 0x603;

        /// <summary>
        ///   PLASMA-LITE
        /// </summary>
        public const ushort PID_PLASMA_LITE = 0x900;

        /// <summary>
        ///   PLASMA-LITE V2
        /// </summary>
        public const ushort PID_PLASMA_LITE_V2 = 0x910;

        /// <summary>
        ///   PLASMA-MM2
        /// </summary>
        public const ushort PID_PLASMA_MM2 = 0x920;

        /// <summary>
        ///   FUSION
        /// </summary>
        public const ushort PID_FUSION = 0x1600;


        public const string PIDN_4X24BH = "4X24BH";
        public const string PIDN_6X16B = "6X16B";
        public const string PIDN_6X13B = "6X13B";
        public const string PIDN_5X18BH = "5X18BH";
        public const string PIDN_PLASMA = "Plasma - USB Adapter";
        public const string PIDN_PLASMA_HOTAS = "Plasma HOTAS - USB Adapter";
        public const string PIDN_PLASMA_DUAL = "Plasma - Dual USB Adapter";
        public const string PIDN_PLASMA_HOTAS_DUAL = "Plasma HOTAS - Dual USB Adapter";
        public const string PIDN_PLASMA_LITE = "Plasma-Lite";
        public const string PIDN_PLASMA_LITE_V2 = "Plasma-Lite";
        public const string PIDN_PLASMA_MM2 = "Plasma-MM2";
        public const string PIDN_FUSION = "Fusion";


        /* --------------------------------------------------------------------------------
         * Configuration devices
         * --------------------------------------------------------------------------------*/

        /// <summary>
        ///   PLASMA-LITE V2 CONFIG HID
        /// </summary>
        public const ushort PIDC_PLASMA_LITE_V2 = 0x0910;

        /// <summary>
        ///   GammaRay V2 CONFIG HID
        /// </summary>
        public const ushort PIDC_GAMMARAY_V2 = 0x0510;

        /// <summary>
        ///   GammaRay V3 CONFIG HID
        /// </summary>
        public const ushort PIDC_GAMMARAY_V3 = 0x0520;

        /// <summary>
        ///   ElectronFlux CONFIG HID
        /// </summary>
        public const ushort PIDC_ELECTRONFLUX = 0x0800;

        /// <summary>
        ///   GammaTron CONFIG HID
        /// </summary>
        public const ushort PIDC_GAMMATRON = 0x0710;

        /// <summary>
        ///   PLASMA-MM2 CONFIG HID
        /// </summary>
        public const ushort PIDC_PLASMA_MM2 = 0x0920;

        /// <summary>
        ///   NITRO-SLG CONFIG HID
        /// </summary>
        public const ushort PIDC_NITRO_SLG = 0x1500;

        /// <summary>
        ///   FUSION CONFIG HID
        /// </summary>
        public const ushort PIDC_FUSION = 0x1600;

        public const string PIDNC_PLASMA_LITE_V2 = "Plasma-Lite CFG";
        public const string PIDNC_GAMMARAY_V2 = "GammaRay V2 CFG";
        public const string PIDNC_GAMMARAY_V3 = "GammaRay V3 CFG";
        public const string PIDNC_ELECTRONFLUX = "ElectronFlux CFG";
        public const string PIDNC_GAMMATRON = "GammaTron CFG";
        public const string PIDNC_PLASMA_MM2 = "Plasma-MM2 CFG";
        public const string PIDNC_NITRO_SLG = "NITRO-SLG CFG";
        public const string PIDNC_FUSION = "FUSION CFG";

        /* --------------------------------------------------------------------------------
         * Flash Loader devices
         * --------------------------------------------------------------------------------*/

        /// <summary>
        ///   PLASMA-LITE V2 Flash Loader
        /// </summary>
        public const ushort PIDF_PLASMA_LITE_V2 = 0x1000;

        /// <summary>
        ///   GammaRay V2 Flash Loader
        /// </summary>
        public const ushort PIDF_GAMMARAY_V2 = 0x1100;

        /// <summary>
        ///   GammaRay V3 Flash Loader
        /// </summary>
        public const ushort PIDF_GAMMARAY_V3 = 0x1110;

        /// <summary>
        ///   ElectronFlux Flash Loader
        /// </summary>
        public const ushort PIDF_ELECTRONFLUX = 0x1200;

        /// <summary>
        ///   GammaTron Flash Loader
        /// </summary>
        public const ushort PIDF_GAMMATRON = 0x1300;

        /// <summary>
        ///   PLASMA-MM2 Flash Loader
        /// </summary>
        public const ushort PIDF_PLASMA_MM2 = 0x1010;

        /// <summary>
        ///   NITRO-SLG Flash Loader
        /// </summary>
        public const ushort PIDF_NITRO_SLG = 0x1400;

        /// <summary>
        ///   FUSION Flash Loader
        /// </summary>
        public const ushort PIDF_FUSION = 0x1700;

        public const string PIDNF_PLASMA_LITE_V2 = "Plasma-Lite";
        public const string PIDNF_GAMMARAY_V2 = "GammaRay V2";
        public const string PIDNF_GAMMARAY_V3 = "GammaRay V3";
        public const string PIDNF_ELECTRONFLUX = "ElectronFlux";
        public const string PIDNF_GAMMATRON = "GammaTron";
        public const string PIDNF_PLASMA_MM2 = "Plasma-MM2";
        public const string PIDNF_NITRO_SLG = "NITRO-SLG";
        public const string PIDNF_FUSION = "FUSION";


        /******************************************************************************
         API constants
        ******************************************************************************/
        public const ushort MAX_DEVICES = 128;
        public const ushort MAX_PORTS = 24;
        public const ushort MAX_INPUTS = 512;
        public const ushort MAX_OUTPUTS = MAX_PORTS*15;
        public const ushort MAX_ANALOG = 8;
        public const ushort MAX_HATS = 4;
        public const ushort MAX_ROTARY = 200;
        public const ushort MAX_PWM = 512;
        public const ushort MAX_LCDS = 200;
        public const ushort MAX_GLCDS = MAX_PORTS;
        public const ushort MAX_MUXDISPLAYS = MAX_PORTS*56;
        public const ushort MAX_ALPHANUMERICS = 100;
        public const ushort MAX_RWR = MAX_PORTS;
        public const ushort MAX_RWR_CONTACTS = 40;
        public const ushort MAX_DACS = MAX_PORTS*10;
        public const ushort MAX_SPI = MAX_PORTS*10;
        public const ushort MAX_LATCHED = MAX_PORTS*64;
        public const ushort MAX_DOTMATRIX = MAX_PORTS*4;
        public const ushort MAX_CHAR = 256;
        public const ushort MAX_RETRY = 6;
        public const ushort MAX_INPUT_REPORT_BYTES = 65;
        public const ushort MAX_OUTPUT_REPORT_BYTES = 65;
        public const ushort MAX_TIMEOUT_MSEC = 1000;


        // RETURN CONSTANTS
        /// <summary>
        ///   Failure reading from device.
        /// </summary>
        public const ushort DEV_FAILED = 0x1;

        /// <summary>
        ///   New data available.
        /// </summary>
        public const ushort DEV_INPUT = 0x2;

        /// <summary>
        ///   No new data available. Waiting for device response.
        /// </summary>
        public const ushort DEV_WAIT = 0x3;

        /// <summary>
        ///   Device did not respond within 1 second.
        /// </summary>
        public const ushort DEV_TIMEOUT = 0x4;

        public const ushort DEV_NODE_FAILED = 0x0005;
        public const ushort DEV_OUTPUT = 0x0006;

        // API CONSTANTS
        public const ushort DT_STANDARD = 0x00;

        /// <summary>
        ///   Reserved
        /// </summary>
        public const ushort DT_FLASH = 0x01;

        /// <summary>
        ///   Detect HID class modules that accept input commands or data from host.  These are typically output class devices.
        /// </summary>
        public const ushort DT_DEVICES = 0x2;

        /// <summary>
        ///   Reserved
        /// </summary>
        public const ushort DT_REPORT = 0x03;

        /// <summary>
        ///   Detects HID class modules. Will ignore all joystick type devices such Plasma class modules.
        /// </summary>
        public const ushort DT_HID = 0x4;

        /// <summary>
        ///   Detects all HID class modules including Joystick type devices such as Plasma class modules.
        /// </summary>
        public const ushort DT_ALL = 0xFF;

        // LCD Commands
        /// <summary>
        ///   Clear contents of LCD.
        /// </summary>
        public const ushort LCD_CLEAR = 0x0;

        public const ushort LCD_PAGE1 = 0x1;
        public const ushort LCD_PAGE2 = 0x2;

        /// <summary>
        ///   Display test message.
        /// </summary>
        public const ushort LCD_TEST = 0x3;

        // GRAPHICAL LCD Commands
        /// <summary>
        ///   Clear contents of LCD.
        /// </summary>
        public const ushort GLCD_CLEAR = 0;

        /// <summary>
        ///   Display test message.
        /// </summary>
        public const ushort GLCD_TEST = 1;

        public const ushort GLCD_DISPLAY = 2;

        /// <summary>
        ///   Set background color.
        /// </summary>
        public const ushort GLCD_BGCOLOR = 3;

        // PortMode CONSTANTS
        /// <summary>
        ///   Port is in Direct mode
        /// </summary>
        public const ushort MODE_DIRECT = 0;

        /// <summary>
        ///   Port is in Multiplex (MUX) mode
        /// </summary>
        public const ushort MODE_MUX = 1;

        /// <summary>
        ///   Port is in CRT RWR mode
        /// </summary>
        public const ushort MODE_RWR = 2;

        /// <summary>
        ///   Port is in Graphical LCD mode
        /// </summary>
        public const ushort MODE_GLCD = 3;

        /// <summary>
        ///   Port is in DAC mode
        /// </summary>
        public const ushort MODE_DAC = 4;

        /// <summary>
        ///   Port is in latched output mode.
        /// </summary>
        public const ushort MODE_LATCHED = 5;

        /// <summary>
        ///   Port is in SPI/DAC mode.
        /// </summary>
        public const ushort MODE_SPI = 6;

        /// <summary>
        ///   Port is in OSRAM / dot-matrix display mode.
        /// </summary>
        public const ushort MODE_DOTMATRIX = 7;

        private BIUSB()
        {
        }


        /******************************************************************************
         API functions
        *****************************************************************************/

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the 8-bit DACs 
        ///   on the port specified by <paramref name = "inPort" />. 
        ///   This function can be used on modules supporting 8-bit DACs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">Data array containing byte values for DACs. A port 
        ///   supporting 10 8- bit DACs will require 
        ///   a 10 byte buffer, each byte corresponding 
        ///   to the data to be written to the 
        ///   corresponding DAC. Note that all DACs on a 
        ///   port are written to at the same time. 
        ///   Writing to a single DAC is not possible, 
        ///   therefore DAC buffer data must be updated 
        ///   in <paramref name = "iDataBuffer" /> for all DACs 
        ///   on the specified port.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.</returns>
        [DllImport("biusb.dll", EntryPoint = "WriteDAC", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteDAC([In] ref DeviceParam iDeviceList, [In] byte inPort, [In] byte[] iDataBuffer);

        /// <summary>
        ///   A non-blocking call which returns input data. The first call to this function 
        ///   initiates a request for input data from the specified module as indicated 
        ///   by the DeviceParam structure and immediately returns form the function. 
        ///   This prevents the main calling application from locking while the module 
        ///   is being polled. The return flag indicate the current status of the request.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "oDataBuffer">Returns a populated data buffer array. 
        ///   The <see cref = "DeviceParam.NumberInputIndices">NumberInputIndices</see> 
        ///   within the <see cref = "DeviceParam">DeviceParam</see> structure indicates 
        ///   the required length of this data buffer array specifying 
        ///   the number of detected inputs for the module.</param>
        /// <param name = "iFlag">Must always be set to <see langword = "false" />.</param>
        /// <returns>
        ///   <see cref = "DEV_TIMEOUT">DEV_TIMEOUT</see> Device did not respond within 1 second.
        ///   <para /><see cref = "DEV_FAILED">DEV_FAILED</see> Failure reading from device. 
        ///   <para /><see cref = "DEV_WAIT">DEV_WAIT</see> No new data available. Waiting for device response. 
        ///   <para /><see cref = "DEV_INPUT">DEV_INPUT</see> New data available.
        /// </returns>
        /// <remarks>
        ///   A return status of <see cref = "DEV_INPUT">DEV_INPUT</see> indicates new data is available at the location pointed
        ///   to by <paramref name = "oDataBuffer" /> for the module specified in the <see cref = "DeviceParam">DeviceParam</see> structure. 
        ///   The <see cref = "DeviceParam.NumberInputIndices">NumberInputIndices</see> within the <see cref = "DeviceParam">DeviceParam</see> 
        ///   structure indicates the number of input values to be read and size of the required array.
        ///   <para />
        ///   If no new data is available, the previously buffered data will be returned in addition to the 
        ///   <see cref = "DEV_WAIT">DEV_WAIT</see> flag indicating the module has not yet responded to 
        ///   the request for input data.
        ///   <para />
        ///   Depending on the specific module, polling frequency may vary in the range 
        ///   from 10ms to 40ms or more before data is flagged as being available. 
        ///   As a general rule, this function should be polled every 10 ms 
        ///   if real-time input data is required.
        ///   <para />
        ///   If a device should become unplugged or is no longer responding, 
        ///   the <see cref = "DEV_TIMEOUT">DEV_TIMEOUT</see> flag will be 
        ///   returned after 1 second has elapsed.
        ///   <para />
        /// </remarks>
        [DllImport("biusb.dll", EntryPoint = "ReadInputData", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ReadInputData([In] ref DeviceParam iDeviceList, [Out] byte[] oDataBuffer,
                                               [In] uint iFlag);

        /// <summary>
        ///   Sends specified command <paramref name = "iCmd" /> to a graphic 
        ///   LCD on the port specified by <paramref name = "inPort" />. This function can be 
        ///   used on modules supporting graphic based LCDs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iBGColor">Set the background color of the LCD display. 
        ///   <para />0 = Black text on white background.
        ///   <para />1 = White text on black background.</param>
        /// <param name = "iCmd">GLCD Commands:
        ///   <para /><see cref = "GLCD_CLEAR">GLCD_CLEAR</see>: Clear contents of LCD.
        ///   <para /><see cref = "GLCD_TEST">GLCD_TEST</see>: Display test message.
        ///   <para /><see cref = "GLCD_BGCOLOR">GLCD_BGCOLOR</see>: Set background color 
        ///   of LCD specified by <paramref name = "iGBColor" />.
        /// </param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.</returns>
        [DllImport("biusb.dll", EntryPoint = "CmdGLCD", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int CmdGLCD([In] ref DeviceParam iDeviceList, [In] byte inPort, [In] byte iBGColor,
                                         [In] byte iCmd);

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the 
        ///   graphic LCD located on the port specified by <paramref name = "inPort" /> at the line number 
        ///   indicated by <paramref name = "inLine" />. 
        ///   This function can be used on modules supporting graphical based LCDs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "inLine">Starting line number to write 
        ///   contents of <paramref name = "iDataBuffer" /> to LCD. 
        ///   Values range from 1 to 5.</param>
        /// <param name = "iNumLines">Number of lines contained in <paramref name = "iDataBuffer" />. 
        ///   Maximum buffer length is 2 lines at 24 characters per line (48 bytes).</param>
        /// <param name = "iBGColor">Set the background color of the LCD display. 
        ///   <para />0 = Black text on white background.
        ///   <para />1 = White text on black background.</param>
        /// <param name = "iDataBuffer">
        ///   Character array. Empty spaces on the LCD should be filled with the 
        ///   ASCII value of 0x20 (blank space). Do not NULL terminate this buffer.
        ///   <para />Special characters:
        ///   <para />
        ///   <para />0x01: Arrow glyph. 
        ///   <para />0x02: Star glyph. 
        ///   <para />0x03: Degree glyph. 
        ///   <para />0x04: Star glyph.
        /// </param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.</returns>
        [DllImport("biusb.dll", EntryPoint = "WriteGLCD", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteGLCD([In] ref DeviceParam iDeviceList, [In] byte inPort, [In] byte inLine,
                                           [In] byte iNumLines, [In] byte iBGColor, [In] byte[] iDataBuffer);


        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the port specified by <paramref name = "inPort" /> configured for latched output mode.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">64-byte array (64 outputs).  Output state value 0 for OFF and 1 for ON.</param>
        /// <param name = "inLevel">Output level range from 0 to 10.  This value sets the pulse width of the output, controlling the intensity if supported.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteLatchedOutput", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteLatchedOutput([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                                    [In] byte[] iDataBuffer, [In] byte inLevel);


        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to 4 OSRAM type dot matrix displays on the port specified by <paramref name = "inPort" />.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">16-byte character array.  Maximum number of display characters is 4 per display (4 displays max per port).  Supported display 
        ///   characters are limited to ASCII values (0-255).  Refer to manufacturer's ASCII table for symbol defines.</param>
        /// <param name = "inLevel">Output level range from 0 to 10.  This value sets the pulse width of the dot matrix display controlling intensity.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteDotMatrix", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteDotMatrix([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                                [In] byte[] iDataBuffer, [In] byte inLevel);


        /// <summary>
        ///   Sends a command to clear contents of the LCD screen. This function can be used on modules supporting character based LCDs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inLCD">Zero based index of LCD to be cleared. Currently not support. Leave as 0.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.</returns>
        [DllImport("biusb.dll", EntryPoint = "ClearLCD", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ClearLCD([In] ref DeviceParam iDeviceList, [In] byte inLCD);

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the 
        ///   LCD specified by <paramref name = "inLCD" /> at the line number indicated 
        ///   by <paramref name = "inLine" />. 
        ///   This function can be used on modules supporting character based LCDs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inLCD">Zero based index of LCD to be cleared. Currently not support. Leave as 0.</param>
        /// <param name = "inLine">Line number to write buffer to. Values range from 1 to 4.</param>
        /// <param name = "iDataBuffer">Character array. Must be at least 20 characters in length. 
        ///   Empty spaces on the LCD should be filled with the 
        ///   ASCII value of 0x20 (blank space). Do not 
        ///   NULL terminate this buffer. 
        ///   NULL value will display character 0x00 
        ///   stored in the LCD ROM.</param>
        /// <param name = "inLevel">LCD backlight brightness level if supported.  Values range from 0 (min brightness) to 255 (maximum brightness).</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.</returns>
        [DllImport("biusb.dll", EntryPoint = "WriteLCD", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteLCD([In] ref DeviceParam iDeviceList, [In] byte inLCD, [In] byte inLine,
                                          [In] byte[] iDataBuffer, [In] byte inLevel);


        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the output 
        ///   pins on the port specified by <paramref name = "inPort" />. This 
        ///   function can be used on modules supporting direct outputs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">Array of <see cref = "DIRECT_OUTPUT">DIRECT_OUTPUT</see> 
        ///   structures containing data to write to the output pins specified by <paramref name = "inPort" />.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteDirectOutput", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteDirectOutput([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                                   [In] DIRECT_OUTPUT[] iDataBuffer);

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the multiplexed 
        ///   output pins on the port specified by <paramref name = "inPort" />. 
        ///   This function can be used on modules supporting multiplexed outputs.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">Array of <see cref = "DIRECT_OUTPUT">DIRECT_OUTPUT</see> 
        ///   structures containing data to write to the output pins specified by <paramref name = "inPort" />.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteMuxOutput", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteMuxOutput([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                                [In] DIRECT_OUTPUT[] iDataBuffer);

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to the 7-segment 
        ///   display on the port specified by <paramref name = "inPort" />. 
        ///   This function can be used on modules supporting 7 x 7- segment displays.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer">Character array. Maximum number of display characters is 7, 
        ///   however, decimals do not count as character spaces. 
        ///   Supported display characters are limited to 
        ///   ASCII characters "0" to "9" , "." , "-" and 
        ///   blank space (<see langword = "null" />).</param>
        /// <param name = "inLevel">Output level range from 0 to 10. This value sets 
        ///   the pulse width of the multiplexed display output controlling intensity.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteDisplayOutput", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteDisplayOutput([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                                    [In] byte[] iDataBuffer, [In] byte inLevel);

        /// <summary>
        ///   Writes contents of <paramref name = "iData" /> to a single 7-segment display on the 
        ///   device specified by <paramref name = "iDeviceList" />. This function can 
        ///   be used on modules supporting a 7-segment display and a single indicator output typically used for a shift light.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "iData">2-byte array.  The first byte value contains the character to be displayed on a 7-segment display. 
        ///   Supported values are 0x00 to 0x09 in hex which will display the corresponding 
        ///   ASCII characters "0" to "9" when using industry standard decoders. Non-supported values will blank the display.  NOTE: do
        ///   not use the ASCII value to be displayed.  
        ///   Example, to display the value "1", 
        ///   set <paramref name = "iData" /> to the hex value 0x01, 
        ///   not 0x31 corresponding to the ASCII value of "1".
        ///   <para />
        ///   The second byte value is used for a shift light indicator with 0 for OFF and 1 for ON.
        ///   <para />
        ///   Value - Displayed Symbol
        ///   <para />
        ///   0  - 0 or N if supported<para />
        ///   1  - 1<para />
        ///   2  - 2<para />
        ///   3  - 3<para />
        ///   4  - 4<para />
        ///   5  - 5<para />
        ///   6  - 6<para />
        ///   7  - 7<para />
        ///   8  - 8<para />
        ///   9  - 9<para />
        ///   10  - "-"<para />
        ///   11  - L<para />
        ///   12  - P<para />
        ///   13  - "="<para />
        ///   14  - Blank<para />
        ///   15  - R<para />
        /// </param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "Write7Segment", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int Write7Segment([In] ref DeviceParam iDeviceList, [In] byte[] iData);

        /// <summary>
        ///   Writes contents of <paramref name = "iDataBuffer" /> to SPI compatible peripherals or DACs on the port specified by <paramref name = "inPort" />.  This function
        ///   can be used on modules supporting either DACs or SPI ports.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "iDataBuffer"><see cref = "DAC_OUTPUTS" /> structure containing data to write to the port specified by the <paramref name = "inPort" /> parameter. 
        ///   A port supporting 10 DAC or SPI outputs will require a 40-byte buffer, each 32 bits long word corresponding to the data to be written to the corresponding DAC/SPI output.
        ///   Note that all SPI/DACs on a port are written to at the same time.  Writing to a single SPI/DAC is not possible; therefore SPI/DAC buffer data must be updated 
        ///   in <paramref name = "iDataBuffer" /> for all SPI/DACs on the specified port.
        ///   <para />
        ///   Actual data length output on a corresponding SPI/DAC channel is determined by the module's capability, not the maximum 
        ///   word length of 32 bits per channel as defined in the <see cref = "DAC_OUTPUTS" /> structure.
        /// </param>
        /// <param name = "inLevel">Output level range from 0 to 7.  This value sets the pulse width of the backlight if supported.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt;0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteSPIDAC", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteSPIDAC([In] ref DeviceParam iDeviceList, [In] byte inPort,
                                             [In] ref DAC_OUTPUTS iDataBuffer, [In] byte inLevel);

        /// <summary>
        ///   Writes contents of <paramref name = "iRWRBuffer" /> to the port specified by <paramref name = "inPort" /> configured for CRTRWR mode.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must 
        ///   contain valid device information returned from call 
        ///   to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "inPort">Zero based index of output Port to write to.</param>
        /// <param name = "inContacts">Total number of contacts to display on screen.  This value is typically same as <paramref name = "inSymbolCnt" />.</param>
        /// <param name = "inSymbolCnt">Number of symbols stored in <paramref name = "iRWRBuffer" />.</param>
        /// <param name = "inContactIndex">index within <paramref name = "iRWRBuffer" /> of the first symbol.</param>
        /// <param name = "iRWRBuffer">Array of <see cref = "CRTRWR_OUTPUT" /> structures.</param>
        /// <returns>
        ///   0: If failure writing to device.
        ///   <para />&gt; 0: if success, returns number of bytes written to device.
        /// </returns>
        [DllImport("biusb.dll", EntryPoint = "WriteCRTRWR", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WriteCRTRWR([In] ref DeviceParam iDeviceList, [In] byte inPort, [In] byte inContacts,
                                             [In] byte inSymbolCnt, [In] byte inContactIndex,
                                             [In] CRTRWR_OUTPUT[] iRWRBuffer);


        /******************************************************************************
         DLL functions
        ******************************************************************************/

        /// <summary>
        ///   Detects all compatible vendor specific USB HID devices. Should be 
        ///   the first function called prior to all other calls or 
        ///   to refresh the device list.
        /// </summary>
        /// <param name = "oDev_Cnt">Returns the total number of matching devices detected.</param>
        /// <param name = "oDeviceList">Returns an array of <see cref = "DeviceParam"></see> structures. All returned data will be stored in this structure array.</param>
        /// <param name = "iFlag">Specifies the device class to detect.
        ///   <para />
        ///   Values can be:<para /> 
        ///   <see cref = "DT_FLASH">DT_FLASH</see> (reserved)
        ///   <para /><see cref = "DT_DEVICES">DT_DEVICES</see> (reserved)
        ///   <para /><see cref = "DT_REPORT">DT_REPORT</see> (reserved)
        ///   <para /><see cref = "DT_HID">DT_HID</see> (Detects vendor defined HID class modules. Will ignore all joystick type devices such Plasma class modules)
        ///   <para /><see cref = "DT_ALL">DT_ALL</see> (Detects all HID class modules including Joystick type devices such as Plasma class modules).</param>
        /// <returns><see langword = "true" /> if success detecting devices, or <see langword = "false" /> if failure detecting devices or no devices found.</returns>
        [DllImport("biusb.dll", EntryPoint = "DetectHID", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DetectHID([Out] out uint oDev_Cnt,
                                            [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] DeviceParam[]
                                                oDeviceList, [In] uint iFlag);


        /// <summary>
        ///   Detects only compatible vendor specific USB HID devices matching the specified product ID. This should be the first function called prior to all other 
        ///   calls or to refresh the device list.
        /// </summary>
        /// <param name = "oDev_Cnt">Returns the total number of matching devices detected.</param>
        /// <param name = "oDeviceList">Returns an array of <see cref = "DeviceParam"></see> structures. All returned data will be stored in this structure array.</param>
        /// <param name = "iProductID">Specifies the product type to detect.
        ///   <para />
        ///   Values can be:<para /> 
        ///   <see cref = "PID_0X64B">PID_0X64B</see> 0x64B input module
        ///   <para />HID Class Devices
        ///   <para /><see cref = "PID_GAMMARAY">PID_GAMMARAY</see> (GammaRay-256 input module)
        ///   <para /><see cref = "PID_GAMMARAY64">PID_GAMMARAY64</see> (GammaRay-64 input module)
        ///   <para /><see cref = "PID_ELECTRONFLUX">PID_ELECTRONFLUX</see> (ElectronFlux input/output module)
        ///   <para /><see cref = "PID_GAMMARAY_V2">PID_GAMMARAY_V2</see> (GammaRay V2 input module)
        ///   <para /><see cref = "PID_GAMMARAY_V3">PID_GAMMARAY_V3</see> (GammaRay V3 input module)
        ///   <para /><see cref = "PID_GAMMATRON">PID_GAMMATRON</see> (GammaTron input module)
        ///   <para /><see cref = "PID_NITRO_SLG">PID_NITRO_SLG</see> (Nitro-SLG output module)
        ///   <para />Joystick Class Devices
        ///   <para /><see cref = "PID_4X24BH">PID_4X24BH</see> (4x24BH joystick module)
        ///   <para /><see cref = "PID_6X16B">PID_6X16B</see> (6x16B joystick module)
        ///   <para /><see cref = "PID_6X13B">PID_6X13B</see> (6x13B joystick module)
        ///   <para /><see cref = "PID_5X18BH">PID_5X18BH</see> (5x18BH joystick module)
        ///   <para /><see cref = "PID_PLASMA">PID_PLASMA</see> (Plasma V1 joystick module)
        ///   <para /><see cref = "PID_PLASMA_HOTAS">PID_PLASMA_HOTAS</see> (Plasma V1 in HOTAS mode joystick module)
        ///   <para /><see cref = "PID_PLASMA_DUAL">PID_PLASMA_DUAL</see> (Plasma V1 in DUAL mode joystick module)
        ///   <para /><see cref = "PID_PLASMA_HOTAS_DUAL">PID_PLASMA_HOTAS_DUAL</see> (Plasma V1 in HOTAS DUAL mode joystick module)
        ///   <para /><see cref = "PID_PLASMA_LITE">PID_PLASMA_LITE</see> (Plasma-Lite joystick module)
        ///   <para /><see cref = "PID_PLASMA_LITE_V2">PID_PLASMA_LITE_V2</see> (Plasma-Lite V2 joystick module)
        ///   <para /><see cref = "PID_PLASMA_MM2">PID_PLASMA_MM2</see> (Plasma-MM2 joystick module)
        ///   <param name = "iFlag">Specifies the device class to detect.
        ///     <para />
        ///     Values can be:<para /> 
        ///     <see cref = "DT_FLASH">DT_FLASH</see> (reserved)
        ///     <para /><see cref = "DT_DEVICES">DT_DEVICES</see> (Detects HID class modules that accept input commands or data from host.  These are typically output class devices.)
        ///     <para /><see cref = "DT_REPORT">DT_REPORT</see> (reserved)
        ///     <para /><see cref = "DT_HID">DT_HID</see> (Detects HID input class modules. Will ignore all joystick type devices such Plasma class modules)
        ///     <para /><see cref = "DT_ALL">DT_ALL</see> (Detects all HID input class modules including Joystick type devices such as Plasma class modules).</param>
        ///   <returns><see langword = "true" /> if success detecting devices, or <see langword = "false" /> if failure detecting devices or no devices found.</returns>
        [DllImport("biusb.dll", EntryPoint = "DetectDevice", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DetectDevice([Out] out uint oDev_Cnt,
                                               [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] DeviceParam[] oDeviceList, [In] uint iProductID, [In] uint iFlag);


        /// <summary>
        ///   Returns the configuration status of all IO ports found on the module. Can be used to verify if ports are active before reading or writing to them. This function is typically called after <see cref = "DetectHID">DetectHID</see>.
        /// </summary>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must contain valid device information returned from call to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "oDeviceStatus">Returns a <see cref = "DeviceStatus">DeviceStatus</see> structure.  All returned data will be stored in this location.</param>
        [DllImport("biusb.dll", EntryPoint = "RetrieveStatus", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern void RetrieveStatus([In] ref DeviceParam iDeviceList, [Out] out DeviceStatus oDeviceStatus);

        /// <summary>
        ///   Releases all detected modules, cancels all pending threads and frees all used memory blocks. This call should be made on program exit and prior to any calls to the <see cref = "DetectHID">DetectHID</see> function in order to refresh the device list.
        /// </summary>
        /// <param name = "iDev_Cnt">Number of detected modules to close in the <paramref name = "iDeviceList" />. This value is returned by a call to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <param name = "iDeviceList">A <see cref = "DeviceParam">DeviceParam</see> structure. Must contain valid device information returned from call to <see cref = "DetectHID">DetectHID</see>.</param>
        /// <returns><see langword = "true" /> if success closing devices, or <see langword = "false" /> if failure closing devices or releasing memory.</returns>
        [DllImport("biusb.dll", EntryPoint = "CloseDevices", ExactSpelling = true, CharSet = CharSet.Auto,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseDevices([In] uint iDev_Cnt, [In] DeviceParam[] iDeviceList);
    }
}