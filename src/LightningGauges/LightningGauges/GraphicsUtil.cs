using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges
{
    internal class GraphicsUtil
    {
        public static void SaveGraphicsState(Graphics g, ref GraphicsState state) { state = g.Save(); }

        public static void RestoreGraphicsState(Graphics g, ref GraphicsState state)
        {
            g.Restore(state);
            state = g.Save();
        }
    }
}