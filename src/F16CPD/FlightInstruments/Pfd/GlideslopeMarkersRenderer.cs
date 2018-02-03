using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class GlideslopeMarkersRenderer
    {
        internal static void DrawGlideslopeMarkers(Graphics g, Point topGlideSlopeMarkerCenterPoint,
            Point upperMiddleGlideSlopeMarkerCenterPoint,
            Point middleGlideSlopeMarkerCenterPoint,
            Point lowerMiddleGlideSlopeMarkerCenterPoint,
            Point bottomGlideSlopeMarkerCenterPoint, bool adiEnableCommandBars, bool adiGlideslopeInvalidFlag, bool adiOffFlag)
        {
            //draw glideslope markers
            if (adiEnableCommandBars && !adiGlideslopeInvalidFlag &&
                !adiOffFlag)
            {
                var blackPen = new Pen(Color.Black);
                var path = new GraphicsPath();
                path.AddEllipse(new Rectangle(topGlideSlopeMarkerCenterPoint.X - 3, topGlideSlopeMarkerCenterPoint.Y - 3,
                    6, 6));
                path.AddEllipse(new Rectangle(topGlideSlopeMarkerCenterPoint.X - 5, topGlideSlopeMarkerCenterPoint.Y - 5,
                    10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new Rectangle(upperMiddleGlideSlopeMarkerCenterPoint.X - 3,
                    upperMiddleGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(upperMiddleGlideSlopeMarkerCenterPoint.X - 5,
                    upperMiddleGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddRectangle(
                    new Rectangle(
                        new Point(
                            middleGlideSlopeMarkerCenterPoint.X - 6,
                            middleGlideSlopeMarkerCenterPoint.Y - 1), new Size(12, 2)));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);


                path.Reset();
                path.AddEllipse(new Rectangle(lowerMiddleGlideSlopeMarkerCenterPoint.X - 3,
                    lowerMiddleGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(lowerMiddleGlideSlopeMarkerCenterPoint.X - 5,
                    lowerMiddleGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new Rectangle(bottomGlideSlopeMarkerCenterPoint.X - 3,
                    bottomGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new Rectangle(bottomGlideSlopeMarkerCenterPoint.X - 5,
                    bottomGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);
            }
        }
    }
}