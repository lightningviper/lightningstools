using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSModeStandbyRenderer
    {
        internal static void DrawEWSModeStandby(Graphics gfx, RectangleF ewmsModeRectangle, StringFormat miscTextStringFormat, Color warnColor, Font font)
        {
            var legendColor = warnColor;
            using (var legendBrush = new SolidBrush(legendColor))
            {
                StringRenderer.DrawString(gfx, "SBY", font, legendBrush, ewmsModeRectangle, miscTextStringFormat);
            }
        }
    }
}