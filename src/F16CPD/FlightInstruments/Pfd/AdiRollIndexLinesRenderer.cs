using Common.Drawing;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiRollIndexLinesRenderer
    {
        internal static void DrawAdiRollIndexLines(Graphics g, int centerXPfd, int centerYPfd)
        {
            var whitePen = new Pen(Color.White) { Width = 2 };
            var baseX = centerXPfd;
            var baseY = centerYPfd + 3;

            //draw roll index lines
            g.DrawLineFast(whitePen, baseX - 32, baseY - 188, baseX - 29, baseY - 171);
            g.DrawLineFast(whitePen, baseX - 64, baseY - 180, baseX - 57, baseY - 162);
            g.DrawLineFast(whitePen, baseX - 99, baseY - 176, baseX - 84, baseY - 150);
            g.DrawLineFast(whitePen, baseX - 171, baseY - 102, baseX - 146, baseY - 87);
            g.DrawLineFast(whitePen, baseX + 33, baseY - 188, baseX + 30, baseY - 171);
            g.DrawLineFast(whitePen, baseX + 58, baseY - 163, baseX + 64, baseY - 179);
            g.DrawLineFast(whitePen, baseX + 85, baseY - 150, baseX + 99, baseY - 176);
            g.DrawLineFast(whitePen, baseX + 147, baseY - 87, baseX + 171, baseY - 102);
        }
    }
}