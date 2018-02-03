using Common.Drawing;
using Common.UI.Layout;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class VerticalVelocityTapeBitmapFactory
    {
        private static Bitmap _vviTape;

        internal static Bitmap GetVerticalVelocityTapeBitmap(int widthPixels, int heightPixels)
        {
            const float verticalVelocity = 0; //override passed-in vertical velocity so tape doesn't move
            const int verticalSeparationBetweenTicks = 20;
            if (_vviTape == null)
            {
                _vviTape = ValuesTapeGenerator.GenerateValuesTape(
                    Color.White, //positiveBackgroundColor
                    Color.Black, //positiveForegroundColor
                    Color.Black, //negativeBackgroundColor
                    Color.White, //positiveBackgroundColor
                    2, //majorUnitInterval
                    1, //minorUnitInterval 
                    11, //majorUnitLineLengthInPixels
                    7, //minorUnitLineLengthInPixels 
                    true, //negativeUnitsLabelled
                    verticalSeparationBetweenTicks, //verticalSeparationBetweenTicksInPixels
                    7, //scaleMaxVal
                    -7, //scaleMinVal
                    widthPixels, //tapeWidthInPixels
                    HAlignment.Left, //ticksAlignment
                    0, //textPaddingPixels
                    new Font("Lucida Console", 10, FontStyle.Bold), //majorUnitFont
                    HAlignment.Right, //textAlignment
                    false, //negativeUnitsHaveNegativeSign
                    null
                    );
                var start = _vviTape;
                var centerPoint = new Point(start.Width/2, start.Height/2);
                const int velocityBetweenTicksInHundredFps = 1;
                const float pixelsSeparationPerHundredFps = verticalSeparationBetweenTicks/
                                                            (float) velocityBetweenTicksInHundredFps;
                var currentValY = centerPoint.Y - ((int) ((verticalVelocity*pixelsSeparationPerHundredFps)));
                var topY = (currentValY - (heightPixels/2));
                var bottomY = (currentValY + (heightPixels/2));

                var topLeftCrop = new Point(0, topY);
                var bottomRightCrop = new Point(start.Width, bottomY);

                if (topLeftCrop.Y < 0) topLeftCrop.Y = 0;
                if (topLeftCrop.Y > start.Height) topLeftCrop.Y = start.Height;
                if (bottomRightCrop.Y < 0) bottomRightCrop.Y = 0;
                if (bottomRightCrop.Y > start.Height) bottomRightCrop.Y = start.Height;

                var cropRectangle = new Rectangle(topLeftCrop.X, topLeftCrop.Y, bottomRightCrop.X - topLeftCrop.X,
                    bottomRightCrop.Y - topLeftCrop.Y);
                var cropped = (Bitmap) Common.Imaging.Util.CropBitmap(start, cropRectangle);

                //cut zero-mark cutout into tape
                using (var g = Graphics.FromImage(cropped))
                {
                    var centerY = (cropped.Height/2);
                    var rightX = cropped.Width;
                    const int leftX = 0;
                    var upperRightTriangleCorner = new Point(rightX, centerY - 20);
                    var lowerRightTriangleCorner = new Point(rightX, centerY + 20);
                    var middleLeftTriangleCorner = new Point(leftX, centerY);
                    g.FillPolygon(Brushes.Fuchsia,
                        new[] {upperRightTriangleCorner, middleLeftTriangleCorner, lowerRightTriangleCorner});
                }
                cropped.MakeTransparent(Color.Fuchsia);
                _vviTape = cropped;
            }
            return _vviTape;
        }
    }
}