using F4SharedMem.Headers;
using System.IO;

namespace F4SharedMemTester
{
    internal static class DrawingDataExtensions
    {
        public static byte[] Serialize(this DrawingData drawingData)
        {
            
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write(drawingData.VersionNum);
                writer.Write(drawingData.HUD_length);
                writer.Write(drawingData.HUD_commands);
                writer.Write(drawingData.RWR_length);
                writer.Write(drawingData.RWR_commands);
                writer.Write(drawingData.HMS_length);
                writer.Write(drawingData.HMS_commands);
                writer.Flush();
                return memoryStream.GetBuffer();
            }
        }
    }
}
