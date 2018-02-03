using System;
using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class SRCHLegendRenderer
    {
        internal static void DrawSRCHLegend(Graphics gfx, Rectangle rightLegend2Rectangle, StringFormat verticalOsbLegendRHSFormat, InstrumentState instrumentState, Font font, Brush brush)
        {
            if (instrumentState.RWRPowerOn
                && (instrumentState.SearchMode || NonVisibleSearchThreatsDetector.AreNonVisibleSearchThreatsDetected(instrumentState) && DateTime.UtcNow.Millisecond % 500 < 250))
                //draw highlighted SRCH legend 
            {
                gfx.FillRectangle(brush, rightLegend2Rectangle);
                StringRenderer.DrawString(gfx, "SRCH", font, Brushes.Black, rightLegend2Rectangle, verticalOsbLegendRHSFormat);
            }
            else //draw non-highlighted SRCH legend
            {
                StringRenderer.DrawString(gfx, "SRCH", font, brush, rightLegend2Rectangle, verticalOsbLegendRHSFormat);
            }
        }
    }
}