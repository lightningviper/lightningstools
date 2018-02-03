using Common.Drawing;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class DesiredHeadingBugRenderer
    {
        internal static void DrawDesiredHeadingBug(Graphics g, RectangleF outerBounds, InstrumentState instrumentState)
        {
            var basicState = g.Save();
            g.TranslateTransform(outerBounds.Width / 2.0f, outerBounds.Height / 2.0f);
            g.RotateTransform(-instrumentState.MagneticHeadingDegrees);
            g.RotateTransform(instrumentState.DesiredHeadingDegrees);
            g.TranslateTransform(-outerBounds.Width / 2.0f, -outerBounds.Height / 2.0f);

            const float headingBugSquareSize = 20;
            const float headingBugGapBetweenSquares = 5;
            const float headingBugSquareTop = 18;
            var centerX = outerBounds.X + outerBounds.Width / 2.0f;
            var leftHeadingBugSquareLocation = new PointF(centerX - headingBugSquareSize - headingBugGapBetweenSquares / 2.0f, headingBugSquareTop);
            var rightHeadingBugSquareLocation = new PointF(centerX + headingBugGapBetweenSquares / 2.0f, headingBugSquareTop);
            var headingBugLeftSquare = new RectangleF(leftHeadingBugSquareLocation, new SizeF(headingBugSquareSize, headingBugSquareSize));
            var headingBugRightSquare = new RectangleF(rightHeadingBugSquareLocation, new SizeF(headingBugSquareSize, headingBugSquareSize));
            var headingBugColor = Color.FromArgb(248, 238, 153);
            using (var headingBugBrush = new SolidBrush(headingBugColor))
            {
                g.FillRectangle(headingBugBrush, headingBugLeftSquare);
                g.FillRectangle(headingBugBrush, headingBugRightSquare);
            }
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
        }
    }
}