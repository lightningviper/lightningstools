using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class BearingToBeaconIndicatorRenderer
    {
        private static Color NeedleColor = Color.FromArgb(102, 190, 157);
        internal static void DrawBearingToBeaconIndicator(Graphics g, RectangleF outerBounds, InstrumentState instrumentState)
        {
            using (var NeedleBrush = new SolidBrush(NeedleColor))
            using (var needlePen = new Pen(NeedleColor) { Width = 4 })
            {
                var basicState = g.Save();

                g.TranslateTransform(outerBounds.Width / 2.0f, outerBounds.Height / 2.0f);
                g.RotateTransform(-instrumentState.MagneticHeadingDegrees);
                g.RotateTransform(instrumentState.BearingToBeaconDegrees);
                g.TranslateTransform(-outerBounds.Width / 2.0f, -outerBounds.Height / 2.0f);

                g.TranslateTransform(outerBounds.Width / 2.0f, 0);

                const float bearingTriangleWidth = 23;
                const float bearingTriangleHeight = 25;

                var bearingTriangleTop = new PointF(0, 5);
                var bearingTriangleLeft = new PointF(-bearingTriangleWidth / 2.0f, bearingTriangleTop.Y + bearingTriangleHeight);
                var bearingTriangleRight = new PointF(bearingTriangleWidth / 2.0f, bearingTriangleTop.Y + bearingTriangleHeight);
                g.FillPolygon(NeedleBrush, new[] { bearingTriangleTop, bearingTriangleLeft, bearingTriangleRight });

                const float bearingLineTopHeight = 23;
                var bearingLineTopTop = new PointF(bearingTriangleTop.X, bearingTriangleLeft.Y);
                var bearingLineTopBottom = new PointF(bearingLineTopTop.X, bearingLineTopTop.Y + bearingLineTopHeight);
                g.DrawLineFast(needlePen, bearingLineTopTop, bearingLineTopBottom);

                const float bearingLineBottomHeight = 37;
                var bearingLineBottomTop = new PointF(bearingTriangleTop.X, 455);
                var bearingLineBottomBottom = new PointF(bearingTriangleTop.X, bearingLineBottomTop.Y + bearingLineBottomHeight);
                g.DrawLineFast(needlePen, bearingLineBottomTop, bearingLineBottomBottom);

                GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            }
        }
    }
}