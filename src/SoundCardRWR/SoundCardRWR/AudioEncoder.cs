using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SoundCardRWR
{
    internal class AudioEncoder
    {
        private const ushort WAVE_FORMAT_EXTENSIBLE = 0xFFFE;
        private const ushort WAVE_FORMAT_IEEE_FLOAT =0x0003;
        public static readonly Guid KSDATAFORMAT_SUBTYPE_IEEE_FLOAT = new Guid("00000003-0000-0010-8000-00aa00389b71");
        internal static void Serialize(DrawingGroup drawingGroup, Rect bounds, Stream stream, PreprocessorOptions preprocessorOptions = null)
        {
            var figures = drawingGroup.GetGeometry().GetFlattenedPathGeometry().Figures;
            var vectors = new List<(Point Point, uint Intensity)>();
            foreach (var figure in figures)
            {
                var figurePoints = figure.GetPoints();
                vectors.Add((figurePoints.First(), 0));
                vectors.AddRange(figurePoints.Skip(1).Take(figurePoints.Count() - 1).Select((point) => (point, uint.MaxValue)));
            }
            vectors = VectorPreprocessor.PreprocessVectors(vectors, preprocessorOptions ?? new PreprocessorOptions()).ToList();
            var scaleWidth = 2.0f;
            var waveAudioData = new List<float>();
            var nChannels = (ushort)2;
            var nSamplesPerSec = (uint)192000;
            var wBitsPerSample = (ushort)32;
            foreach (var vector in vectors)
            {
                waveAudioData.Add((float)((((vector.Point.X - bounds.Left) / bounds.Width)) * scaleWidth) - 1.0f);
                waveAudioData.Add((float)((((vector.Point.Y - bounds.Top) / bounds.Height)) * scaleWidth) - 1.0f);
                if (nChannels >= 3)
                {
                    waveAudioData.Add((float)(((float)vector.Intensity / (float)uint.MaxValue) * scaleWidth) - 1.0f);
                }
            }
            var wFormatTag = 
                (
                    (nChannels >0 && nChannels <= 2) 
                       && 
                    (wBitsPerSample==8 || wBitsPerSample==16 || wBitsPerSample==32) 
                       && 
                    (nSamplesPerSec == 44100 || nSamplesPerSec ==48000 || nSamplesPerSec == 96000 || nSamplesPerSec == 128000 || nSamplesPerSec == 192000 )
                )
                    ? WAVE_FORMAT_IEEE_FLOAT
                    : WAVE_FORMAT_EXTENSIBLE;
            var avgBytesPerSec = (uint)(nSamplesPerSec * nChannels * (wBitsPerSample / 8));
            var nBlockAlign = (ushort)(nChannels * (wBitsPerSample / 8));
            var fmtChunkSize = (wFormatTag==WAVE_FORMAT_IEEE_FLOAT) 
                                    ? (uint)16 
                                    : (uint)40;
            var dataChunkSize = (uint)(waveAudioData.Count * (wBitsPerSample / 8));
            var cbSize = (ushort)22; // sizeof(WAVEFORMATEXTENSIBLE)-sizeof(WAVEFORMATEX)
            var dwChannelMask = (uint)(ChannelMask.SPEAKER_FRONT_LEFT | ChannelMask.SPEAKER_FRONT_RIGHT);
            if (nChannels >= 3)
            {
                dwChannelMask |= (uint)ChannelMask.SPEAKER_FRONT_CENTER;
            }
            var SubFormat = KSDATAFORMAT_SUBTYPE_IEEE_FLOAT;
            var factChunkCkSize = (uint)4;
            var dwSampleLength = (uint)(nChannels * waveAudioData.Count);
            using (var writer = new BinaryWriter(stream, encoding: System.Text.Encoding.Default, leaveOpen: true))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write((uint)0);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(fmtChunkSize);
                writer.Write(wFormatTag);
                writer.Write(nChannels);
                writer.Write(nSamplesPerSec);
                writer.Write(avgBytesPerSec);
                writer.Write(nBlockAlign);
                writer.Write(wBitsPerSample);
                if (wFormatTag == WAVE_FORMAT_EXTENSIBLE)
                {
                    writer.Write(cbSize);
                    writer.Write(wBitsPerSample);
                    writer.Write(dwChannelMask);
                    writer.Write(SubFormat.ToByteArray());

                    writer.Write(("fact").ToCharArray());
                    writer.Write(factChunkCkSize);
                    writer.Write(dwSampleLength);
                }

                writer.Write("data".ToCharArray());
                writer.Write(dataChunkSize);
                foreach (float sample in waveAudioData)
                {
                    writer.Write(sample);
                }
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((uint)(stream.Length - 8));
                writer.Flush();
            }
           
        }
    }
}