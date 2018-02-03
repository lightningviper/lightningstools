using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.UI.Layout;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AirspeedTapeBitmapFactory
    {
        private static Bitmap _airspeedTape;


        internal static Bitmap GetAirspeedTapeBitmap(float indicatedAirspeedKnots, int widthPixels, int heightPixels, float airspeedIndexInKnots)
        {
            const int verticalSeparationBetweenTicks = 9;
            const int scaleMaxVal = 1200;
            const int scaleMinVal = -1200;
            if (indicatedAirspeedKnots > scaleMaxVal) indicatedAirspeedKnots = scaleMaxVal;
            if (indicatedAirspeedKnots < scaleMinVal) indicatedAirspeedKnots = scaleMinVal;


            if (_airspeedTape == null)
            {
                _airspeedTape = ValuesTapeGenerator.GenerateValuesTape(
                    Color.FromArgb(160, Color.Gray), //positiveBackgroundColor
                    Color.White, //positiveForegroundColor
                    Color.FromArgb(160, Color.Gray), //negativeBackgroundColor
                    Color.White, //negativeForegroundColor
                    100, //majorUnitInterval
                    20, //minorUnitInterval 
                    12, //majorUnitLineLengthInPixels
                    7, //minorUnitLineLengthInPixels,
                    false, //negativeUnitsLabelled
                    verticalSeparationBetweenTicks, //verticalSeparationBetweenTicksInPixels
                    scaleMaxVal, //scaleMaxVal
                    scaleMinVal, //scaleMinVal
                    widthPixels, //tapeWidthInPixels
                    HAlignment.Right, //ticsAlignment
                    3, //paddingPixels
                    new Font("Lucida Console", 11, FontStyle.Bold), //majorUnitFont
                    HAlignment.Right, //textAlignment
                    true, //negativeUnitsHaveNegativeSign
                    null
                    );
            }

            var start = (Bitmap) _airspeedTape.Clone();

            var centerPoint = new Point(start.Width/2, start.Height/2);
            const int knotsBetweenTicks = 20;
            const float pixelsSeparationPerKnot = verticalSeparationBetweenTicks/(float) knotsBetweenTicks;
            var currentAirspeedY = centerPoint.Y - ((int) ((indicatedAirspeedKnots*pixelsSeparationPerKnot)));


            var topY = (currentAirspeedY - (heightPixels/2));
            var bottomY = (currentAirspeedY + (heightPixels/2));

            var topLeftCrop = new Point(0, topY);
            var bottomRightCrop = new Point(start.Width, bottomY);

            if (topLeftCrop.Y < 0) topLeftCrop.Y = 0;
            if (topLeftCrop.Y > start.Height) topLeftCrop.Y = start.Height;
            if (bottomRightCrop.Y < 0) bottomRightCrop.Y = 0;
            if (bottomRightCrop.Y > start.Height) bottomRightCrop.Y = start.Height;

            var cropRectangle = new Rectangle(topLeftCrop.X, topLeftCrop.Y, bottomRightCrop.X - topLeftCrop.X,
                bottomRightCrop.Y - topLeftCrop.Y);


            //add index bug to airspeed tape
            var bugA = new Point(-2, 0);
            var bugB = new Point(-2, 5);
            var bugC = new Point(5, 10);
            var bugD = new Point(5, 11);
            var bugE = new Point(-2, 16);
            var bugF = new Point(-2, 21);
            var bugG = new Point(10, 21);
            var bugH = new Point(10, 0);

            var bugPoints = new[] {bugA, bugB, bugC, bugD, bugE, bugF, bugG, bugH};
            var airspeedIndexBugColor = Color.Magenta;
            Brush airspeedIndexBugBrush = new SolidBrush(airspeedIndexBugColor);
            using (var h = Graphics.FromImage(start))
            {
                h.SmoothingMode = SmoothingMode.AntiAlias;
                var origTransform = h.Transform;
                var airspeedIndexBugY = centerPoint.Y - ((int) ((airspeedIndexInKnots*pixelsSeparationPerKnot)));
                if (airspeedIndexBugY < (cropRectangle.Top + 3)) airspeedIndexBugY = cropRectangle.Top + 3;
                if (airspeedIndexBugY > (cropRectangle.Bottom - 3)) airspeedIndexBugY = cropRectangle.Bottom - 3;

                h.TranslateTransform(start.Width - 10, airspeedIndexBugY - 11);
                h.FillPolygon(airspeedIndexBugBrush, bugPoints);
                h.Transform = origTransform;
            }


            var cropped = (Bitmap) Common.Imaging.Util.CropBitmap(start, cropRectangle);
            return cropped;
        }
    }
}