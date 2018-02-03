using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiAuxFlagRenderer
    {
        private static Font _adiAuxFont = new Font("Lucida Console", 25, FontStyle.Bold);

        internal static void DrawAdiAuxFlag(Graphics g, Point location, bool adiAuxFlag)
        {
            //draw ADI aux flag
            if (adiAuxFlag)
            {
                var path = new GraphicsPath();
                var adiAuxStringFormat = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                var adiAuxTextLayoutRectangle = new Rectangle(location, new Size(60, 25));
                path.AddString("AUX", _adiAuxFont.FontFamily, (int)_adiAuxFont.Style, _adiAuxFont.SizeInPoints,
                    adiAuxTextLayoutRectangle, adiAuxStringFormat);
                g.FillRectangle(Brushes.Yellow, adiAuxTextLayoutRectangle);
                g.DrawRectangleFast(Pens.Black, adiAuxTextLayoutRectangle);
                g.FillPathFast(Brushes.Black, path);
            }
        }
    }
}