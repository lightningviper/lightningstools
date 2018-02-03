using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    public const int JITTER_BUFFER_OK = 0;
    public const int JITTER_BUFFER_MISSING = 1;
    public const int JITTER_BUFFER_INSERTION = 2;
    public const int JITTER_BUFFER_INTERNAL_ERROR = -1;
    public const int JITTER_BUFFER_BAD_ARGUMENT = -2;
    public const int JITTER_BUFFER_SET_MARGIN = 0;
    public const int JITTER_BUFFER_GET_MARGIN = 1;
    public const int JITTER_BUFFER_GET_AVAILABLE_COUNT = 3;
    public const int JITTER_BUFFER_GET_AVALIABLE_COUNT = 3;
    public const int JITTER_BUFFER_SET_DESTROY_CALLBACK = 4;
    public const int JITTER_BUFFER_GET_DESTROY_CALLBACK = 5;
    public const int JITTER_BUFFER_SET_DELAY_STEP = 6;
    public const int JITTER_BUFFER_GET_DELAY_STEP = 7;
    public const int JITTER_BUFFER_SET_CONCEALMENT_SIZE = 8;
    public const int JITTER_BUFFER_GET_CONCEALMENT_SIZE = 9;
    public const int JITTER_BUFFER_SET_MAX_LATE_RATE = 10;
    public const int JITTER_BUFFER_GET_MAX_LATE_RATE = 11;
    public const int JITTER_BUFFER_SET_LATE_COST = 12;
    public const int JITTER_BUFFER_GET_LATE_COST = 13;

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr jitter_buffer_init(int step_size);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void jitter_buffer_reset(IntPtr jitter);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void jitter_buffer_destroy(IntPtr jitter);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void jitter_buffer_put(IntPtr jitter, ref JitterBufferPacket packet);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int jitter_buffer_get(IntPtr jitter, ref JitterBufferPacket packet, int desired_span,
                                               [Out] out int start_offset);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int jitter_buffer_get_another(IntPtr jitter, ref JitterBufferPacket packet);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int jitter_buffer_get_pointer_timestamp(IntPtr jitter);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void jitter_buffer_tick(IntPtr jitter);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void jitter_buffer_remaining_span(IntPtr jitter, UInt32 rem);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int jitter_buffer_ctl(IntPtr jitter, int request, IntPtr ptr);

    [DllImport("libspeexdsp.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int jitter_buffer_update_delay(IntPtr jitter, ref JitterBufferPacket packet,
                                                        [Out] out int start_offset);

    #region Nested type: JitterBufferPacket

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JitterBufferPacket
    {
        public IntPtr data;
        public UInt32 len;
        public UInt32 timestamp;
        public UInt32 span;
        public UInt16 sequence;
        public UInt16 user_data;
    } ;

    #endregion
}