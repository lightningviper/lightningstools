using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    public static float[] SPEEX_STEREO_STATE_INIT = {1, 0.5f, 1, 1, 0, 0};

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr speex_stereo_state_init();

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_stereo_state_reset(ref SpeexStereoState stereo);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_stereo_state_destroy(ref SpeexStereoState stereo);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_encode_stereo(float[] data, int frame_size, ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_encode_stereo_int(Int16[] data, int frame_size, ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_decode_stereo(float[] data, int frame_size, ref SpeexStereoState stereo);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_decode_stereo_int(Int16[] data, int frame_size, ref SpeexStereoState stereo);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_std_stereo_request_handler(ref SpeexBits bits, IntPtr state, IntPtr data);

    #region Nested type: SpeexStereoState

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpeexStereoState
    {
        public float balance;
        public float e_ratio;
        public float smooth_left;
        public float smooth_right;
        public float reserved1;
        public float reserved2;
    }

    #endregion
}