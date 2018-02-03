using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    #region ResamplerError enum

    public enum ResamplerError
    {
        RESAMPLER_ERR_SUCCESS = 0,
        RESAMPLER_ERR_ALLOC_FAILED = 1,
        RESAMPLER_ERR_BAD_STATE = 2,
        RESAMPLER_ERR_INVALID_ARG = 3,
        RESAMPLER_ERR_PTR_OVERLAP = 4,
        RESAMPLER_ERR_MAX_ERROR = 5
    } ;

    #endregion

    public const int SPEEX_RESAMPLER_QUALITY_MAX = 10;
    public const int SPEEX_RESAMPLER_QUALITY_MIN = 0;
    public const int SPEEX_RESAMPLER_QUALITY_DEFAULT = 4;
    public const int SPEEX_RESAMPLER_QUALITY_VOIP = 3;
    public const int SPEEX_RESAMPLER_QUALITY_DESKTOP = 5;

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_resampler_init(UInt32 nb_channels,
                                                     UInt32 in_rate,
                                                     UInt32 out_rate,
                                                     int quality,
                                                     [Out] out int err);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr speex_resampler_init_frac(UInt32 nb_channels,
                                                          UInt32 ratio_num,
                                                          UInt32 ratio_den,
                                                          UInt32 in_rate,
                                                          UInt32 out_rate,
                                                          int quality,
                                                          [Out] out int err);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_destroy(IntPtr st);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_process_float(IntPtr st,
                                                           UInt32 channel_index,
                                                           float[] f_in,
                                                           [In] [Out] ref UInt32 in_len,
                                                           float[] f_out,
                                                           [In] [Out] ref UInt32 out_len);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_process_int(IntPtr st,
                                                         UInt32 channel_index,
                                                         Int16[] i_in,
                                                         [In] [Out] ref UInt32 in_len,
                                                         Int16[] i_out,
                                                         [In] [Out] ref UInt32 out_len);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_process_interleaved_float(IntPtr st,
                                                                       float[] f_in,
                                                                       [In] [Out] ref UInt32 in_len,
                                                                       float[] f_out,
                                                                       [In] [Out] ref UInt32 out_len);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_process_interleaved_int(IntPtr st,
                                                                     Int16[] i_in,
                                                                     [In] [Out] ref UInt32 in_len,
                                                                     Int16[] i_out,
                                                                     [In] [Out] ref UInt32 out_len);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_set_rate(IntPtr st,
                                                      UInt32 in_rate,
                                                      UInt32 out_rate);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_get_rate(IntPtr st,
                                                       [Out] out UInt32 in_rate,
                                                       [Out] out UInt32 out_rate);


    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_set_rate_frac(IntPtr st,
                                                           UInt32 ratio_num,
                                                           UInt32 ratio_den,
                                                           UInt32 in_rate,
                                                           UInt32 out_rate);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_get_ratio(IntPtr st,
                                                        [Out] out UInt32 ratio_num,
                                                        [Out] out UInt32 ratio_den);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_set_quality(IntPtr st,
                                                         int quality);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_get_quality(IntPtr st,
                                                          [Out] out int quality);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_set_input_stride(IntPtr st,
                                                               UInt32 stride);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_get_input_stride(IntPtr st,
                                                               [Out] out UInt32 stride);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_set_output_stride(IntPtr st,
                                                                Int32 stride);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_resampler_get_output_stride(IntPtr st,
                                                                [Out] out UInt32 stride);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_get_input_latency(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_get_output_latency(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_skip_zeros(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int speex_resampler_reset_mem(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern byte[] speex_resampler_strerror(int err);
}