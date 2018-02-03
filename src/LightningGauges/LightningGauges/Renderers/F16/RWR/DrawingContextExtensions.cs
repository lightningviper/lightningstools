using System;
using System.Windows.Media;
using System.Windows;

namespace LightningGauges.Renderers.F16.RWR
{
    public static class DrawingContextExtensions
    {
        /// <summary>
        /// Draw an Arc of an ellipse or circle. Static extension method of DrawingContext.
        /// </summary>
        /// <param name="dc">DrawingContext</param>
        /// <param name="pen">Pen for outline. set to null for no outline.</param>
        /// <param name="brush">Brush for fill. set to null for no fill.</param>
        /// <param name="rect">Box to hold the whole ellipse described by the arc</param>
        /// <param name="startDegrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
        /// <param name="sweepDegrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
        public static void DrawArc(this DrawingContext dc, Pen pen, Brush brush, Rect rect, double startDegrees, double sweepDegrees)
        {
            GeometryDrawing arc = CreateArcDrawing(rect, startDegrees, sweepDegrees);
            dc.DrawGeometry(brush, pen, arc.Geometry);
        }

        /// <summary>
        /// Create an Arc geometry drawing of an ellipse or circle
        /// </summary>
        /// <param name="rect">Box to hold the whole ellipse described by the arc</param>
        /// <param name="startDegrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
        /// <param name="sweepDegrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
        /// <returns>GeometryDrawing object</returns>
        private static GeometryDrawing CreateArcDrawing(Rect rect, double startDegrees, double sweepDegrees)
        {
            // degrees to radians conversion
            double startRadians = startDegrees * Math.PI / 180.0;
            double sweepRadians = sweepDegrees * Math.PI / 180.0;

            // x and y radius
            double dx = rect.Width / 2;
            double dy = rect.Height / 2;

            // determine the start point 
            double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
            double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

            // determine the end point 
            double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
            double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);

            // draw the arc into a stream geometry
            StreamGeometry streamGeom = new StreamGeometry();
            using (StreamGeometryContext ctx = streamGeom.Open())
            {
                bool isLargeArc = Math.Abs(sweepDegrees) > 180;
                SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

                ctx.BeginFigure(new Point(xs, ys), false, false);
                ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
            }

            // create the drawing
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Geometry = streamGeom;
            return drawing;
        }
    }
}
