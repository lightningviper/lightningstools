using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class AirspeedTapeRenderer
    {
        private static readonly ThreadLocal<StringFormat> AirspeedDigitFormat = new ThreadLocal<StringFormat>(()=> new StringFormat
        {
            Alignment = StringAlignment.Far,
            FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        });

        private static readonly Color AirspeedDigitColor = Color.White;
        private static readonly ThreadLocal<Brush> AirspeedDigitBrush = new ThreadLocal<Brush>(()=> new SolidBrush(AirspeedDigitColor));
        private static readonly ThreadLocal<Pen> AirspeedDigitPen = new ThreadLocal<Pen>(()=>new Pen(AirspeedDigitColor) {Width = 2});
        private static readonly Color BackgroundColor = Color.FromArgb(117, 123, 121);
        private static readonly ThreadLocal<Brush> BackgroundBrush = new ThreadLocal<Brush>(()=>new SolidBrush(BackgroundColor));
        private static ThreadLocal<Font> _airspeedDigitFont;

        internal static void DrawAirspeedTape(Graphics gfx, ref GraphicsState basicState, int height, InstrumentState instrumentState, PrivateFontCollection fonts)
        {
            if (_airspeedDigitFont == null) _airspeedDigitFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], 22, FontStyle.Regular, GraphicsUnit.Point));
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            var airspeedKnots = instrumentState.AirspeedKnots;
            const float airspeedTapeWidth = 42;
            DrawAirspeedTape(gfx, new SizeF(airspeedTapeWidth, height), airspeedKnots, fonts);
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }

        private static void DrawAirspeedTape(Graphics g, SizeF size, float airspeedKnots, PrivateFontCollection fonts)
        {
            //draw the background
            g.FillRectangle(BackgroundBrush.Value, new Rectangle(0, 0, (int) size.Width, (int) size.Height));
            g.DrawLineFast(AirspeedDigitPen.Value, new PointF(size.Width, 0), new PointF(size.Width, size.Height));

            var originalTransform = g.Transform;
            var pixelsPerKnot = size.Height / 2.0f / 70.0f;
            g.TranslateTransform(0, pixelsPerKnot * airspeedKnots);

            DrawAirspeedTapeDigits(g, size, airspeedKnots, pixelsPerKnot);
            g.Transform = originalTransform;
            //calculate digits
            float hundreds = (int) Math.Floor(airspeedKnots / 100.0f % 10);
            float tens = (int) Math.Floor(airspeedKnots / 10.0f % 10);
            var ones = airspeedKnots % 10;

            if (ones > 9) tens += ones - 9;
            if (tens > 9) hundreds += tens - 9;

            const float airspeedBoxHeight = 35;
            var airspeedBoxWidth = size.Width + 5;
            const float airspeedDigitFontSize = 22;
            var outerRectangle = new RectangleF(0, size.Height / 2.0f - airspeedBoxHeight / 2.0f, airspeedBoxWidth, airspeedBoxHeight);
            g.FillRectangle(Brushes.Black, outerRectangle);
            g.DrawRectangleFast(AirspeedDigitPen.Value, (int) outerRectangle.X, (int) outerRectangle.Y, (int) outerRectangle.Width, (int) outerRectangle.Height);
            var onesRectangle = new RectangleF(airspeedBoxWidth - airspeedBoxWidth / 3.0f, size.Height / 2.0f - airspeedBoxHeight / 2.0f, airspeedBoxWidth / 3.0f, airspeedBoxHeight);
            onesRectangle.Offset(outerRectangle.X, -10);

            AirspeedDigitsRenderer.DrawAirspeedDigits(g, ones, onesRectangle, outerRectangle, airspeedDigitFontSize, true, fonts);

            var tensRectangle = new RectangleF(airspeedBoxWidth - airspeedBoxWidth / 3.0f * 2, size.Height / 2.0f - airspeedBoxHeight / 2.0f, airspeedBoxWidth / 3.0f, airspeedBoxHeight);
            tensRectangle.Offset(outerRectangle.X, -10);
            if (airspeedKnots >= 10) AirspeedDigitsRenderer.DrawAirspeedDigits(g, tens, tensRectangle, outerRectangle, airspeedDigitFontSize, true, fonts);

            var hundredsRectangle = new RectangleF(airspeedBoxWidth - airspeedBoxWidth / 3.0f * 3, size.Height / 2.0f - airspeedBoxHeight / 2.0f, airspeedBoxWidth / 3.0f, airspeedBoxHeight);
            hundredsRectangle.Offset(outerRectangle.X + 1, -10);
            if (airspeedKnots >= 100) AirspeedDigitsRenderer.DrawAirspeedDigits(g, hundreds, hundredsRectangle, outerRectangle, airspeedDigitFontSize, true, fonts);
        }

        private static void DrawAirspeedTapeDigits(Graphics g, SizeF size, float airspeedKnots, float pixelsPerKnot)
        {
            for (var i = 0; i < 2000; i += 10)
            {
                if (i < airspeedKnots - 100 || i > airspeedKnots + 50) continue;

                if (i % 20 == 0)
                {
                    var toDisplay = $"{i:####0}";
                    var toDisplaySize = g.MeasureString(toDisplay, _airspeedDigitFont.Value);
                    const float x = 3;
                    var y = -i * pixelsPerKnot - toDisplaySize.Height / 2.0f + size.Height / 2.0f;
                    var layoutRect = new RectangleF(x, y, size.Width, toDisplaySize.Height);
                    g.DrawStringFast(toDisplay, _airspeedDigitFont.Value, AirspeedDigitBrush.Value, layoutRect, AirspeedDigitFormat.Value);
                }
                else if (i % 10 == 0)
                {
                    const int lineWidth = 15;
                    var x = size.Width - lineWidth;
                    var y = -i * pixelsPerKnot + size.Height / 2.0f;
                    g.DrawLineFast(AirspeedDigitPen.Value, new PointF(x, y), new PointF(size.Width, y));
                }
            }
        }
    }
}