using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class ALTLegendRenderer
    {
        internal static void DrawALTLegend(Graphics gfx, Rectangle rightLegend3Rectangle, StringFormat verticalOsbLegendRHSFormat, InstrumentState instrumentState, Brush brush, Font font)
        {
            if (instrumentState.RWRPowerOn && instrumentState.LowAltitudeMode) //draw highlighted ALT legend 
            {
                gfx.FillRectangle(brush, rightLegend3Rectangle);
                StringRenderer.DrawString(gfx, "ALT", font, Brushes.Black, rightLegend3Rectangle, verticalOsbLegendRHSFormat);
            }
            else //draw non-highlighted ALT legend
            {
                StringRenderer.DrawString(gfx, "ALT", font, brush, rightLegend3Rectangle, verticalOsbLegendRHSFormat);
            }
        }
    }
}