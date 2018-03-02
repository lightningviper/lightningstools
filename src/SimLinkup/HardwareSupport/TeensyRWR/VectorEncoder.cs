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
        internal static void Serialize(DrawingGroup drawingGroup, Rect bounds, Stream stream, uint DacPrecisionBits, PreprocessorOptions preprocessorOptions = null)
        {
            var figures = drawingGroup.GetGeometry().GetFlattenedPathGeometry().Figures;
            var vectors = new List<(Point Point, bool BeamOn)>();
            foreach (var figure in figures)
            {
                var figurePoints = figure.GetPoints();
                vectors.Add((figurePoints.First(), false));
                vectors.AddRange(figurePoints.Skip(1).Take(figurePoints.Count() - 1).Select((point) => (point, true)));
                vectors.Add((figurePoints.First(), true));
            }
            vectors = VectorPreprocessor.PreprocessVectors(vectors, preprocessorOptions ?? new PreprocessorOptions()).ToList();
            var fullScaleValue = (Math.Pow(2,DacPrecisionBits)-1);
            using (var writer = new StreamWriter(stream, encoding: System.Text.Encoding.Default, bufferSize:2048, leaveOpen:true))
            {
                foreach (var vector in vectors)
                {
                    var x = (ulong)Math.Round((((vector.Point.X - bounds.Left) / bounds.Width)) * fullScaleValue, 0, MidpointRounding.AwayFromZero);
                    var y = (ulong)Math.Round((((vector.Point.Y - bounds.Top) / bounds.Height)) * fullScaleValue, 0, MidpointRounding.AwayFromZero);
                    writer.WriteLine(string.Format("{0}{1},{2}", vector.BeamOn ? "L" : "M", x, y));
                }
                writer.WriteLine("Z");
            }
           
        }
    }
}