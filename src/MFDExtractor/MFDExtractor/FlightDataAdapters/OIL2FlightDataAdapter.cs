using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IOIL2FlightDataAdapter
    {
        void Adapt(IOilPressureGauge oil2, FlightData flightData);
    }

    class OIL2FlightDataAdapter : IOIL2FlightDataAdapter
    {
        public void Adapt(IOilPressureGauge oil2, FlightData flightData)
        {
            oil2.InstrumentState.OilPressurePercent = flightData.oilPressure2;
        }
    }
}
