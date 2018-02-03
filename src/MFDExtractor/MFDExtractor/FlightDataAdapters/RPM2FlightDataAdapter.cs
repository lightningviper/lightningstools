using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IRPM2FlightDataAdapter
    {
        void Adapt(ITachometer rpm2, FlightData flightData);
    }

    class RPM2FlightDataAdapter : IRPM2FlightDataAdapter
    {
        public void Adapt(ITachometer rpm2, FlightData flightData)
        {
            rpm2.InstrumentState.RPMPercent = flightData.rpm2;
        }
    }
}
