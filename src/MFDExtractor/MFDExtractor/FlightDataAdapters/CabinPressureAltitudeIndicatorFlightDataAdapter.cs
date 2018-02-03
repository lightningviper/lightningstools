using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface ICabinPressureAltitudeIndicatorFlightDataAdapter
    {
        void Adapt(ICabinPressureAltitudeIndicator cabinPressureAltitudeIndicator, FlightData flightData);
    }

    class CabinPressureAltitudeIndicatorFlightDataAdapter : ICabinPressureAltitudeIndicatorFlightDataAdapter
    {
        public void Adapt(ICabinPressureAltitudeIndicator cabinPressureAltitudeIndicator, FlightData flightData)
        {
            cabinPressureAltitudeIndicator.InstrumentState.CabinPressureAltitudeFeet = flightData.cabinAlt;
        }
    }
}
