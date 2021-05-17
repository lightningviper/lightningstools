using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal static class BeamMovementSpeedOptimizationHelper
    {
        internal static IEnumerable<DrawPoint> ApplyBeamMovementSpeedOptimization(this IEnumerable<DrawPoint> drawPoints)
        {
            var filteredDrawPoints = RemoveExtraneousBeamOffThenOnTransitions(drawPoints);
            filteredDrawPoints = RemoveDuplicateDrawPoints(filteredDrawPoints);
            return filteredDrawPoints;
        }
        private static IEnumerable<DrawPoint> RemoveExtraneousBeamOffThenOnTransitions(this IEnumerable<DrawPoint> drawPoints)
        {
            var toReturn = new List<DrawPoint>();
            DrawPoint? previousDrawPoint = null;
            var dpArray = drawPoints.ToArray();
            var numDrawPoints = dpArray.Length;
            if (numDrawPoints> 0)
            {
                toReturn.Add(dpArray[0]);
                previousDrawPoint = dpArray[0];
            }
            for (var i = 1; i < numDrawPoints; i++)
            {
                var thisDrawPoint = dpArray[i];
                if (previousDrawPoint.Value.BeamOn && !thisDrawPoint.BeamOn && IsAdjacent(thisDrawPoint, previousDrawPoint.Value))
                {
                    thisDrawPoint.BeamOn = true; //keep beam on when moving to adjacent points where beam is already on
                }
                toReturn.Add(thisDrawPoint);
                previousDrawPoint = thisDrawPoint;
            }
            return toReturn;
        }
        private static  IEnumerable<DrawPoint> RemoveDuplicateDrawPoints(this IEnumerable<DrawPoint> drawPoints)
        {
            var toReturn = new List<DrawPoint>();
            var dpArray = drawPoints.ToArray();
            DrawPoint? previousDrawPoint = null;
            if (dpArray.Length > 0)
            {
                toReturn.Add(dpArray[0]);
                previousDrawPoint = dpArray[0];
            }
            for (var i = 1; i< dpArray.Length; i++)
            {
                var thisDrawPoint = dpArray[i];
                if (previousDrawPoint == null) break;
                if (thisDrawPoint.BeamOn == previousDrawPoint.Value.BeamOn && thisDrawPoint.X == previousDrawPoint.Value.X && thisDrawPoint.Y == previousDrawPoint.Value.Y)
                {
                    continue;
                }
                toReturn.Add(thisDrawPoint);
                previousDrawPoint = thisDrawPoint;
            }
            return toReturn;
        }
        private static bool IsAdjacent(DrawPoint dp1, DrawPoint dp2, double threshold = 1.0)
        {
            return (Math.Abs(dp1.X - dp2.X) <= threshold && Math.Abs(dp1.Y - dp2.Y) <= threshold);
        }
    }
}