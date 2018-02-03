using System;
using System.Runtime.InteropServices;

public static partial class Speex
{
    [DllImport("libspeex.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern void speex_bits_init(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_init_buffer(ref SpeexBits bits, byte[] buff, int buf_size);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_set_bit_buffer(ref SpeexBits bits, byte[] buff, int buf_size);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_destroy(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_reset(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_rewind(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_read_from(ref SpeexBits bits, byte[] bytes, int len);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_read_whole_bytes(ref SpeexBits bits, byte[] bytes, int len);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_bits_write(ref SpeexBits bits, byte[] bytes, int max_len);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_bits_write_whole_bytes(ref SpeexBits bits, byte[] bytes, int max_len);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_pack(ref SpeexBits bits, int data, int nbBits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_bits_unpack_signed(ref SpeexBits bits, int nbBits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint speex_bits_unpack_unsigned(ref SpeexBits bits, int nbBits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_bits_nbytes(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint speex_bits_peek_unsigned(ref SpeexBits bits, int nbBits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint speex_bits_peek(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_advance(ref SpeexBits bits, int n);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int speex_bits_remaining(ref SpeexBits bits);

    [DllImport("libspeex.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void speex_bits_insert_terminator(ref SpeexBits bits);

    #region Nested type: SpeexBits

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpeexBits
    {
        public IntPtr chars;
        public int nbBits;
        public int charPtr;
        public int bitPtr;
        public bool owner;
        public bool overflow;
        public int buf_size;
        public int reserved1;
        public IntPtr reserved2;
    } ;

    #endregion
}