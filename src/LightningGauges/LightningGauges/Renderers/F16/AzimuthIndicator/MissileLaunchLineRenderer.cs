using System;
using Common.Drawing;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class MissileLaunchLineRenderer
    {
        internal static void DrawMissileLaunchLine(
            Graphics gfx, float outerRingTop, float innerRingTop, PointF center, Blip blip, Pen launchLinePen, float angle, Brush missileWarningBrush, Options options, Font missileWarningFont)
        {
            //draw missile launch line
            if (options.Style != AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay) return;

            var endPoint = new PointF(center.X, outerRingTop);
            if (blip.MissileActivity > 0 || blip.MissileLaunch > 0) gfx.DrawLineFast(launchLinePen, center.X, innerRingTop, endPoint.X, endPoint.Y);

            if (blip.MissileActivity <= 0 && blip.MissileLaunch <= 0) return;

            var angleToDisplay = angle % 360;
            if (angleToDisplay < 0) angleToDisplay = 360 - Math.Abs(angleToDisplay);
            if (angleToDisplay == 0) angleToDisplay = 360;
            var angleString = $"{angleToDisplay:000}";

            var textWidth = gfx.MeasureString(angleString, missileWarningFont).Width;
            var missileWarningTextLocation = new PointF(endPoint.X, endPoint.Y - (endPoint.Y - innerRingTop) / 2.0f - textWidth / 2.0f);
            var oldTransform = gfx.Transform;
            gfx.TranslateTransform(missileWarningTextLocation.X, missileWarningTextLocation.Y);
            gfx.RotateTransform(-angle);
            gfx.TranslateTransform(-missileWarningTextLocation.X, -missileWarningTextLocation.Y);

            gfx.DrawStringFast(angleString, missileWarningFont, missileWarningBrush, missileWarningTextLocation);
            gfx.Transform = oldTransform;
        }
    }
}