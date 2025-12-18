using System;
using System.Linq;
using System.Windows;

namespace LightningGauges.Renderers.VectorDrawing
{
    internal class TriangleFillHelper
    {
        public static void DrawTriangleFilledByLines(Point point1, Point point2, Point point3, Action<Point,Point> drawLineFunc, double yStep=0.1)
        {
            if (Enumerable.Any(new[] { point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y }, p => double.IsNaN(p))) return;
            /* at first sort the three vertices by y-coordinate ascending so point1 is the topmost vertice */
            var sortedPoints = new[] { point1, point2, point3 }.OrderBy(p => p.Y).ToArray();
            point1 = sortedPoints[0];
            point2 = sortedPoints[1];
            point3 = sortedPoints[2];

            /* here we know that point1.Y <= point3.Y <= point3.Y */
            /* check for trivial case of bottom-flat triangle */
            if (ApproximatelyEquals(point2.Y, point3.Y))
            {
                FillBottomFlatTriangle(point1, point2, point3, drawLineFunc, yStep);
            }
            /* check for trivial case of top-flat triangle */
            else if (ApproximatelyEquals(point1.Y, point2.Y))
            {
                FillTopFlatTriangle(point1, point2, point3, drawLineFunc, yStep);
            }
            else
            {
                /* general case - split the triangle in a topflat and bottom-flat one */
                var point4 = new Point(
                  point1.X + (point2.Y - point1.Y) / (point3.Y - point1.Y) * (point3.X - point1.X), point2.Y);
                FillBottomFlatTriangle(point1, point2, point4, drawLineFunc, yStep);
                FillTopFlatTriangle(point2, point4, point3, drawLineFunc, yStep);
            }
        }
        private static void FillTopFlatTriangle(Point point1, Point point2, Point point3, Action<Point, Point> drawLineFunc, double yStep=0.1)
        {
            var invslope1 = (point3.X - point1.X) / (point3.Y - point1.Y);
            var invslope2 = (point3.X - point2.X) / (point3.Y - point2.Y);

            var curx1 = point3.X;
            var curx2 = point3.X;

            for (var y = point3.Y; y > point1.Y; y-=yStep)
            {
                drawLineFunc(new Point(curx1, y), new Point(curx2, y));
                curx1 -= (invslope1 * yStep);
                curx2 -= (invslope2 * yStep);
            }
        }
        private static void FillBottomFlatTriangle(Point point1, Point point2, Point point3, Action<Point, Point> drawLineFunc, double yStep = 0.1)
        {
            var invslope1 =  (point2.X - point1.X) / (point2.Y - point1.Y);
            var invslope2 = (point3.X - point1.X) / (point3.Y - point1.Y);

            var curx1 = point1.X;
            var curx2 = point1.X;

            for (var y = point1.Y; y <= point2.Y; y+=yStep)
            {
                drawLineFunc(new Point(curx1, y), new Point(curx2, y));
                curx1 += (invslope1 * yStep);
                curx2 += (invslope2 * yStep);
            }
        }
        private static bool ApproximatelyEquals(double d1, double d2, double tolerance = 0.5)
        {
            return Math.Abs(d1 - d2) <= tolerance;
        }

    }
}
