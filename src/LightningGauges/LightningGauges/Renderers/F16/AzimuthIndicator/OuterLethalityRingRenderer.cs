using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class OuterLethalityRingRenderer
    {
        internal static void DrawOuterLethalityRing(Graphics gfx, float outerRingLeft, float outerRingTop, Pen grayPen, float atdOuterRingDiameter, int lineLength)
        {
            var toRestore = gfx.Transform;
            gfx.TranslateTransform(outerRingLeft, outerRingTop);
            gfx.DrawEllipse(grayPen, 0, 0, atdOuterRingDiameter, atdOuterRingDiameter);

            for (var i = 0; i <= 180; i += 30)
            {
                var previousTransform = gfx.Transform;
                gfx.TranslateTransform(atdOuterRingDiameter / 2.0f, atdOuterRingDiameter / 2.0f);
                gfx.RotateTransform(i);
                gfx.TranslateTransform(-atdOuterRingDiameter / 2.0f, -atdOuterRingDiameter / 2.0f);
                if (i % 90 == 0)
                {
                    gfx.DrawLineFast(grayPen, atdOuterRingDiameter / 2.0f, 0, atdOuterRingDiameter / 2.0f, lineLength * 2);
                    gfx.DrawLineFast(grayPen, atdOuterRingDiameter / 2.0f, atdOuterRingDiameter, atdOuterRingDiameter / 2.0f, atdOuterRingDiameter - lineLength * 2);
                }
                else
                {
                    gfx.DrawLineFast(grayPen, atdOuterRingDiameter / 2.0f, 0, atdOuterRingDiameter / 2.0f, lineLength);
                    gfx.DrawLineFast(grayPen, atdOuterRingDiameter / 2.0f, atdOuterRingDiameter, atdOuterRingDiameter / 2.0f, atdOuterRingDiameter - lineLength);
                }
                gfx.Transform = previousTransform;
            }

            gfx.Transform = toRestore;
        }
    }
}