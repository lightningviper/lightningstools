using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AirspeedTapeRenderer
    {

        internal static void DrawAirspeedTape(Graphics g, float indicatedAirspeedInDecimalFeetPerSecond, float airspeedIndexInKnots)
        {
            //************************************
            //AIRSPEED TAPE
            //************************************

            var airspeedIndexFont = new Font("Lucida Console", 13, FontStyle.Bold);
            var airspeedKtsFont = new Font("Lucida Console", 14, FontStyle.Bold);

            var indexTextColor = Color.FromArgb(255, 128, 255);
            Brush purpleBrush = new SolidBrush(indexTextColor);

            //draw bounding box for airspeed tape
            var airspeedStripBoundingBox = new Rectangle(new Point(5, 100), new Size(54, 279));
            var airspeedIndexBox =
                new Rectangle(new Point(airspeedStripBoundingBox.Left, airspeedStripBoundingBox.Top - 19),
                    new Size(airspeedStripBoundingBox.Width, 19));
            var airspeedIndexTextBox =
                new Rectangle(new Point(airspeedStripBoundingBox.Left, airspeedStripBoundingBox.Top - 20),
                    new Size(airspeedStripBoundingBox.Width, 25));

            var airspeedFps = indicatedAirspeedInDecimalFeetPerSecond;
            var airspeedKnots = (float) (Math.Round((airspeedFps/Common.Math.Constants.FPS_PER_KNOT), 1));
            float airspeedIndexKnots = airspeedIndexInKnots;


            var airspeedBitmap = AirspeedTapeBitmapFactory.GetAirspeedTapeBitmap(airspeedKnots, airspeedStripBoundingBox.Width,
                airspeedStripBoundingBox.Height, airspeedIndexKnots);
            g.DrawImageFast(
                airspeedBitmap,
                airspeedStripBoundingBox.Left,
                airspeedStripBoundingBox.Top,
                new Rectangle(
                    new Point(0, 0),
                    new Size(airspeedBitmap.Width, airspeedStripBoundingBox.Height)
                    ), GraphicsUnit.Pixel
                );
            //trace the outline of the airspeed tape with black
            g.DrawRectangleFast(new Pen(Color.Black), airspeedStripBoundingBox);

            //draw airspeed index box
            var airspeedIndexFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.FillRectangle(Brushes.Black, airspeedIndexBox);

            //add airspeed index text to index box
            g.DrawStringFast(string.Format("{0:000}", airspeedIndexKnots), airspeedIndexFont, purpleBrush,
                airspeedIndexTextBox, airspeedIndexFormat);

            //draw white line under airspeed index box
            var whitePen = new Pen(Color.White) {Width = 2};
            g.DrawLineFast(whitePen, airspeedIndexBox.Left, airspeedIndexBox.Bottom, airspeedIndexBox.Right,
                airspeedIndexBox.Bottom);

            //draw white line at bottom of airspeed strip
            whitePen.Width = 2;
            g.DrawLineFast(whitePen, airspeedStripBoundingBox.Left, airspeedStripBoundingBox.Bottom,
                airspeedStripBoundingBox.Right, airspeedStripBoundingBox.Bottom);

            //draw airspeed counter box
            var pointA = new Point(
                2, 222);
            var pointB = new Point(
                2, 255);
            var pointC = new Point(
                26, 255);
            var pointD = new Point(
                26, 267);
            var pointE = new Point(
                47, 267);
            var pointF = new Point(
                47, 243);
            var pointG = new Point(
                54, 239);
            var pointH = new Point(
                47, 234);
            var pointI = new Point(
                47, 212);
            var pointJ = new Point(
                26, 212);
            var pointK = new Point(
                26, 222);

            whitePen.Width = 1;
            var points = new[] {pointA, pointB, pointC, pointD, pointE, pointF, pointG, pointH, pointI, pointJ, pointK};
            g.FillPolygon(Brushes.Black, points);
            g.DrawPolygon(whitePen, points);

            var airspeedString = string.Format("{0:0000}", Math.Truncate(airspeedKnots));
            var hundredsDigit = SingleDigitBitmapFactory.GetSingleDigitBitmap(Int32.Parse(new String(airspeedString[1], 1)));
            var tensDigit = SingleDigitBitmapFactory.GetSingleDigitBitmap(Int32.Parse(new String(airspeedString[2], 1)));
            var onesVal = Int32.Parse(new String(airspeedString[3], 1));
            var onesFrac = (float) Math.Round((onesVal + (airspeedKnots - Math.Truncate(airspeedKnots))), 1);
            var onesDigits = SingleDigitBitmapFactory.GetSingleDigitBitmap(onesFrac, true);
            var hundredsDigitRectangle = new Rectangle(new Point(5, 230),
                new Size(hundredsDigit.Width, hundredsDigit.Height));
            var tensDigitRectangle = new Rectangle(new Point(18, 230), new Size(tensDigit.Width, tensDigit.Height));
            var onesDigitsRectangle = new Rectangle(new Point(32, 217), new Size(onesDigits.Width, onesDigits.Height));

            //draw airspeed counter digits
            g.DrawImageFast(hundredsDigit, hundredsDigitRectangle);
            g.DrawImageFast(tensDigit, tensDigitRectangle);
            g.DrawImageFast(onesDigits, onesDigitsRectangle);

            var airspeedKtsRectangle = new Rectangle(new Point(15, 200), new Size(40, 15));
            var airspeedKtsFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            airspeedKtsFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;
            var path = new GraphicsPath();
            path.AddString("KTS", airspeedKtsFont.FontFamily, (int) airspeedKtsFont.Style, airspeedKtsFont.SizeInPoints,
                airspeedKtsRectangle, airspeedKtsFormat);
            g.DrawPathFast(new Pen(Color.Black), path);
            g.FillPathFast(Brushes.White, path);
        }
    }
}