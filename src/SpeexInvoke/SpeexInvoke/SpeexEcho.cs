using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    public const int SPEEX_ECHO_GET_FRAME_SIZE = 3;
    public const int SPEEX_ECHO_SET_SAMPLING_RATE = 24;
    public const int SPEEX_ECHO_GET_SAMPLING_RATE = 25;
    public const int SPEEX_ECHO_GET_IMPULSE_RESPONSE_SIZE = 27;
    public const int SPEEX_ECHO_GET_IMPULSE_RESPONSE = 29;

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_echo_state_init(int frame_size, int filter_length);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_echo_state_init_mc(int frame_size, int filter_length, int nb_mic, int nb_speakers);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_state_destroy(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_cancellation(IntPtr st, Int16[] rec, Int16[] play, Int16[] i_out);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_cancel(IntPtr st, Int16[] rec, Int16[] play, Int16[] i_out, Int32[] Yout);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_capture(IntPtr st, Int16[] rec, Int16[] i_out);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_playback(IntPtr st, Int16[] play);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_echo_state_reset(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_echo_ctl(IntPtr st, int request, IntPtr ptr);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_decorrelate_new(int rate, int channels, int frame_size);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_decorrelate(IntPtr st, Int16[] i_in, Int16[] i_out, int strength);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_decorrelate_destroy(IntPtr st);
}