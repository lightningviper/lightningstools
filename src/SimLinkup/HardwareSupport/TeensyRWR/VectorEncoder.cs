using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class VectorEncoder
    {
        internal static void Serialize(DrawingGroup drawingGroup, Rect bounds, Stream stream)
        {
            var basicGeometry = drawingGroup.GetGeometry();
            var figures = basicGeometry.GetFlattenedPathGeometry(0.01, ToleranceType.Absolute).Figures;
            var vectors = new List<(Point Point, bool BeamOn)>();
            foreach (var figure in figures)
            {
                var figurePoints = figure.GetPoints();
                vectors.Add((figure.StartPoint, false));
                vectors.AddRange(figurePoints.Select((point) => (point, true)));
                if (figure.IsClosed)
                {
                    vectors.Add((figure.StartPoint, true));
                }
            }
            //vectors = VectorPreprocessor.PreprocessVectors(vectors, new PreprocessorOptions { EqualizeBrightness = true }).ToList();
            ushort vectorCount = 0;
            stream.Write(BitConverter.GetBytes(vectorCount), 0, sizeof(ushort));
            (Point Point, bool BeamOn) prevVector = (Point:new Point(0,0), BeamOn:false);
            foreach (var curVector in vectors)
            {
                var x = (ushort)(((curVector.Point.X - bounds.Left) / bounds.Width) * 4095);
                var y = (ushort)(((curVector.Point.Y - bounds.Top) / bounds.Height) * 4095);
                var z = curVector.BeamOn ? 1 : 0;
                var xyzCombined = (uint)(((uint)(x & 0xFFF)) << 12 | ((uint)(y & 0xFFE)) | ((uint)(z & 0x1)));
                if (!(curVector.Point.X == prevVector.Point.X && curVector.Point.Y == prevVector.Point.Y && curVector.BeamOn == prevVector.BeamOn))
                {
                    stream.WriteByte((byte)((xyzCombined & 0xFF0000) >> 16));
                    stream.WriteByte((byte)((xyzCombined & 0xFF00) >> 8));
                    stream.WriteByte((byte)(xyzCombined & 0xFF));
                    vectorCount++;
                }
                prevVector = curVector;
            }
            var position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(vectorCount).Reverse().ToArray(), 0, sizeof(ushort));
            stream.Seek(position, SeekOrigin.Begin);
        }
        private static uint SwapUInt32(uint v) { return (uint)(((SwapUInt16((ushort)v) & 0xffff) << 0x10) | (SwapUInt16((ushort)(v >> 0x10)) & 0xffff)); }
        public static ushort SwapUInt16(ushort v) { return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));}
    }
}