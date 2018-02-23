using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal static class PathFigureExtensions
    {
        public static IEnumerable<Point> GetPoints(this PathFigure pathFigure)
        {
            var points = new List<Point> { pathFigure.StartPoint };
            foreach (var segment in pathFigure.Segments)
            {
                if (segment is LineSegment ls)
                {
                    points.Add(ls.Point);
                }
                else if (segment is PolyLineSegment pls)
                {
                    foreach (var point in pls.Points)
                    {
                        points.Add(point);
                    }
                }
            }
            return points;
        }
    }
}
