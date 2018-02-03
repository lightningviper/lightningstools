using System.Collections.Generic;
using Common.Drawing;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiFixedPitchReferenceBarsRenderer
    {
        internal static void DrawAdiFixedPitchReferenceBars(Graphics g, int centerXPfd, int centerYPfd, bool adiOffFlag)
        {
            var whitePen = new Pen(Color.White) { Width = 2 };
            var baseY = centerYPfd - 3;
            var baseX = centerXPfd - 100;
            var pointsList = new List<Point>();
            if (!adiOffFlag)
            {
                //draw fixed pitch reference bars
                pointsList.Add(new Point(baseX, baseY));
                pointsList.Add(new Point(baseX, baseY + 6));
                pointsList.Add(new Point(baseX + 67, baseY + 6));
                pointsList.Add(new Point(baseX + 67, baseY + 23));
                pointsList.Add(new Point(baseX + 72, baseY + 23));
                pointsList.Add(new Point(baseX + 72, baseY));
                g.FillPolygon(Brushes.Black, pointsList.ToArray());
                g.DrawPolygon(whitePen, pointsList.ToArray());
                pointsList.Clear();
                pointsList.Add(new Point(baseX + 128, baseY));
                pointsList.Add(new Point(baseX + 128, baseY + 23));
                pointsList.Add(new Point(baseX + 133, baseY + 23));
                pointsList.Add(new Point(baseX + 133, baseY + 6));
                pointsList.Add(new Point(baseX + 197, baseY + 6));
                pointsList.Add(new Point(baseX + 197, baseY));
                g.FillPolygon(Brushes.Black, pointsList.ToArray());
                g.DrawPolygon(whitePen, pointsList.ToArray());
            }
        }
    }
}