using System;
using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class UNKLegendRenderer
    {
        internal static void DrawUNKLegend(Graphics gfx, Rectangle leftLegend1Rectangle, StringFormat verticalOsbLegendLHSFormat, InstrumentState instrumentState, Font font, Brush brush)
        {
            if (instrumentState.RWRPowerOn
                && (instrumentState.UnknownThreatScanMode
                    || NonVisibleUnknownThreatsDetector.AreNonVisibleUnknownThreatsDetected(instrumentState) && DateTime.UtcNow.Millisecond % 500 < 250)) //draw highlighted UNK legend 
            {
                gfx.FillRectangle(brush, leftLegend1Rectangle);
                StringRenderer.DrawString(gfx, "UNK", font, Brushes.Black, leftLegend1Rectangle, verticalOsbLegendLHSFormat);
            }
            else //draw non-highlighted UNK legend
            {
                StringRenderer.DrawString(gfx, "UNK", font, brush, leftLegend1Rectangle, verticalOsbLegendLHSFormat);
            }
        }
    }
}