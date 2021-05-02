using System;

namespace SimLinkup.HardwareSupport.TeensyEWMU
{
    internal class TeensyEWMUCommunicationProtocolHeaders
    {
        [Flags]
        internal enum TeensyEWMUPacketFields : byte
        {
            EWMU_DISPLAY_STRING = 0x01,
            CMDS_DISPLAY_STRING = 0x02,
            EWPI_DISPLAY_STRING = 0x04,
            EWPI_LIGHTBITS = 0x08,
            CMDS_LIGHTBITS = 0x10,
            INVERT_STATES = 0x20
        };

        [Flags]
        internal enum EWPILightbits : byte
        {
            PRI = 0x01,
            UNK = 0x02,
            ML = 0x04,
        };

        [Flags]
        internal enum CMDSLightbits : byte
        {
            NOGO = 0x01,
            GO = 0x02,
            DISPENSE_READY = 0x04,
        };

        [Flags]
        internal enum InvertBits : UInt32
        {
            CMDS_O1 = 0x1,
            CMDS_O2 = 0x2,
            CMDS_CH = 0x4,
            CMDS_FL = 0x8,
            CMDS_AND_EWMU_Jettison = 0x10,
            CMDS_PRGM_BIT = 0x20,
            CMDS_PRGM_1 = 0x40,
            CMDS_PRGM_2 = 0x80,
            CMDS_PRGM_3 = 0x100,
            CMDS_PRGM_4 = 0x200,
            CMDS_AND_EWMU_MWS = 0x400,
            CMDS_AND_EWMU_JMR = 0x800,
            CMDS_AND_EWMU_RWR = 0x1000,
            EWMU_DISP = 0x2000,
            CMDS_AND_EWMU_MODE_OFF = 0x4000,
            CMDS_AND_EWMU_MODE_STBY = 0x8000,
            CMDS_AND_EWMU_MODE_MAN = 0x10000,
            CMDS_AND_EWMU_MODE_SEMI = 0x20000,
            CMDS_AND_EWMU_MODE_AUTO = 0x40000,
            CMDS_MODE_BYP = 0x80000,
            EWMU_MWS_MENU = 0x100000,
            EWMU_JMR_MENU = 0x200000,
            EWMU_RWR_MENU = 0x400000,
            EWMU_DISP_MENU = 0x800000,
            EWPI_PRI = 0x100000,
            EWPI_SEP = 0x200000,
            EWPI_UNK = 0x400000,
            EWPI_MD = 0x800000,
            EWMU_SET1 = 0x1000000,
            EWMU_SET2 = 0x2000000,
            EWMU_SET3 = 0x4000000,
            EWMU_SET4 = 0x8000000,
            EWMU_NXT_UP = 0x10000000,
            EWMU_NXT_DOWN = 0x20000000,
            EWMU_RTN = 0x40000000
        };
    }
}
