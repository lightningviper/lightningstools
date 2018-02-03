using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiLocFlagRenderer
    {
        private static Font _adiLocFlagFont = new Font("Lucida Console", 25, FontStyle.Bold);

        internal static void DrawAdiLocFlag(Graphics g, Point location, bool adiLocalizerInvalidFlag, bool nightMode)
        {
            //draw ADI LOC flag
            if (adiLocalizerInvalidFlag)
            {

                var path = new GraphicsPath();
                var adiLocFlagStringFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                var adiLocFlagTextLayoutRectangle = new Rectangle(location, new Size(60, 25));
                path.AddString("LOC", _adiLocFlagFont.FontFamily, (int)_adiLocFlagFont.Style, _adiLocFlagFont.SizeInPoints,
                    adiLocFlagTextLayoutRectangle, adiLocFlagStringFormat);
                var locFlagBrush = Brushes.Red;
                var locFlagTextBrush = Brushes.Black;
                if (nightMode)
                {
                    locFlagBrush = Brushes.Black;
                    locFlagTextBrush = Brushes.White;
                }
                g.FillRectangle(locFlagBrush, adiLocFlagTextLayoutRectangle);
                g.DrawRectangleFast(Pens.Black, adiLocFlagTextLayoutRectangle);
                g.FillPathFast(locFlagTextBrush, path);
            }
        }
    }
}