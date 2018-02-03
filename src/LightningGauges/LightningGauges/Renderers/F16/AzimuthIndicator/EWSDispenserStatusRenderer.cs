using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EWSDispenserStatusRenderer
    {
        internal static void DrawEWSDispenserStatus(
            Graphics gfx, RectangleF goNogoRect, StringFormat miscTextStringFormat, Color warnColor, Color okColor, RectangleF rdyRectangle, RectangleF pflRectangle, Color scopeGreenColor, Font font,
            InstrumentState instrumentState)
        {
            if (instrumentState.EWSGo)
            {
                var legendColor = scopeGreenColor;
                using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "GO", font, legendBrush, goNogoRect, miscTextStringFormat);}
        }
            else if (instrumentState.EWSNoGo)
            {
                var legendColor = warnColor;
                using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "NOGO", font, legendBrush, goNogoRect, miscTextStringFormat); }
            }
            else if (instrumentState.EWSDispenseReady)
            {
                var legendColor = scopeGreenColor;
                using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "DISP", font, legendBrush, goNogoRect, miscTextStringFormat); }
            }

            if (instrumentState.EWSDispenseReady)
            {
                var legendColor = okColor;
                using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "RDY", font, legendBrush, rdyRectangle, miscTextStringFormat); }
            }

            if (instrumentState.EWSDegraded) //draw PFL legend 
            {
                var legendColor = warnColor;
                using (var legendBrush = new SolidBrush(legendColor)) { StringRenderer.DrawString(gfx, "PFL", font, legendBrush, pflRectangle, miscTextStringFormat); }
            }
        }
    }
}