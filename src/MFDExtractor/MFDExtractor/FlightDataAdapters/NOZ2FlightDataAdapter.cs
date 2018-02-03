using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface INOZ2FlightDataAdapter
    {
        void Adapt(INozzlePositionIndicator nozzlePositionIndicator, FlightData flightData);
    }

    class NOZ2FlightDataAdapter : INOZ2FlightDataAdapter
    {
        public void Adapt(INozzlePositionIndicator nozzlePositionIndicator, FlightData flightData)
        {
            nozzlePositionIndicator.InstrumentState.NozzlePositionPercent = flightData.nozzlePos2 * 100.0f;
        }
    }
}
