using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class MissileActivitySymbolRenderer
    {
        internal static void DrawMissileActivitySymbol(Graphics gfx, int backgroundWidth, int backgroundHeight, Pen emitterPen, int width)
        {
            if (DateTime.UtcNow.Millisecond % 250 >= 125) return;
            //draw missile activity symbol
            emitterPen.DashStyle = DashStyle.Dash;
            gfx.DrawEllipse(emitterPen, new RectangleF(backgroundWidth / 2.0f + 4, backgroundHeight / 2.0f + 4, width - 8, width - 8));
            emitterPen.DashStyle = DashStyle.Solid;
        }
    }
}