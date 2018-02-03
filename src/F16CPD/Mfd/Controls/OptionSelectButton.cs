using System;
using Common.Drawing;
using System.Linq;
using Common.Imaging;
using Common.UI.Layout;

namespace F16CPD.Mfd.Controls
{
    public class OptionSelectButton : MomentaryButtonMfdInputControl
    {
        protected MfdMenuPage _page;

        protected OptionSelectButton()
        {
        }

        public OptionSelectButton(MfdMenuPage page)
            : this()
        {
            _page = page;
            ForeColor = Color.FromArgb(0, 255, 0);
            BackColor = Color.Black;
            TextFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold, GraphicsUnit.Point);
            TextHAlignment = HAlignment.Center;
            TextVAlignment = VAlignment.Center;
            TriangleLegLength = 15;
            Visible = true;
        }

        public int TriangleLegLength { get; set; }
        public string LabelText { get; set; }
        public bool InvertLabelText { get; set; }
        public Size LabelSize { get; set; }
        public Point LabelLocation { get; set; }
        public HAlignment TextHAlignment { get; set; }
        public VAlignment TextVAlignment { get; set; }

        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Font TextFont { get; set; }
        public float PositionNumber { get; set; }

        public MfdMenuPage Page
        {
            get { return _page; }
        }

        public string FunctionName { get; set; }
        public bool Visible { get; set; }

        public void DrawLabel(Graphics g)
        {
            var origTransform = g.Transform;
            g.TranslateTransform(LabelLocation.X, LabelLocation.Y);
            var text = LabelText;
            var labelSize = LabelSize;
            var font = TextFont;

            var foreColor = InvertLabelText ? BackColor : ForeColor;
            var backColor = InvertLabelText ? ForeColor : BackColor;

            if (foreColor == Color.Transparent) foreColor = Color.Black;
            var forecolorBrush = new SolidBrush(foreColor);
            var backcolorBrush = new SolidBrush(backColor);

            var backgroundRectangle = new Rectangle(new Point(0, 0), labelSize);

            var maxTextAreaWidth = backgroundRectangle.Width;
            var numLinesOfText = 1 + text.Count(thisChar => thisChar == '\n');
            var textSize = new Size(maxTextAreaWidth, (font.Height*numLinesOfText));
            var textFormat = new StringFormat
                                 {
                                     Trimming = StringTrimming.EllipsisCharacter,
                                     LineAlignment = StringAlignment.Center,
                                     Alignment = StringAlignment.Center
                                 };
            string textToMeasure = text;
            var minTextSize = g.MeasureString(textToMeasure, font, maxTextAreaWidth, textFormat);
            if (textToMeasure == @"\/" || textToMeasure == "^")
            {
                minTextSize.Width = 16;
                minTextSize.Height = 16;
            }
            textSize.Width = (int)Math.Ceiling(minTextSize.Width);
            textSize.Height = (int)minTextSize.Height;

            var textX = 0;
            var textY = 0;
            if (TextVAlignment == VAlignment.Top)
            {
                textFormat.LineAlignment = StringAlignment.Near;
                textY = backgroundRectangle.Top;
            }
            else if (TextVAlignment == VAlignment.Bottom)
            {
                textFormat.LineAlignment = StringAlignment.Far;
                textY = backgroundRectangle.Bottom - textSize.Height;
            }
            else if (TextVAlignment == VAlignment.Center)
            {
                textFormat.LineAlignment = StringAlignment.Center;
                textY = backgroundRectangle.Top +
                        (((backgroundRectangle.Bottom - backgroundRectangle.Top) - textSize.Height)/2);
            }

            if (TextHAlignment == HAlignment.Left)
            {
                textFormat.Alignment = StringAlignment.Near;
                textX = backgroundRectangle.Left;
            }
            else if (TextHAlignment == HAlignment.Right)
            {
                textFormat.Alignment = StringAlignment.Far;
                textX = backgroundRectangle.Right - textSize.Width;
            }
            else if (TextHAlignment == HAlignment.Center)
            {
                textFormat.Alignment = StringAlignment.Center;
                textX = backgroundRectangle.Left + ((backgroundRectangle.Width - textSize.Width)/2);
            }
            var textBoundingRectangle = new Rectangle(new Point(textX, textY), textSize);
            if (!String.IsNullOrEmpty(text.Trim()))
            {
                g.FillRectangle(backcolorBrush, textBoundingRectangle);
                if (text.Trim() == "^" || text.Trim() == @"\/")
                {
                    var xCoordinate = 0;
                    var yCoordinate = 0;

                    if (TextHAlignment == HAlignment.Left)
                    {
                        xCoordinate = 0;
                    }
                    else if (TextHAlignment == HAlignment.Right)
                    {
                        xCoordinate = backgroundRectangle.Width - TriangleLegLength;
                    }
                    else if (TextHAlignment == HAlignment.Center)
                    {
                        xCoordinate = ((backgroundRectangle.Width - TriangleLegLength)/2);
                    }

                    if (TextVAlignment == VAlignment.Top)
                    {
                        yCoordinate = 0;
                    }
                    else if (TextVAlignment == VAlignment.Center)
                    {
                        yCoordinate = ((backgroundRectangle.Height - TriangleLegLength)/2);
                    }
                    else if (TextVAlignment == VAlignment.Bottom)
                    {
                        yCoordinate = backgroundRectangle.Height - TriangleLegLength;
                    }

                    var points = new Point[3];
                    var boundingRectangle = new Rectangle(new Point(0, 0),
                                                          new Size(TriangleLegLength, TriangleLegLength));
                    if (text.Trim() == "^")
                    {
                        points[0] = new Point(boundingRectangle.Left + boundingRectangle.Width/2,
                                              ((boundingRectangle.Height - TriangleLegLength)/2));
                        //top point of triangle
                        points[1] = new Point(((boundingRectangle.Width - TriangleLegLength)/2),
                                              ((boundingRectangle.Height - TriangleLegLength)/2) + TriangleLegLength);
                        //lower left point of triangle
                        points[2] = new Point(((boundingRectangle.Width - TriangleLegLength)/2) + TriangleLegLength,
                                              ((boundingRectangle.Height - TriangleLegLength)/2) + TriangleLegLength);
                        //lower right point of triangle
                    }
                    if (text.Trim() == @"\/")
                    {
                        points[0] = new Point(boundingRectangle.Left + boundingRectangle.Width/2,
                                              ((boundingRectangle.Height - TriangleLegLength)/2) + TriangleLegLength);
                        //bottom point of triangle
                        points[1] = new Point(((boundingRectangle.Width - TriangleLegLength)/2),
                                              ((boundingRectangle.Height - TriangleLegLength)/2));
                        //upper left point of triangle
                        points[2] = new Point(((boundingRectangle.Width - TriangleLegLength)/2) + TriangleLegLength,
                                              ((boundingRectangle.Height - TriangleLegLength)/2));
                        //upper right point of triangle
                    }
                    g.TranslateTransform(xCoordinate, yCoordinate);
                    g.FillPolygon(forecolorBrush, points);
                    g.TranslateTransform(-xCoordinate, -yCoordinate);
                }
                else
                {
                    g.DrawStringFast(text, font, forecolorBrush, textBoundingRectangle, textFormat);
                }
                g.Transform = origTransform;
            }
        }
    }
}