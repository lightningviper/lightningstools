using F4SharedMem;
using F4SharedMem.Headers;
using F4Utils.Terrain;
using LightningGauges.Renderers.F16.ISIS;
using F4Utils.SimSupport;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IISISFlightDataAdapter
    {
        void Adapt(IISIS isis, FlightData flightData);
    }
    class ISISFlightDataAdapter:IISISFlightDataAdapter
    {
        public void Adapt(IISIS isis, FlightData flightData)
        {
            var altbits = (AltBits) flightData.altBits;
            var hsibits = (HsiBits) flightData.hsiBits;
            isis.InstrumentState.AirspeedKnots = flightData.kias;
            isis.InstrumentState.IndicatedAltitudeFeetMSL = -flightData.aauz;
	        isis.Options.PressureAltitudeUnits = ((altbits & AltBits.CalType) == AltBits.CalType) 
				? PressureUnits.InchesOfMercury 
				: PressureUnits.Millibars;

	        isis.InstrumentState.BarometricPressure = flightData.AltCalReading;
            isis.InstrumentState.RadarAltitudeAGL = flightData.RALT;
			isis.InstrumentState.MachNumber = flightData.mach;
            isis.InstrumentState.MagneticHeadingDegrees = (360 +(flightData.yaw/Common.Math.Constants.RADIANS_PER_DEGREE))%360;
            isis.InstrumentState.NeverExceedSpeedKnots = 850;
            isis.InstrumentState.PitchDegrees = ((flightData.pitch/Common.Math.Constants.RADIANS_PER_DEGREE));
            isis.InstrumentState.RollDegrees = ((flightData.roll/Common.Math.Constants.RADIANS_PER_DEGREE));
            isis.InstrumentState.VerticalVelocityFeetPerMinute = -flightData.zDot*60.0f;
            isis.InstrumentState.OffFlag = ((hsibits & HsiBits.ADI_OFF) == HsiBits.ADI_OFF);
            isis.InstrumentState.AuxFlag = ((hsibits & HsiBits.ADI_AUX) == HsiBits.ADI_AUX);
            isis.InstrumentState.GlideslopeFlag = ((hsibits & HsiBits.ADI_GS) == HsiBits.ADI_GS);
            isis.InstrumentState.LocalizerFlag = ((hsibits & HsiBits.ADI_LOC) ==HsiBits.ADI_LOC);
        }
    }
}
