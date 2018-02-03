using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class HeadingBugRenderer
    {
        internal static void DrawHeadingBug(Graphics destinationGraphics, ref GraphicsState basicState, float centerX, float centerY, InstrumentState instrumentState, Image hsiHeadingBugMaskedImage)
        {
            //draw heading bug
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(centerX, centerY);
            destinationGraphics.RotateTransform(instrumentState.DesiredHeadingDegrees - instrumentState.MagneticHeadingDegrees);
            destinationGraphics.TranslateTransform(-centerX, -centerY);
            destinationGraphics.DrawImageFast(hsiHeadingBugMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}