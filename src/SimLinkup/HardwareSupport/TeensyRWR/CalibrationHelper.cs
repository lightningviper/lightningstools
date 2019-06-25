using System;
using System.Collections.Generic;
using System.Linq;
using Common.MacroProgramming;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class CalibrationHelper
    {
        public static DrawPoint CalibrateDrawPoint(DrawPoint drawPoint,
            IEnumerable<CalibrationPoint> xAxisCalibrationData, IEnumerable<CalibrationPoint> yAxisCalibrationData)
        {
            return new DrawPoint()
            {
                BeamOn = drawPoint.BeamOn, X = CalibratedOutput(xAxisCalibrationData, drawPoint.X),
                Y = CalibratedOutput(yAxisCalibrationData, drawPoint.Y)
            };
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
            
            if (calTableInputSegmentRange == 0 || double.IsNaN(1 / calTableInputSegmentRange) )
            {
                return inputVal;
            }

            var inputOffsetPctOfCalTableSegmentInputRange = (inputVal - floor.Input) / calTableInputSegmentRange;

            var outputVal =  floor.Output + (inputOffsetPctOfCalTableSegmentInputRange * (ceil.Output - floor.Output));
            return outputVal;
        }
    }
}
