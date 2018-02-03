using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class CenterRWRSearchModeIndicationRenderer
    {
        internal static void DrawCenterRWRSearchModeIndication(Graphics gfx, Font font)
        {
            var pageLegendStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap
            };

            var RwrLegendRectangle = new Rectangle(113, 119, 30, 30);
            StringRenderer.DrawString(gfx, "S", font, Brushes.Lime, RwrLegendRectangle, pageLegendStringFormat);
        }
    }
}