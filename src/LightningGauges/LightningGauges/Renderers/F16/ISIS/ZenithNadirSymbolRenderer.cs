using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class ZenithNadirSymbolRenderer
    {
        internal static void DrawZenithNadirSymbol(Graphics g, float pitchLadderHeight, float pixelsPerDegree, float pitchLadderWidth, float zenithNadirSymbolWidth, Pen pitchBarPen)
        {
            {
                var y = pitchLadderHeight / 2.0f - 90 * pixelsPerDegree;
                var zenithOrNadirRectangle = new RectangleF(pitchLadderWidth / 2.0f - zenithNadirSymbolWidth / 2.0f, y - zenithNadirSymbolWidth / 2.0f, zenithNadirSymbolWidth, zenithNadirSymbolWidth);
                g.DrawEllipse(pitchBarPen, zenithOrNadirRectangle);
            }
            {
                var y = pitchLadderHeight / 2.0f - -90 * pixelsPerDegree;
                var zenithOrNadirRectangle = new RectangleF(pitchLadderWidth / 2.0f - zenithNadirSymbolWidth / 2.0f, y - zenithNadirSymbolWidth / 2.0f, zenithNadirSymbolWidth, zenithNadirSymbolWidth);
                g.DrawEllipse(pitchBarPen, zenithOrNadirRectangle);
                g.DrawLineFast(pitchBarPen, new PointF(zenithOrNadirRectangle.Left, zenithOrNadirRectangle.Top), new PointF(zenithOrNadirRectangle.Right, zenithOrNadirRectangle.Bottom));
                g.DrawLineFast(pitchBarPen, new PointF(zenithOrNadirRectangle.Left, zenithOrNadirRectangle.Bottom), new PointF(zenithOrNadirRectangle.Right, zenithOrNadirRectangle.Top));
            }
        }
    }
}