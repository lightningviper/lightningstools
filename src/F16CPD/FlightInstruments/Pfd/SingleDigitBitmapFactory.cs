using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class SingleDigitBitmapFactory
    {
        private static readonly Bitmap[] SingleDigitBitmaps = new Bitmap[1000];
        private static readonly Bitmap[] TripleDigitBitmaps = new Bitmap[1000];

        internal static Bitmap GetSingleDigitBitmap(int digit)
        {
            return GetSingleDigitBitmap(digit, false);
        }
        internal static Bitmap GetSingleDigitBitmap(float digit, bool showExtraDigitsTopAndBottom)
        {
            if (showExtraDigitsTopAndBottom)
            {
                if (TripleDigitBitmaps[(int) (digit*10.0f)] != null)
                {
                    return TripleDigitBitmaps[(int) (digit*10.0f)];
                }
            }
            else
            {
                if (SingleDigitBitmaps[(int) (digit*10.0f)] != null)
                {
                    return SingleDigitBitmaps[(int) (digit*10.0f)];
                }
            }
            if (digit < 1 && digit >= 0) digit += 10;
            const int digitHeight = 15;
            const int digitVerticalMargin = 4;

            var verticalNumberStrip = VerticalNumberStripFactory.GetVerticalNumberStrip();

            const int leftX = 0;
            var rightX = verticalNumberStrip.Width;
            var topY = (int) (verticalNumberStrip.Height - ((digitVerticalMargin*2) + digitHeight)*(digit + 1));
            topY += digitVerticalMargin;
            var bottomY = topY + digitHeight + digitVerticalMargin;

            if (showExtraDigitsTopAndBottom)
            {
                const int overRunArea = (int) ((digitHeight + (digitVerticalMargin*2))*0.6);
                topY -= (overRunArea + digitVerticalMargin);
                bottomY += overRunArea;
            }
            var toReturn = new Bitmap(rightX - leftX, bottomY - topY, PixelFormat.Format16bppRgb565);
            toReturn.MakeTransparent();
            using (var g = Graphics.FromImage(toReturn))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var sourceRectangle = new Rectangle(new Point(leftX, topY), new Size(rightX - leftX, bottomY - topY));
                Rectangle destRectangle;
                if (showExtraDigitsTopAndBottom)
                {
                    destRectangle = new Rectangle(new Point(0, 0), new Size(verticalNumberStrip.Width, bottomY - topY));
                }
                else
                {
                    destRectangle = new Rectangle(new Point(0, digitVerticalMargin),
                        new Size(verticalNumberStrip.Width, bottomY - topY));
                }
                g.DrawImageFast(verticalNumberStrip, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            if (showExtraDigitsTopAndBottom)
            {
                TripleDigitBitmaps[(int) (digit*10.0f)] = toReturn;
            }
            else
            {
                SingleDigitBitmaps[(int) (digit*10.0f)] = toReturn;
            }

            return toReturn;
        }
    }
}