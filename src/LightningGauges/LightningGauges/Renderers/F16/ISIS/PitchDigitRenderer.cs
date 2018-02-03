using System;
using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class PitchDigitRenderer
    {
        internal static void DrawPitchDigits(Graphics g, float pitchLadderWidth, Font pitchDigitFont, Brush pitchDigitBrush, StringFormat pitchDigitFormat, float i, float lineWidth, float y)
        {
            var toDisplayVal = (int) Math.Abs(i);
            var toDisplayString = $"{toDisplayVal:##}";

            var digitSize = g.MeasureString(toDisplayString, pitchDigitFont);
            var lhsRect = new RectangleF(new PointF(pitchLadderWidth / 2.0f - lineWidth / 2.0f - digitSize.Width, y - digitSize.Height / 2.0f), digitSize);
            g.DrawStringFast(toDisplayString, pitchDigitFont, pitchDigitBrush, lhsRect, pitchDigitFormat);

            var rhsRect = new RectangleF(new PointF(pitchLadderWidth / 2.0f + lineWidth / 2.0f, y - digitSize.Height / 2.0f), digitSize);
            g.DrawStringFast(toDisplayString, pitchDigitFont, pitchDigitBrush, rhsRect, pitchDigitFormat);
        }
    }
}