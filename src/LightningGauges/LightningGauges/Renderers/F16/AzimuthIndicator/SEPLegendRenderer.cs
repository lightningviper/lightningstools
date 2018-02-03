using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal class SEPLegendRenderer
    {
        internal static void DrawSEPLegend(Graphics gfx, Rectangle leftLegend3Rectangle, StringFormat verticalOsbLegendLHSFormat, InstrumentState instrumentState, Font font, Brush brush)
        {
            if (instrumentState.RWRPowerOn && instrumentState.SeparateMode) //draw highlighted SEP legend 
            {
                gfx.FillRectangle(brush, leftLegend3Rectangle);
                StringRenderer.DrawString(gfx, "SEP", font, Brushes.Black, leftLegend3Rectangle, verticalOsbLegendLHSFormat);
            }
            else //draw non-highlighted SEP legend
            {
                StringRenderer.DrawString(gfx, "SEP", font, brush, leftLegend3Rectangle, verticalOsbLegendLHSFormat);
            }
        }
    }
}