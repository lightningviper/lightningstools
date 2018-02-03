using Common.Drawing;

namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface IInnerMapRangeCircleNorthMarkerRenderer
    {
        void DrawNorthMarkerOnInnerMapRangeCircle(Graphics g, Brush mapRingBrush,
            Rectangle innerMapRingBoundingRect, int innerMapRingBoundingRectMiddleX);
    }

    internal class InnerMapRangeCircleNorthMarkerRenderer : IInnerMapRangeCircleNorthMarkerRenderer
    {
        public void DrawNorthMarkerOnInnerMapRangeCircle(Graphics g, Brush mapRingBrush,
            Rectangle innerMapRingBoundingRect, int innerMapRingBoundingRectMiddleX)
        {
            //draw north marker on inner map range circle
            var northMarkerPoints = new PointF[3];
            northMarkerPoints[0] = new PointF(innerMapRingBoundingRectMiddleX, innerMapRingBoundingRect.Top - (15*(innerMapRingBoundingRect.Width*0.01f)));
            northMarkerPoints[1] = new PointF(innerMapRingBoundingRectMiddleX - (12*(innerMapRingBoundingRect.Width*0.01f)),
                innerMapRingBoundingRect.Top + (innerMapRingBoundingRect.Width*0.01f));
            northMarkerPoints[2] = new PointF(innerMapRingBoundingRectMiddleX + (12*(innerMapRingBoundingRect.Width * 0.01f)),
                innerMapRingBoundingRect.Top + (innerMapRingBoundingRect.Width * 0.01f));
            g.FillPolygon(mapRingBrush, northMarkerPoints);
        }
    }
}