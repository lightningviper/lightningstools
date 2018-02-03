using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class HOFFLegendRenderer
    {
        internal static void DrawHOFFLegend(Graphics gfx, Rectangle leftLegend2Rectangle, StringFormat verticalOsbLegendLHSFormat, InstrumentState instrumentState, Font font, Brush brush)
        {
            if (instrumentState.RWRPowerOn && instrumentState.Handoff) //draw highlighted HOFF legend 
            {
                gfx.FillRectangle(brush, leftLegend2Rectangle);
                StringRenderer.DrawString(gfx, "HOFF", font, Brushes.Black, leftLegend2Rectangle, verticalOsbLegendLHSFormat);
            }
            else //draw non-highlighted HOFF legend
            {
                StringRenderer.DrawString(gfx, "HOFF", font, brush, leftLegend2Rectangle, verticalOsbLegendLHSFormat);
            }
        }
    }
}