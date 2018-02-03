using System;
using Common.Drawing;
using Common.Drawing.Drawing2D;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class BlipRenderer
    {
        internal static void DrawBlip(
            Graphics gfx, ref GraphicsState basicState, Matrix initialTransform, float atdRingOffsetTranslateX, float atdRingOffsetTranslateY, float atdRingScale, int backgroundWidth,
            int backgroundHeight, Color missileColor, float outerRingTop, float innerRingTop, Color scopeGreenColor, Color airborneThreatColor, Color groundThreatColor, Color searchThreatColor,
            Color navalThreatColor, Color unknownThreatColor, Blip blip, InstrumentState instrumentState, Options options, Font missileWarningFont, int width, Font largeFont, Font smallFont)
        {
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);

            if (options.Style == AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay)
            {
                gfx.Transform = initialTransform;
                gfx.TranslateTransform(atdRingOffsetTranslateX, atdRingOffsetTranslateY);
                gfx.ScaleTransform(atdRingScale, atdRingScale);
            }
            //calculate position of symbols
            var lethality = blip.Lethality;
            var translateY = lethality > 1 ? -((2.0f - lethality) * 100.0f) * 0.95f : -((1.0f - lethality) * 100.0f) * 0.95f;
            if (options.Style == AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay) translateY *= 0.92f;
            //rotate and translate symbol into place
            var angle = -instrumentState.MagneticHeadingDegrees + blip.BearingDegrees;
            if (instrumentState.Inverted) angle = -angle;
            //rotate the background image so that this emitter's bearing line points toward the top
            gfx.TranslateTransform(backgroundWidth / 2.0f, backgroundHeight / 2.0f);
            gfx.RotateTransform(angle);
            gfx.TranslateTransform(-(float) backgroundWidth / 2.0f, -(float) backgroundHeight / 2.0f);

            using (var missileWarningBrush = new SolidBrush(missileColor))
            using (var launchLinePen = new Pen(missileColor))
            {
                var center = new PointF(backgroundWidth / 2.0f, backgroundHeight / 2.0f);

                MissileLaunchLineRenderer.DrawMissileLaunchLine(gfx, outerRingTop, innerRingTop, center, blip, launchLinePen, angle, missileWarningBrush, options, missileWarningFont);
            }
            //position the emitter symbol at the correct distance from the center of the RWR, given its lethality
            gfx.TranslateTransform(0, translateY);

            //rotate the emitter symbol graphic so that it appears upright when background is rotated back and displayed to user
            gfx.TranslateTransform(backgroundWidth / 2.0f, backgroundHeight / 2.0f);
            gfx.RotateTransform(-angle);
            gfx.TranslateTransform(-backgroundWidth / 2.0f, -backgroundHeight / 2.0f);

            //draw the emitter symbol
            var usePrimarySymbol = DateTime.UtcNow.Millisecond < 500;
            var useLargeSymbol = DateTime.UtcNow.Millisecond < 500 && blip.NewDetection > 0;

            var emitterColor = EmitterColorChooser.GetEmitterColor(
                scopeGreenColor, missileColor, airborneThreatColor, groundThreatColor, searchThreatColor, navalThreatColor, unknownThreatColor, blip, options);
            var emitterSymbolDestinationRectangle = new RectangleF((int) ((backgroundWidth - width) / 2.0f), (int) ((backgroundHeight - width) / 2.0f), width, width);
            EmitterSymbolRenderer.DrawEmitterSymbol(blip.SymbolID, gfx, emitterSymbolDestinationRectangle, useLargeSymbol, usePrimarySymbol, emitterColor, largeFont, smallFont, width);

            using (var emitterPen = new Pen(emitterColor))
            {
                gfx.TranslateTransform(-width / 2.0f, -width / 2.0f);
                if (blip.Selected > 0) SelectedThreatDiamondRenderer.DrawSelectedThreatDiamond(gfx, backgroundWidth, backgroundHeight, emitterPen, width);
                if (blip.MissileActivity > 0 && blip.MissileLaunch == 0) { MissileActivitySymbolRenderer.DrawMissileActivitySymbol(gfx, backgroundWidth, backgroundHeight, emitterPen, width); }
                else if (blip.MissileActivity > 0 && blip.MissileLaunch > 0) MissileLaunchSymbolRenderer.DrawMissileLaunchSymbol(gfx, backgroundWidth, backgroundHeight, emitterPen, width);
            }
            GraphicsUtil.RestoreGraphicsState(gfx, ref basicState);
        }
    }
}