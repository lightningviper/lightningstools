using System;
using System.IO;
using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class ILSBarsRenderer
    {
        private const string ISIS_ILS_DOT_IMAGE_FILENAME = "isis_ilsdot.bmp";
        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        internal static readonly Bitmap MarkerDiamond;

        static ILSBarsRenderer()
        {
            if (MarkerDiamond == null)
            {
                using (var markerDiamond = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ISIS_ILS_DOT_IMAGE_FILENAME))
                {
                    markerDiamond.MakeTransparent(Color.FromArgb(255, 0, 255));
                    MarkerDiamond = (Bitmap) Util.ResizeBitmap(markerDiamond, new Size(15, 15));
                }
            }
        }

        internal static void DrawIlsBars(Graphics g, int width, int height, ref GraphicsState basicState, InstrumentState instrumentState)
        {
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);

            //prepare to draw glideslope and localizer stuff
            const float distanceBetweenDots = 19.5f;
            const float farLeftLocalizerMarkerX = 82;
            float farLeftLocalizerMarkerY = height - 55;
            var farLeftLocalizerMarkerCenterPoint = new PointF(farLeftLocalizerMarkerX, farLeftLocalizerMarkerY);
            var leftMiddleLocalizerMarkerCenterPoint = new PointF(farLeftLocalizerMarkerCenterPoint.X + distanceBetweenDots, farLeftLocalizerMarkerCenterPoint.Y);
            var middleLocalizerMarkerCenterPoint = new PointF(farLeftLocalizerMarkerCenterPoint.X + distanceBetweenDots * 2, farLeftLocalizerMarkerCenterPoint.Y);
            var rightMiddleLocalizerMarkerCenterPoint = new PointF(farLeftLocalizerMarkerCenterPoint.X + distanceBetweenDots * 3, farLeftLocalizerMarkerCenterPoint.Y);
            var farRightLocalizerMarkerCenterPoint = new PointF(farLeftLocalizerMarkerCenterPoint.X + distanceBetweenDots * 4, farLeftLocalizerMarkerCenterPoint.Y);

            float topLeftGlideslopeMarkerX = width - 76;
            const float topLeftGlideslopeMarkerY = 89;
            var topGlideSlopeMarkerCenterPoint = new PointF(topLeftGlideslopeMarkerX, topLeftGlideslopeMarkerY);
            var upperMiddleGlideSlopeMarkerCenterPoint = new PointF(topGlideSlopeMarkerCenterPoint.X, topGlideSlopeMarkerCenterPoint.Y + distanceBetweenDots);
            var middleGlideSlopeMarkerCenterPoint = new PointF(topGlideSlopeMarkerCenterPoint.X, topGlideSlopeMarkerCenterPoint.Y + distanceBetweenDots * 2);
            var lowerMiddleGlideSlopeMarkerCenterPoint = new PointF(topGlideSlopeMarkerCenterPoint.X, topGlideSlopeMarkerCenterPoint.Y + distanceBetweenDots * 3);
            var bottomGlideSlopeMarkerCenterPoint = new PointF(topGlideSlopeMarkerCenterPoint.X, topGlideSlopeMarkerCenterPoint.Y + distanceBetweenDots * 4);

            LocalizerCommandBarRenderer.DrawLocalizerCommandBar(
                g, instrumentState, farLeftLocalizerMarkerCenterPoint, farRightLocalizerMarkerCenterPoint, topGlideSlopeMarkerCenterPoint, bottomGlideSlopeMarkerCenterPoint);

            GlideslopeCommandBarRenderer.DrawGlideslopeCommandBar(
                g, instrumentState, topGlideSlopeMarkerCenterPoint, bottomGlideSlopeMarkerCenterPoint, farLeftLocalizerMarkerCenterPoint, farRightLocalizerMarkerCenterPoint);

            GlideslopeMarkersRenderer.DrawGlideslopeMarkers(
                g, topGlideSlopeMarkerCenterPoint, upperMiddleGlideSlopeMarkerCenterPoint, middleGlideSlopeMarkerCenterPoint, lowerMiddleGlideSlopeMarkerCenterPoint, bottomGlideSlopeMarkerCenterPoint,
                instrumentState);
            LocalizerMarkersRenderer.DrawLocalizerMarkers(
                g, farLeftLocalizerMarkerCenterPoint, leftMiddleLocalizerMarkerCenterPoint, middleLocalizerMarkerCenterPoint, farRightLocalizerMarkerCenterPoint, rightMiddleLocalizerMarkerCenterPoint,
                instrumentState);
            GraphicsUtil.RestoreGraphicsState(g, ref basicState);
        }
    }
}