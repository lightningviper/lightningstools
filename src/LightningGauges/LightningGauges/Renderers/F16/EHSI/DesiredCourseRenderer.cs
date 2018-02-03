using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class DesiredCourseRenderer
    {
        private static ThreadLocal<Font> _digitsFont;
        private static ThreadLocal<Font> _crsFont;

        internal static void DrawDesiredCourse(Graphics g, RectangleF outerBounds, PrivateFontCollection fonts, InstrumentState instrumentState, Options options)
        {
            var fontFamily = fonts.Families[0];
            if (_digitsFont == null) _digitsFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 27.5f, FontStyle.Bold, GraphicsUnit.Point));
            if (_crsFont == null) _crsFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 20, FontStyle.Bold, GraphicsUnit.Point));
            var desiredCourseDigitStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var crsStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var initialState = g.Save();

            var basicState = g.Save();

            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            var desiredCourse = $"{instrumentState.DesiredCourseDegrees:000.0}";
            var hundredsDigit = desiredCourse.Substring(0, 1);
            var tensDigit = desiredCourse.Substring(1, 1);
            var onesDigit = desiredCourse.Substring(2, 1);

            const float digitWidth = 22;
            const float digitHeight = 32;
            const float digitSeparationPixels = -2;
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            const float margin = 8;
            var hundredsRect = new RectangleF(outerBounds.Width - margin - (digitWidth + digitSeparationPixels) * 3, margin, digitWidth, digitHeight);
            var tensRect = new RectangleF(hundredsRect.X + digitWidth + digitSeparationPixels, hundredsRect.Y, digitWidth, digitHeight);
            var onesRect = new RectangleF(tensRect.X + digitWidth + digitSeparationPixels, tensRect.Y, digitWidth, digitHeight);

            g.DrawStringFast(hundredsDigit, _digitsFont.Value, Brushes.White, hundredsRect, desiredCourseDigitStringFormat);
            g.DrawStringFast(tensDigit, _digitsFont.Value, Brushes.White, tensRect, desiredCourseDigitStringFormat);
            g.DrawStringFast(onesDigit, _digitsFont.Value, Brushes.White, onesRect, desiredCourseDigitStringFormat);

            var crsRect = new RectangleF(hundredsRect.X, 45, (digitWidth + digitSeparationPixels) * 3, 20);
            g.DrawStringFast("CRS", _crsFont.Value, Brushes.White, crsRect, crsStringFormat);

            GraphicsUtil.RestoreGraphicsState(g, ref initialState);
        }
    }
}