using System;
using Common.Math;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal static class BezierCurveHelper
    {
        public static void PointOnLine(double p0x, double p0y, double p1x, double p1y, double t, out double outX, out double outY)
        {
            outX = CalculateLinearLineParameter(p0x, p1x, t);
            outY = CalculateLinearLineParameter(p0y, p1y, t);
        }

        public static double CalculateLinearLineParameter(double x0, double x1, double t)
        {
            return x0 + (x1 - x0) * t;
        }

        public static void PointOnQuadraticBezierCurve(double p0x, double p0y, double p1x, double p1y, double p2x, double p2y,
            double t, out double outX, out double outY)
        {
            outX = CalculateQuadraticBezierParameter(p0x, p1x, p2x, t);
            outY = CalculateQuadraticBezierParameter(p0y, p1y, p2y, t);
        }

        public static double CalculateQuadraticBezierParameter(double x0, double x1, double x2, double t)
        {
            return Math.Pow(1.0 - t, 2.0) * x0 + 2.0 * t * (1.0 - t) * x1 + Math.Pow(t, 2.0) * x2;
        }

        public static void PointOnCubicBezierCurve(double p0x, double p0y, double p1x, double p1y, double p2x, double p2y,
            double p3x, double p3y, double t, out double outX, out double outY)
        {
            outX = CalculateCubicBezierParameter(p0x, p1x, p2x, p3x, t);
            outY = CalculateCubicBezierParameter(p0y, p1y, p2y, p3y, t);
        }

        public static double CalculateCubicBezierParameter(double x0, double x1, double x2, double x3, double t)
        {
            return Math.Pow(1.0 - t, 3.0) * x0 + 3.0 * t * Math.Pow(1.0 - t, 2.0) * x1 +
                   3.0 * (1.0 - t) * Math.Pow(t, 2.0) * x2 + Math.Pow(t, 3.0) * x3;
        }

        public static double ToRadians(double angle)
        {
            return angle * Constants.PI_OVER_180;
        }

        public static double AngleBetween(double v0x, double v0y, double v1x, double v1y)
        {
            var p = v0x * v1x + v0y * v1y;
            var n = Math.Sqrt((Math.Pow(v0x, 2.0) + Math.Pow(v0y, 2.0)) * (Math.Pow(v1x, 2.0) + Math.Pow(v1y, 2.0)));
            var sign = v0x * v1y - v0y * v1x < 0 ? -1.0 : 1.0;
            var angle = sign * Math.Acos(p / n);
            return angle;
        }

        public static void PointOnEllipticalArc(double p0x, double p0y, double rx, double ry, double xAxisRotation,
            bool largeArcFlag, bool sweepFlag, double p1x, double p1y, double t, out double outX, out double outY)
        {
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            xAxisRotation = Math.IEEERemainder(xAxisRotation, 360.0);
            var xAxisRotationRadians = ToRadians(xAxisRotation);
            if (Math.Abs(p0x - p1x) < 0.001 && Math.Abs(p0y - p1y) < 0.001)
            {
                outX = p0x;
                outY = p0y;
                return;
            }

            if (Math.Abs(rx) < 0.001 || Math.Abs(ry) < 0.001)
            {
                PointOnLine(p0x, p0y, p1x, p1y, t, out outX, out outY);
                return;
            }

            var dx = (p0x - p1x) / 2.0;
            var dy = (p0y - p1y) / 2.0;
            var transformedPointX = Math.Cos(xAxisRotationRadians) * dx + Math.Sin(xAxisRotationRadians) * dy;
            var transformedPointY = -Math.Sin(xAxisRotationRadians) * dx + Math.Cos(xAxisRotationRadians) * dy;

            var radiiCheck = Math.Pow(transformedPointX, 2.0) / Math.Pow(rx, 2.0) +
                             Math.Pow(transformedPointY, 2.0) / Math.Pow(ry, 2.0);
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
            }

            var cSquareNumerator = Math.Pow(rx, 2.0) * Math.Pow(ry, 2.0) -
                                   Math.Pow(rx, 2.0) * Math.Pow(transformedPointY, 2.0) -
                                   Math.Pow(ry, 2.0) * Math.Pow(transformedPointX, 2.0);
            var cSquareRootDenom = Math.Pow(rx, 2.0) * Math.Pow(transformedPointY, 2.0) +
                                   Math.Pow(ry, 2.0) * Math.Pow(transformedPointY, 2.0);
            var cRadicand = cSquareNumerator / cSquareRootDenom;

            cRadicand = cRadicand < 0  || double.IsNaN(cRadicand) ? 0 : cRadicand;
            var cCoef = (largeArcFlag != sweepFlag ? 1 : -1) * Math.Sqrt(cRadicand);
            var transformedCenterX = cCoef * (rx * transformedPointY / ry);
            var transformedCenterY = cCoef * (-(ry * transformedPointY) / rx);
            var centerX = Math.Cos(xAxisRotationRadians) * transformedCenterX -
                          Math.Sin(xAxisRotationRadians) * transformedCenterY + (p0x + p1x) / 2.0;
            var centerY = Math.Sin(xAxisRotationRadians) * transformedCenterX +
                          Math.Cos(xAxisRotationRadians) * transformedCenterY + (p0y + p1y) / 2.0;
            var startVectorX = (transformedPointX - transformedCenterY) / rx;
            var startVectorY = (transformedPointY - transformedCenterY) / ry;
            var startAngle = AngleBetween(1.0, 0.0, startVectorX, startVectorY);
            var endVectorX = (-transformedPointX - transformedCenterX) / rx;
            var endVectorY = (-transformedPointY - transformedCenterY) / ry;
            var sweepAngle = AngleBetween(startVectorX, startVectorY, endVectorX, endVectorY);

            if (!sweepFlag && sweepAngle > 0)
            {
                sweepAngle -= Constants.TWO_PI;
            }
            else if (sweepFlag && sweepAngle < 0)
            {
                sweepAngle += Constants.TWO_PI;
            }

            sweepAngle = Math.IEEERemainder(sweepAngle, Constants.TWO_PI);

            var angle = startAngle + sweepAngle * t;
            var ellipseComponentX = rx * Math.Cos(angle);
            var ellipseComponentY = ry * Math.Sin(angle);
            outX = Math.Cos(xAxisRotationRadians) * ellipseComponentX -
                   Math.Sin(xAxisRotationRadians) * ellipseComponentY + centerX;
            outY = Math.Sin(xAxisRotationRadians) * ellipseComponentX +
                   Math.Cos(xAxisRotationRadians) * ellipseComponentY + centerY;
        }
    }
}
