using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class FixedAirplaneSymbolRenderer
    {
        internal static float DrawFixedAirplaneSymbol(Graphics g, int width, int height, ref GraphicsState basicState, out float centerY)
        {
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            const float airplaneSymbolBarThickness = 6;
            const float airplaneSymbolWidthEdgeToEdge = 80;
            var centerX = width / 2.0f;
            centerY = height / 2.0f;
            const float sidebarSpaceFromCenter = 10.0f;

            var lhsTopLeftX = centerX - airplaneSymbolWidthEdgeToEdge / 2.0f - sidebarSpaceFromCenter;
            var lhsTopLeftY = centerY - airplaneSymbolBarThickness / 2.0f;
            var lhsTopRightX = centerX - sidebarSpaceFromCenter;
            var lhsTopRightY = lhsTopLeftY;
            var lhsBottomRightX = lhsTopRightX;
            var lhsBottomRightY = lhsTopRightY + airplaneSymbolBarThickness * 2;
            var lhsBottomCenterX = lhsBottomRightX - airplaneSymbolBarThickness;
            var lhsBottomCenterY = lhsBottomRightY;
            var lhsMiddleCenterX = lhsBottomCenterX;
            var lhsMiddleCenterY = lhsBottomCenterY - airplaneSymbolBarThickness;
            var lhsMiddleLeftX = lhsTopLeftX;
            var lhsMiddleLeftY = lhsMiddleCenterY;

            var rhsTopLeftX = centerX + sidebarSpaceFromCenter;
            var rhsTopLeftY = centerY - airplaneSymbolBarThickness / 2.0f;
            var rhsTopRightX = centerX + sidebarSpaceFromCenter + airplaneSymbolWidthEdgeToEdge / 2.0f;
            var rhsTopRightY = rhsTopLeftY;
            var rhsMiddleRightX = rhsTopRightX;
            var rhsMiddleRightY = rhsTopRightY + airplaneSymbolBarThickness;
            var rhsMiddleCenterX = rhsTopLeftX + airplaneSymbolBarThickness;
            var rhsMiddleCenterY = rhsMiddleRightY;
            var rhsBottomCenterX = rhsMiddleCenterX;
            var rhsBottomCenterY = rhsTopLeftY + airplaneSymbolBarThickness * 2;
            var rhsBottomLeftX = rhsTopLeftX;
            var rhsBottomLeftY = rhsBottomCenterY;

            var airplaneSymbolOutlineColor = Color.White;
            var airplaneSymbolOutlinePen = new Pen(airplaneSymbolOutlineColor) {Width = 2};
            var airplaneSymbolInsideBrush = Brushes.Black;
            var airplaneSymbolLhsPoints = new[]
            {
                new PointF(lhsTopLeftX, lhsTopLeftY), //LHS top-left
                new PointF(lhsTopRightX, lhsTopRightY), //LHS top-right
                new PointF(lhsBottomRightX, lhsBottomRightY), //LHS bottom-right
                new PointF(lhsBottomCenterX, lhsBottomCenterY), //LHS bottom-center
                new PointF(lhsMiddleCenterX, lhsMiddleCenterY), //LHS middle-center
                new PointF(lhsMiddleLeftX, lhsMiddleLeftY) //LHS middle-left
            };
            g.FillPolygon(airplaneSymbolInsideBrush, airplaneSymbolLhsPoints);
            g.DrawPolygon(airplaneSymbolOutlinePen, airplaneSymbolLhsPoints);

            var airplaneSymbolRhsPoints = new[]
            {
                new PointF(rhsTopLeftX, rhsTopLeftY), //rhs top-left
                new PointF(rhsTopRightX, rhsTopRightY), //rhs top-right
                new PointF(rhsMiddleRightX, rhsMiddleRightY), //rhs middle-right
                new PointF(rhsMiddleCenterX, rhsMiddleCenterY), //rhs middle-center
                new PointF(rhsBottomCenterX, rhsBottomCenterY), //rhs bottom-center
                new PointF(rhsBottomLeftX, rhsBottomLeftY) //rhs bottom-right
            };

            g.FillPolygon(airplaneSymbolInsideBrush, airplaneSymbolRhsPoints);
            g.DrawPolygon(airplaneSymbolOutlinePen, airplaneSymbolRhsPoints);

            var airplaneSymbolCenterRectangle = new RectangleF(
                centerX - airplaneSymbolBarThickness / 2.0f, centerY - airplaneSymbolBarThickness / 2.0f, airplaneSymbolBarThickness, airplaneSymbolBarThickness);
            g.FillRectangle(airplaneSymbolInsideBrush, airplaneSymbolCenterRectangle);
            g.DrawRectangleFast(
                airplaneSymbolOutlinePen,
                new Rectangle((int) airplaneSymbolCenterRectangle.X, (int) airplaneSymbolCenterRectangle.Y, (int) airplaneSymbolCenterRectangle.Width, (int) airplaneSymbolCenterRectangle.Height));

            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            return centerX;
        }
    }
}