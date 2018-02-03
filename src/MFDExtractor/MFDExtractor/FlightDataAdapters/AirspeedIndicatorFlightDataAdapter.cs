using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IAirspeedIndicatorFlightDataAdapter
    {
        void Adapt(IAirspeedIndicator airspeedIndicator, FlightData flightData);
    }

    class AirspeedIndicatorFlightDataAdapter : IAirspeedIndicatorFlightDataAdapter
    {
        public void Adapt(IAirspeedIndicator airspeedIndicator, FlightData flightData)
        {
            airspeedIndicator.InstrumentState.AirspeedKnots = flightData.kias;
            airspeedIndicator.InstrumentState.MachNumber = flightData.mach;
        }
    }
}
