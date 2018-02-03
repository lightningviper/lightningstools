namespace F4Utils.PlayerOp
{
    public enum PO_SIM_FLAGS
    {
        SIM_AUTO_TARGET = 0x01,
        SIM_NO_BLACKOUT = 0x02,
        SIM_UNLIMITED_FUEL = 0x04,
        SIM_UNLIMITED_AMMO = 0x08,
        SIM_UNLIMITED_CHAFF = 0x10,
        SIM_NO_COLLISIONS = 0x20,
        SIM_NAMETAGS = 0x40,
        SIM_LIFTLINE_CUE = 0x80,
        SIM_BULLSEYE_CALLS = 0x100,
        SIM_INVULNERABLE = 0x200,
        //SIM_CAMPAIGNTAGS = 0x400,

        SIM_PRESET_FLAGS = 0x27E,
        SIM_RULES_FLAGS = 0x277,
    }
}