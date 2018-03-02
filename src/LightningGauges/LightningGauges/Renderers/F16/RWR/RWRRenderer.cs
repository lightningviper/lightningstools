using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Common.SimSupport;
using System.Windows.Media.Imaging;
using System.IO;

namespace LightningGauges.Renderers.F16.RWR
{
    public interface IRWRRenderer : IInstrumentRenderer
    {
        void Render(DrawingContext drawingContext);
        InstrumentState InstrumentState { get; set; }
    }
    public class RWRRenderer : InstrumentRendererBase, IRWRRenderer
    {
        protected string[] rwrInfo;
        protected const float DTR = 0.01745329F;
        protected const double ActualWidth = 500;
        protected const double ActualHeight = 500;
        protected double offsetX = 0;
        protected double offsetY = 0;
        protected const double BigFontSize = (30.0/300.0)*ActualHeight;
        protected const double SmallFontSize = (20.0/300.0)*ActualHeight;
        protected const double BigFontVOffset = (2.0/300.0)*ActualHeight;
        protected const double SmallFontVOffset = (1.0/300.0)*ActualHeight;
        protected SolidColorBrush brush { get { return new SolidColorBrush(Color.FromRgb(5, 248, 7)); } }
        string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
        protected Typeface typeface { get; } = new Typeface(new FontFamily(new Uri("file:///" + AppDomain.CurrentDomain.BaseDirectory + @"\"), "#Lekton"),
			FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal);

        public InstrumentState InstrumentState { get; set; } = new InstrumentState();
        public virtual void Render(DrawingContext drawingContext){}
        public override void Render(Common.Drawing.Graphics destinationGraphics, Common.Drawing.Rectangle destinationRectangle)
        {
            DrawingVisual dv = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(dv, BitmapScalingMode.Fant);
            RenderOptions.SetClearTypeHint(dv, ClearTypeHint.Enabled);
            RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased);
            DrawingContext dc = dv.RenderOpen();
            Render(dc);
            dc.Close();
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96,PixelFormats.Default);
            rtb.Render(dv);
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using (Common.Drawing.Bitmap bmp = new Common.Drawing.Bitmap(ms))
                {
                    destinationGraphics.DrawImage(bmp, destinationRectangle, new Common.Drawing.Rectangle(0,0, bmp.Width, bmp.Height), Common.Drawing.GraphicsUnit.Pixel);
                }
            }
        }

        protected void SetDisplayOrigin(double x, double y)
        {
            offsetX = x;
            offsetY = y;
        }

        protected void ZeroDisplayOrigin()
        {
            offsetX = 0;
            offsetY = 0;
        }

        protected double ConvertXPos(double pos)
        {
            double center = ActualWidth / 2.0;
            return center + (pos * center);
        }

        protected double ConvertYPos(double pos)
        {
            double center = ActualWidth / 2.0;
            return center + (-pos * center);
        }
        protected double ConvertLen(double len)
        {
            double half = ActualWidth / 2.0;
            return half * len;
        }


        protected void DrawLine(DrawingContext drawingContext, double x1, double y1, double x2, double y2)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = brush;
            drawingContext.DrawLine(pen, new Point(ConvertXPos(x1 + offsetX), ConvertYPos(y1 + offsetY)), new Point(ConvertXPos(x2 + offsetX), ConvertYPos(y2 + offsetY)));
        }

        protected void DrawSolidBox(DrawingContext drawingContext, double x1, double y1, double x2, double y2, Color color)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = new SolidColorBrush(color);
            Rect size = new Rect(new Point(ConvertXPos(x1 + offsetX), ConvertYPos(y1 + offsetY)), new Point(ConvertXPos(x2 + offsetX), ConvertYPos(y2 + offsetY)));
            drawingContext.DrawRectangle(new SolidColorBrush(color), pen, size);
        }

        protected void DrawCircle(DrawingContext drawingContext, double x, double y, double radius)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = brush;

            drawingContext.DrawEllipse(null, pen, new Point(ConvertXPos(x + offsetX), ConvertYPos(y + offsetY)), ConvertLen(radius), ConvertLen(radius));
        }

        protected void DrawSolidCircle(DrawingContext drawingContext, double x, double y, double radius)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = brush;

            drawingContext.DrawEllipse(brush, pen, new Point(ConvertXPos(x + offsetX), ConvertYPos(y + offsetY)), ConvertLen(radius), ConvertLen(radius));
        }

        protected void DrawArc(DrawingContext drawingContext, double x, double y, double radius, double start, double stop)
        {
            Pen pen = new Pen();
            pen.Thickness = 2;
            pen.Brush = brush;

            drawingContext.DrawArc(pen, null, new Rect(ConvertXPos(x - radius + offsetX), ConvertYPos(y + radius + offsetY), ConvertLen(radius) * 2d, ConvertLen(radius) * 2d), start, stop);
        }

        protected double TextHeight(bool big)
        {
            double fontSize;

            if (big)
            {
                fontSize = BigFontSize;
            }
            else
            {
                fontSize = SmallFontSize;
            }

            return (fontSize * 0.85) / (ActualHeight / 2.0);
        }

        protected double TextWidth(string text, bool big)
        {
            double fontSize;

            if (big)
            {
                fontSize = BigFontSize;
            }
            else
            {
                fontSize = SmallFontSize;
            }

            FormattedText ft = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);
            double width = ft.Width;

            return width / (ActualWidth / 2.0);
        }

        protected void DrawTextCenteredVertival(DrawingContext drawingContext, string text, double x, double y, bool big)
        {
            double fontSize, vOffset;

            x = ConvertXPos(x + offsetX);
            y = ConvertYPos(y + offsetY);

            if (big)
            {
                fontSize = BigFontSize;
                vOffset = BigFontVOffset;
            }
            else
            {
                fontSize = SmallFontSize;
                vOffset = SmallFontVOffset;
            }

            FormattedText ft = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);
            double width = ft.Width;
            double height = ft.Height;

            drawingContext.DrawText(
                new FormattedText(text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize, brush),
                new Point(x - (width / 2.0), y - (height / 2.0) + vOffset));
        }

        protected void DrawTextCenter(DrawingContext drawingContext, string text, double x, double y, bool big)
        {
            double fontSize, vOffset;

            x = ConvertXPos(x + offsetX);
            y = ConvertYPos(y + offsetY);

            if (big)
            {
                fontSize = BigFontSize;
                vOffset = BigFontVOffset*2;
            }
            else
            {
                fontSize = SmallFontSize;
                vOffset = SmallFontVOffset *2;
            }

            FormattedText ft = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);
            double width = ft.Width;
            double height = ft.Height;

            drawingContext.DrawText(
                new FormattedText(text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize, brush),
                new Point(x - (width / 2.0), y - (fontSize * 0.5) + vOffset));
        }

        protected void DrawTextLeft(DrawingContext drawingContext, string text, double x, double y, bool big)
        {
            double fontSize, vOffset;

            x = ConvertXPos(x + offsetX);
            y = ConvertYPos(y + offsetY);

            if (big)
            {
                fontSize = BigFontSize;
                vOffset = BigFontVOffset *2;
            }
            else
            {
                fontSize = SmallFontSize;
                vOffset = SmallFontVOffset *2;
            }

            FormattedText ft = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);
            double width = ft.Width;
            double height = ft.Height;

            drawingContext.DrawText(
                new FormattedText(text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize, brush),
                new Point(x, y - (fontSize * 0.5) + vOffset));
        }

        protected void DrawTextRight(DrawingContext drawingContext, string text, double x, double y, bool big)
        {
            double fontSize, vOffset;

            x = ConvertXPos(x + offsetX);
            y = ConvertYPos(y + offsetY);

            if (big)
            {
                fontSize = BigFontSize;
                vOffset = BigFontVOffset *2;
            }
            else
            {
                fontSize = SmallFontSize;
                vOffset = SmallFontVOffset *2;
            }

            FormattedText ft = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);
            double width = ft.Width;
            double height = ft.Height;

            drawingContext.DrawText(
                new FormattedText(text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize, brush),
                new Point(x - width, y - (fontSize * 0.5) + vOffset));
        }


    }
}
