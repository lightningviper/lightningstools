using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class HeadingTriangleRenderer
    {
        internal static void DrawHeadingTriangle(Graphics gfx, ref GraphicsState basicState, int width, int height, SizeF headingTapeSize)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            const float headingTriangleHeight = 15;
            const float headingTriangleWidth = 10;
            var headingTriangleCenter = new PointF(width / 2.0f - 8.5f, height - headingTapeSize.Height);
            var headingTriangleLeft = new PointF(width / 2.0f - 8.5f - headingTriangleWidth / 2.0f, height - headingTriangleHeight - headingTapeSize.Height);
            var headingTriangleRight = new PointF(width / 2.0f - 8.5f + headingTriangleWidth / 2.0f, height - headingTriangleHeight - headingTapeSize.Height);
            gfx.FillPolygon(Brushes.White, new[] {headingTriangleCenter, headingTriangleLeft, headingTriangleRight});
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }
    }
}