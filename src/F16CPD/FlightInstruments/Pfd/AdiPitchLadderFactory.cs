using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Imaging;
namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiPitchLadderFactory
    {
        private static Bitmap _adiPitchBars;

        internal static Bitmap GetAdiPitchLadder(float degreesPitch, float degreesRoll)
        {
            if (_adiPitchBars == null)
            {
                _adiPitchBars = AdiPitchBarBitmapGenerator.GenerateAdiPitchBarBitmap();
            }
            var start = _adiPitchBars;
            var centerPoint = new Point(start.Width / 2, start.Height / 2);

            const int verticalDistanceBetweenPitchLines = 25;
            const int degreesBetweenTicks = 5;
            const float pixelsSeparationPerDegreeOfPitch = verticalDistanceBetweenPitchLines / degreesBetweenTicks;
            var currentPitchY = centerPoint.Y - ((int)((degreesPitch * pixelsSeparationPerDegreeOfPitch)));
            var topY = (int)(currentPitchY - (25 * pixelsSeparationPerDegreeOfPitch));
            var bottomY = (int)(currentPitchY + (25 * pixelsSeparationPerDegreeOfPitch) + 1);

            var topLeftCrop = new Point(0, topY);
            var bottomRightCrop = new Point(start.Width, bottomY);

            if (topLeftCrop.Y < 0) topLeftCrop.Y = 0;
            if (topLeftCrop.Y > start.Height) topLeftCrop.Y = start.Height;
            if (bottomRightCrop.Y < 0) bottomRightCrop.Y = 0;
            if (bottomRightCrop.Y > start.Height) bottomRightCrop.Y = start.Height;

            var cropRectangle = new Rectangle(topLeftCrop.X, topLeftCrop.Y, bottomRightCrop.X - topLeftCrop.X,
                bottomRightCrop.Y - topLeftCrop.Y);

            const int heightOffset = 44;
            var adi = new Bitmap(cropRectangle.Height + (heightOffset * 2), cropRectangle.Height + (heightOffset * 2),
                PixelFormat.Format16bppRgb565);
            adi.MakeTransparent();
            using (var g = Graphics.FromImage(adi))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.TranslateTransform((int)(adi.Width / (float)2), (int)(adi.Height / (float)2));
                g.RotateTransform(-degreesRoll);
                g.TranslateTransform((int)(-adi.Width / (float)2), (int)(-adi.Height / (float)2));
                g.DrawImageFast(start, new Rectangle(heightOffset, heightOffset, cropRectangle.Width, cropRectangle.Height),
                    cropRectangle, GraphicsUnit.Pixel);

                //draw sky pointer triangle
                Brush whiteBrush = new SolidBrush(Color.White);
                var greyPen = new Pen(Color.DarkGray);
                var centerX = (adi.Width / 2);
                var skyPointerTriangle = new[]
                {
                    new Point(centerX, 0), new Point(centerX - 12, 25),
                    new Point(centerX + 12, 25)
                };
                g.FillPolygon(whiteBrush, skyPointerTriangle);
                g.DrawPolygon(greyPen, skyPointerTriangle);
            }
            return adi;
        }
    }
}