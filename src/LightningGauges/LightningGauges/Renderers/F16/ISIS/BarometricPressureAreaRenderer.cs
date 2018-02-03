using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class BarometricPressureAreaRenderer
    {
        private static ThreadLocal<Font> _baroStringFont;
        private static ThreadLocal<Font> _unitsFont;

        internal static void DrawBarometricPressureArea(
            Graphics gfx, ref GraphicsState basicState, RectangleF topRectangle, InstrumentState instrumentState, Options options, PrivateFontCollection fonts)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            const int barometricPressureAreaWidth = 65;
            var barometricPressureStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Far,
                Trimming = StringTrimming.None
            };

            var pressure = instrumentState.BarometricPressure;

            var barometricPressureRectangle = new RectangleF(topRectangle.Width - barometricPressureAreaWidth - 15, 20, barometricPressureAreaWidth, topRectangle.Height - 20);
            var barometricPressureBrush = Brushes.White;

            string baroString = null;
            string units = null;
            if (options.PressureAltitudeUnits == PressureUnits.InchesOfMercury)
            {
                //baroString = string.Format("{0:#0.00}", pressure);
                baroString = $"{pressure / 100:#0.00}";
                units = "in";
                barometricPressureRectangle = new RectangleF(topRectangle.Width - barometricPressureAreaWidth - 15, 20, barometricPressureAreaWidth, topRectangle.Height - 20);
            }
            else if (options.PressureAltitudeUnits == PressureUnits.Millibars)
            {
                baroString = $"{pressure:###0}";
                units = "hPa";
                barometricPressureRectangle = new RectangleF(topRectangle.Width - barometricPressureAreaWidth - 25, 20, barometricPressureAreaWidth, topRectangle.Height - 20);
            }
            if (_baroStringFont == null) _baroStringFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], 20, FontStyle.Regular, GraphicsUnit.Point));
            gfx.DrawStringFast(baroString, _baroStringFont.Value, barometricPressureBrush, barometricPressureRectangle, barometricPressureStringFormat);

            if (_unitsFont == null) _unitsFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], 8, FontStyle.Regular, GraphicsUnit.Point));
            var unitsRectangle = new RectangleF(topRectangle.Width - 22, 18, 15, topRectangle.Height - 20);
            gfx.DrawStringFast(units, _unitsFont.Value, barometricPressureBrush, unitsRectangle, barometricPressureStringFormat);

            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }
    }
}