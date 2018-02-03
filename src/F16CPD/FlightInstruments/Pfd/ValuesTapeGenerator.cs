using System;
using System.Collections.Generic;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;
using Common.UI.Layout;

namespace F16CPD.FlightInstruments.Pfd
{
    internal struct TapeEdgeColoringInstruction
    {
        public Color Color;
        public int MaxVal;
        public int MinVal;
    }
    internal class ValuesTapeGenerator
    {
        internal static Bitmap GenerateValuesTape(Color positiveBackgroundColor, Color positiveForegroundColor,
            Color negativeBackgroundColor, Color negativeForegroundColor,
            int majorUnitInterval, int minorUnitInterval,
            int majorUnitLineLengthInPixels, int minorUnitLineLengthInPixels,
            bool negativeUnitsLabelled, int verticalSeparationBetweenTicksInPixels,
            int scaleMaxVal, int scaleMinVal, int tapeWidthInPixels,
            HAlignment ticsAlignment, int textPaddingPixels, Font majorUnitFont,
            HAlignment textAlignment, bool negativeUnitsHaveNegativeSign,
            IEnumerable<TapeEdgeColoringInstruction> coloringInstructions)
        {
            var tapeHeightInPixels = ((((scaleMaxVal - scaleMinVal)/minorUnitInterval)*
                                       verticalSeparationBetweenTicksInPixels));
            var positiveRange = scaleMaxVal;
            if (scaleMinVal > 0) positiveRange = scaleMaxVal - scaleMinVal;
            var positiveRegionHeightInPixels = ((positiveRange/minorUnitInterval)*verticalSeparationBetweenTicksInPixels);
            var negativeRange = Math.Abs(scaleMinVal);
            if (scaleMaxVal <= 0)
            {
                negativeRange = Math.Abs(scaleMaxVal - scaleMinVal);
            }
            if (scaleMinVal >= 0) negativeRange = 0;
            var negativeRegionHeightInPixels = (negativeRange/minorUnitInterval)*verticalSeparationBetweenTicksInPixels;
            var toReturn = new Bitmap(tapeWidthInPixels, tapeHeightInPixels, PixelFormat.Format16bppRgb565);
            toReturn.MakeTransparent();
            var positiveRegionBoundingRectangle = new Rectangle(new Point(0, 0),
                new Size(toReturn.Width, positiveRegionHeightInPixels));
            var negativeRegionBoundingRectangle = new Rectangle(new Point(0, positiveRegionBoundingRectangle.Bottom),
                new Size(toReturn.Width, negativeRegionHeightInPixels));
            var baseFontSize = majorUnitFont.SizeInPoints;
            using (var g = Graphics.FromImage(toReturn))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var origTransform = g.Transform;
                Brush negativeBrush = new SolidBrush(negativeBackgroundColor);
                Brush positiveBrush = new SolidBrush(positiveBackgroundColor);
                var blackPen = new Pen(Color.Black);
                var positiveForegroundPen = new Pen(positiveForegroundColor);
                var negativeForegroundPen = new Pen(negativeForegroundColor);
                blackPen.Width = 2;
                Brush positiveForegroundBrush = new SolidBrush(positiveForegroundColor);
                Brush negativeForegroundBrush = new SolidBrush(negativeForegroundColor);

                //color negative portion of tape
                g.FillRectangle(negativeBrush, negativeRegionBoundingRectangle);
                //color positive portion of tape
                g.FillRectangle(positiveBrush, positiveRegionBoundingRectangle);

                //draw black line between negative and positive portion of tape
                g.DrawLineFast(blackPen, negativeRegionBoundingRectangle.Left, negativeRegionBoundingRectangle.Top,
                    negativeRegionBoundingRectangle.Right, negativeRegionBoundingRectangle.Top);
                if (scaleMaxVal >= 0)
                {
                    var positiveScaleMin = 0;
                    if (scaleMinVal > 0) positiveScaleMin = scaleMinVal;
                    //draw positive unit marks and numbers
                    for (var i = positiveScaleMin; i <= scaleMaxVal; i += minorUnitInterval)
                    {
                        if ((i%minorUnitInterval == 0) && (i%majorUnitInterval != 0)) //this is a minor unit
                        {
                            //draw minor unit tick mark
                            if (ticsAlignment == HAlignment.Right)
                            {
                                //draw tick on the right hand side
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Right - minorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Right,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Left)
                            {
                                //draw tic on the left hand side
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Left,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Left + minorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Center)
                            {
                                //draw tic in the center 
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Left +
                                    ((positiveRegionBoundingRectangle.Width - minorUnitLineLengthInPixels)/2),
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Left +
                                    ((positiveRegionBoundingRectangle.Width - minorUnitLineLengthInPixels)/2) +
                                    minorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                        }
                        else if (i%majorUnitInterval == 0) //this is a major unit
                        {
                            //draw major unit tick mark
                            if (ticsAlignment == HAlignment.Right)
                            {
                                //draw tic on the right hand side
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Right - majorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Right,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Left)
                            {
                                //draw tic on the left hand side
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Left,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Left + majorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Center)
                            {
                                //draw tic in the center
                                g.DrawLineFast(positiveForegroundPen,
                                    positiveRegionBoundingRectangle.Left +
                                    ((positiveRegionBoundingRectangle.Width - majorUnitLineLengthInPixels)/2),
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels),
                                    positiveRegionBoundingRectangle.Left +
                                    ((positiveRegionBoundingRectangle.Width - majorUnitLineLengthInPixels)/2) +
                                    majorUnitLineLengthInPixels,
                                    positiveRegionBoundingRectangle.Bottom -
                                    (((i - positiveScaleMin)/minorUnitInterval)*
                                     verticalSeparationBetweenTicksInPixels));
                            }
                            var majorUnitTextBoundingRectangle = Rectangle.Empty;
                            if (ticsAlignment == HAlignment.Right)
                            {
                                //tic is on the right, so draw major unit text to left of tic 
                                majorUnitTextBoundingRectangle = new Rectangle(
                                    new Point(0,
                                        positiveRegionBoundingRectangle.Bottom - (
                                            ((i - positiveScaleMin)/
                                             minorUnitInterval)*
                                            verticalSeparationBetweenTicksInPixels
                                            ) -
                                        verticalSeparationBetweenTicksInPixels
                                        ),
                                    new Size(
                                        positiveRegionBoundingRectangle.Width - majorUnitLineLengthInPixels -
                                        textPaddingPixels, verticalSeparationBetweenTicksInPixels*2)
                                    );
                            }
                            else if (ticsAlignment == HAlignment.Left)
                            {
                                //tic is on the left, so draw major unit text to right of tic
                                majorUnitTextBoundingRectangle = new Rectangle(
                                    new Point(majorUnitLineLengthInPixels + textPaddingPixels,
                                        positiveRegionBoundingRectangle.Bottom - (
                                            ((i - positiveScaleMin)/
                                             minorUnitInterval)*
                                            verticalSeparationBetweenTicksInPixels
                                            ) -
                                        verticalSeparationBetweenTicksInPixels
                                        ),
                                    new Size(
                                        positiveRegionBoundingRectangle.Width - majorUnitLineLengthInPixels -
                                        textPaddingPixels, verticalSeparationBetweenTicksInPixels*2)
                                    );
                            }
                            else if (ticsAlignment == HAlignment.Center)
                            {
                                var lineLength = majorUnitLineLengthInPixels;
                                if (majorUnitLineLengthInPixels == 0) lineLength = minorUnitLineLengthInPixels;
                                lineLength += 8;

                                majorUnitTextBoundingRectangle = new Rectangle(
                                    new Point((positiveRegionBoundingRectangle.Width - lineLength)/2,
                                        positiveRegionBoundingRectangle.Bottom - (
                                            ((i - positiveScaleMin)/
                                             minorUnitInterval)*
                                            verticalSeparationBetweenTicksInPixels
                                            ) -
                                        verticalSeparationBetweenTicksInPixels
                                        ),
                                    new Size(lineLength, verticalSeparationBetweenTicksInPixels*2)
                                    );
                            }

                            var majorUnitString = String.Empty;

                            if (i.ToString().Length > 3) // num >= 1000
                            {
                                majorUnitString = String.Format("{0:0000}", i);
                                majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize - 2,
                                    majorUnitFont.Style);
                            }
                            else if (i.ToString().Length > 2) // num >= 100
                            {
                                majorUnitString = String.Format("{0:000}", i);
                                majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                    majorUnitFont.Style);
                            }
                            else if (i.ToString().Length > 1) // num >= 10
                            {
                                majorUnitString = String.Format("{0:00}", i);
                                majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                    majorUnitFont.Style);
                            }
                            else if (i.ToString().Length == 1) // num between 1 and 10
                            {
                                majorUnitString = String.Format("{0:0}", i);
                                majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                    majorUnitFont.Style);
                            }
                            if (i == 0) majorUnitString = "0";
                            var majorUnitStringFormat = new StringFormat(StringFormatFlags.NoWrap);
                            if (textAlignment == HAlignment.Right)
                            {
                                majorUnitStringFormat.Alignment = StringAlignment.Far;
                            }
                            else if (textAlignment == HAlignment.Left)
                            {
                                majorUnitStringFormat.Alignment = StringAlignment.Near;
                            }
                            else if (textAlignment == HAlignment.Center)
                            {
                                majorUnitStringFormat.Alignment = StringAlignment.Center;
                            }
                            majorUnitStringFormat.LineAlignment = StringAlignment.Center;
                            g.TranslateTransform(0, 2);
                            g.DrawStringFast(majorUnitString, majorUnitFont, positiveForegroundBrush,
                                majorUnitTextBoundingRectangle, majorUnitStringFormat);
                            g.Transform = origTransform;
                        } //end else
                    } //end for
                }
                if (scaleMinVal <= 0)
                {
                    //draw negative unit marks and numbers
                    for (var i = 0 - minorUnitInterval; i >= scaleMinVal; i -= minorUnitInterval)
                    {
                        if ((i%minorUnitInterval == 0) && (i%majorUnitInterval != 0)) //this is a minor unit
                        {
                            if (ticsAlignment == HAlignment.Right)
                            {
                                //draw minor unit tic mark on right hand side
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Right - minorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Right,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)
                                    );
                            }
                            else if (ticsAlignment == HAlignment.Left)
                            {
                                //draw minor unit tic mark on left hand side
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Left,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Left + minorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)
                                    );
                            }
                            else if (ticsAlignment == HAlignment.Center)
                            {
                                //draw minor unit tic mark in center
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Left +
                                    ((negativeRegionBoundingRectangle.Width - minorUnitLineLengthInPixels)/2),
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Left +
                                    ((negativeRegionBoundingRectangle.Width - minorUnitLineLengthInPixels)/2) +
                                    minorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)
                                    );
                            }
                        }
                        else if (i%majorUnitInterval == 0) //this is a major unit
                        {
                            if (ticsAlignment == HAlignment.Right)
                            {
                                //draw major unit tick mark on right hand side
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Right - majorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Right,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Left)
                            {
                                //draw major unit tick mark on left hand side
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Left,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Left + majorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels));
                            }
                            else if (ticsAlignment == HAlignment.Center)
                            {
                                //draw major unit tick mark in center
                                g.DrawLineFast(negativeForegroundPen,
                                    negativeRegionBoundingRectangle.Left +
                                    ((negativeRegionBoundingRectangle.Width - majorUnitLineLengthInPixels)/2),
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels),
                                    negativeRegionBoundingRectangle.Left +
                                    ((negativeRegionBoundingRectangle.Width - majorUnitLineLengthInPixels)/2) +
                                    majorUnitLineLengthInPixels,
                                    negativeRegionBoundingRectangle.Top +
                                    ((Math.Abs(i)/minorUnitInterval)*verticalSeparationBetweenTicksInPixels));
                            }
                            if (negativeUnitsLabelled) //if we're supposed to add text labels to negative units...
                            {
                                var majorUnitTextBoundingRectangle = Rectangle.Empty;
                                //draw major unit text
                                if (ticsAlignment == HAlignment.Right)
                                {
                                    //tics are on the right so draw text on the left
                                    majorUnitTextBoundingRectangle = new Rectangle(
                                        new Point(0,
                                            negativeRegionBoundingRectangle.Top + (
                                                ((Math.Abs(i) - 1)/
                                                 minorUnitInterval)*
                                                verticalSeparationBetweenTicksInPixels
                                                )
                                            ),
                                        new Size(
                                            negativeRegionBoundingRectangle.Width - majorUnitLineLengthInPixels -
                                            textPaddingPixels, verticalSeparationBetweenTicksInPixels*2)
                                        );
                                }
                                else if (ticsAlignment == HAlignment.Left)
                                {
                                    //tics are on the left so draw text to the right of them
                                    majorUnitTextBoundingRectangle = new Rectangle(
                                        new Point(majorUnitLineLengthInPixels + textPaddingPixels,
                                            negativeRegionBoundingRectangle.Top + (
                                                ((Math.Abs(i) - 1)/
                                                 minorUnitInterval)*
                                                verticalSeparationBetweenTicksInPixels
                                                )
                                            ),
                                        new Size(
                                            negativeRegionBoundingRectangle.Width - majorUnitLineLengthInPixels -
                                            textPaddingPixels, verticalSeparationBetweenTicksInPixels*2)
                                        );
                                }
                                else if (ticsAlignment == HAlignment.Center)
                                {
                                    var lineLength = majorUnitLineLengthInPixels;
                                    if (majorUnitLineLengthInPixels == 0) lineLength = minorUnitLineLengthInPixels;
                                    //tic is in the center so draw text in the center
                                    majorUnitTextBoundingRectangle = new Rectangle(
                                        new Point(
                                            ((negativeRegionBoundingRectangle.Width - lineLength)/2) - 15,
                                            negativeRegionBoundingRectangle.Top + (
                                                ((Math.Abs(i) -
                                                  1/minorUnitInterval)*
                                                 verticalSeparationBetweenTicksInPixels
                                                    )
                                                )),
                                        new Size(negativeRegionBoundingRectangle.Width,
                                            verticalSeparationBetweenTicksInPixels*2)
                                        );
                                }
                                //*****

                                var majorUnitString = String.Empty;
                                var majorUnitVal = i;
                                if (!negativeUnitsHaveNegativeSign && majorUnitVal < 0)
                                {
                                    majorUnitVal = Math.Abs(majorUnitVal);
                                }
                                if (Math.Abs(i).ToString().Length > 3) // num >= 1000
                                {
                                    majorUnitString = String.Format("{0:0000}", majorUnitVal);
                                    majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize - 2,
                                        majorUnitFont.Style);
                                }
                                else if (Math.Abs(i).ToString().Length > 2) // num >= 100
                                {
                                    majorUnitString = String.Format("{0:000}", majorUnitVal);
                                    majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                        majorUnitFont.Style);
                                }
                                else if (Math.Abs(i).ToString().Length > 1) // num >= 10
                                {
                                    majorUnitString = String.Format("{0:00}", majorUnitVal);
                                    majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                        majorUnitFont.Style);
                                }
                                else if (Math.Abs(i).ToString().Length == 1) // num between 1 and 10
                                {
                                    majorUnitString = String.Format("{0:0}", majorUnitVal);
                                    majorUnitFont = new Font(majorUnitFont.FontFamily.Name, baseFontSize,
                                        majorUnitFont.Style);
                                }
                                if (i == 0) majorUnitString = "0";
                                var majorUnitStringFormat =
                                    new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
                                if (textAlignment == HAlignment.Right)
                                {
                                    majorUnitStringFormat.Alignment = StringAlignment.Far;
                                }
                                else if (textAlignment == HAlignment.Left)
                                {
                                    majorUnitStringFormat.Alignment = StringAlignment.Near;
                                }
                                else if (textAlignment == HAlignment.Center)
                                {
                                    majorUnitStringFormat.Alignment = StringAlignment.Center;
                                }
                                majorUnitStringFormat.LineAlignment = StringAlignment.Center;

                                g.TranslateTransform(0, 2);
                                g.DrawStringFast(majorUnitString, majorUnitFont, negativeForegroundBrush,
                                    majorUnitTextBoundingRectangle, majorUnitStringFormat);
                                g.ResetTransform();
                                //*****
                            }
                        } //end else
                    } //end for         
                }
                if (coloringInstructions != null)
                {
                    foreach (var instruction in coloringInstructions)
                    {
                        var color = instruction.Color;
                        Brush colorBrush = new SolidBrush(color);
                        Rectangle rect;
                        if (instruction.MinVal > 0)
                        {
                            rect = new Rectangle(
                                new Point(
                                    positiveRegionBoundingRectangle.Right - 5,
                                    positiveRegionBoundingRectangle.Bottom -
                                    ((instruction.MaxVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)
                                    ),
                                new Size(
                                    5,
                                    (positiveRegionBoundingRectangle.Bottom -
                                     ((instruction.MinVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)) -
                                    (positiveRegionBoundingRectangle.Bottom -
                                     ((instruction.MaxVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels))
                                    )
                                );
                        }
                        else
                        {
                            rect = new Rectangle(
                                new Point(
                                    negativeRegionBoundingRectangle.Right - 5,
                                    (negativeRegionBoundingRectangle.Top +
                                     ((instruction.MinVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels))
                                    ),
                                new Size(
                                    5,
                                    (negativeRegionBoundingRectangle.Top +
                                     ((instruction.MaxVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels)) -
                                    (negativeRegionBoundingRectangle.Top +
                                     ((instruction.MinVal/minorUnitInterval)*verticalSeparationBetweenTicksInPixels))
                                    )
                                );
                        }
                        g.FillRectangle(colorBrush, rect);
                    }
                }
            }
            return toReturn;
        }
    }
}