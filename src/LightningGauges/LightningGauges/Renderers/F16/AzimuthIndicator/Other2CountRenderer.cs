using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class Other2CountRenderer
    {
        internal static void DrawOther2Count(
            Color severeColor, Color warnColor, Color okColor, Graphics gfx, RectangleF other2CountRectangle, StringFormat chaffFlareCountStringFormat, InstrumentState instrumentState, Font font)
        {
            Color other2CountColor;
            if (instrumentState.Other2Count == 0) { other2CountColor = severeColor; }
            else if (instrumentState.Other2Low) { other2CountColor = warnColor; }
            else { other2CountColor = okColor; }
            using (var other2CountBrush = new SolidBrush(other2CountColor))
            {
                StringRenderer.DrawString(gfx, "OTR2", font, other2CountBrush, other2CountRectangle, chaffFlareCountStringFormat);
                other2CountRectangle.Offset(0, 12);
                StringRenderer.DrawString(gfx, $"{instrumentState.Other2Count:00}", font, other2CountBrush, other2CountRectangle, chaffFlareCountStringFormat);
            }
        }
    }
}