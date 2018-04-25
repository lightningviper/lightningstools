using System;
using System.IO;
using System.Text;

namespace BMSVoiceGen.Audio.IO
{
    internal interface IPCMSerializer
    {
        void Serialize(Stream pcmStream, string wavFilePath);
    }
    internal class PCMAudioSerializer:IPCMSerializer
    {
        public void Serialize(Stream pcmStream, string wavFilePath)
        {
            using (var fs = new FileStream(wavFilePath, FileMode.Create))
            {

                //WAV header chunk
                fs.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);//ChunkID 
                fs.Write(BitConverter.GetBytes((uint)0), 0, sizeof(uint));//ChunkSize placeholder
                fs.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);//Format
                                                                //WAV format chunk
                fs.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);//SubChunk1ID
                fs.Write(BitConverter.GetBytes((uint)16), 0, sizeof(uint)); //SubChunk1Size
                fs.Write(BitConverter.GetBytes((ushort)1), 0, sizeof(ushort));//AudioFormat
                fs.Write(BitConverter.GetBytes((ushort)1), 0, sizeof(ushort));//NumChannels
                fs.Write(BitConverter.GetBytes((uint)8000), 0, sizeof(uint));//SampleRate
                fs.Write(BitConverter.GetBytes((uint)8000 * 2), 0, sizeof(uint));//ByteRate
                fs.Write(BitConverter.GetBytes((ushort)2), 0, sizeof(ushort));//BlockAlign
                fs.Write(BitConverter.GetBytes((ushort)16), 0, sizeof(ushort));//BitsPerSample
                                                                                //WAV data chunk
                fs.Write(Encoding.ASCII.GetBytes("data"), 0, 4);//SubChunk2ID 
                fs.Write(BitConverter.GetBytes((uint)0), 0, 4);//SubChunk2Size placeholder
                fs.Flush();
                var positionStart = fs.Position;
                pcmStream.CopyTo(fs);
                fs.Flush();
                var positionEnd = fs.Position;
                var dataLength = positionEnd - positionStart;
                fs.Seek(positionStart - sizeof(uint), SeekOrigin.Begin);
                fs.Write(BitConverter.GetBytes((uint)dataLength), 0, sizeof(uint));
                fs.Seek(4, SeekOrigin.Begin);
                fs.Write(BitConverter.GetBytes((uint)(36 + dataLength)), 0, sizeof(uint));
                fs.Flush();
                fs.Close();
            }
        }
    }
}
