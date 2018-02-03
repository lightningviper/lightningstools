using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class CourseDeviationNeedlesRenderer
    {
        internal static void DrawCourseDeviationNeedles(Graphics g, RectangleF outerBounds, InstrumentState instrumentState)
        {
            var initialState = g.Save();
            var redFlagColor = Color.FromArgb(224, 43, 48);
            var needleColor = Color.FromArgb(102, 190, 157);
            using (var redFlagBrush = new SolidBrush(redFlagColor))
            using (var needleBrush = new SolidBrush(needleColor))
            using (var needlePen = new Pen(needleColor))
            {
                const float pointerNeedleThickWidth = 8;
                const float pointerNeedleThinWidth = 3;
                const float maxDeviationX = 110;
                const float dotHeight = 15;
                var dotY = (outerBounds.Height - dotHeight) / 2.0f;
                const float leftInnerDotX = -maxDeviationX / 2.0f - dotHeight / 2.0f;
                const float leftOuterDotX = -maxDeviationX - dotHeight / 2.0f;
                const float rightInnerDotX = maxDeviationX / 2.0f - dotHeight / 2.0f;
                const float rightOuterDotX = maxDeviationX - dotHeight / 2.0f;
                const float topIndicatorLineHeight = 50;
                const float coursePointerTriangleWidth = 30;
                const float coursePointerTriangleHeight = 20;
                const float pointerNeedleThickHeightTop = 40;
                const float cdiNeedleHeight = 198;
                var deviationTranslateX = maxDeviationX
                                          * Math.Sign(instrumentState.CourseDeviationDegrees)
                                          * (Math.Abs(instrumentState.CourseDeviationDegrees) / Math.Abs(instrumentState.CourseDeviationLimitDegrees));
                const float pointerNeedleThickHeightBottom = 60;
                const float bottomIndicatorLineHeight = 50;
                var courseNeedleTopLineTop = new PointF(0, 43);
                var courseNeedleTopLineBottom = new PointF(0, courseNeedleTopLineTop.Y + topIndicatorLineHeight);
                var coursePointerTriangleLeft = new PointF(courseNeedleTopLineBottom.X - coursePointerTriangleWidth / 2.0f, courseNeedleTopLineBottom.Y + coursePointerTriangleHeight);
                var coursePointerTriangleRight = new PointF(courseNeedleTopLineBottom.X + coursePointerTriangleWidth / 2.0f, coursePointerTriangleLeft.Y);
                var coursePointerTriangleTop = new PointF(0, courseNeedleTopLineBottom.Y - 2);
                var pointerNeedleThickTopTop = new PointF(0, coursePointerTriangleLeft.Y);
                var pointerNeedleThickTopBottom = new PointF(0, pointerNeedleThickTopTop.Y + pointerNeedleThickHeightTop);
                var deviationInvalidFlagRect = new RectangleF(new PointF(-80, pointerNeedleThickTopBottom.Y), new SizeF(60, 30));
                var toFromFlagHeight = deviationInvalidFlagRect.Height;
                var toFromFlagWidth = deviationInvalidFlagRect.Height;
                var cdiLineTop = new PointF(0, pointerNeedleThickTopBottom.Y + 2);
                var cdiLineBottom = new PointF(0, cdiLineTop.Y + cdiNeedleHeight);
                var pointerNeedleThickBottomTop = new PointF(0, cdiLineBottom.Y + 2);
                var pointerNeedleThickBottomBottom = new PointF(0, pointerNeedleThickBottomTop.Y + pointerNeedleThickHeightBottom);
                var courseNeedleBottomLineTop = new PointF(0, pointerNeedleThickBottomBottom.Y);
                var courseNeedleBottomLineBottom = new PointF(0, courseNeedleBottomLineTop.Y + bottomIndicatorLineHeight);
                var toFlagTop = new PointF(maxDeviationX / 2.0f, pointerNeedleThickTopBottom.Y);
                var toFlagLeft = new PointF(toFlagTop.X - toFromFlagWidth / 2.0f, toFlagTop.Y + toFromFlagHeight);
                var toFlagRight = new PointF(toFlagTop.X + toFromFlagWidth / 2.0f, toFlagTop.Y + toFromFlagHeight);
                var fromFlagBottom = new PointF(maxDeviationX / 2.0f, pointerNeedleThickBottomTop.Y);
                var fromFlagLeft = new PointF(fromFlagBottom.X - toFromFlagWidth / 2.0f, fromFlagBottom.Y - toFromFlagHeight);
                var fromFlagRight = new PointF(fromFlagBottom.X + toFromFlagWidth / 2.0f, fromFlagBottom.Y - toFromFlagHeight);

                g.TranslateTransform(outerBounds.Width / 2.0f, outerBounds.Height / 2.0f);
                g.RotateTransform(-instrumentState.MagneticHeadingDegrees);
                g.RotateTransform(instrumentState.DesiredCourseDegrees);
                g.TranslateTransform(-outerBounds.Width / 2.0f, -outerBounds.Height / 2.0f);

                g.TranslateTransform(outerBounds.Width / 2.0f, 0);

                //draw deviation dots
                g.FillEllipse(Brushes.White, new RectangleF(leftInnerDotX, dotY, dotHeight, dotHeight));
                g.FillEllipse(Brushes.White, new RectangleF(leftOuterDotX, dotY, dotHeight, dotHeight));
                g.FillEllipse(Brushes.White, new RectangleF(rightInnerDotX, dotY, dotHeight, dotHeight));
                g.FillEllipse(Brushes.White, new RectangleF(rightOuterDotX, dotY, dotHeight, dotHeight));

                //draw thin line on top of pointer arrow
                needlePen.Width = pointerNeedleThinWidth;
                g.DrawLineFast(needlePen, courseNeedleTopLineTop, courseNeedleTopLineBottom);

                //draw pointer arrow
                g.FillPolygon(needleBrush, new[] {coursePointerTriangleTop, coursePointerTriangleLeft, coursePointerTriangleRight});

                //draw thick line just below pointer arrow
                needlePen.Width = pointerNeedleThickWidth;
                g.DrawLineFast(needlePen, pointerNeedleThickTopTop, pointerNeedleThickTopBottom);

                if (instrumentState.DeviationInvalidFlag)
                {
                    //draw deviation invalid flag
                    g.FillRectangle(redFlagBrush, deviationInvalidFlagRect);
                }
                //draw CDI needle
                needlePen.Width = pointerNeedleThickWidth;
                if (instrumentState.DeviationInvalidFlag) needlePen.DashPattern = new[] {2, 1.75f};
                g.TranslateTransform(deviationTranslateX, 0);
                g.DrawLineFast(needlePen, cdiLineTop, cdiLineBottom);
                g.TranslateTransform(-deviationTranslateX, 0);

                needlePen.DashStyle = DashStyle.Solid;

                //draw thick line just below CDI needle  
                needlePen.Width = pointerNeedleThickWidth;
                g.DrawLineFast(needlePen, pointerNeedleThickBottomTop, pointerNeedleThickBottomBottom);

                //draw thin line indicating reciprocal-of-course
                needlePen.Width = pointerNeedleThinWidth;
                g.DrawLineFast(needlePen, courseNeedleBottomLineTop, courseNeedleBottomLineBottom);

                if (instrumentState.ShowToFromFlag)
                {
                    if (instrumentState.ToFlag) g.FillPolygon(Brushes.White, new[] {toFlagTop, toFlagLeft, toFlagRight});
                    if (instrumentState.FromFlag) g.FillPolygon(Brushes.White, new[] {fromFlagBottom, fromFlagLeft, fromFlagRight});
                }

                GraphicsUtil.RestoreGraphicsState(g, ref initialState);
            }
        }
    }
}