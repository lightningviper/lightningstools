using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Drawing.Imaging;
using Common.Drawing.Text;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class VerticalNumberStripFactory
    {
        private static Bitmap _verticalNumberStrip;

        internal static Bitmap GetVerticalNumberStrip()
        {
            if (_verticalNumberStrip != null)
            {
                return _verticalNumberStrip;
            }
            const int digitWidth = 9;
            const int digitHeight = 15;

            const int digitHorizontalMargin = 4;
            const int digitVerticalMargin = 4;

            var digitRectangle = new Rectangle(
                new Point(0, 0),
                new Size(
                    digitWidth + (digitHorizontalMargin*2),
                    digitHeight + (digitVerticalMargin*2)
                    )
                );
            var overallRectangle = new Rectangle(
                new Point(0, 0),
                new Size(
                    digitRectangle.Width,
                    (digitRectangle.Height*12)
                    )
                );

            var toReturn = new Bitmap(overallRectangle.Width, overallRectangle.Height, PixelFormat.Format16bppRgb565);

            toReturn.MakeTransparent();
            using (var g = Graphics.FromImage(toReturn))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush whiteBrush = new SolidBrush(Color.White);
                var font = new Font("Lucida Console", 12, FontStyle.Bold);
                digitRectangle.Offset(0, digitVerticalMargin);

                for (var i = 11; i >= 0; i--)
                {
                    g.DrawStringFast((i%10).ToString(), font, whiteBrush, digitRectangle);
                    digitRectangle.Offset(0, digitRectangle.Height);
                }
                g.DrawStringFast("9", font, whiteBrush, digitRectangle);
                digitRectangle.Offset(0, digitRectangle.Height);
            }
            _verticalNumberStrip = toReturn;
            return toReturn;
        }
    }
}