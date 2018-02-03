using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IEPUFuelFlightDataAdapter
    {
        void Adapt(IEPUFuelGauge epuFuelGauge, FlightData flightData);
    }

    class EPUFuelFlightDataAdapter : IEPUFuelFlightDataAdapter
    {
        public void Adapt(IEPUFuelGauge epuFuelGauge, FlightData flightData)
        {
            epuFuelGauge.InstrumentState.FuelRemainingPercent = flightData.epuFuel;
        }
    }
}
