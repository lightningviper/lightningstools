using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class FlareCountRenderer
    {
        internal static void DrawFlareCount(
            Color severeColor, Color warnColor, Color okColor, Graphics gfx, RectangleF flareCountRectangle, StringFormat chaffFlareCountStringFormat, InstrumentState instrumentState, Font font)
        {
            Color flareCountColor;
            if (instrumentState.FlareCount == 0) { flareCountColor = severeColor; }
            else if (instrumentState.FlareLow) { flareCountColor = warnColor; }
            else { flareCountColor = okColor; }
            using (var flareCountBrush = new SolidBrush(flareCountColor))
            {
                StringRenderer.DrawString(gfx, "FLAR", font, flareCountBrush, flareCountRectangle, chaffFlareCountStringFormat);
                flareCountRectangle.Offset(0, 12);
                StringRenderer.DrawString(gfx, $"{instrumentState.FlareCount:00}", font, flareCountBrush, flareCountRectangle, chaffFlareCountStringFormat);
            }
        }
    }
}