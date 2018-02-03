using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiOffFlagRenderer
    {
        private static Font _adiOffFlagFont = new Font("Lucida Console", 25, FontStyle.Bold);

        internal static void DrawAdiOffFlag(Graphics g, Point location, bool adiOffFlag, bool nightMode)
        {
            //draw ADI OFF flag
            if (adiOffFlag)
            {
                var path = new GraphicsPath();
                var adiOffFlagStringFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                var adiOffFlagTextLayoutRectangle = new Rectangle(location, new Size(75, 25));
                path.AddString("OFF", _adiOffFlagFont.FontFamily, (int)_adiOffFlagFont.Style, _adiOffFlagFont.SizeInPoints,
                    adiOffFlagTextLayoutRectangle, adiOffFlagStringFormat);
                var adiOffFlagBrush = Brushes.Red;
                var adiOffFlagTextBrush = Brushes.Black;
                if (nightMode)
                {
                    adiOffFlagBrush = Brushes.Black;
                    adiOffFlagTextBrush = Brushes.White;
                }
                g.FillRectangle(adiOffFlagBrush, adiOffFlagTextLayoutRectangle);
                g.DrawRectangleFast(Pens.Black, adiOffFlagTextLayoutRectangle);
                g.FillPathFast(adiOffFlagTextBrush, path);
            }
        }
    }
}