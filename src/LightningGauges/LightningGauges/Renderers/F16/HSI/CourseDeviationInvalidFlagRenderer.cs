using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class CourseDeviationInvalidFlagRenderer
    {
        internal static void DrawCourseDeviationInvalidFlag(
            Graphics destinationGraphics, ref GraphicsState basicState, float centerX, float centerY, InstrumentState instrumentState, Image hsiCDIFlagMaskedImage)
        {
            //draw the CDI flag
            if (instrumentState.DeviationInvalidFlag)
            {
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(centerX, centerY);
                destinationGraphics.RotateTransform(-instrumentState.MagneticHeadingDegrees);
                destinationGraphics.RotateTransform(instrumentState.DesiredCourseDegrees);
                destinationGraphics.TranslateTransform(-centerX, -centerY);
                destinationGraphics.DrawImageFast(hsiCDIFlagMaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            }
        }
    }
}