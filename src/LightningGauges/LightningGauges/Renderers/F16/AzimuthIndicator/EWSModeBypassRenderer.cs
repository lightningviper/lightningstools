using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeBypassRenderer
    {
        internal static void DrawEWSModeBypass(Color severeColor, Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Font font)
        {
            var legendColor = severeColor;
            using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "BYP", font, legendBrush, ewmsModeRectangle, miscTextStringFormat); }
        }
    }
}