using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AOAOffFlagRenderer
    {

        internal static void DrawAoaOffFlag(Graphics g, Point location, bool aoaOffFlag, bool nightMode)
        {
            //draw AOA OFF flag
            if (aoaOffFlag)
            {
                var aoaOffFlagFont = new Font("Lucida Console", 25, FontStyle.Bold);
                var path = new GraphicsPath();
                var aoaOffFlagStringFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                var aoaOffFlagTextLayoutRectangle = new Rectangle(location, new Size(75, 25));
                path.AddString("OFF", aoaOffFlagFont.FontFamily, (int) aoaOffFlagFont.Style, aoaOffFlagFont.SizeInPoints,
                    aoaOffFlagTextLayoutRectangle, aoaOffFlagStringFormat);
                var aoaOffFlagBrush = Brushes.Red;
                var aoaOffFlagTextBrush = Brushes.Black;
                if (nightMode)
                {
                    aoaOffFlagBrush = Brushes.Black;
                    aoaOffFlagTextBrush = Brushes.White;
                }
                g.FillRectangle(aoaOffFlagBrush, aoaOffFlagTextLayoutRectangle);
                g.DrawRectangleFast(Pens.Black, aoaOffFlagTextLayoutRectangle);
                g.FillPathFast(aoaOffFlagTextBrush, path);
            }
        }
    }
}