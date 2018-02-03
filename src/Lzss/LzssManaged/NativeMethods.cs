using System;
using System.Runtime.InteropServices;

namespace Lzss
{
    public static class NativeMethods
    {
        [DllImport("LzssNative.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int LZSS_Compress(IntPtr nonCompressedDataBuffer, IntPtr compressedDataBuffer,
                                                 int nonCompressedDataSize);

        [DllImport("LzssNative.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int LZSS_Expand(IntPtr compressedDataBuffer, IntPtr decompressedDataBuffer,
                                               int decompressedDataSize);
    }
}