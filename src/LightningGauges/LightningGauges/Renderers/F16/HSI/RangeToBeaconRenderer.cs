using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.HSI
{
    internal static class RangeToBeaconRenderer
    {
        internal static void DrawRangeToBeacon(Graphics destinationGraphics, ref GraphicsState basicState, InstrumentState instrumentState, FontGraphic rangeFontGraphic)
        {
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
            var distanceToBeacon = instrumentState.DistanceToBeaconNauticalMiles;
            if (distanceToBeacon > 999.9) distanceToBeacon = 999.9f;
            var distanceToBeaconString = $"{distanceToBeacon:000.0}";
            var distanceToBeaconHundreds = distanceToBeaconString[0];
            var distanceToBeaconTens = distanceToBeaconString[1];
            var distanceToBeaconOnes = distanceToBeaconString[2];
            var distanceToBeaconHundredsImage = rangeFontGraphic.GetCharImage(distanceToBeaconHundreds);
            var distanceToBeaconTensImage = rangeFontGraphic.GetCharImage(distanceToBeaconTens);
            var distanceToBeaconOnesImage = rangeFontGraphic.GetCharImage(distanceToBeaconOnes);

            var currentX = 29;
            const int y = 45;
            const int spacingPixels = -5;
            destinationGraphics.DrawImageFast(distanceToBeaconHundredsImage, new Point(currentX, y));
            currentX += distanceToBeaconHundredsImage.Width + spacingPixels;
            destinationGraphics.DrawImageFast(distanceToBeaconTensImage, new Point(currentX, y));
            currentX += distanceToBeaconTensImage.Width + spacingPixels;
            destinationGraphics.DrawImageFast(distanceToBeaconOnesImage, new Point(currentX, y));
            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
        }
    }
}