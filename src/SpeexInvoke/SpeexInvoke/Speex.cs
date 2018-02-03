using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    public const int SPEEX_SET_ENH = 0;
    public const int SPEEX_GET_ENH = 1;
    public const int SPEEX_GET_FRAME_SIZE = 3;
    public const int SPEEX_SET_QUALITY = 4;
    public const int SPEEX_SET_MODE = 6;
    public const int SPEEX_GET_MODE = 7;
    public const int SPEEX_SET_LOW_MODE = 8;
    public const int SPEEX_GET_LOW_MODE = 9;
    public const int SPEEX_SET_HIGH_MODE = 10;
    public const int SPEEX_GET_HIGH_MODE = 11;
    public const int SPEEX_SET_VBR = 12;
    public const int SPEEX_GET_VBR = 13;
    public const int SPEEX_SET_VBR_QUALITY = 14;
    public const int SPEEX_GET_VBR_QUALITY = 15;
    public const int SPEEX_SET_COMPLEXITY = 16;
    public const int SPEEX_GET_COMPLEXITY = 17;
    public const int SPEEX_SET_BITRATE = 18;
    public const int SPEEX_GET_BITRATE = 19;
    public const int SPEEX_SET_HANDLER = 20;
    public const int SPEEX_SET_USER_HANDLER = 22;
    public const int SPEEX_SET_SAMPLING_RATE = 24;
    public const int SPEEX_GET_SAMPLING_RATE = 25;
    public const int SPEEX_RESET_STATE = 26;
    public const int SPEEX_GET_RELATIVE_QUALITY = 29;
    public const int SPEEX_SET_VAD = 30;
    public const int SPEEX_GET_VAD = 31;
    public const int SPEEX_SET_ABR = 32;
    public const int SPEEX_GET_ABR = 33;
    public const int SPEEX_SET_DTX = 34;
    public const int SPEEX_GET_DTX = 35;
    public const int SPEEX_SET_SUBMODE_ENCODING = 36;
    public const int SPEEX_GET_SUBMODE_ENCODING = 37;
    public const int SPEEX_GET_LOOKAHEAD = 39;
    public const int SPEEX_SET_PLC_TUNING = 40;
    public const int SPEEX_GET_PLC_TUNING = 41;
    public const int SPEEX_SET_VBR_MAX_BITRATE = 42;
    public const int SPEEX_GET_VBR_MAX_BITRATE = 43;
    public const int SPEEX_SET_HIGHPASS = 44;
    public const int SPEEX_GET_HIGHPASS = 45;
    public const int SPEEX_GET_ACTIVITY = 47;
    public const int SPEEX_SET_PF = 0;
    public const int SPEEX_GET_PF = 1;
    public const int SPEEX_MODE_FRAME_SIZE = 0;
    public const int SPEEX_SUBMODE_BITS_PER_FRAME = 1;
    public const int SPEEX_LIB_GET_MAJOR_VERSION = 1;
    public const int SPEEX_LIB_GET_MINOR_VERSION = 3;
    public const int SPEEX_LIB_GET_MICRO_VERSION = 5;
    public const int SPEEX_LIB_GET_EXTRA_VERSION = 7;
    public const int SPEEX_LIB_GET_VERSION_STRING = 9;
    public const int SPEEX_NB_MODES = 3;
    public const int SPEEX_MODEID_NB = 0;
    public const int SPEEX_MODEID_WB = 1;
    public const int SPEEX_MODEID_UWB = 2;

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct SpeexMode
    //{
    //    public IntPtr mode;
    //    public mode_query_func query;
    //    public IntPtr modeName;
    //    public int modeID;
    //    public int bitstream_version;
    //    public encoder_init_func enc_init;
    //    public encoder_destroy_func enc_destroy;
    //    public encode_func enc;
    //    public decoder_init_func dec_init;
    //    public decoder_destroy_func dec_destroy;
    //    public decode_func dec;
    //    public encoder_ctl_func enc_ctl;
    //    public decoder_ctl_func dec_ctl;
    //};
    //public delegate IntPtr encoder_init_func(IntPtr mode);
    //public delegate void encoder_destroy_func(IntPtr st);
    //public delegate int encode_func(IntPtr state, IntPtr p_in, ref SpeexBits bits);
    //public delegate int encoder_ctl_func(IntPtr state, int request, IntPtr ptr);
    //public delegate IntPtr decoder_init_func(IntPtr mode);
    //public delegate void decoder_destroy_func(IntPtr st);
    //public delegate int decode_func(IntPtr state, ref SpeexBits bits, IntPtr p_out);
    //public delegate int decoder_ctl_func(IntPtr state, int request, IntPtr ptr);
    //public delegate int mode_query_func(IntPtr mode, int request, IntPtr ptr);

    [DllImport("libspeex.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_encoder_init(IntPtr mode);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_encoder_destroy(IntPtr state);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_encode(IntPtr state, float[] f_in, ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_encode_int(IntPtr state, Int16[] i_in, ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_encoder_ctl(IntPtr state, int request, IntPtr ptr);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr speex_decoder_init(IntPtr mode);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_decoder_destroy(IntPtr state);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_decode(IntPtr state, ref SpeexBits bits, float[] f_out);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_decode_int(IntPtr state, ref SpeexBits bits, Int16[] i_out);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_decoder_ctl(IntPtr state, int request, IntPtr ptr);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_mode_query(IntPtr mode, int request, IntPtr ptr);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_lib_ctl(int request, IntPtr ptr);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr speex_lib_get_mode(int mode);
}