using Common.Drawing;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AdiSkyAndGroundRenderer
    {
        internal static void DrawAdiSkyAndGround(Graphics g, Size renderSize, int centerYPfd, int centerXPfd,
            float pitchAngleDegrees, float rollAngleDegrees,
            float pixelsSeparationPerDegreeOfPitch, bool adiOffFlag)
        {
            //draw sky/ground
            var skyColor = Color.FromArgb(50, 145, 255);
            var groundColor = Color.FromArgb(215, 97, 55);
            Brush skyBrush = new SolidBrush(skyColor);

            Brush groundBrush = new SolidBrush(groundColor);
            {
                var curTransform = g.Transform;
                var centerX = centerXPfd;
                var centerY = centerYPfd;
                if (!adiOffFlag)
                {
                    g.TranslateTransform(centerX, centerY);
                    g.RotateTransform(-rollAngleDegrees);
                    g.TranslateTransform(-centerX, -centerY);
                    g.TranslateTransform(0, (pixelsSeparationPerDegreeOfPitch * pitchAngleDegrees));
                }
                g.FillRectangle(skyBrush,
                    new Rectangle(-(renderSize.Width / 2),
                        (int)(-pixelsSeparationPerDegreeOfPitch * 180) + centerY,
                        (renderSize.Width * 2), (int)(pixelsSeparationPerDegreeOfPitch * 180)));
                g.FillRectangle(groundBrush,
                    new Rectangle(-(renderSize.Width / 2), centerY, (renderSize.Width * 2),
                        (int)(pixelsSeparationPerDegreeOfPitch * 180)));
                var horizonLinePen = new Pen(Color.Black) { Width = 1 };
                g.DrawLineFast(horizonLinePen, -1000, centerY, renderSize.Width + 1000, centerYPfd);
                g.Transform = curTransform;
            }
        }
    }
}