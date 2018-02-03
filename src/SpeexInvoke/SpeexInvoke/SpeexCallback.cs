using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    #region Delegates

    public delegate int speex_callback_func(ref SpeexBits bits, IntPtr state, IntPtr data);

    #endregion

    public const int SPEEX_MAX_CALLBACKS = 16;
    public const int SPEEX_INBAND_ENH_REQUEST = 0;
    public const int SPEEX_INBAND_RESERVED1 = 1;
    public const int SPEEX_INBAND_MODE_REQUEST = 2;
    public const int SPEEX_INBAND_LOW_MODE_REQUEST = 3;
    public const int SPEEX_INBAND_HIGH_MODE_REQUEST = 4;
    public const int SPEEX_INBAND_VBR_QUALITY_REQUEST = 5;
    public const int SPEEX_INBAND_ACKNOWLEDGE_REQUEST = 6;
    public const int SPEEX_INBAND_VBR_REQUEST = 7;
    public const int SPEEX_INBAND_CHAR = 8;
    public const int SPEEX_INBAND_STEREO = 9;
    public const int SPEEX_INBAND_MAX_BITRATE = 10;
    public const int SPEEX_INBAND_ACKNOWLEDGE = 12;

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_inband_handler(ref SpeexBits bits, SpeexCallback[] callback_list, IntPtr state);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_mode_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_high_mode_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_char_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_default_user_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_low_mode_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_vbr_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_enh_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_vbr_quality_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    #region Nested type: SpeexCallback

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpeexCallback
    {
        public int callback_id;
        public speex_callback_func func;
        public IntPtr data;
        public IntPtr reserved1;
        public int reserved2;
    }

    #endregion
}