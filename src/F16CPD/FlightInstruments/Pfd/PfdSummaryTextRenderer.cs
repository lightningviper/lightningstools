using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class PfdSummaryTextRenderer
    {

        internal static void DrawPfdSummaryText(Graphics g, float groundSpeedInDecimalFeetPerSecond, float machNumber, 
            float automaticLowAltitudeWarningInFeet, float altitudeAboveGroundLevelInDecimalFeet, bool radarAltimeterOffFlag
            )
        {
            /**************************
             * TEXT AT BOTTOM
             **************************/
            var whitePen = new Pen(Color.White);
            var blackPen = new Pen(Color.Black);

            var groundSpeedFont = new Font("Lucida Console", 17, FontStyle.Bold);
            var machFont = new Font("Lucida Console", 17, FontStyle.Bold);
            var alowFont = new Font("Lucida Console", 17, FontStyle.Bold);
            var aglFont = new Font("Lucida Console", 17, FontStyle.Bold);


            var groundSpeed = string.Format("{0:GS 000}", groundSpeedInDecimalFeetPerSecond/
                                                          Common.Math.Constants.FPS_PER_KNOT);
            var mach = string.Format("{0:M 0.00}", machNumber);
            var alow = string.Format("{0:00000}", automaticLowAltitudeWarningInFeet);
            var agl = altitudeAboveGroundLevelInDecimalFeet;
            agl = (int) (10.0f*Math.Floor(agl/10.0f));
            var aglString = string.Format("{0:00000}", agl);
            aglString = aglString.Substring(0, 4) + "0";

            if (radarAltimeterOffFlag)
            {
                aglString = "";
            }

            var groundSpeedRectangle = new Rectangle(new Point(7, 430), new Size(85, 18));
            var machRectangle = new Rectangle(new Point(7, 455), new Size(85, 18));
            var alowTextRectangle = new Rectangle(new Point(463, 429), new Size(59, 16));
            var aglTextRectangle = new Rectangle(new Point(475, 456), new Size(43, 16));
            var alowRectangle = new Rectangle(new Point(537, 430), new Size(63, 16));
            var aglRectangle = new Rectangle(new Point(537, 457), new Size(63, 16));
            var aglOutlineRectangle = new Rectangle(new Point(532, 453), new Size(74, 24));

            var groundSpeedFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            groundSpeedFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;

            var machFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            machFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;

            var alowFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };
            alowFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;

            var aglFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };
            aglFormat.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;

            var path = new GraphicsPath();

            path.AddString("AGL", aglFont.FontFamily, (int) aglFont.Style, aglFont.SizeInPoints, aglTextRectangle,
                aglFormat);
            path.AddString(aglString, aglFont.FontFamily, (int) aglFont.Style, aglFont.SizeInPoints, aglRectangle,
                aglFormat);

            path.AddString("ALOW", alowFont.FontFamily, (int) alowFont.Style, alowFont.SizeInPoints, alowTextRectangle,
                alowFormat);
            path.AddString(alow, alowFont.FontFamily, (int) alowFont.Style, alowFont.SizeInPoints, alowRectangle,
                alowFormat);
            g.DrawRectangleFast(whitePen, aglOutlineRectangle);

            path.AddString(mach, machFont.FontFamily, (int) machFont.Style, machFont.SizeInPoints, machRectangle,
                machFormat);
            path.AddString(groundSpeed, groundSpeedFont.FontFamily, (int) groundSpeedFont.Style,
                groundSpeedFont.SizeInPoints, groundSpeedRectangle, groundSpeedFormat);
            blackPen.Width = 1;
            whitePen.Width = 1;
            g.DrawPathFast(blackPen, path);
            g.FillPathFast(Brushes.White, path);
        }
    }
}