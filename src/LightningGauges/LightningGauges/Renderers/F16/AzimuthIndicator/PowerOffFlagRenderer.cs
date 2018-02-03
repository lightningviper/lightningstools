using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class PowerOffFlagRenderer
    {
        internal static void DrawPowerOffFlag(float atdInnerRingDiameter, int backgroundWidth, int backgroundHeight, Color severeColor, Graphics gfx)
        {
            var rwrOffTextHeight = atdInnerRingDiameter;
            var rwrOffTextWidth = atdInnerRingDiameter;
            var rwrRectangle = new RectangleF(backgroundWidth / 2.0f - rwrOffTextWidth / 2.0f, backgroundHeight / 2.0f - rwrOffTextHeight / 2.0f, rwrOffTextWidth, rwrOffTextHeight);
            rwrRectangle.Inflate(-5, -5);
            var legendColor = severeColor;
            var legendPen = new Pen(legendColor);
            gfx.DrawLineFast(legendPen, new PointF(rwrRectangle.Left, rwrRectangle.Top), new PointF(rwrRectangle.Right, rwrRectangle.Bottom));
            gfx.DrawLineFast(legendPen, new PointF(rwrRectangle.Left, rwrRectangle.Bottom), new PointF(rwrRectangle.Right, rwrRectangle.Top));
        }
    }
}