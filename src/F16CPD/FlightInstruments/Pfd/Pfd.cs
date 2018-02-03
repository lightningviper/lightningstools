using Common.Drawing;

namespace F16CPD.FlightInstruments.Pfd
{
    //TODO: if WOW or on-ground or zero VVI, climb/dive marker should be zero too -- but BMS sharedmem doesn't do this sometimes in catastrophic situations...
    public sealed class Pfd 
    {

        public const float ADI_ILS_GLIDESLOPE_DEVIATION_LIMIT_DECIMAL_DEGREES = 1.0f;
        public const float ADI_ILS_LOCALIZER_DEVIATION_LIMIT_DECIMAL_DEGREES = 5.0f;
        public const float MAX_INDICATED_RATE_OF_TURN_DECIMAL_DEGREES_PER_SECOND = 3.0f;


        public F16CpdMfdManager Manager { get; set; }

        public void Render(Graphics g, Size renderSize)
        {
            var origTransform = g.Transform;

            var topBox = new Rectangle(0, 0, renderSize.Width, 239);
            var centerYPfd = topBox.Bottom;
            const int centerXPfd = 300;

            if (Manager.FlightData.PfdOffFlag)
            {
                PfdOffRenderer.DrawPfdOff(g, renderSize);
                return;
            }

            AttitudeIndicatorRenderer.DrawAttitudeIndicator(Manager.FlightData, g, renderSize, centerYPfd, centerXPfd, Manager.NightMode);

            //draw airspeed tape Bitmap
            AirspeedTapeRenderer.DrawAirspeedTape(g, Manager.FlightData.IndicatedAirspeedInDecimalFeetPerSecond, Manager.AirspeedIndexInKnots);

            //draw AOA tape
            AoaTapeRenderer.DrawAoaTape(g, centerYPfd, Manager.FlightData.AngleOfAttackInDegrees, Manager.FlightData.AoaOffFlag, Manager.NightMode);
            //draw VVI
            VVITapeRenderer.DrawVVITape(g, centerYPfd, Manager.FlightData.VerticalVelocityInDecimalFeetPerSecond, Manager.FlightData.VviOffFlag, Manager.NightMode);

            g.Transform = origTransform;

            //draw altitude tape
            AltitudeTapeRenderer.DrawAltitudeTape(g, Manager.FlightData.IndicatedAltitudeAboveMeanSeaLevelInDecimalFeet, Manager.FlightData.BarometricPressure, Manager.FlightData.AltimeterUnits, Manager.AltitudeIndexInFeet);

            //draw PFD summary text
            PfdSummaryTextRenderer.DrawPfdSummaryText(g, Manager.FlightData.GroundSpeedInDecimalFeetPerSecond, Manager.FlightData.MachNumber, Manager.FlightData.AutomaticLowAltitudeWarningInFeet, Manager.FlightData.AltitudeAboveGroundLevelInDecimalFeet, Manager.FlightData.RadarAltimeterOffFlag);
        }
    }
}