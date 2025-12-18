using F4SharedMem.Headers;
using System.IO;

namespace F4SharedMemTester
{
    internal static class StringDataExtensions
    {
        public static byte[] Serialize(this StringData stringData)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write(stringData.VersionNum);
                writer.Write(stringData.NoOfStrings);
                writer.Write(stringData.dataSize);
                
                foreach (var stringStruct in stringData.data)
                {
                    writer.Write(stringStruct.strId);
                    writer.Write(stringStruct.strLength);
                    writer.Write(stringStruct.strData);
                }
                writer.Flush();
                return memoryStream.GetBuffer();
            }
        }
    }
}
