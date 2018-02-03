using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IFuelFlowFlightDataAdapter
    {
        void Adapt(IFuelFlow fuelFlow, FlightData flightData);
    }

    class FuelFlowFlightDataAdapter : IFuelFlowFlightDataAdapter
    {
        public void Adapt(IFuelFlow fuelFlow, FlightData flightData)
        {
            fuelFlow.InstrumentState.FuelFlowPoundsPerHour = flightData.fuelFlow;
        }
    }
}
