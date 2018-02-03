using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class BackgroundRenderer
    {
        internal static void DrawBackground(Graphics destinationGraphics, ref GraphicsState basicState, Image hsiBackgroundMaskedImage)
        {
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(0, -11);
            destinationGraphics.DrawImageFast(hsiBackgroundMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}