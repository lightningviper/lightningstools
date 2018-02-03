using Common.Drawing;
using Common.UI.Layout;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AOATapeBitmapFactory
    {
        private static Bitmap _aoaTape;

        private const int AOA_YELLOW_RANGE_MIN_ANGLE_DEGREES = 8;
        private const int AOA_YELLOW_RANGE_MAX_ANGLE_DEGREES = 11;
        private const int AOA_GREEN_RANGE_MIN_ANGLE_DEGREES = 11;
        private const int AOA_GREEN_RANGE_MAX_ANGLE_DEGREES = 14;
        private const int AOA_RED_RANGE_MIN_ANGLE_DEGREES = 14;
        private const int AOA_RED_RANGE_MAX_ANGLE_DEGREES = 17;

        internal static Bitmap GetAoATapeBitmap(float aoaInDegrees, int widthPixels, int heightPixels)
        {
            const int verticalSeparationBetweenTicks = 10;
            const int scaleMaxVal = 90;
            const int scaleMinVal = -90;
            if (aoaInDegrees > scaleMaxVal) aoaInDegrees = scaleMaxVal;
            if (aoaInDegrees < scaleMinVal) aoaInDegrees = scaleMinVal;
            if (_aoaTape == null)
            {
                var aoaYellow = new TapeEdgeColoringInstruction
                {
                    Color = Color.Yellow,
                    MinVal = AOA_YELLOW_RANGE_MIN_ANGLE_DEGREES,
                    MaxVal = AOA_YELLOW_RANGE_MAX_ANGLE_DEGREES
                };

                var aoaGreen = new TapeEdgeColoringInstruction
                {
                    Color = Color.Green,
                    MinVal = AOA_GREEN_RANGE_MIN_ANGLE_DEGREES,
                    MaxVal = AOA_GREEN_RANGE_MAX_ANGLE_DEGREES
                };


                var aoaRed = new TapeEdgeColoringInstruction
                {
                    Color = Color.Red,
                    MinVal = AOA_RED_RANGE_MIN_ANGLE_DEGREES,
                    MaxVal = AOA_RED_RANGE_MAX_ANGLE_DEGREES
                };

                _aoaTape = ValuesTapeGenerator.GenerateValuesTape(
                    Color.FromArgb(160, Color.Gray), //positiveBackgroundColor
                    Color.White, //postiveForegroundColor
                    Color.FromArgb(160, Color.Gray), //negativeBackgroundColor
                    Color.White, //negativeForegroundColor
                    5, //majorUnitInterval
                    1, //minorUnitInterval
                    0, //majorUnitLineLengthPixels
                    21, //minorUnitLineLengthPixels
                    true, //negativeUnitsLabelled 
                    verticalSeparationBetweenTicks, //verticalSeparationBetweenTicksPixels
                    scaleMaxVal, //scaleMaxVal 
                    scaleMinVal, //scaleMinVal 
                    widthPixels, //tapeWidthPixels
                    HAlignment.Center, //ticksAlignment
                    0, //textPaddingPixels
                    new Font("Lucida Console", 12, FontStyle.Bold), //majorUnitFont 
                    HAlignment.Right, //textAlignment
                    true, //negativeUnitsHaveNegativeSign
                    new[] {aoaYellow, aoaGreen, aoaRed}
                    );
            }

            var start = _aoaTape;
            var centerPoint = new Point(start.Width/2, start.Height/2);
            const int quantityBetweenTicksInDegreesAoa = 1;
            const float pixelsSeparationPerDegreeAoa =
                verticalSeparationBetweenTicks/(float) quantityBetweenTicksInDegreesAoa;
            var currentValY = centerPoint.Y - ((int) ((aoaInDegrees*pixelsSeparationPerDegreeAoa)));
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

            return cropped;
        }
    }
}