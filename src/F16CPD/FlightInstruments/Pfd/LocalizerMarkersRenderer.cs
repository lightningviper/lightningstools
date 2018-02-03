using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class LocalizerMarkersRenderer
    {
        internal static void DrawLocalizerMarkers(Graphics g, Point farLeftLocalizerMarkerCenterPoint,
            Point leftMiddleLocalizerMarkerCenterPoint,
            Point middleLocalizerMarkerCenterPoint,
            Point farRightLocalizerMarkerCenterPoint,
            Point rightMiddleLocalizerMarkerCenterPoint, bool adiEnableCommandBars, bool adiLocalizerInvalidFlag, bool adiOffFlag)
        {
            //draw localizer markers
            if (adiEnableCommandBars && !adiLocalizerInvalidFlag &&
                !adiOffFlag)
            {
                var blackPen = new Pen(Color.Black);

                var path = new GraphicsPath();
                path.AddEllipse(new Rectangle(farLeftLocalizerMarkerCenterPoint.X - 3,
                    farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(farLeftLocalizerMarkerCenterPoint.X - 5,
                    farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new Rectangle(leftMiddleLocalizerMarkerCenterPoint.X - 3,
                    farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(leftMiddleLocalizerMarkerCenterPoint.X - 5,
                    farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddRectangle(
                    new Rectangle(
                        new Point(middleLocalizerMarkerCenterPoint.X - 1, middleLocalizerMarkerCenterPoint.Y - 6),
                        new Size(2, 12)));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new Rectangle(rightMiddleLocalizerMarkerCenterPoint.X - 3,
                    farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(rightMiddleLocalizerMarkerCenterPoint.X - 5,
                    farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new Rectangle(farRightLocalizerMarkerCenterPoint.X - 3,
                    farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(farRightLocalizerMarkerCenterPoint.X - 5,
                    farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);
            }
        }
    }
}