using Common.MacroProgramming;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal static class DrawPointExtensions
    {
        internal static IEnumerable<DrawPoint> ApplyScaling(this IEnumerable<DrawPoint> drawPoints, double scaleX, double scaleY)
        {
            return drawPoints.Select(dp => new DrawPoint
            {
                X = dp.X * scaleX,
                Y = dp.Y * scaleY,
                BeamOn = dp.BeamOn
            });

        }
        internal static IEnumerable<DrawPoint> ApplyClipping(this IEnumerable<DrawPoint> drawPoints, double width, double height)
        {
            return drawPoints.Select(dp => new DrawPoint {
                X = dp.X < 0 ? 0 : dp.X > width ? width : dp.X, 
                Y = dp.Y < 0 ? 0 : dp.Y > height ? height : dp.Y,
                BeamOn = dp.BeamOn });

        }
        internal static IEnumerable<DrawPoint> ApplyRotation(this IEnumerable<DrawPoint> drawPoints, double cx, double cy, double angleDegrees)
        {
            return drawPoints.Select(dp => {
                var sinTheta = Math.Sin(angleDegrees * Common.Math.Constants.RADIANS_PER_DEGREE);
                var cosTheta = Math.Cos(angleDegrees * Common.Math.Constants.RADIANS_PER_DEGREE);

                return new DrawPoint
                {
                    X = (cosTheta * (dp.X - cx)) - (sinTheta * (dp.Y - cy)) + cx,
                    Y = (sinTheta * (dp.X - cx)) + (cosTheta * (dp.Y - cy)) + cy,
                    BeamOn = dp.BeamOn
                };
            });
        }
        internal static IEnumerable<DrawPoint> ApplyInversion(this IEnumerable<DrawPoint> drawPoints, double width, double height, bool invertX, bool invertY)
        {
            return drawPoints.Select(dp => new DrawPoint
            {
                X = invertX ? width - dp.X : dp.X,
                Y = invertY ? height - dp.Y : dp.Y,
                BeamOn = dp.BeamOn
            });
        }

        internal static IEnumerable<DrawPoint> ApplyCentering(this IEnumerable<DrawPoint> drawPoints,
            short offsetX, short offsetY)
        {
            return drawPoints.Select(drawPoint => new DrawPoint
            {
                BeamOn = drawPoint.BeamOn,
                X = drawPoint.X + offsetX,
                Y = drawPoint.Y + offsetY
            });
        }
        internal static IEnumerable<DrawPoint> ApplyCalibration(this IEnumerable<DrawPoint> drawPoints, IEnumerable<CalibrationPoint> xAxisCalibrationData, IEnumerable<CalibrationPoint> yAxisCalibrationData)
        {
            return drawPoints.Select(dp => new DrawPoint()
            {
                BeamOn = dp.BeamOn,
                X = CalibratedOutput(xAxisCalibrationData, dp.X),
                Y = CalibratedOutput(yAxisCalibrationData, dp.Y)
            });
        }


        private static double CalibratedOutput(IEnumerable<CalibrationPoint> calibrationTable, double inputVal)
        {
            var calTable = calibrationTable.ToList();
            if (!calTable.Any()) return inputVal;

            var floorQuery = calTable
                .Where(c => c.Input <= inputVal)
                .OrderByDescending(x => x.Input)
                .ToList();

            CalibrationPoint floor;
            if (floorQuery.Any())
            {
                floor = floorQuery.First();
            }
            else
            {
                return inputVal;
            }

            CalibrationPoint ceil;
            var ceilQuery = calTable
                .Where(x => x.Input >= inputVal)
                .OrderBy(x => x.Input)
                .ToList();
            if (ceilQuery.Any())
            {
                ceil = ceilQuery.First();
            }
            else
            {
                return inputVal;
            }

            var calTableInputSegmentRange = (ceil.Input - floor.Input);

            if (calTableInputSegmentRange == 0 || double.IsNaN(1 / calTableInputSegmentRange))
            {
                return inputVal;
            }

            var inputOffsetPctOfCalTableSegmentInputRange = (inputVal - floor.Input) / calTableInputSegmentRange;

            var outputVal = floor.Output + (inputOffsetPctOfCalTableSegmentInputRange * (ceil.Output - floor.Output));
            return outputVal;
        }
    }
}
