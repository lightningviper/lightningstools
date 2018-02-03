using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface INWSFlightDataAdapter
    {
        void Adapt(INosewheelSteeringIndexer nwsIndexer, FlightData flightData);
    }

    class NWSFlightDataAdapter : INWSFlightDataAdapter
    {
        public void Adapt(INosewheelSteeringIndexer nwsIndexer, FlightData flightData)

        {
            nwsIndexer.InstrumentState.DISC = ((flightData.lightBits & (int)LightBits.RefuelDSC) == (int)LightBits.RefuelDSC);
            nwsIndexer.InstrumentState.AR_NWS = ((flightData.lightBits & (int)LightBits.RefuelAR) == (int)LightBits.RefuelAR);
            nwsIndexer.InstrumentState.RDY = ((flightData.lightBits & (int)LightBits.RefuelRDY) == (int)LightBits.RefuelRDY);
        }
    }
}
