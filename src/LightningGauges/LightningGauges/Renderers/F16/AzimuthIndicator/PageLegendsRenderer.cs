using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class PageLegendsRenderer
    {
        internal static void DrawPageLegends(int backgroundHeight, Graphics gfx, InstrumentState instrumentState, Font font)
        {
            //Draw the page legends
            var pageLegendStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap
            };

            const int pageLegendHeight = 15;
            const int pageLegendWidth = 35;
            const int pageLegendSeparation = 15;
            var tacLegendRectangle = new Rectangle(57, backgroundHeight - pageLegendHeight - 5, pageLegendWidth, pageLegendHeight);
            var sysLegendRectangle = new Rectangle(tacLegendRectangle.Right + pageLegendSeparation, tacLegendRectangle.Y, pageLegendWidth, pageLegendHeight);
            var tstLegendRectangle = new Rectangle(sysLegendRectangle.Right + pageLegendSeparation, tacLegendRectangle.Y, pageLegendWidth, pageLegendHeight);

            //draw highlighted TAC legend
            gfx.FillRectangle(Brushes.White, tacLegendRectangle);
            StringRenderer.DrawString(gfx, "TAC", font, Brushes.Black, tacLegendRectangle, pageLegendStringFormat);

            //draw non-highlighted SYS legend
            StringRenderer.DrawString(gfx, "SYS", font, Brushes.White, sysLegendRectangle, pageLegendStringFormat);

            if (instrumentState.RWRTest1 || instrumentState.RWRTest2) //Added Falcas 10-11-2012
            {
                //draw highlighted TST legend
                gfx.FillRectangle(Brushes.White, tstLegendRectangle);
                StringRenderer.DrawString(gfx, "TST", font, Brushes.Black, tstLegendRectangle, pageLegendStringFormat);
            }
            else
            {
                //draw non-highlighted TST legend
                StringRenderer.DrawString(gfx, "TST", font, Brushes.White, tstLegendRectangle, pageLegendStringFormat);
            }
        }
    }
}