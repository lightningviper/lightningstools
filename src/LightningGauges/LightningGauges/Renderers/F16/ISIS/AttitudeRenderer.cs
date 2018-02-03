using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Text;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class AttitudeRenderer
    {
        internal static void DrawAttitude(Graphics g, int width, int height, InstrumentState instrumentState, PrivateFontCollection fonts)
        {
            var basicState = g.Save();
            //draw the pitch ladder
            float rollDegrees;
            var pixelsPerDegreePitch = DrawPitchLadder(g, width, height, out rollDegrees, ref basicState, instrumentState, fonts);

            //draw the fixed airplane symbol
            float centerY;
            var centerX = FixedAirplaneSymbolRenderer.DrawFixedAirplaneSymbol(g, width, height, ref basicState, out centerY);

            //draw the roll angle index marks
            Pen rollIndexPen;
            var radiusFromCenterToBottomOfIndexLine = RollAngleIndexMarksRenderer.DrawRollAngleIndexMarks(g, width, height, pixelsPerDegreePitch, ref basicState, out rollIndexPen);

            //draw zero degree triangle
            var rollTriangleWidthAtBase = centerY - radiusFromCenterToBottomOfIndexLine;
            var center = new PointF(centerX, centerY - radiusFromCenterToBottomOfIndexLine);
            var topLeft = new PointF(centerX - rollTriangleWidthAtBase / 8.0f, center.Y - 6);
            var topRight = new PointF(centerX + rollTriangleWidthAtBase / 8.0f, center.Y - 6);
            if (Math.Abs(rollDegrees) < 0.5f) { g.FillPolygon(Brushes.White, new[] {topLeft, topRight, center}); }
            else { g.DrawPolygon(Pens.White, new[] {topLeft, topRight, center}); }
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);

            //draw sky pointer triangle
            SkyPointerTriangleRenderer.DrawSkyPointerTriangle(g, width, height, basicState, rollDegrees, centerY, radiusFromCenterToBottomOfIndexLine, rollIndexPen);
        }

        private static float DrawPitchLadder(Graphics g, int width, int height, out float rollDegrees, ref GraphicsState basicState, InstrumentState instrumentState, PrivateFontCollection fonts)
        {
            float pitchLadderHeight = width * 4;
            float pitchLadderWidth = height * 4;
            var pixelsPerDegreePitch = pitchLadderHeight / (180.0f + 90);
            var translateX = width / 2.0f - pitchLadderWidth / 2.0f;
            var pitchDegrees = instrumentState.PitchDegrees;
            rollDegrees = instrumentState.RollDegrees;
            var translateY = pixelsPerDegreePitch * pitchDegrees - pitchLadderHeight / 2.0f + height / 2.0f;
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            g.TranslateTransform(width / 2.0f, height / 2.0f);
            g.RotateTransform(-rollDegrees);
            g.TranslateTransform(-(float) width / 2.0f, -height / 2.0f);
            g.TranslateTransform(translateX, translateY);
            PitchLadderRenderer.DrawPitchLadder(g, new RectangleF(0, 0, pitchLadderWidth, pitchLadderHeight), pitchDegrees, fonts);
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            return pixelsPerDegreePitch;
        }
    }
}