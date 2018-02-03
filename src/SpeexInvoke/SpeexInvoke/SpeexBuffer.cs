using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr speex_buffer_init(int size);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_buffer_destroy(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_buffer_write(IntPtr st, IntPtr data, int len);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_buffer_writezeros(IntPtr st, int len);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_buffer_read(IntPtr st, IntPtr data, int len);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_buffer_get_available(IntPtr st);

    [DllImport("libspeexdsp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_buffer_resize(IntPtr st, int len);
}