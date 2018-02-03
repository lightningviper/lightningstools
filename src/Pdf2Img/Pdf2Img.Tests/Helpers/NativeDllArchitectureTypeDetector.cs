using System.IO;

namespace Pdf2Img.Tests.Helpers
{
    internal class NativeDllArchitectureTypeDetector
    {
        internal const ushort ThirtyTwoBitArchitecture = 0x10b;
        internal const ushort SixtyFourBitArchitecture = 0x20b;

        public static ushort DetectNativeDllArchitecture(byte[] nativeDll)
        {
            using (var stream = new MemoryStream(nativeDll))
            using (var reader = new BinaryReader(stream))
            {
                stream.Seek(60, SeekOrigin.Begin);
                stream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                stream.Seek(24, SeekOrigin.Current);
                return reader.ReadUInt16();
            }
        }
    }
}