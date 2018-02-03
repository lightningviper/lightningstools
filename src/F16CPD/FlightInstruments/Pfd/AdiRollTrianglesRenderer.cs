using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiRollTrianglesRenderer
    {
        internal static Point[] DrawAdiRollTriangles(Graphics g, int centerXPfd, int centerYPfd, float rollDegrees,
            bool off)
        {
            var baseX = centerXPfd;
            var baseY = centerYPfd + 3;
            var blackPen = new Pen(Color.Black);
            //draw roll triangles
            var zeroDegreeTriangle = new[]
            {
                new Point(baseX - 14, baseY - 203), new Point(baseX, baseY - 173),
                new Point(baseX + 14, baseY - 203)
            };
            if (rollDegrees >= -0.5f && rollDegrees <= 0.5f && !off)
            {
                g.FillPolygon(Brushes.White, zeroDegreeTriangle);
            }
            g.DrawPolygon(blackPen, zeroDegreeTriangle);
            var leftTriangleOutside = new[]
            {
                new Point(baseX - 130, baseY - 154), new Point(baseX - 150, baseY - 134),
                new Point(baseX - 119, baseY - 123)
            };
            var leftTriangleInside = new[]
            {
                new Point(baseX - 131, baseY - 147), new Point(baseX - 143, baseY - 135),
                new Point(baseX - 125, baseY - 128)
            };

            var path = new GraphicsPath();
            path.AddPolygon(leftTriangleInside);
            path.AddPolygon(leftTriangleOutside);
            g.FillPathFast(Brushes.White, path);
            if (rollDegrees >= 44 && rollDegrees <= 46 && !off)
            {
                g.FillPolygon(Brushes.White, leftTriangleInside);
            }
            g.DrawPathFast(blackPen, path);

            var rightTriangleOutside = new[]
            {
                new Point(baseX + 130, baseY - 154), new Point(baseX + 120, baseY - 123),
                new Point(baseX + 151, baseY - 134)
            };
            var rightTriangleInside = new[]
            {
                new Point(baseX + 132, baseY - 147), new Point(baseX + 125, baseY - 128),
                new Point(baseX + 144, baseY - 136)
            };


            path.Reset();
            path.AddPolygon(rightTriangleInside);
            path.AddPolygon(rightTriangleOutside);
            g.FillPathFast(Brushes.White, path);

            if (rollDegrees <= -44 && rollDegrees >= -46 && !off)
            {
                g.FillPolygon(Brushes.White, rightTriangleInside);
            }
            g.DrawPathFast(blackPen, path);

            return zeroDegreeTriangle;
        }
    }
}