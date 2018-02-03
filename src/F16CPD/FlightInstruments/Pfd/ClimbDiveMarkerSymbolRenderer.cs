using F16CPD.Properties;
using Common.Drawing;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class ClimbDiveMarkerSymbolRenderer
    {
        private static Bitmap _climbDiveMarkerSymbol;
        private static Bitmap _cagedClimbDiveMarkerSymbol;

        internal static void DrawClimbDiveMarkerSymbol(Graphics g, int centerYPfd, int centerXPfd, float aoaAngleDegrees,
            float pixelsSeparationPerDegreeOfPitch, bool adiOffFlag)
        {

            if (_climbDiveMarkerSymbol == null)
            {
                _climbDiveMarkerSymbol = Resources.climbDiveMarker;
                _climbDiveMarkerSymbol.MakeTransparent(Color.FromArgb(0, 255, 0, 128));
            }

            if (_cagedClimbDiveMarkerSymbol == null)
            {
                _cagedClimbDiveMarkerSymbol = Resources.cagedClimbDiveMarker;
                _cagedClimbDiveMarkerSymbol.MakeTransparent(Color.FromArgb(0, 255, 0, 128));
            }
            //draw climb/dive marker symbol
            var climbDiveMarkerCenterX = (centerXPfd - (_climbDiveMarkerSymbol.Width / 2)) - 2;
            var climbDiveMarkerCenterY =
                (int)
                    (centerYPfd - (_climbDiveMarkerSymbol.Height / 2) + (aoaAngleDegrees * pixelsSeparationPerDegreeOfPitch) - 7) +
                1;
            const int minCdmCenterX = 130;
            const int minCdmCenterY = 30;
            const int maxCdmX = 470;
            const int maxCdmY = 420;

            var climbDiveMarkerOutOfBounds = false;
            if (climbDiveMarkerCenterX < minCdmCenterX)
            {
                climbDiveMarkerCenterX = minCdmCenterX;
                climbDiveMarkerOutOfBounds = true;
            }
            else if (climbDiveMarkerCenterX > maxCdmX)
            {
                climbDiveMarkerCenterX = maxCdmX;
                climbDiveMarkerOutOfBounds = true;
            }
            else if (climbDiveMarkerCenterY < minCdmCenterY)
            {
                climbDiveMarkerCenterY = minCdmCenterY;
                climbDiveMarkerOutOfBounds = true;
            }
            else if (climbDiveMarkerCenterY > maxCdmY)
            {
                climbDiveMarkerCenterY = maxCdmY;
                climbDiveMarkerOutOfBounds = true;
            }
            if (!adiOffFlag)
            {
                g.DrawImageFast(climbDiveMarkerOutOfBounds ? _cagedClimbDiveMarkerSymbol : _climbDiveMarkerSymbol,
                    climbDiveMarkerCenterX, climbDiveMarkerCenterY);
            }
        }
    }
}