using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class BearingToBeaconIndicatorRenderer
    {
        internal static void DrawBearingToBeaconIndicator(
            Graphics destinationGraphics, ref GraphicsState basicState, float centerX, float centerY, InstrumentState instrumentState, Image hsiBearingToBeaconNeedleMaskedImage)
        {
            //draw the bearing to beacon indicator needle
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(centerX, centerY);
            destinationGraphics.RotateTransform(-(instrumentState.MagneticHeadingDegrees - instrumentState.BearingToBeaconDegrees));
            destinationGraphics.TranslateTransform(-centerX, -centerY);
            destinationGraphics.TranslateTransform(1, 0);
            destinationGraphics.DrawImageFast(hsiBearingToBeaconNeedleMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}