using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class HeadingAndCourseAdjustLabelsRenderer
    {
        private static ThreadLocal<Font> _labelFont;

        internal static void DrawHeadingAndCourseAdjustLabels(Graphics g, RectangleF outerBounds, PrivateFontCollection fonts)
        {
            if (_labelFont == null)
            {
                var fontFamily = fonts.Families[0];
                _labelFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 20, FontStyle.Bold, GraphicsUnit.Point));
            }
            var labelStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            const float letterHeight = 20;
            const float letterWidth = 20;
            const float toAddX = -5;
            const float toAddY = -5;
            const float margin = 40;
            //draw HDG label
            var hdgHRect = new RectangleF(margin, outerBounds.Height * 0.82f, letterWidth, letterHeight);
            g.DrawStringFast("H", _labelFont.Value, Brushes.White, hdgHRect, labelStringFormat);

            var hdgDRect = new RectangleF(hdgHRect.X + hdgHRect.Width + toAddX, hdgHRect.Y + hdgHRect.Height + toAddY, letterWidth, letterHeight);
            g.DrawStringFast("D", _labelFont.Value, Brushes.White, hdgDRect, labelStringFormat);

            var hdgGRect = new RectangleF(hdgDRect.X + hdgDRect.Width + toAddX, hdgDRect.Y + hdgDRect.Height + toAddY, letterWidth, letterHeight);
            g.DrawStringFast("G", _labelFont.Value, Brushes.White, hdgGRect, labelStringFormat);

            //draw CRS label
            var crsCRect = new RectangleF(outerBounds.Width - (letterWidth + toAddX) * 3 - hdgHRect.X, hdgGRect.Y, letterWidth, letterHeight);
            g.DrawStringFast("C", _labelFont.Value, Brushes.White, crsCRect, labelStringFormat);

            var crsRRect = new RectangleF(crsCRect.X + crsCRect.Width + toAddX, hdgDRect.Y, letterWidth, letterHeight);
            g.DrawStringFast("R", _labelFont.Value, Brushes.White, crsRRect, labelStringFormat);

            var crsSRect = new RectangleF(crsRRect.X + crsRRect.Width + toAddX, hdgHRect.Y, letterWidth, letterHeight);
            g.DrawStringFast("S", _labelFont.Value, Brushes.White, crsSRect, labelStringFormat);
        }
    }
}