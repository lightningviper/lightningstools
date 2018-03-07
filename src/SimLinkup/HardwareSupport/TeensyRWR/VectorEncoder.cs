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
            var figures = drawingGroup.GetGeometry().GetFlattenedPathGeometry().Figures;
            var vectors = new List<(Point Point, bool BeamOn)>();
            foreach (var figure in figures)
            {
                var figurePoints = figure.GetPoints();
                vectors.Add((figurePoints.First(), false));
                vectors.AddRange(figurePoints.Skip(1).Take(figurePoints.Count() - 1).Select((point) => (point, true)));
                vectors.Add((figurePoints.First(), true));
            }
            using (var writer = new StreamWriter(stream, encoding: System.Text.Encoding.Default, bufferSize:2048, leaveOpen:true))
            {
                var lastVectorString = String.Empty;
                foreach (var vector in vectors)
                {
                    var x = (((vector.Point.X - bounds.Left) / bounds.Width) * 2.0) - 1.0;
                    var y = (((vector.Point.Y - bounds.Top) / bounds.Height) * 2.0) - 1.0;
                    var thisVectorString = string.Format("{0}{1:0.####},{2:0.####}", vector.BeamOn ? "L" : "M", x, y);
                    if (thisVectorString != lastVectorString)
                    {
                        writer.Write(thisVectorString);
                    }
                    lastVectorString = thisVectorString;
                }
                writer.Write("Z");
            }
           
        }
    }
}