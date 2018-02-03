using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IOIL1FlightDataAdapter
    {
        void Adapt(IOilPressureGauge oil1, FlightData flightData);
    }

    class OIL1FlightDataAdapter : IOIL1FlightDataAdapter
    {
        public void Adapt(IOilPressureGauge oil1, FlightData flightData)
        {
            oil1.InstrumentState.OilPressurePercent = flightData.oilPressure;
        }
    }
}
