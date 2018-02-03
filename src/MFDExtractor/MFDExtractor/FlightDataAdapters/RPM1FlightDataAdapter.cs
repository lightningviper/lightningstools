using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IRPM1FlightDataAdapter
    {
        void Adapt(ITachometer rpm1, FlightData flightData);
    }

    class RPM1FlightDataAdapter : IRPM1FlightDataAdapter
    {
        public void Adapt(ITachometer rpm1, FlightData flightData)
        {
            rpm1.InstrumentState.RPMPercent = flightData.rpm;
        }
    }
}
