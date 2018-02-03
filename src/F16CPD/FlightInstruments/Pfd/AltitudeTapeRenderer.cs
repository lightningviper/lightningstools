using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AltitudeTapeRenderer
    {
        internal static void DrawAltitudeTape(Graphics g, 
             float indicatedAltitudeAboveMeanSeaLevelInDecimalFeet,
            float barometricPressure, AltimeterUnits altimeterUnits, float altitudeIndexInFeet)
        {
            //*******************************
            //* ALTITUDE TAPE
            //*******************************

            var altitudeStripBoundingBox = new Rectangle(new Point(535, 99), new Size(70, 278));
            var altitudeIndexBox =
                new Rectangle(new Point(altitudeStripBoundingBox.Left, altitudeStripBoundingBox.Top - 19),
                    new Size(altitudeStripBoundingBox.Width, 19));
            var altitudeIndexTextBox =
                new Rectangle(new Point(altitudeStripBoundingBox.Left, altitudeStripBoundingBox.Top - 20),
                    new Size(altitudeStripBoundingBox.Width, 25));
            var barometricPressureBox =
                new Rectangle(new Point(altitudeStripBoundingBox.Left, altitudeStripBoundingBox.Bottom + 1),
                    new Size(altitudeStripBoundingBox.Width, 19));

            var greenColor = Color.FromArgb(0, 255, 0);
            var indexTextColor = Color.FromArgb(255, 128, 255);

            Brush purpleBrush = new SolidBrush(indexTextColor);
            Brush greenBrush = new SolidBrush(greenColor);

            var whitePen = new Pen(Color.White);
            var blackPen = new Pen(Color.Black);

            var altitudeIndexFont = new Font("Lucida Console", 13, FontStyle.Bold);
            var barometricPressureFont = new Font("Lucida Console", 10, FontStyle.Bold);
            var altitudeFeetTextFont = new Font("Lucida Console", 14, FontStyle.Bold);

            var indicatedAltitude = (indicatedAltitudeAboveMeanSeaLevelInDecimalFeet);

            var altitudeTapeBitmap = AltitudeTapeBitmapFactory.GetAltitudeTapeBitmap(indicatedAltitude, altitudeStripBoundingBox.Width - 1,
                altitudeStripBoundingBox.Height, altitudeIndexInFeet);

            g.DrawImageFast(
                altitudeTapeBitmap,
                altitudeStripBoundingBox.Left + 1,
                altitudeStripBoundingBox.Top + 1,
                new Rectangle(
                    new Point(0, 0),
                    new Size(altitudeTapeBitmap.Width, altitudeStripBoundingBox.Height)
                    ), GraphicsUnit.Pixel
                );
            //trace the outline of the altitude tape with black
            g.DrawRectangleFast(blackPen, altitudeStripBoundingBox);

            //draw altitude index box
            var altitudeIndexFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.FillRectangle(Brushes.Black, altitudeIndexBox);

            //add altitude index text to altitude index box
            var altitudeIndex = string.Format("{0:00000}", altitudeIndexInFeet);
            if (altitudeIndex[0] == '-') altitudeIndex = altitudeIndex.Substring(1, altitudeIndex.Length - 1);
            g.DrawStringFast(altitudeIndex, altitudeIndexFont, purpleBrush, altitudeIndexTextBox, altitudeIndexFormat);

            //draw white line under altitude index box
            whitePen.Width = 2;
            g.DrawLineFast(whitePen, altitudeIndexBox.Left, altitudeIndexBox.Bottom, altitudeIndexBox.Right,
                altitudeIndexBox.Bottom);

            //draw white line at bottom of altitude strip
            whitePen.Width = 2;
            g.DrawLineFast(whitePen, altitudeStripBoundingBox.Left, altitudeStripBoundingBox.Bottom,
                altitudeStripBoundingBox.Right, altitudeStripBoundingBox.Bottom);

            //draw barometric pressure box
            var baroPressureFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.FillRectangle(Brushes.Black, barometricPressureBox);

            //add barometric pressure text to barometric pressure box
            g.DrawStringFast(string.Format("{0:00.00}{1}", barometricPressure, (altimeterUnits == AltimeterUnits.Hg) ? "IN":  "MB"), barometricPressureFont, greenBrush,
                barometricPressureBox, baroPressureFormat);

            //draw altitude counter
            var pointA = new Point(546, 221);
            var pointB = new Point(546, 233);
            var pointC = new Point(539, 239);
            var pointD = new Point(546, 244);
            var pointE = new Point(546, 255);
            var pointF = new Point(581, 255);
            var pointG = new Point(581, 265);
            var pointH = new Point(608, 265);
            var pointI = new Point(608, 210);
            var pointJ = new Point(581, 210);
            var pointK = new Point(581, 221);
            whitePen.Width = 1;
            var points = new[] {pointA, pointB, pointC, pointD, pointE, pointF, pointG, pointH, pointI, pointJ, pointK};
            g.FillPolygon(Brushes.Black, points);
            g.DrawPolygon(whitePen, points);


            //draw altitude counter digits
            var altitudeString = string.Format("{0:00000}", indicatedAltitude);
            if (altitudeString[0] == '-') altitudeString = altitudeString.Substring(1, altitudeString.Length - 1);

            var tenThousandsDigitBitmap = SingleDigitBitmapFactory.GetSingleDigitBitmap(Int32.Parse(new String(altitudeString[0], 1)));
            var thousandsDigitBitmap = SingleDigitBitmapFactory.GetSingleDigitBitmap(Int32.Parse(new String(altitudeString[1], 1)));
            var hundredsDigitBitmap = SingleDigitBitmapFactory.GetSingleDigitBitmap(Int32.Parse(new String(altitudeString[2], 1)));
            var tensDigitFrac = (Int32.Parse(new String(altitudeString[4], 1))/10.0f);
            float tensDigitsVal = (Int32.Parse(new String(altitudeString[3], 1)));
            var tensDigits = tensDigitsVal + tensDigitFrac;
            var tensDigitsBitmap = SingleDigitBitmapFactory.GetSingleDigitBitmap(tensDigits, true);
            var onesDigitBitmap = SingleDigitBitmapFactory.GetSingleDigitBitmap(0);

            var tenThousandsDigitRectangle = new Rectangle(new Point(547, 230),
                new Size(tenThousandsDigitBitmap.Width,
                    tenThousandsDigitBitmap.Height));
            var thousandsDigitRectangle = new Rectangle(new Point(558, 230),
                new Size(thousandsDigitBitmap.Width, thousandsDigitBitmap.Height));
            var hundredsDigitRectangle = new Rectangle(new Point(569, 230),
                new Size(hundredsDigitBitmap.Width, hundredsDigitBitmap.Height));
            var tensDigitsRectangle = new Rectangle(new Point(581, 218), new Size(tensDigitsBitmap.Width, 47));
            var onesDigitRectangle = new Rectangle(new Point(592, 230),
                new Size(onesDigitBitmap.Width, onesDigitBitmap.Height));

            g.DrawImageFast(tenThousandsDigitBitmap, tenThousandsDigitRectangle);
            g.DrawImageFast(thousandsDigitBitmap, thousandsDigitRectangle);
            g.DrawImageFast(hundredsDigitBitmap, hundredsDigitRectangle);
            g.DrawImageFast(tensDigitsBitmap, tensDigitsRectangle);
            g.DrawImageFast(onesDigitBitmap, onesDigitRectangle);

            var altitudeFeetTextRectangle = new Rectangle(new Point(540, 210), new Size(45, 10));
            var altitudeFeetTextFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            altitudeFeetTextFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;
            var path = new GraphicsPath();
            path.AddString("FEET", altitudeFeetTextFont.FontFamily, (int) altitudeFeetTextFont.Style,
                altitudeFeetTextFont.SizeInPoints, altitudeFeetTextRectangle, altitudeFeetTextFormat);
            g.DrawPathFast(blackPen, path);
            g.FillPathFast(Brushes.White, path);
        }
    }
}