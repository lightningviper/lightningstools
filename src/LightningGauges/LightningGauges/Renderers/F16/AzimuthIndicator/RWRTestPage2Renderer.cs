using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class RWRTestPage2Renderer
    {
        internal static void DrawRWRTestPage2(Graphics gfx, Font font)
        {
            var pageLegendStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap
            };
            var RwrLegendRectangle = new Rectangle(86, 80, 30, 30);
            StringRenderer.DrawString(gfx, "F16C", font, Brushes.Lime, RwrLegendRectangle, pageLegendStringFormat);
            var Rwr1LegendRectangle = new Rectangle(105, 100, 30, 30);
            StringRenderer.DrawString(gfx, "1*OFP  0020", font, Brushes.Lime, Rwr1LegendRectangle, pageLegendStringFormat);
            var Rwr2LegendRectangle = new Rectangle(105, 120, 30, 30);
            StringRenderer.DrawString(gfx, "2*WO   0040", font, Brushes.Lime, Rwr2LegendRectangle, pageLegendStringFormat);
            var Rwr3LegendRectangle = new Rectangle(119, 140, 30, 30);
            StringRenderer.DrawString(gfx, "PA   0050", font, Brushes.Lime, Rwr3LegendRectangle, pageLegendStringFormat);
            var Rwr4LegendRectangle = new Rectangle(119, 160, 30, 30);
            StringRenderer.DrawString(gfx, "US   0060", font, Brushes.Lime, Rwr4LegendRectangle, pageLegendStringFormat);
        }
    }
}