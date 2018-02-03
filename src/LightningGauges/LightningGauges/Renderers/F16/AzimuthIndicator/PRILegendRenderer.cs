using System;
using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class PRILegendRenderer
    {
        internal static void DrawPRILegend(Graphics gfx, Rectangle leftLegend4Rectangle, StringFormat verticalOsbLegendLHSFormat, InstrumentState instrumentState, Brush brush, Font font)
        {
            if (instrumentState.RWRPowerOn
                && (instrumentState.PriorityMode && !NonVisiblePriorityThreatsDetector.AreNonVisiblePriorityThreatsDetected(instrumentState)
                    || instrumentState.PriorityMode && NonVisiblePriorityThreatsDetector.AreNonVisiblePriorityThreatsDetected(instrumentState) && DateTime.UtcNow.Millisecond % 500 < 250))
            {
                gfx.FillRectangle(brush, leftLegend4Rectangle);
                StringRenderer.DrawString(gfx, "PRI", font, Brushes.Black, leftLegend4Rectangle, verticalOsbLegendLHSFormat);
            }
            else //draw non-highlighted PRI legend
            {
                StringRenderer.DrawString(gfx, "PRI", font, brush, leftLegend4Rectangle, verticalOsbLegendLHSFormat);
            }
        }
    }
}