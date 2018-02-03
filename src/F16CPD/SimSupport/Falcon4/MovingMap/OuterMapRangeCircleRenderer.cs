using System;
using Common.Drawing;
using Common.Imaging;

namespace F16CPD.SimSupport.Falcon4.MovingMap
{
    internal interface IOuterMapRangeCircleRenderer
    {
        void DrawOuterMapRangeCircle(Graphics g, Size theaterMapSizePixels,
            int outerMapRingRadiusPixels, Pen mapRingPen);
    }

    internal class OuterMapRangeCircleRenderer : IOuterMapRangeCircleRenderer
    {
        public void DrawOuterMapRangeCircle(Graphics g, Size theaterMapSizePixels,
            int outerMapRingRadiusPixels,  Pen mapRingPen)
        {
            var mapRingLineLength = outerMapRingRadiusPixels/4;
            //rotate 45 degrees before drawing outer map range circle
            var preRotate = g.Transform;
            //capture current rotation so we can set it back before drawing inner map range circle
            g.TranslateTransform(theaterMapSizePixels.Width / 2.0f, theaterMapSizePixels.Height / 2.0f);
            g.RotateTransform(-45);
            g.TranslateTransform(-theaterMapSizePixels.Width / 2.0f, -theaterMapSizePixels.Height / 2.0f);

            var outerMapRingBoundingRect =
                new Rectangle(((theaterMapSizePixels.Width - (outerMapRingRadiusPixels * 2)) / 2),
                    ((theaterMapSizePixels.Height - (outerMapRingRadiusPixels * 2)) / 2),
                    outerMapRingRadiusPixels * 2, outerMapRingRadiusPixels * 2);
            g.DrawEllipse(mapRingPen, outerMapRingBoundingRect);
            var outerMapRingBoundingRectMiddleX = outerMapRingBoundingRect.X +
                                                  (int) (Math.Floor(outerMapRingBoundingRect.Width/(float) 2));
            var outerMapRingBoundingRectMiddleY = outerMapRingBoundingRect.Y +
                                                  (int) (Math.Floor(outerMapRingBoundingRect.Height/(float) 2));

            g.DrawLineFast(mapRingPen, new Point(outerMapRingBoundingRectMiddleX, outerMapRingBoundingRect.Top),
                new Point(outerMapRingBoundingRectMiddleX,
                    outerMapRingBoundingRect.Top + mapRingLineLength));
            g.DrawLineFast(mapRingPen, new Point(outerMapRingBoundingRect.X, outerMapRingBoundingRectMiddleY),
                new Point(outerMapRingBoundingRect.X + mapRingLineLength, outerMapRingBoundingRectMiddleY));
            g.DrawLineFast(mapRingPen,
                new Point(outerMapRingBoundingRect.X + outerMapRingBoundingRect.Width,
                    outerMapRingBoundingRectMiddleY),
                new Point(
                    outerMapRingBoundingRect.X + outerMapRingBoundingRect.Width - mapRingLineLength,
                    outerMapRingBoundingRectMiddleY));
            g.DrawLineFast(mapRingPen, new Point(outerMapRingBoundingRectMiddleX, outerMapRingBoundingRect.Bottom),
                new Point(outerMapRingBoundingRectMiddleX,
                    outerMapRingBoundingRect.Bottom - mapRingLineLength));

            //set rotation back before drawing inner map range circle
            g.Transform = preRotate;
        }
    }
}