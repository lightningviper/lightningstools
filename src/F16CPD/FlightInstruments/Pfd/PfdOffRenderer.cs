using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class PfdOffRenderer
    {
        internal static void DrawPfdOff(Graphics g, Size renderSize)
        {
            var greenColor = Color.FromArgb(0, 255, 0);
            Brush greenBrush = new SolidBrush(greenColor);

            const string toDisplay = "NO PFD DATA";
            var path = new GraphicsPath();
            var sf = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var layoutRectangle = new Rectangle(new Point(0, 0), renderSize);
            path.AddString(toDisplay, FontFamily.GenericMonospace, (int) FontStyle.Bold, 20, layoutRectangle, sf);
            g.FillPathFast(greenBrush, path);
            return;
        }
    }
}