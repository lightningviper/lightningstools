using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class MissileLaunchSymbolRenderer
    {
        internal static void DrawMissileLaunchSymbol(Graphics gfx, int backgroundWidth, int backgroundHeight, Pen emitterPen, int width)
        {
            //draw missile launch symbol 
            gfx.DrawEllipse(emitterPen, new RectangleF(backgroundWidth / 2.0f, backgroundHeight / 2.0f, width, width));
        }
    }
}