using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class Other1CountRenderer
    {
        internal static void DrawOther1Count(
            Color severeColor, Color warnColor, Color okColor, Graphics gfx, RectangleF other1CountRectangle, StringFormat chaffFlareCountStringFormat, InstrumentState instrumentState, Font font)
        {
            Color other1CountColor;
            if (instrumentState.Other1Count == 0) { other1CountColor = severeColor; }
            else if (instrumentState.Other1Low) { other1CountColor = warnColor; }
            else { other1CountColor = okColor; }
            using (var other1CountBrush = new SolidBrush(other1CountColor))
            {
                StringRenderer.DrawString(gfx, "OTR1", font, other1CountBrush, other1CountRectangle, chaffFlareCountStringFormat);
                other1CountRectangle.Offset(0, 12);
                StringRenderer.DrawString(gfx, $"{instrumentState.Other1Count:00}", font, other1CountBrush, other1CountRectangle, chaffFlareCountStringFormat);
            }
        }
    }
}