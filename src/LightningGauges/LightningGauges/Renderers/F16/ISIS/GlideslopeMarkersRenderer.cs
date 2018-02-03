using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class GlideslopeMarkersRenderer
    {
        internal static void DrawGlideslopeMarkers(
            Graphics g, PointF topGlideSlopeMarkerCenterPoint, PointF upperMiddleGlideSlopeMarkerCenterPoint, PointF middleGlideSlopeMarkerCenterPoint, PointF lowerMiddleGlideSlopeMarkerCenterPoint,
            PointF bottomGlideSlopeMarkerCenterPoint, InstrumentState instrumentState)
        {
            //draw glideslope markers
            if (instrumentState.ShowCommandBars && !instrumentState.GlideslopeFlag && !instrumentState.OffFlag)
            {
                var blackPen = new Pen(Color.Black);
                var path = new GraphicsPath();
                path.AddEllipse(new RectangleF(topGlideSlopeMarkerCenterPoint.X - 3, topGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(topGlideSlopeMarkerCenterPoint.X - 5, topGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(upperMiddleGlideSlopeMarkerCenterPoint.X - 3, upperMiddleGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(upperMiddleGlideSlopeMarkerCenterPoint.X - 5, upperMiddleGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddRectangle(new RectangleF(new PointF(middleGlideSlopeMarkerCenterPoint.X - 6, middleGlideSlopeMarkerCenterPoint.Y - 1), new Size(12, 2)));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(lowerMiddleGlideSlopeMarkerCenterPoint.X - 3, lowerMiddleGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(lowerMiddleGlideSlopeMarkerCenterPoint.X - 5, lowerMiddleGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(bottomGlideSlopeMarkerCenterPoint.X - 3, bottomGlideSlopeMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(bottomGlideSlopeMarkerCenterPoint.X - 5, bottomGlideSlopeMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);
            }
        }
    }
}