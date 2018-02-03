using System;
using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class HeartbeatTickRenderer
    {
        internal static void DrawHeartbeatTick(Graphics gfx, int backgroundWidth, int backgroundHeight, Pen scopeGreenPen)
        {
            gfx.DrawLineFast(
                scopeGreenPen, new PointF(backgroundWidth / 2.0f + 10, backgroundHeight / 2.0f),
                DateTime.UtcNow.Millisecond < 500 ? new PointF(backgroundWidth / 2.0f + 10, backgroundHeight / 2.0f + 5) : new PointF(backgroundWidth / 2.0f + 10, backgroundHeight / 2.0f - 5));
        }
    }
}