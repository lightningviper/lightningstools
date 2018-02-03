using F4SharedMem;
using F4Utils.SimSupport;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface INOZ1FlightDataAdapter
    {
        void Adapt(INozzlePositionIndicator nozzlePositionIndicator, FlightData flightData);
    }

    class NOZ1FlightDataAdapter : INOZ1FlightDataAdapter
    {
        public void Adapt(INozzlePositionIndicator nozzlePositionIndicator, FlightData flightData)
        {
            nozzlePositionIndicator.InstrumentState.NozzlePositionPercent = flightData.nozzlePos * 100.0f;
        }
    }
}
