using System.Collections.Generic;
using System.Linq;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class CenteringHelper
    {
        public static IEnumerable<DrawPoint> ApplyCentering(IEnumerable<DrawPoint> drawPoints,
            TeensyRWRHardwareSupportModuleConfig.CenteringConfig centeringConfig)
        {
            return drawPoints.Select(drawPoint => new DrawPoint
            {
                BeamOn = drawPoint.BeamOn,
                X = drawPoint.X + centeringConfig.OffsetX,
                Y = drawPoint.Y + centeringConfig.OffsetY
            });
        }
    }
}
