using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class RWRTestPage1Renderer
    {
        internal static void DrawRWRTestPage1(Graphics gfx, Font font)
        {
            var pageLegendStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap
            };
            var RwrLegendRectangle = new Rectangle(114, 98, 30, 30);
            StringRenderer.DrawString(gfx, "RWR", font, Brushes.Lime, RwrLegendRectangle, pageLegendStringFormat);
            var Rwr2LegendRectangle = new Rectangle(114, 118, 30, 30);
            StringRenderer.DrawString(gfx, "SYSTEM GO", font, Brushes.Lime, Rwr2LegendRectangle, pageLegendStringFormat);
        }
    }
}