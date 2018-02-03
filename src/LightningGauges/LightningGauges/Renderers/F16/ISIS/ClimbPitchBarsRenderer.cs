using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class ClimbPitchBarsRenderer
    {
        internal static void DrawClimbPitchBars(
            Graphics g, float pitchDegrees, int minorLineWidth, float pitchLadderWidth, float pitchLadderHeight, float pixelsPerDegree, Pen pitchBarPen, Font pitchDigitFont, Brush pitchDigitBrush,
            StringFormat pitchDigitFormat)
        {
            //draw the climb pitch bars
            for (float i = -85; i <= 85; i += 2.5f)
            {
                if (i < pitchDegrees - 15 || i > pitchDegrees + 15.0f) continue;

                float lineWidth = minorLineWidth;
                if (i % 5 == 0) lineWidth *= 2;
                if (i % 10 == 0) lineWidth *= 2;
                if (i == 0) lineWidth = pitchLadderWidth;
                var y = pitchLadderHeight / 2.0f - i * pixelsPerDegree;

                //draw this line
                g.DrawLineFast(pitchBarPen, new PointF(pitchLadderWidth / 2.0f - lineWidth / 2.0f, y), new PointF(pitchLadderWidth / 2.0f + lineWidth / 2.0f, y));

                //draw the pitch digits
                if (i % 10 == 0) PitchDigitRenderer.DrawPitchDigits(g, pitchLadderWidth, pitchDigitFont, pitchDigitBrush, pitchDigitFormat, i, lineWidth, y);
            }
        }
    }
}