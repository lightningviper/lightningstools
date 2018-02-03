using System;
using Common.Drawing;
using Common.Imaging;
using F16CPD.Properties;

namespace F16CPD.FlightInstruments.Pfd
{
    internal class AttitudeIndicatorRenderer
    {
        private static Bitmap _markerDiamond;

        internal static void DrawAttitudeIndicator(FlightData flightData, Graphics g, Size renderSize, int centerYPfd, int centerXPfd, bool nightMode)
        {
            //************************************
            //ADI 
            //************************************
            var pitchAngleDegrees = flightData.PitchAngleInDecimalDegrees;
            var rollAngleDegrees = flightData.RollAngleInDecimalDegrees;
            var aoaAngleDegrees = flightData.AngleOfAttackInDegrees;
            const int verticalDistanceBetweenPitchLines = 25;
            const int degreesBetweenTicks = 5;
            const float pixelsSeparationPerDegreeOfPitch = verticalDistanceBetweenPitchLines/degreesBetweenTicks;


            AdiSkyAndGroundRenderer.DrawAdiSkyAndGround(g, renderSize, centerYPfd, centerXPfd, pitchAngleDegrees, rollAngleDegrees,
                pixelsSeparationPerDegreeOfPitch, flightData.AdiOffFlag);
            AdiPitchLadderRenderer.DrawAdiPitchLadder(g, centerYPfd, centerXPfd, pitchAngleDegrees, rollAngleDegrees, flightData.AdiOffFlag);
            AdiRollIndexLinesRenderer.DrawAdiRollIndexLines(g, centerXPfd, centerYPfd);

            var whitePen = new Pen(Color.White);
            var blackPen = new Pen(Color.Black);
            whitePen.Width = 2;
            blackPen.Width = 1;


            var zeroDegreeTriangle = AdiRollTrianglesRenderer.DrawAdiRollTriangles(g, centerXPfd, centerYPfd, flightData.RollAngleInDecimalDegrees, flightData.AdiOffFlag);
            AdiGsFlagRenderer.DrawAdiGsFlag(g, new Point(zeroDegreeTriangle[0].X - 89, zeroDegreeTriangle[0].Y - 30), flightData.AdiGlideslopeInvalidFlag, nightMode);
            AdiAuxFlagRenderer.DrawAdiAuxFlag(g, new Point(zeroDegreeTriangle[0].X - 16, zeroDegreeTriangle[0].Y - 30), flightData.AdiAuxFlag);
            AdiLocFlagRenderer.DrawAdiLocFlag(g, new Point(zeroDegreeTriangle[0].X + 56, zeroDegreeTriangle[0].Y - 30), flightData.AdiLocalizerInvalidFlag, nightMode);
            AdiOffFlagRenderer.DrawAdiOffFlag(g, new Point(zeroDegreeTriangle[0].X - 22, centerYPfd - 12), flightData.AdiOffFlag, nightMode);
            AdiFixedPitchReferenceBarsRenderer.DrawAdiFixedPitchReferenceBars(g, centerXPfd, centerYPfd, flightData.AdiOffFlag);
            RateOfTurnIndicatorRenderer.DrawRateOfTurnIndicator(g, flightData.RateOfTurnInDecimalDegreesPerSecond);

            whitePen.Width = 2;

            //prepare to draw glideslope and localizer stuff
            var farLeftLocalizerMarkerCenterPoint = new Point(208, 400);
            var leftMiddleLocalizerMarkerCenterPoint = new Point(farLeftLocalizerMarkerCenterPoint.X + 45,
                farLeftLocalizerMarkerCenterPoint.Y);
            var middleLocalizerMarkerCenterPoint = new Point(farLeftLocalizerMarkerCenterPoint.X + 90,
                farLeftLocalizerMarkerCenterPoint.Y);
            var rightMiddleLocalizerMarkerCenterPoint = new Point(farLeftLocalizerMarkerCenterPoint.X + 135,
                farLeftLocalizerMarkerCenterPoint.Y);
            var farRightLocalizerMarkerCenterPoint = new Point(farLeftLocalizerMarkerCenterPoint.X + 180,
                farLeftLocalizerMarkerCenterPoint.Y);

            var topGlideSlopeMarkerCenterPoint = new Point(440, 139);
            var upperMiddleGlideSlopeMarkerCenterPoint = new Point(topGlideSlopeMarkerCenterPoint.X,
                topGlideSlopeMarkerCenterPoint.Y + 50);
            var middleGlideSlopeMarkerCenterPoint = new Point(topGlideSlopeMarkerCenterPoint.X,
                topGlideSlopeMarkerCenterPoint.Y + 100);
            var lowerMiddleGlideSlopeMarkerCenterPoint = new Point(topGlideSlopeMarkerCenterPoint.X,
                topGlideSlopeMarkerCenterPoint.Y + 150);
            var bottomGlideSlopeMarkerCenterPoint = new Point(topGlideSlopeMarkerCenterPoint.X,
                topGlideSlopeMarkerCenterPoint.Y + 200);


            if (_markerDiamond == null)
            {
                var markerDiamond = Resources.adidiamond;
                markerDiamond.MakeTransparent(Color.FromArgb(255, 0, 255));
                _markerDiamond = (Bitmap) Common.Imaging.Util.ResizeBitmap(markerDiamond, new Size(15, 15));
            }

            //prepare draw localizer indicator line

            const float minIlsHorizontalPositionVal = -Pfd.ADI_ILS_LOCALIZER_DEVIATION_LIMIT_DECIMAL_DEGREES;
            const float maxIlsHorizontalPositionVal = Pfd.ADI_ILS_LOCALIZER_DEVIATION_LIMIT_DECIMAL_DEGREES;
            const float IlsHorizontalPositionRange = maxIlsHorizontalPositionVal - minIlsHorizontalPositionVal;
            var currentIlsHorizontalPositionVal = flightData.AdiIlsLocalizerDeviationInDecimalDegrees +
                                                  Math.Abs(minIlsHorizontalPositionVal);
            if (currentIlsHorizontalPositionVal < 0) currentIlsHorizontalPositionVal = 0;
            if (currentIlsHorizontalPositionVal > IlsHorizontalPositionRange)
                currentIlsHorizontalPositionVal = IlsHorizontalPositionRange;

            var minIlsBarX = farLeftLocalizerMarkerCenterPoint.X;
            var maxIlsBarX = farRightLocalizerMarkerCenterPoint.X;
            var ilsBarXRange = (maxIlsBarX - minIlsBarX) + 1;

            var currentIlsBarX =
                (int) (minIlsBarX + ((currentIlsHorizontalPositionVal/IlsHorizontalPositionRange)*ilsBarXRange));

            var ilsBarTop = new Point(currentIlsBarX, topGlideSlopeMarkerCenterPoint.Y);
            var ilsBarBottom = new Point(currentIlsBarX, bottomGlideSlopeMarkerCenterPoint.Y);

            var localizerBarPen = new Pen(Color.Yellow) {Width = 3};
            var glideslopeBarPen = new Pen(Color.Yellow) {Width = 3};
            if (flightData.AdiEnableCommandBars && !flightData.AdiLocalizerInvalidFlag &&
                !flightData.AdiOffFlag)
            {
                //draw localizer command bar
                g.DrawLineFast(localizerBarPen, ilsBarTop, ilsBarBottom);
                g.DrawImageFast(_markerDiamond, currentIlsBarX - (_markerDiamond.Width / 2),
                    farLeftLocalizerMarkerCenterPoint.Y - (_markerDiamond.Width/2));
            }

            //prepare to draw glideslope bar
            const float minIlsVerticalPositionVal = -Pfd.ADI_ILS_GLIDESLOPE_DEVIATION_LIMIT_DECIMAL_DEGREES;
            const float maxIlsVerticalPositionVal = Pfd.ADI_ILS_GLIDESLOPE_DEVIATION_LIMIT_DECIMAL_DEGREES;
            const float IlsVerticalPositionRange = maxIlsVerticalPositionVal - minIlsVerticalPositionVal;

            var currentIlsVerticalPositionVal = (-flightData.AdiIlsGlideslopeDeviationInDecimalDegrees) +
                                                Math.Abs(minIlsVerticalPositionVal);
            if (currentIlsVerticalPositionVal < 0) currentIlsVerticalPositionVal = 0;
            if (currentIlsVerticalPositionVal > IlsVerticalPositionRange)
                currentIlsVerticalPositionVal = IlsVerticalPositionRange;


            var minIlsBarY = topGlideSlopeMarkerCenterPoint.Y;
            var maxIlsBarY = bottomGlideSlopeMarkerCenterPoint.Y;
            var ilsBarYRange = (maxIlsBarY - minIlsBarY) + 1;

            var currentIlsBarY =
                (int) (minIlsBarY + ((currentIlsVerticalPositionVal/IlsVerticalPositionRange)*ilsBarYRange));

            var ilsBarLeft = new Point(farLeftLocalizerMarkerCenterPoint.X - 7, currentIlsBarY);
            var ilsBarRight = new Point(farRightLocalizerMarkerCenterPoint.X + 7, currentIlsBarY);
            MarkerBeaconRenderer.DrawMarkerBeacon(g, topGlideSlopeMarkerCenterPoint, flightData);

            //draw glideslope bar
            if (flightData.AdiEnableCommandBars && !flightData.AdiGlideslopeInvalidFlag &&
                !flightData.AdiOffFlag)
            {
                g.DrawLineFast(glideslopeBarPen, ilsBarLeft, ilsBarRight);
                g.DrawImageFast(_markerDiamond, topGlideSlopeMarkerCenterPoint.X - (_markerDiamond.Width / 2),
                    currentIlsBarY - (_markerDiamond.Width/2));
            }

            GlideslopeMarkersRenderer.DrawGlideslopeMarkers(g, topGlideSlopeMarkerCenterPoint, upperMiddleGlideSlopeMarkerCenterPoint,
                middleGlideSlopeMarkerCenterPoint, lowerMiddleGlideSlopeMarkerCenterPoint,
                bottomGlideSlopeMarkerCenterPoint, flightData.AdiEnableCommandBars, flightData.AdiGlideslopeInvalidFlag, flightData.AdiOffFlag);
            LocalizerMarkersRenderer.DrawLocalizerMarkers(g, farLeftLocalizerMarkerCenterPoint, leftMiddleLocalizerMarkerCenterPoint,
                middleLocalizerMarkerCenterPoint, farRightLocalizerMarkerCenterPoint,
                rightMiddleLocalizerMarkerCenterPoint, flightData.AdiEnableCommandBars, flightData.AdiLocalizerInvalidFlag, flightData.AdiOffFlag);
            ClimbDiveMarkerSymbolRenderer.DrawClimbDiveMarkerSymbol(g, centerYPfd, centerXPfd, aoaAngleDegrees, pixelsSeparationPerDegreeOfPitch, flightData.AdiOffFlag);
        }

    }

}