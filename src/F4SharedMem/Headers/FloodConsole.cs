using System;

namespace F4SharedMem.Headers
{
    [Serializable]
    // instrument backlight brightness
    public enum FloodConsole : byte
    {
        FLOOD_CONSOLE_OFF = 0,
        FLOOD_CONSOLE_1 = 1,
        FLOOD_CONSOLE_2 = 2,
        FLOOD_CONSOLE_3 = 3,
        FLOOD_CONSOLE_4 = 4,
        FLOOD_CONSOLE_5 = 5,
        FLOOD_CONSOLE_6 = 6,
    };
}