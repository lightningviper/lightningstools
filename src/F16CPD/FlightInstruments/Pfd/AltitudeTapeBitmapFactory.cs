using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.UI.Layout;

namespace F16CPD.FlightInstruments.Pfd
{
    internal  sealed class AltitudeTapeBitmapFactory
    {
        private static readonly Bitmap[] AltitudeTapes = new Bitmap[200];
        private static Font _majorUnitFont = new Font("Lucida Console", 12, FontStyle.Bold);
        internal static Bitmap GetAltitudeTapeBitmap(float altitudeInFeet, int widthPixels, int heightPixels, float altitudeIndexInFeet)
        {
            const int verticalSeparationBetweenTicks = 10;

            var thousands = (int) (altitudeInFeet/1000.0f);

            if (AltitudeTapes[thousands + 4] == null)
            {
                AltitudeTapes[thousands + 4] = ValuesTapeGenerator.GenerateValuesTape(
                    Color.FromArgb(160, Color.Gray), //positiveBackgroundColor
                    Color.White, //positiveForegroundColor
                    Color.FromArgb(160, Color.Gray), //negativeBackgroundColor
                    Color.White, //negativeForegroundColor
                    100, //majorUnitInterval 
                    20, //minorUnitInterval
                    12, //majorUnitLineLengthPixels 
                    5, //minorUnitLineLengthPixels
                    true, //negativeUnitsLabelled
                    verticalSeparationBetweenTicks, //verticalSeparationBetweenTicksInPixels
                    (thousands*1000) + 2000, //scaleMaxVal
                    (thousands*1000) - 2000, //scaleMinVal
                    widthPixels, //tapeWidthInPixels
                    HAlignment.Left, //ticksAlignment
                    0, //textPaddingPixels
                    _majorUnitFont, //majorUnitFont 
                    HAlignment.Left, //textAlignment
                    true, //negativeUnitsHaveSign
                    null
                    );
            }

            var start = (Bitmap) AltitudeTapes[thousands + 4].Clone();
            var centerPoint = new Point(start.Width/2, start.Height/2);
            const int altitudeBetweenTicksInFeet = 20;
            const float pixelsSeparationPerFootOfAltitude =
                verticalSeparationBetweenTicks/(float) altitudeBetweenTicksInFeet;
            var currentValY = centerPoint.Y -
                              ((int) (((altitudeInFeet - (thousands*1000))*pixelsSeparationPerFootOfAltitude)));
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

            //add index bug to altitude tape
            var bugA = new Point(-2, 0);
            var bugB = new Point(-2, 21);
            var bugC = new Point(10, 21);
            var bugD = new Point(10, 16);
            var bugE = new Point(3, 11);
            var bugF = new Point(3, 10);
            var bugG = new Point(10, 5);
            var bugH = new Point(10, 0);
            var bugPoints = new[] {bugA, bugB, bugC, bugD, bugE, bugF, bugG, bugH};
            var altitudeIndexBugColor = Color.Magenta;
            Brush altitudeIndexBugBrush = new SolidBrush(altitudeIndexBugColor);
            using (var h = Graphics.FromImage(start))
            {
                var origTransform = h.Transform;
                h.SmoothingMode = SmoothingMode.AntiAlias;
                var altitudeIndexBugY = centerPoint.Y -
                                        ((int)
                                            (((altitudeIndexInFeet - (thousands*1000))*
                                              pixelsSeparationPerFootOfAltitude)));
                if (altitudeIndexBugY < (cropRectangle.Top + 3)) altitudeIndexBugY = cropRectangle.Top + 3;
                if (altitudeIndexBugY > (cropRectangle.Bottom - 3)) altitudeIndexBugY = cropRectangle.Bottom - 3;
                h.TranslateTransform(0, altitudeIndexBugY - 11);
                h.FillPolygon(altitudeIndexBugBrush, bugPoints);
                h.Transform = origTransform;
            }

            var cropped = (Bitmap) Common.Imaging.Util.CropBitmap(start, cropRectangle);
            return cropped;
        }
    }
}