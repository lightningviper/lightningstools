using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class LocalizerMarkersRenderer
    {
        internal static void DrawLocalizerMarkers(
            Graphics g, PointF farLeftLocalizerMarkerCenterPoint, PointF leftMiddleLocalizerMarkerCenterPoint, PointF middleLocalizerMarkerCenterPoint, PointF farRightLocalizerMarkerCenterPoint,
            PointF rightMiddleLocalizerMarkerCenterPoint, InstrumentState instrumentState)
        {
            //draw localizer markers
            if (instrumentState.ShowCommandBars && !instrumentState.LocalizerFlag && !instrumentState.OffFlag)
            {
                var blackPen = new Pen(Color.Black);

                var path = new GraphicsPath();
                path.AddEllipse(new RectangleF(farLeftLocalizerMarkerCenterPoint.X - 3, farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(farLeftLocalizerMarkerCenterPoint.X - 5, farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(leftMiddleLocalizerMarkerCenterPoint.X - 3, farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(leftMiddleLocalizerMarkerCenterPoint.X - 5, farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddRectangle(new RectangleF(new PointF(middleLocalizerMarkerCenterPoint.X - 1, middleLocalizerMarkerCenterPoint.Y - 6), new SizeF(2, 12)));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(rightMiddleLocalizerMarkerCenterPoint.X - 3, farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(rightMiddleLocalizerMarkerCenterPoint.X - 5, farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);

                path.Reset();
                path.AddEllipse(new RectangleF(farRightLocalizerMarkerCenterPoint.X - 3, farLeftLocalizerMarkerCenterPoint.Y - 3, 6, 6));
                path.AddEllipse(new RectangleF(farRightLocalizerMarkerCenterPoint.X - 5, farLeftLocalizerMarkerCenterPoint.Y - 5, 10, 10));
                g.DrawPathFast(blackPen, path);
                g.FillPathFast(Brushes.White, path);
            }
        }
    }
}