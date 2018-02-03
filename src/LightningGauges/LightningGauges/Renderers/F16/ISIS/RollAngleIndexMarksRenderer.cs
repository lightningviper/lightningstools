using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class RollAngleIndexMarksRenderer
    {
        private static readonly ThreadLocal<Pen> RollIndexPen = new ThreadLocal<Pen>(()=>new Pen(Color.White) {Width = 2});

        internal static float DrawRollAngleIndexMarks(Graphics g, int width, int height, float pixelsPerDegreePitch, ref GraphicsState basicState, out Pen rollIndexPen)
        {
            rollIndexPen = RollIndexPen.Value;
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            const float majorIndexLineLength = 15;
            const float minorIndexLineLength = majorIndexLineLength / 2.0f;
            var radiusFromCenterToBottomOfIndexLine = pixelsPerDegreePitch * 20.0f;
            var startingTransform = g.Transform;
            for (var i = -60; i <= 60; i += 5)
            {
                var drawLine = false;
                var lineLength = minorIndexLineLength;
                if (Math.Abs(i) == 60 || Math.Abs(i) == 30)
                {
                    drawLine = true;
                    lineLength = majorIndexLineLength;
                }
                else if (Math.Abs(i) == 45 || Math.Abs(i) == 20 || Math.Abs(i) == 10) drawLine = true;
                g.Transform = startingTransform;
                g.TranslateTransform(width / 2.0f, height / 2.0f);
                g.RotateTransform(i);
                g.TranslateTransform(-(float) width / 2.0f, -(float) height / 2.0f);

                if (drawLine)
                {
                    g.DrawLineFast(
                        RollIndexPen.Value, new PointF(width / 2.0f, height / 2.0f - radiusFromCenterToBottomOfIndexLine),
                        new PointF(width / 2.0f, height / 2.0f - radiusFromCenterToBottomOfIndexLine - lineLength));
                }
            }

            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
            return radiusFromCenterToBottomOfIndexLine;
        }
    }
}