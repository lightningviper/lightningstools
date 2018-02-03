using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeOffRenderer
    {
        internal static void DrawEWSModeOff(Color severeColor, Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Font font)
        {
            var legendColor = severeColor;
            using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "OFF", font, legendBrush, ewmsModeRectangle, miscTextStringFormat); }
        }
    }
}