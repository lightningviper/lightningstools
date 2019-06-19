using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class DrawPointInterpolationHelper
    {
        public static void InsertInterpolatedDrawPoints(IList<DrawPoint> drawPoints, double x, double y, bool beamOn, byte dacPrecisionBits=12, bool invertX=false, bool invertY=false, double stepSize=12)
        {
            var maxDacValue = (ushort)(Math.Pow(2, dacPrecisionBits) - 1);
            var finalXDAC = (ushort)Math.Round(invertX? maxDacValue - x : x);
            var finalYDAC = (ushort)Math.Round(invertY ? maxDacValue - y : y);

            if (finalXDAC > maxDacValue) finalXDAC = maxDacValue;
            if (finalYDAC > maxDacValue) finalYDAC = maxDacValue;

            var startXDAC = drawPoints.Count >0 ? drawPoints.Last().X : 0;
            var startYDAC = drawPoints.Count > 0 ? drawPoints.Last().Y : 0;

            double xDistance = finalXDAC - startXDAC;
            double yDistance = finalYDAC - startYDAC;
            var euclideanDistance = Math.Abs(Math.Sqrt(xDistance * xDistance + yDistance * yDistance));
            var numSteps = beamOn ? (ulong)(Math.Ceiling(euclideanDistance)/stepSize) : 1;

            if (numSteps == 0) return;
            var dx = xDistance / numSteps;
            var dy = yDistance / numSteps;
            for (var i = 1ul; i <= numSteps; i++)
            {
                var xDAC = (ushort)(startXDAC + dx * i);
                var yDAC = (ushort)(startYDAC + dy * i);

                if (drawPoints.Count == 0 || drawPoints.Count > 0 &&
                    (Math.Abs(drawPoints.Last().X - xDAC) >= 1 || Math.Abs(drawPoints.Last().Y - yDAC) >= 1))
                {
                    drawPoints.Add(new DrawPoint() { BeamOn = beamOn, X = xDAC, Y = yDAC });
                }
            }

            if (Math.Abs(drawPoints.Last().X - finalXDAC) < 1 && Math.Abs(drawPoints.Last().Y - finalYDAC) < 1) return;
            drawPoints.Add(new DrawPoint() { BeamOn = beamOn, X = finalXDAC, Y = finalYDAC });
        }
    }
}
