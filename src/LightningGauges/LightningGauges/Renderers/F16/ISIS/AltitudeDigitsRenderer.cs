using System;
using System.Collections.Generic;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class AltitudeDigitsRenderer
    {
        private static readonly ThreadLocal<Dictionary<StringAlignment, StringFormat>> DigitStringFormats = new ThreadLocal<Dictionary<StringAlignment, StringFormat>>(()=>new Dictionary<StringAlignment, StringFormat>());
        private static readonly ThreadLocal<Brush> DigitBrush = new ThreadLocal<Brush>(()=>Brushes.White);
        private static ThreadLocal<Font> _digitFont;

        internal static void DrawAltitudeDigits(
            Graphics g, float digit, RectangleF layoutRectangle, RectangleF clipRectangle, float pointSize, bool goByTwenty, bool cyclical, StringAlignment alignment, PrivateFontCollection fonts)
        {
            if (_digitFont == null) _digitFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], pointSize, FontStyle.Regular, GraphicsUnit.Point));
            if (!DigitStringFormats.Value.ContainsKey(alignment))
            {
                DigitStringFormats.Value.Add(
                    alignment,
                    new StringFormat
                    {
                        Alignment = alignment,
                        FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.None
                    });
            }
            var digitSF = DigitStringFormats.Value[alignment];

            var initialClip = g.Clip;
            var initialState = g.Save();

            g.SetClip(clipRectangle);
            var basicState = g.Save();
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);

            const float digitSpacing = 0;
            if (cyclical)
            {
                for (var i = -1; i <= 11; i++)
                {
                    int thisDigit;
                    if (i >= 0) { thisDigit = i % 10; }
                    else { thisDigit = 10 - Math.Abs(i) % 10; }

                    var toDraw = $"{thisDigit:#0}";
                    if (goByTwenty)
                    {
                        toDraw = $"{thisDigit * 20 % 100:00}";
                        if (toDraw == "100") toDraw = "00";
                    }
                    var digitSize = g.MeasureString(toDraw, _digitFont.Value);
                    var layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y - i * (digitSize.Height + digitSpacing), layoutRectangle.Width, digitSize.Height);
                    layoutRectangle2.Offset(0, (digitSize.Height + digitSpacing) * digit);
                    g.DrawStringFast(toDraw, _digitFont.Value, DigitBrush.Value, layoutRectangle2, digitSF);
                }
            }
            else
            {
                var thisDigit = (int) Math.Floor(digit);
                var toDraw = $"{thisDigit:0}";
                if (toDraw.Length > 1) toDraw = toDraw.Substring(0, 1);
                if (goByTwenty) toDraw = $"{thisDigit * 20:00}";
                var digitSize = g.MeasureString(toDraw, _digitFont.Value);
                var layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y - digit * (digitSize.Height + digitSpacing), layoutRectangle.Width, digitSize.Height);
                layoutRectangle2.Offset(0, (digitSize.Height + digitSpacing) * digit);
                g.DrawStringFast(toDraw, _digitFont.Value, DigitBrush.Value, layoutRectangle2, digitSF);
            }

            GraphicsUtil.RestoreGraphicsState(g, ref initialState);
            g.Clip = initialClip;
        }
    }
}