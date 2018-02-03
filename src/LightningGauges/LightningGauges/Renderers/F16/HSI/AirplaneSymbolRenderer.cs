using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class AirplaneSymbolRenderer
    {
        internal static void DrawAirplaneSymbol(Graphics destinationGraphics, ref GraphicsState basicState, Image airplaneSymbolMaskedImage)
        {
            //draw airplane symbol
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            destinationGraphics.TranslateTransform(0, 5);
            destinationGraphics.DrawImageFast(airplaneSymbolMaskedImage, new Point(0, 0));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}