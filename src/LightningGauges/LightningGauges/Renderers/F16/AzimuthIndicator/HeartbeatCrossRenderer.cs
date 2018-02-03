using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class HeartbeatCrossRenderer
    {
        internal static void DrawHeartbeatCross(Graphics gfx, ref GraphicsState basicState, int backgroundWidth, int backgroundHeight, Pen scopeGreenPen)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);

            //draw heartbeat cross
            gfx.DrawLineFast(scopeGreenPen, new PointF(backgroundWidth / 2.0f - 20, backgroundHeight / 2.0f), new PointF(backgroundWidth / 2.0f - 10, backgroundHeight / 2.0f));
            gfx.DrawLineFast(scopeGreenPen, new PointF(backgroundWidth / 2.0f + 20, backgroundHeight / 2.0f), new PointF(backgroundWidth / 2.0f + 10, backgroundHeight / 2.0f));
            gfx.DrawLineFast(scopeGreenPen, new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f - 20), new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f - 10));
            gfx.DrawLineFast(scopeGreenPen, new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f + 20), new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f + 10));
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }
    }
}