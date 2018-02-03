using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class SelectedThreatDiamondRenderer
    {
        internal static void DrawSelectedThreatDiamond(Graphics gfx, int backgroundWidth, int backgroundHeight, Pen emitterPen, int width)
        {
            //draw "selected threat " diamond
            var points = new[]
            {
                new PointF(backgroundWidth / 2.0f + width / 2.0f, backgroundHeight / 2.0f), new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f + width / 2.0f),
                new PointF(backgroundWidth / 2.0f + width / 2.0f, backgroundHeight / 2.0f + width), new PointF(backgroundWidth / 2.0f + width, backgroundHeight / 2.0f + width / 2.0f)
            };
            gfx.DrawPolygon(emitterPen, points);
        }
    }
}