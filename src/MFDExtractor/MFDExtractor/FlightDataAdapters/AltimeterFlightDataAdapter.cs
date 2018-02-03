using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IAltimeterFlightDataAdapter
    {
        void Adapt(IAltimeter altimeter, FlightData flightData);
    }

    class AltimeterFlightDataAdapter : IAltimeterFlightDataAdapter
    {
        public void Adapt(IAltimeter altimeter, FlightData flightData)
        {
            var altbits = (AltBits)flightData.altBits;
            AdaptAltimeter(altimeter, flightData, altbits);
        }

       
        private static void AdaptAltimeter(IAltimeter altimeter, FlightData fromFalcon, AltBits altbits)
        {
            altimeter.Options.PressureAltitudeUnits = ((altbits & AltBits.CalType) == AltBits.CalType)
                ? Altimeter.AltimeterOptions.PressureUnits.InchesOfMercury
                : Altimeter.AltimeterOptions.PressureUnits.Millibars;

            altimeter.InstrumentState.IndicatedAltitudeFeetMSL = -fromFalcon.aauz;
            altimeter.InstrumentState.BarometricPressure = fromFalcon.AltCalReading;
            altimeter.InstrumentState.PneumaticModeFlag = ((altbits & AltBits.PneuFlag) == AltBits.PneuFlag);
            altimeter.InstrumentState.StandbyModeFlag = ((altbits & AltBits.PneuFlag) == AltBits.PneuFlag);
        }
    }
}
