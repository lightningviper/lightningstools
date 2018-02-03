using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiPitchBarBitmapGenerator
    {
        private static Font _labelFont = new Font("Lucida Console", 10, FontStyle.Bold);

        internal static Bitmap GenerateAdiPitchBarBitmap()
        {
            const int positivePitchLineExtenderHeightPixels = 14;
            const int negativePitchLineExtenderHeightPixels = 13;
            const int negativePitchInsideDashWidthPixels = 13;
            const int negativePitchDashGapPixels = 13;
            const int negativePitchMiddleDashWidthPixels = 11;
            const int negativePitchOuterDashWidthPixels = 11;

            const int majorPositivePitchLineWidthPixels = 61;
            const int minorPositivePitchLineWidthPixels = 39;
            const int verticalDistanceBetweenPitchLines = 25;
            const int degreesBetweenTicks = 5;
            const int pixelsSeparationPerDegreeOfPitch = verticalDistanceBetweenPitchLines / degreesBetweenTicks;
            const int horizontalOffsetPixelsFromCenterLine = 28;

            var boundingRectangle = new Rectangle(0, 0, 250, (pixelsSeparationPerDegreeOfPitch * 360) + 2);
            //Rectangle boundingRectangle = new Rectangle(0, 0, (pixelsSeparationPerDegreeOfPitch * 360) + 2,(pixelsSeparationPerDegreeOfPitch * 360) + 2);
            var positiveBoundingRectangle = new Rectangle(new Point(0, 0),
                new Size(boundingRectangle.Width, boundingRectangle.Height / 2));
            var negativeBoundingRectangle = new Rectangle(new Point(0, positiveBoundingRectangle.Bottom),
                new Size(boundingRectangle.Width,
                    boundingRectangle.Height -
                    positiveBoundingRectangle.Height));

            var bitmap = new Bitmap(boundingRectangle.Width, boundingRectangle.Height, PixelFormat.Format16bppRgb565);
            bitmap.MakeTransparent();
            var centerPointX = boundingRectangle.Left + (boundingRectangle.Width / 2);
            var whitePen = new Pen(Color.White);
            Brush whiteBrush = new SolidBrush(Color.White);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                whitePen.Width = 2;

                //produce positive pitch lines
                for (var i = 5; i < 90; i += degreesBetweenTicks)
                {
                    var pitchLineCenterY = positiveBoundingRectangle.Bottom - (i * pixelsSeparationPerDegreeOfPitch);
                    var pitchLineWidthPixels = minorPositivePitchLineWidthPixels;
                    if (i % 10 == 0)
                    {
                        pitchLineWidthPixels = majorPositivePitchLineWidthPixels;

                        //print the label for this pitch line
                        var labelFormat = new StringFormat(StringFormatFlags.NoWrap)
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawStringFast(string.Format("{0:0}", i), _labelFont, whiteBrush,
                            new Point(0, pitchLineCenterY + 7), labelFormat);
                    }
                    var lhsPitchLineMinX = centerPointX - pitchLineWidthPixels - horizontalOffsetPixelsFromCenterLine;
                    var lhsPitchLineMaxX = centerPointX - horizontalOffsetPixelsFromCenterLine;
                    var lhsPitchLineExtenderBottomY = pitchLineCenterY + positivePitchLineExtenderHeightPixels;

                    var rhsPitchLineMinX = centerPointX + horizontalOffsetPixelsFromCenterLine;
                    var rhsPitchLineMaxX = centerPointX + pitchLineWidthPixels + horizontalOffsetPixelsFromCenterLine;
                    var rhsPitchLineExtenderBottomY = pitchLineCenterY + positivePitchLineExtenderHeightPixels;

                    g.DrawLineFast(whitePen, lhsPitchLineMinX, pitchLineCenterY, lhsPitchLineMaxX, pitchLineCenterY);
                    //draw LHS pitch line
                    g.DrawLineFast(whitePen, lhsPitchLineMinX, pitchLineCenterY, lhsPitchLineMinX,
                        lhsPitchLineExtenderBottomY); //draw LHS pitch line extender

                    g.DrawLineFast(whitePen, rhsPitchLineMinX, pitchLineCenterY, rhsPitchLineMaxX, pitchLineCenterY);
                    //draw RHS pitch line
                    g.DrawLineFast(whitePen, rhsPitchLineMaxX, pitchLineCenterY, rhsPitchLineMaxX,
                        rhsPitchLineExtenderBottomY); //draw RHS pitch line extender
                }

                //draw the zenith symbol
                whitePen.Width = 1;
                const int zenithSymbolWidth = 25;
                var zenithRectangle =
                    new Rectangle(
                        new Point(((positiveBoundingRectangle.Width - zenithSymbolWidth) / 2),
                            (positiveBoundingRectangle.Bottom - (90 * pixelsSeparationPerDegreeOfPitch) -
                             (zenithSymbolWidth / 2))), new Size(zenithSymbolWidth, zenithSymbolWidth));
                g.DrawEllipse(whitePen, zenithRectangle);
                g.DrawLineFast(whitePen, zenithRectangle.Left + (zenithRectangle.Width / 2), zenithRectangle.Bottom,
                    zenithRectangle.Left + (zenithRectangle.Width / 2), zenithRectangle.Bottom + 10);

                whitePen.Width = 2;
                //produce negative pitch lines
                for (var i = -5; i > -90; i -= degreesBetweenTicks)
                {
                    var pitchLineCenterY = negativeBoundingRectangle.Top +
                                           ((Math.Abs(i) * pixelsSeparationPerDegreeOfPitch));
                    var pitchLineExtenderY = pitchLineCenterY - negativePitchLineExtenderHeightPixels;
                    var lhsPitchLineInsideDashMinX = centerPointX - negativePitchInsideDashWidthPixels -
                                                     horizontalOffsetPixelsFromCenterLine;
                    var lhsPitchLineInsideDashMaxX = centerPointX - horizontalOffsetPixelsFromCenterLine;
                    var lhsPitchLineMiddleDashMinX = lhsPitchLineInsideDashMinX - negativePitchDashGapPixels -
                                                     negativePitchMiddleDashWidthPixels;
                    var lhsPitchLineMiddleDashMaxX = lhsPitchLineInsideDashMinX - negativePitchDashGapPixels;
                    var lhsPitchLineOuterDashMinX = lhsPitchLineMiddleDashMinX - negativePitchDashGapPixels -
                                                    negativePitchOuterDashWidthPixels;
                    var lhsPitchLineOuterDashMaxX = lhsPitchLineMiddleDashMinX - negativePitchDashGapPixels;

                    var rhsPitchLineInsideDashMinX = centerPointX + horizontalOffsetPixelsFromCenterLine;
                    var rhsPitchLineInsideDashMaxX = centerPointX + negativePitchInsideDashWidthPixels +
                                                     horizontalOffsetPixelsFromCenterLine;
                    var rhsPitchLineMiddleDashMinX = rhsPitchLineInsideDashMaxX + negativePitchDashGapPixels;
                    var rhsPitchLineMiddleDashMaxX = rhsPitchLineInsideDashMaxX + negativePitchDashGapPixels +
                                                     negativePitchMiddleDashWidthPixels;
                    var rhsPitchLineOuterDashMinX = rhsPitchLineMiddleDashMaxX + negativePitchDashGapPixels;
                    var rhsPitchLineOuterDashMaxX = rhsPitchLineMiddleDashMaxX + negativePitchDashGapPixels +
                                                    negativePitchOuterDashWidthPixels;

                    g.DrawLineFast(whitePen, lhsPitchLineInsideDashMinX, pitchLineCenterY, lhsPitchLineInsideDashMaxX,
                        pitchLineCenterY); //draw LHS inside dash
                    g.DrawLineFast(whitePen, lhsPitchLineMiddleDashMinX, pitchLineCenterY, lhsPitchLineMiddleDashMaxX,
                        pitchLineCenterY); //draw LHS middle dash
                    if (i % 10 == 0)
                    {
                        //if a major pitch line, then draw the LHS outer dash
                        g.DrawLineFast(whitePen, lhsPitchLineOuterDashMinX, pitchLineCenterY, lhsPitchLineOuterDashMaxX,
                            pitchLineCenterY);

                        //print the label for this pitch line

                        var labelFormat = new StringFormat(StringFormatFlags.NoWrap)
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawStringFast(string.Format("{0:0}", Math.Abs(i)), _labelFont, whiteBrush,
                            new Point(0, pitchLineCenterY), labelFormat);
                    }
                    //draw the LHS pitch extender line
                    g.DrawLineFast(whitePen, lhsPitchLineInsideDashMaxX, pitchLineCenterY, lhsPitchLineInsideDashMaxX,
                        pitchLineExtenderY);

                    g.DrawLineFast(whitePen, rhsPitchLineInsideDashMinX, pitchLineCenterY, rhsPitchLineInsideDashMaxX,
                        pitchLineCenterY); //draw RHS inside dash
                    g.DrawLineFast(whitePen, rhsPitchLineMiddleDashMinX, pitchLineCenterY, rhsPitchLineMiddleDashMaxX,
                        pitchLineCenterY); //draw RHS middle dash
                    if (i % 10 == 0)
                    {
                        //if a major pitch line, then draw the RHS outer dash
                        g.DrawLineFast(whitePen, rhsPitchLineOuterDashMinX, pitchLineCenterY, rhsPitchLineOuterDashMaxX,
                            pitchLineCenterY);
                    }
                    //draw the RHS pitch extender line
                    g.DrawLineFast(whitePen, rhsPitchLineInsideDashMinX, pitchLineCenterY, rhsPitchLineInsideDashMinX,
                        pitchLineExtenderY);
                }

                //draw the nadir symbol
                const int nadirSymbolWidth = 25;
                whitePen.Width = 1;
                var nadirRectangle =
                    new Rectangle(
                        new Point(((negativeBoundingRectangle.Width - nadirSymbolWidth) / 2),
                            (negativeBoundingRectangle.Top + (90 * pixelsSeparationPerDegreeOfPitch) -
                             (nadirSymbolWidth / 2))), new Size(nadirSymbolWidth, nadirSymbolWidth));
                g.FillEllipse(whiteBrush, nadirRectangle);
                g.DrawLineFast(whitePen, nadirRectangle.Left + (nadirRectangle.Width / 2), nadirRectangle.Top,
                    nadirRectangle.Left + (nadirRectangle.Width / 2), nadirRectangle.Top - 10);
            }

            return bitmap;
        }
    }
}