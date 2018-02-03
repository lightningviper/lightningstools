using Common.Drawing;
using Common.Drawing.Drawing2D;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class MarkerBeaconRenderer
    {
        internal static void DrawMarkerBeacon(Graphics g, Point topGlideSlopeMarkerCenterPoint, FlightData flightData)
        {
            //Draw MARKER BEACON light
            var greenColor = Color.FromArgb(0, 255, 0);
            var greenBrush = new SolidBrush(greenColor);
            var markerBeaconFont = new Font("Lucida Console", 10, FontStyle.Bold);

            var markerBeaconIndicatorRectangle = new Rectangle(topGlideSlopeMarkerCenterPoint.X, 430, 30, 30);
            Brush markerBeaconBackgroundBrush;
            if (flightData.MarkerBeaconOuterMarkerFlag)
            {
                markerBeaconBackgroundBrush = greenBrush;
            }
            else if (flightData.MarkerBeaconMiddleMarkerFlag)
            {
                markerBeaconBackgroundBrush = greenBrush;
            }
            else
            {
                markerBeaconBackgroundBrush = Brushes.Black;
            }
            g.FillEllipse(markerBeaconBackgroundBrush, markerBeaconIndicatorRectangle);
            const string markerBeaconString = "MRK\nBCN";
            var path = new GraphicsPath();
            var sf = new StringFormat {LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
            path.AddString(markerBeaconString, markerBeaconFont.FontFamily, (int) markerBeaconFont.Style,
                markerBeaconFont.SizeInPoints, markerBeaconIndicatorRectangle, sf);
            Brush markerBeaconTextBrush;
            if (flightData.MarkerBeaconOuterMarkerFlag || flightData.MarkerBeaconMiddleMarkerFlag)
            {
                markerBeaconTextBrush = Brushes.Black;
            }
            else
            {
                markerBeaconTextBrush = Brushes.White;
            }
            g.FillPathFast(markerBeaconTextBrush, path);
        }
    }
}