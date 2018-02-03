using Common.Drawing;
using Common.Imaging;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class RateOfTurnIndicatorRenderer
    {
        internal static void DrawRateOfTurnIndicator(Graphics g, float rateOfTurnInDecimalDegreesPerSecond)
        {
            var whitePen = new Pen(Color.White);
            //draw rate of turn indicator
            const int dashWidth = 31;
            const int dashHeight = 5;
            const int leftDashX = 222;
            const int leftDashY = 450;
            var dashWhitePen = new Pen(Color.White);
            var dashRedPen = new Pen(Color.FromArgb(89, 1, 0));
            Pen pen;
            for (var i = 1; i <= 5; i++)
            {
                pen = i % 2 == 1 ? dashWhitePen : dashRedPen;
                pen.Width = dashHeight;
                g.DrawLineFast(pen, new Point(leftDashX + (dashWidth * (i - 1)), leftDashY),
                    new Point(leftDashX + (dashWidth * (i)), leftDashY));
            }
            const int rateOfTurnXRange = (int)((dashWidth * 4.0f));
            var rateOfTurnCenterXPos = leftDashX + (rateOfTurnXRange / 2) + (dashWidth / 2);
            rateOfTurnCenterXPos =
                (int)((rateOfTurnInDecimalDegreesPerSecond / Pfd.MAX_INDICATED_RATE_OF_TURN_DECIMAL_DEGREES_PER_SECOND) * (rateOfTurnXRange / 2)) +
                rateOfTurnCenterXPos;

            whitePen.Width = 5;
            g.DrawLineFast(whitePen, rateOfTurnCenterXPos - (dashWidth / 2), leftDashY + 7,
                rateOfTurnCenterXPos + (dashWidth / 2), leftDashY + 7);
            g.DrawLineFast(whitePen, rateOfTurnCenterXPos, leftDashY + 7, rateOfTurnCenterXPos, leftDashY + 14);
        }
        
    }
}