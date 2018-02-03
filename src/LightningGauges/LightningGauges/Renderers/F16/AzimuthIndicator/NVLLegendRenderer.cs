using System;
using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class NVLLegendRenderer
    {
        internal static void DrawNVLLegend(Graphics gfx, Rectangle rightLegend1Rectangle, StringFormat verticalOsbLegendRHSFormat, InstrumentState instrumentState, Brush brush, Font font)
        {
            if (instrumentState.RWRPowerOn
                && (instrumentState.NavalMode || NonVisibleNavalThreatsDetector.AreNonVisibleNavalThreatsDetected(instrumentState) && DateTime.UtcNow.Millisecond % 500 < 250))
                //draw highlighted NVL legend 
            {
                gfx.FillRectangle(brush, rightLegend1Rectangle);
                StringRenderer.DrawString(gfx, "NVL", font, Brushes.Black, rightLegend1Rectangle, verticalOsbLegendRHSFormat);
            }
            else //draw non-highlighted NVL legend
            {
                StringRenderer.DrawString(gfx, "NVL", font, brush, rightLegend1Rectangle, verticalOsbLegendRHSFormat);
            }
        }
    }
}