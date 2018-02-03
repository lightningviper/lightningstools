using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class CourseDeviationIndicatorRenderer
    {
        internal static void DrawCourseDeviationIndicator(
            Graphics destinationGraphics, ref GraphicsState basicState, float centerX, float centerY, InstrumentState instrumentState, Image hsiCourseDeviationIndicatorMaskedImage)
        {
            //draw course deviation indicator
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(centerX, centerY);
            destinationGraphics.RotateTransform(-instrumentState.MagneticHeadingDegrees);
            destinationGraphics.RotateTransform(instrumentState.DesiredCourseDegrees);
            destinationGraphics.TranslateTransform(-centerX, -centerY);
            var cdiPct = instrumentState.CourseDeviationDegrees / instrumentState.CourseDeviationLimitDegrees;
            const float cdiRange = 46.0f;
            var cdiPos = cdiPct * cdiRange;
            destinationGraphics.TranslateTransform(cdiPos, -2);
            try { destinationGraphics.DrawImageFast(hsiCourseDeviationIndicatorMaskedImage, new Point(0, 0)); }
            catch (OverflowException) { }
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}