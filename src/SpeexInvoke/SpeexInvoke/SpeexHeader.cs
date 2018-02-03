using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    public const int SPEEX_HEADER_STRING_LENGTH = 8;
    public const int SPEEX_HEADER_VERSION_LENGTH = 20;

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_init_header(ref SpeexHeader header, int rate, int nb_channels, IntPtr m);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] speex_header_to_packet(ref SpeexHeader header, [Out] out int size);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SpeexHeader speex_packet_to_header(byte[] packet, int size);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_header_free(IntPtr ptr);

    #region Nested type: SpeexHeader

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpeexHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SPEEX_HEADER_STRING_LENGTH)] public byte[] speex_string;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SPEEX_HEADER_VERSION_LENGTH)] public byte[] speex_version;
        public int speex_version_id;
        public int header_size;
        public int rate;
        public int mode;
        public int mode_bitstream_version;
        public int nb_channels;
        public int bitrate;
        public int frame_size;
        public int vbr;
        public int frames_per_packet;
        public int extra_headers;
        public int reserved1;
        public int reserved2;
    } ;

    #endregion
}