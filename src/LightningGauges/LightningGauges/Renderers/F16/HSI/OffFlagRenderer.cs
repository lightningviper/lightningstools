using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class OffFlagRenderer
    {
        internal static void DrawOffFlag(Graphics destinationGraphics, ref GraphicsState basicState, InstrumentState instrumentState, Image hsiOffFlagMaskedImage)
        {
            //draw the OFF flag
            if (!instrumentState.OffFlag) return;

            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.RotateTransform(-25);
            destinationGraphics.TranslateTransform(20, 50);
            destinationGraphics.DrawImageFast(hsiOffFlagMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}