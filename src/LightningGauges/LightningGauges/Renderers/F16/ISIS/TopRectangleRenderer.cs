using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class TopRectangleRenderer
    {
        internal static RectangleF DrawTopRectangle(Graphics gfx, int width, ref GraphicsState basicState)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            var topRectangle = new RectangleF(0, 0, width, 42);
            gfx.FillRectangle(Brushes.Black, topRectangle);
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            return topRectangle;
        }
    }
}