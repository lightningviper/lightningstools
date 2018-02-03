using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class GlideslopeCommandBarRenderer
    {
        internal static void DrawGlideslopeCommandBar(
            Graphics g, InstrumentState instrumentState, PointF topGlideSlopeMarkerCenterPoint, PointF bottomGlideSlopeMarkerCenterPoint, PointF farLeftLocalizerMarkerCenterPoint,
            PointF farRightLocalizerMarkerCenterPoint)
        {
            //prepare to draw glideslope bar
            var minIlsVerticalPositionVal = -instrumentState.GlideslopeDeviationLimitDegrees;
            var maxIlsVerticalPositionVal = instrumentState.GlideslopeDeviationLimitDegrees;
            var IlsVerticalPositionRange = maxIlsVerticalPositionVal - minIlsVerticalPositionVal;

            var currentIlsVerticalPositionVal = -instrumentState.GlideslopeDeviationDegrees + Math.Abs(minIlsVerticalPositionVal);
            if (currentIlsVerticalPositionVal < 0) currentIlsVerticalPositionVal = 0;
            if (currentIlsVerticalPositionVal > IlsVerticalPositionRange) currentIlsVerticalPositionVal = IlsVerticalPositionRange;

            var minIlsBarY = topGlideSlopeMarkerCenterPoint.Y;
            var maxIlsBarY = bottomGlideSlopeMarkerCenterPoint.Y;
            float ilsBarYRange = (int) (maxIlsBarY - minIlsBarY) + 1;

            var currentIlsBarY = (int) (minIlsBarY + currentIlsVerticalPositionVal / IlsVerticalPositionRange * ilsBarYRange);

            var ilsBarLeft = new PointF(farLeftLocalizerMarkerCenterPoint.X - 7, currentIlsBarY);
            var ilsBarRight = new PointF(farRightLocalizerMarkerCenterPoint.X + 7, currentIlsBarY);

            //draw glideslope bar
            if (instrumentState.ShowCommandBars && !instrumentState.GlideslopeFlag && !instrumentState.OffFlag)
            {
                var glideslopeBarPen = new Pen(Color.Yellow) {Width = 3};
                if (instrumentState.GlideslopeFlag)
                {
                    glideslopeBarPen.DashStyle = DashStyle.Dash;
                    glideslopeBarPen.DashOffset = 3;
                }
                g.DrawLineFast(glideslopeBarPen, ilsBarLeft, ilsBarRight);
                g.DrawImageFast(ILSBarsRenderer.MarkerDiamond, topGlideSlopeMarkerCenterPoint.X - ILSBarsRenderer.MarkerDiamond.Width / 2, currentIlsBarY - ILSBarsRenderer.MarkerDiamond.Width / 2);
            }
        }
    }
}