using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class DistanceToBeaconRenderer
    {
        private static ThreadLocal<Font> _digitsFont;
        private static ThreadLocal<Font> _nmFont;

        internal static void DrawDistanceToBeacon(Graphics g, PrivateFontCollection fonts, InstrumentState instrumentState, Options options)
        {
            var fontFamily = fonts.Families[0];
            if (_digitsFont == null) _digitsFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 27.5f, FontStyle.Bold, GraphicsUnit.Point));
            if (_nmFont == null) _nmFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 20, FontStyle.Bold, GraphicsUnit.Point));
            var distanceDigitStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var nmStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var initialState = g.Save();
            var basicState = g.Save();

            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            var distanceToBeaconString = $"{instrumentState.DistanceToBeaconNauticalMiles:000.0}";
            var hundredsDigit = distanceToBeaconString.Substring(0, 1);
            var tensDigit = distanceToBeaconString.Substring(1, 1);
            var onesDigit = distanceToBeaconString.Substring(2, 1);
            var tenthsDigit = distanceToBeaconString.Substring(4, 1);

            const float digitWidth = 22;
            const float digitHeight = 32;
            const float digitSeparationPixels = -4;
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            var hundredsRect = new RectangleF(12, 8, digitWidth, digitHeight);
            var tensRect = new RectangleF(hundredsRect.X + digitWidth + digitSeparationPixels, hundredsRect.Y, digitWidth, digitHeight);
            var onesRect = new RectangleF(tensRect.X + digitWidth + digitSeparationPixels, tensRect.Y, digitWidth, digitHeight);
            var tenthsRect = new RectangleF(onesRect.X + digitWidth + 4, onesRect.Y, digitWidth, digitHeight);

            g.DrawStringFast(hundredsDigit, _digitsFont.Value, Brushes.White, hundredsRect, distanceDigitStringFormat);
            g.DrawStringFast(tensDigit, _digitsFont.Value, Brushes.White, tensRect, distanceDigitStringFormat);
            g.DrawStringFast(onesDigit, _digitsFont.Value, Brushes.White, onesRect, distanceDigitStringFormat);

            g.FillRectangle(Brushes.White, tenthsRect);
            g.DrawStringFast(tenthsDigit, _digitsFont.Value, Brushes.Black, tenthsRect, distanceDigitStringFormat);

            if (instrumentState.DmeInvalidFlag)
            {
                var dmeInvalidFlagUpperLeft = new PointF(hundredsRect.X, hundredsRect.Y + 8);
                var dmeInvalidFlagSize = new SizeF(tenthsRect.X + tenthsRect.Width - hundredsRect.X, 16);
                var dmeInvalidFlagRect = new RectangleF(dmeInvalidFlagUpperLeft, dmeInvalidFlagSize);
                var redFlagColor = Color.FromArgb(224, 43, 48);

                using (var redFlagBrush = new SolidBrush(redFlagColor)) { g.FillRectangle(redFlagBrush, dmeInvalidFlagRect); }
            }

            var nmRect = new RectangleF(hundredsRect.X, 45, 30, 20);
            g.DrawStringFast("NM", _nmFont.Value, Brushes.White, nmRect, nmStringFormat);

            GraphicsUtil.RestoreGraphicsState(g, ref initialState);
        }
    }
}