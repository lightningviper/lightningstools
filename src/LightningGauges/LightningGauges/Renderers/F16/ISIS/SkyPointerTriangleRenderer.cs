using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class SkyPointerTriangleRenderer
    {
        internal static void DrawSkyPointerTriangle(
            Graphics g, int width, int height, GraphicsState basicState, float rollDegrees, float centerY, float radiusFromCenterToBottomOfIndexLine, Pen rollIndexPen)
        {
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            g.TranslateTransform(width / 2.0f, height / 2.0f);
            g.RotateTransform(-rollDegrees);
            g.TranslateTransform(-(float) width / 2.0f, -height / 2.0f);
            const float triangleWidth = 15;
            const float triangleHeight = 10;
            var bottomLeft = new PointF(width / 2.0f - triangleWidth / 2.0f, centerY - radiusFromCenterToBottomOfIndexLine + triangleHeight);
            var topCenter = new PointF(width / 2.0f, centerY - radiusFromCenterToBottomOfIndexLine);
            var bottomRight = new PointF(width / 2.0f + triangleWidth / 2.0f, centerY - radiusFromCenterToBottomOfIndexLine + triangleHeight);
            rollIndexPen.Width = 4;
            g.FillPolygon(Brushes.White, new[] {bottomLeft, topCenter, bottomRight});
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
        }
    }
}