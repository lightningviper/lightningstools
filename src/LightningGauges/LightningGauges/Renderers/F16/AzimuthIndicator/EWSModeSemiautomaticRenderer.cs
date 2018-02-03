using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeSemiautomaticRenderer
    {
        internal static void DrawEWSModeSemiautomatic(Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Color okColor, Font font)
        {
            var legendColor = okColor;
            using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "SEM", font, legendBrush, ewmsModeRectangle, miscTextStringFormat); }
        }
    }
}