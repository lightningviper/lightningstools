using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class InnerLethalityRingRenderer
    {
        internal static void DrawInnerLethalityRing(Graphics gfx, Pen grayPen, float innerRingLeft, float innerRingTop, float atdInnerRingDiameter)
        {
            gfx.DrawEllipse(grayPen, innerRingLeft, innerRingTop, atdInnerRingDiameter, atdInnerRingDiameter);
        }
    }
}