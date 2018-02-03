using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class StringRenderer
    {
        private static readonly ThreadLocal<GraphicsPath> FontPath = new ThreadLocal<GraphicsPath>(()=>new GraphicsPath());

        internal static void DrawString(Graphics g, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            FontPath.Value.Reset();
            FontPath.Value.AddString(s, font.FontFamily, (int) font.Style, font.SizeInPoints, layoutRectangle, format);
            g.FillPathFast(brush, FontPath.Value);
        }
    }
}