using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class DesiredCourseRenderer
    {
        internal static void DrawDesiredCourse(Graphics destinationGraphics, ref GraphicsState basicState, InstrumentState instrumentState, FontGraphic rangeFontGraphic)
        {
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            var desiredCourseString = $"{instrumentState.DesiredCourseDegrees:000}";
            var desiredCourseHundreds = desiredCourseString[0];
            var desiredCourseTens = desiredCourseString[1];
            var desiredCourseOnes = desiredCourseString[2];
            var desiredCourseHundredsImage = rangeFontGraphic.GetCharImage(desiredCourseHundreds);
            var desiredCourseTensImage = rangeFontGraphic.GetCharImage(desiredCourseTens);
            var desiredCourseOnesImage = rangeFontGraphic.GetCharImage(desiredCourseOnes);

            var currentX = 182;
            const int y = 45;
            const int spacingPixels = -5;
            destinationGraphics.DrawImageFast(desiredCourseHundredsImage, new Point(currentX, y));
            currentX += desiredCourseHundredsImage.Width + spacingPixels;
            destinationGraphics.DrawImageFast(desiredCourseTensImage, new Point(currentX, y));
            currentX += desiredCourseTensImage.Width + spacingPixels;
            destinationGraphics.DrawImageFast(desiredCourseOnesImage, new Point(currentX, y));

            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}