using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeManualRenderer
    {
        internal static void DrawEWSModeManual(Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Color okColor, Font font)
        {
            var legendColor = okColor;
            using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "MAN", font, legendBrush, ewmsModeRectangle, miscTextStringFormat); }
        }
    }
}