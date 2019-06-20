using System.Text;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    internal class GridCalibrationPattern
    {
        public static string Get()
        {
            return getVLinesSVG() + getHLinesSVG() + getSquareSVG(0,0,4095,4095);
        }
        private static string getSquareSVG(float x, float y, float width, float height)
        {
            return $"M{(int)x},{(int)y} " +
                   $"L{(int)(x + width)},{(int)y} " +
                   $"L{(int)(x + width)},{(int)(y + height)} " +
                   $"L{(int)x},{(int)(y + height)} " +
                   $"L{(int)x},{(int)y} ";
        }

        private static string getVLinesSVG()
        {
            var sb = new StringBuilder();
            for (var x = 0.0; x < 4095; x += 100)
            {
                sb.Append($"M{x},0 L{x},4095 ");
            }

            return sb.ToString();
        }
        private static string getHLinesSVG()
        {
            var sb = new StringBuilder();
            for (var y = 0.0; y < 4095; y += 100)
            {
                sb.Append($"M0,{y} L4095,{y} ");
            }

            return sb.ToString();
        }

    }
}
