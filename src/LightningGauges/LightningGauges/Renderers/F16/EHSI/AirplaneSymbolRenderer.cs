using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class AirplaneSymbolRenderer
    {
        internal static void DrawAirplaneSymbol(Graphics g, RectangleF outerBounds)
        {
            var middle = new PointF(outerBounds.X + outerBounds.Width / 2.0f, outerBounds.Y + outerBounds.Height / 2.0f);
            const float airplaneFuselageLength = 45;
            const float airplaneTailWidth = 23;
            const float airplaneWingWidth = 45;
            const float gap = airplaneFuselageLength / 2.0f;

            var symbolPen = new Pen(Color.White) {Width = 3};

            //draw wings
            var wingLeft = new PointF(middle.X - airplaneWingWidth / 2.0f, middle.Y - gap / 2.0f);
            var wingRight = new PointF(middle.X + airplaneWingWidth / 2.0f, middle.Y - gap / 2.0f);
            g.DrawLineFast(symbolPen, wingLeft, wingRight);

            //draw tail
            var tailLeft = new PointF(middle.X - airplaneTailWidth / 2.0f, middle.Y + gap / 2.0f);
            var tailRight = new PointF(middle.X + airplaneTailWidth / 2.0f, middle.Y + gap / 2.0f);
            g.DrawLineFast(symbolPen, tailLeft, tailRight);

            //draw fuselage
            var fuselageTop = new PointF(middle.X, middle.Y - airplaneFuselageLength / 2.0f);
            var fuselageBottom = new PointF(middle.X, middle.Y + airplaneFuselageLength / 2.0f);
            g.DrawLineFast(symbolPen, fuselageTop, fuselageBottom);
        }
    }
}