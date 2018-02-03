using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class LocalizerCommandBarRenderer
    {
        internal static void DrawLocalizerCommandBar(
            Graphics g, InstrumentState instrumentState, PointF farLeftLocalizerMarkerCenterPoint, PointF farRightLocalizerMarkerCenterPoint, PointF topGlideSlopeMarkerCenterPoint,
            PointF bottomGlideSlopeMarkerCenterPoint)
        {
            //prepare draw localizer indicator line

            var minIlsHorizontalPositionVal = -instrumentState.LocalizerDeviationLimitDegrees;
            var maxIlsHorizontalPositionVal = instrumentState.LocalizerDeviationLimitDegrees;
            var ilsHorizontalPositionRange = maxIlsHorizontalPositionVal - minIlsHorizontalPositionVal;
            var currentIlsHorizontalPositionVal = instrumentState.LocalizerDeviationDegrees + Math.Abs(minIlsHorizontalPositionVal);
            if (currentIlsHorizontalPositionVal < 0) currentIlsHorizontalPositionVal = 0;
            if (currentIlsHorizontalPositionVal > ilsHorizontalPositionRange) currentIlsHorizontalPositionVal = ilsHorizontalPositionRange;

            var minIlsBarX = farLeftLocalizerMarkerCenterPoint.X;
            var maxIlsBarX = farRightLocalizerMarkerCenterPoint.X;
            float ilsBarXRange = (int) (maxIlsBarX - minIlsBarX) + 1;

            float currentIlsBarX = (int) (minIlsBarX + currentIlsHorizontalPositionVal / ilsHorizontalPositionRange * ilsBarXRange);

            var ilsBarTop = new PointF(currentIlsBarX, topGlideSlopeMarkerCenterPoint.Y);
            var ilsBarBottom = new PointF(currentIlsBarX, bottomGlideSlopeMarkerCenterPoint.Y);

            var localizerBarPen = new Pen(Color.Yellow) {Width = 3};
            if (instrumentState.ShowCommandBars && !instrumentState.LocalizerFlag && !instrumentState.OffFlag)
            {
                if (instrumentState.LocalizerFlag)
                {
                    localizerBarPen.DashStyle = DashStyle.Dash;
                    localizerBarPen.DashOffset = 3;
                }
                //draw localizer command bar
                g.DrawLineFast(localizerBarPen, ilsBarTop, ilsBarBottom);
                g.DrawImageFast(ILSBarsRenderer.MarkerDiamond, currentIlsBarX - ILSBarsRenderer.MarkerDiamond.Width / 2, farLeftLocalizerMarkerCenterPoint.Y - ILSBarsRenderer.MarkerDiamond.Width / 2);
            }
        }
    }
}