using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class AltitudeTapeRenderer
    {
        private static ThreadLocal<Font> _altitudeDigitFontSmall;
        private static ThreadLocal<Font> _altitudeDigitFontLarge;

        private static readonly ThreadLocal<StringFormat> AltitudeDigitFormat = new ThreadLocal<StringFormat>(()=>new StringFormat
        {
            Alignment = StringAlignment.Far,
            FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        });

        private static readonly Color AltitudeDigitColor = Color.White;
        private static readonly ThreadLocal<Brush> AltitudeDigitBrush = new ThreadLocal<Brush>(()=>new SolidBrush(AltitudeDigitColor));
        private static readonly ThreadLocal<Pen> AltitudeDigitPen = new ThreadLocal<Pen>(()=>new Pen(AltitudeDigitColor) {Width = 2});
        private static readonly Color BackgroundColor = Color.FromArgb(117, 123, 121);
        private static readonly ThreadLocal<Brush> BackgroundBrush = new ThreadLocal<Brush>(()=>new SolidBrush(BackgroundColor));

        internal static void DrawAltitudeTape(Graphics gfx, ref GraphicsState basicState, int width, int height, InstrumentState instrumentState, PrivateFontCollection fonts)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            var altitudeMsl = instrumentState.IndicatedAltitudeFeetMSL;
            const float altitudeTapeWidth = 55;
            gfx.TranslateTransform(width - altitudeTapeWidth, 0);
            DrawAltitudeTape(gfx, new SizeF(altitudeTapeWidth, height), altitudeMsl, fonts);
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }

        private static void DrawAltitudeTape(Graphics g, SizeF size, float altitudeFeetMSL, PrivateFontCollection fonts)
        {
            if (_altitudeDigitFontSmall == null) _altitudeDigitFontSmall = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], 16, FontStyle.Regular, GraphicsUnit.Point));
            if (_altitudeDigitFontLarge == null) _altitudeDigitFontLarge = new ThreadLocal<Font>(() => new Font(fonts.Families[0], 22, FontStyle.Regular, GraphicsUnit.Point));

            var absAltitudeFeetMSL = Math.Abs(altitudeFeetMSL);
            var originalTransform = g.Transform;

            //draw the background
            g.FillRectangle(BackgroundBrush.Value, new Rectangle(0, 0, (int) size.Width, (int) size.Height));
            g.DrawLineFast(AltitudeDigitPen.Value, new PointF(0, 43), new PointF(size.Width, 43));
            g.DrawLineFast(AltitudeDigitPen.Value, new PointF(0, 0), new PointF(0, size.Height));
            var pixelsPerFoot = size.Height / 2.0f / 550.0f;
            g.TranslateTransform(0, pixelsPerFoot * altitudeFeetMSL);
            for (var i = -10000; i < 100000; i += 100)
            {
                if (Math.Abs(i) < absAltitudeFeetMSL - 550 || Math.Abs(i) > absAltitudeFeetMSL + 550) continue;

                if (Math.Abs(i) % 200 == 0)
                {
                    var altitudeString = $"{Math.Abs(i):00000}";
                    var hundredsString = altitudeString.Substring(2, 3);
                    var thousandsString = altitudeString.Substring(0, 2);
                    while (thousandsString.StartsWith("0")) thousandsString = thousandsString.Substring(1, thousandsString.Length - 1);

                    if (i < 0) thousandsString = "-" + thousandsString;

                    var hundredsDisplaySize = g.MeasureString(hundredsString, _altitudeDigitFontSmall.Value, size, AltitudeDigitFormat.Value);
                    const float offsetBoth = 20;
                    const float offsetHundreds = 2;
                    const float offsetThousands = -6;
                    const float x = offsetBoth + offsetHundreds;
                    var y = -i * pixelsPerFoot - hundredsDisplaySize.Height / 2.0f + size.Height / 2.0f;
                    var layoutRect = new RectangleF(x, y, hundredsDisplaySize.Width, hundredsDisplaySize.Height);
                    g.DrawStringFast(hundredsString, _altitudeDigitFontSmall.Value, AltitudeDigitBrush.Value, layoutRect, AltitudeDigitFormat.Value);

                    var thousandsDisplaySize = g.MeasureString(thousandsString, _altitudeDigitFontLarge.Value, size, AltitudeDigitFormat.Value);
                    y = -i * pixelsPerFoot - thousandsDisplaySize.Height / 2.0f + size.Height / 2.0f;
                    var layoutRect2 = new RectangleF(
                        size.Width - hundredsDisplaySize.Width - thousandsDisplaySize.Width + offsetBoth + offsetThousands, y, thousandsDisplaySize.Width, thousandsDisplaySize.Height);
                    g.DrawStringFast(thousandsString, _altitudeDigitFontLarge.Value, AltitudeDigitBrush.Value, layoutRect2, AltitudeDigitFormat.Value);
                }
                else if (Math.Abs(i) % 100 == 0)
                {
                    const int lineWidth = 15;
                    var y = -i * pixelsPerFoot + size.Height / 2.0f;
                    g.DrawLineFast(AltitudeDigitPen.Value, new PointF(0, y), new PointF(lineWidth, y));
                }
            }

            g.Transform = originalTransform;
            //calculate digits
            float tenThousands = (int) Math.Floor(absAltitudeFeetMSL / 10000.0f % 10);
            float thousands = (int) Math.Floor(absAltitudeFeetMSL / 1000.0f % 10);
            float hundreds = (int) Math.Floor(absAltitudeFeetMSL / 100.0f) % 10;
            var twenties = absAltitudeFeetMSL / 20.0f % 5;
            if (twenties > 4) hundreds += twenties - 4;
            if (hundreds > 9) thousands += hundreds - 9;
            if (thousands > 9) tenThousands += thousands - 9;

            const float altitudeBoxHeight = 35;
            var altitudeBoxWidth = size.Width + 5;
            const float altitudeDigitFontSize = 22;
            const float altitudeDigitFontSizeSmall = 16;
            var outerRectangle = new RectangleF(-5, size.Height / 2.0f - altitudeBoxHeight / 2.0f, altitudeBoxWidth, altitudeBoxHeight);
            g.FillRectangle(Brushes.Black, outerRectangle);
            g.DrawRectangleFast(AltitudeDigitPen.Value, (int) outerRectangle.X, (int) outerRectangle.Y, (int) outerRectangle.Width, (int) outerRectangle.Height);

            var twentiesRectangle = new RectangleF(altitudeBoxWidth - altitudeBoxWidth / 6.0f * 2, size.Height / 2.0f - altitudeBoxHeight / 2.0f, altitudeBoxWidth / 6.0f * 2, altitudeBoxHeight);
            twentiesRectangle.Offset(outerRectangle.X, 4);

            AltitudeDigitsRenderer.DrawAltitudeDigits(g, twenties, twentiesRectangle, outerRectangle, altitudeDigitFontSizeSmall, true, true, StringAlignment.Center, fonts);

            var hundredsRectangle = new RectangleF(altitudeBoxWidth - altitudeBoxWidth / 6.0f * 3, size.Height / 2.0f - altitudeBoxHeight / 2.0f, altitudeBoxWidth / 6.0f, altitudeBoxHeight);
            hundredsRectangle.Offset(outerRectangle.X - 1, 4);

            AltitudeDigitsRenderer.DrawAltitudeDigits(g, hundreds, hundredsRectangle, outerRectangle, altitudeDigitFontSizeSmall, false, true, StringAlignment.Center, fonts);

            var thousandsRectangle = new RectangleF(
                altitudeBoxWidth - altitudeBoxWidth / 6.0f * 4.5f, size.Height / 2.0f - altitudeBoxHeight / 2.0f, altitudeBoxWidth / 6.0f * 1.5f, altitudeBoxHeight);
            thousandsRectangle.Offset(outerRectangle.X - 5, 0);
            AltitudeDigitsRenderer.DrawAltitudeDigits(g, thousands, thousandsRectangle, outerRectangle, altitudeDigitFontSize, false, true, StringAlignment.Near, fonts);

            var tenThousandsRectangle = new RectangleF(
                altitudeBoxWidth - altitudeBoxWidth / 6.0f * 6, size.Height / 2.0f - altitudeBoxHeight / 2.0f, altitudeBoxWidth / 6.0f * 1.5f, altitudeBoxHeight);
            tenThousandsRectangle.Offset(outerRectangle.X - 4, 0);
            if (tenThousands > 0)
            {
                AltitudeDigitsRenderer.DrawAltitudeDigits(g, tenThousands, tenThousandsRectangle, outerRectangle, altitudeDigitFontSize, false, true, StringAlignment.Near, fonts);
            }
            if (altitudeFeetMSL < 0) AltitudeDigitsRenderer.DrawAltitudeDigits(g, -1, tenThousandsRectangle, outerRectangle, altitudeDigitFontSize, false, false, StringAlignment.Near, fonts);
        }
    }
}