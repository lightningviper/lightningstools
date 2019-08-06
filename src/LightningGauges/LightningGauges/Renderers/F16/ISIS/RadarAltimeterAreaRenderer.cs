using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class RadarAltimeterAreaRenderer
    {
        private static ThreadLocal<Font> _raltFont;

        internal static void DrawRadarAltimeterArea(Graphics gfx, ref GraphicsState basicState, RectangleF topRectangle, InstrumentState instrumentState, Options options, PrivateFontCollection fonts)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
            const float raltRectangleWidth = 80;
            var raltRectangle = new RectangleF(topRectangle.Width / 2.0f - raltRectangleWidth / 2.0f, 10, raltRectangleWidth, topRectangle.Height - 10);
            var raltRectanglePen = Pens.White;
            raltRectangle.Offset(-5, 0);
            gfx.DrawRectangleFast(raltRectanglePen, raltRectangle.X, raltRectangle.Y, raltRectangle.Width, raltRectangle.Height);

            var raltStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var ralt = Math.Floor(instrumentState.RadarAltitudeAGL /10.0f) * 10.0f;
            var raltString = instrumentState.RadarAltitudeValid ?  $"{ralt:#####0}" : "";
            var raltColor = Color.FromArgb(183, 243, 244);
            Brush raltBrush = new SolidBrush(raltColor);
            float fontSize = 20;

            if (instrumentState.RadarAltitudeValid)
            {
                if (!raltString.StartsWith("-", System.StringComparison.Ordinal) && raltString.Length > 4 || raltString.StartsWith("-") && raltString.Length > 5) fontSize = 18;
                if (!raltString.StartsWith("-") && raltString.Length > 5 || raltString.StartsWith("-") && raltString.Length > 6) fontSize = 15;
                if (options.RadarAltitudeUnits == AltitudeUnits.Meters) { raltString += "m"; }
                else { raltString += "ft"; }
            }
            if (_raltFont == null) _raltFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], fontSize, FontStyle.Regular, GraphicsUnit.Point));
            gfx.DrawStringFast(raltString, _raltFont.Value, raltBrush, raltRectangle, raltStringFormat);
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }
    }
}