using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiGsFlagRenderer
    {
        private static Font _adiGsFlagFont = new Font("Lucida Console", 25, FontStyle.Bold);

        internal static void DrawAdiGsFlag(Graphics g, Point location, bool adiGlideslopeInvalidFlag, bool nightMode)
        {
            //draw ADI GS flag
            if (adiGlideslopeInvalidFlag)
            {
                var path = new GraphicsPath();
                var adiGsFlagStringFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                var adiGsFlagTextLayoutRectangle = new Rectangle(location, new Size(60, 25));
                path.AddString("GS", _adiGsFlagFont.FontFamily, (int)_adiGsFlagFont.Style, _adiGsFlagFont.SizeInPoints,
                    adiGsFlagTextLayoutRectangle, adiGsFlagStringFormat);
                var gsFlagBrush = Brushes.Red;
                var gsFlagTextBrush = Brushes.Black;
                if (nightMode)
                {
                    gsFlagBrush = Brushes.Black;
                    gsFlagTextBrush = Brushes.White;
                }
                g.FillRectangle(gsFlagBrush, adiGsFlagTextLayoutRectangle);
                g.DrawRectangleFast(Pens.Black, adiGsFlagTextLayoutRectangle);
                g.FillPathFast(gsFlagTextBrush, path);
            }
        }
    }
}