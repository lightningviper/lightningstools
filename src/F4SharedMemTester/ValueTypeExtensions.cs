using System.Runtime.InteropServices;
using System;

namespace F4SharedMemTester
{
    internal static class ValueTypeExtensions
    {
        public static byte[] Serialize<T>(this T structure) where T : struct
        {
            var size = Marshal.SizeOf(structure);
            var bytes = new byte[size];
            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return bytes;
        }
    }
}
