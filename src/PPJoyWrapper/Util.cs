using System;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   <see cref = "Util" /> contains static utility methods that are used throughout the PPJoy wrapper
    ///   library.
    /// </summary>
    [ComVisible(false)]
    internal static class Util
    {
        /// <summary>
        ///   Converts a byte array to an object of a specific type.
        /// </summary>
        /// <param name = "rawData">an array of Bytes to convert to an object of the specified <see cref = "System.Type" /></param>
        /// <param name = "overlayType">A <see cref = "System.Type" />, to convert the supplied array of Bytes to</param>
        /// <returns>A <see cref = "System.Object" /> of the <see cref = "System.Type" /> specified, as constructed from 
        ///   the supplied array of bytes.</returns>
        public static object RawDataToObject(ref byte[] rawData, Type overlayType)
        {
            object result = null;

            var pinnedRawData = GCHandle.Alloc(rawData,
                                               GCHandleType.Pinned);
            try
            {
                // Get the address of the data array
                var pinnedRawDataPtr =
                    pinnedRawData.AddrOfPinnedObject();

                // overlay the data type on top of the raw data
                result = Marshal.PtrToStructure(
                    pinnedRawDataPtr,
                    overlayType);
            }
            finally
            {
                // must explicitly release
                pinnedRawData.Free();
            }

            return result;
        }
    }
}