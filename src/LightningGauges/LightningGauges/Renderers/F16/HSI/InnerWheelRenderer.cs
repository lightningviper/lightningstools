using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class InnerWheelRenderer
    {
        internal static void DrawInnerWheel(Graphics destinationGraphics, ref GraphicsState basicState, float centerX, float centerY, InstrumentState instrumentState, Image innerWheelMaskedImage)
        {
            //draw inner wheel
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(centerX, centerY);
            destinationGraphics.RotateTransform(-instrumentState.MagneticHeadingDegrees);
            destinationGraphics.RotateTransform(instrumentState.DesiredCourseDegrees);
            destinationGraphics.TranslateTransform(-centerX, -centerY);
            destinationGraphics.TranslateTransform(0.5f, -2.0f);
            destinationGraphics.DrawImageFast(innerWheelMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}