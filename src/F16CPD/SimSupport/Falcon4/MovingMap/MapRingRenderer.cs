using Common.Drawing;

namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface IMapRingRenderer
    {
        void DrawMapRing(Graphics g, Size theaterMapSizePixels, int outerMapRingRadiusPixels,
            float magneticHeadingInDecimalDegrees);
    }

    internal class MapRingRenderer : IMapRingRenderer
    {
        private readonly IOuterMapRangeCircleRenderer _outerMapRangeCircleRenderer;
        private readonly IInnerMapRangeCircleRenderer _innerMapRangeCircleRenderer;
        private readonly IInnerMapRangeCircleNorthMarkerRenderer _innerMapRangeCircleNorthMarkerRenderer;

        public MapRingRenderer(
            IOuterMapRangeCircleRenderer outerMapRangeCircleRenderer = null,
            IInnerMapRangeCircleRenderer innerMapRangeCircleRenderer = null,
            IInnerMapRangeCircleNorthMarkerRenderer innerMapRangeCircleNorthMarkerRenderer = null)
        {
            _outerMapRangeCircleRenderer = outerMapRangeCircleRenderer ?? new OuterMapRangeCircleRenderer();
            _innerMapRangeCircleRenderer = innerMapRangeCircleRenderer ?? new InnerMapRangeCircleRenderer();
            _innerMapRangeCircleNorthMarkerRenderer = innerMapRangeCircleNorthMarkerRenderer ?? new InnerMapRangeCircleNorthMarkerRenderer();
        }

        public void DrawMapRing(Graphics g,Size theaterMapSizePixels, int outerMapRingRadiusPixels,
            float magneticHeadingInDecimalDegrees)
        {
            var mapRingPen = new Pen(Color.Magenta);
            var mapRingBrush = new SolidBrush(Color.Magenta);
            mapRingPen.Width = 4;

            var originalGTransform = g.Transform;

            g.TranslateTransform(theaterMapSizePixels.Width / 2.0f, theaterMapSizePixels.Height / 2.0f);
            g.RotateTransform(-magneticHeadingInDecimalDegrees);
            g.TranslateTransform(-theaterMapSizePixels.Width / 2.0f, -theaterMapSizePixels.Height / 2.0f);

            _outerMapRangeCircleRenderer.DrawOuterMapRangeCircle(g, theaterMapSizePixels, outerMapRingRadiusPixels, mapRingPen);

            Rectangle innerMapRingBoundingRect;
            int innerMapRingBoundingRectMiddleX;
            _innerMapRangeCircleRenderer.DrawInnerMapRangeCircle(g, theaterMapSizePixels, mapRingPen, 
                outerMapRingRadiusPixels/2, out innerMapRingBoundingRect, out innerMapRingBoundingRectMiddleX);
            _innerMapRangeCircleNorthMarkerRenderer.DrawNorthMarkerOnInnerMapRangeCircle(g, mapRingBrush, innerMapRingBoundingRect,
                innerMapRingBoundingRectMiddleX);
            g.Transform = originalGTransform;
        }
    }
}