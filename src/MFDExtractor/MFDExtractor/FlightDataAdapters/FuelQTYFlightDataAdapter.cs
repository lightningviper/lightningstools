using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IFuelQuantityFlightDataAdapter
    {
        void Adapt(IFuelQuantityIndicator fuelQuantityIndicator, FlightData flightData);
    }

    class FuelQuantityFlightDataAdapter : IFuelQuantityFlightDataAdapter
    {
        public void Adapt(IFuelQuantityIndicator fuelQuantityIndicator, FlightData flightData)
        {
            fuelQuantityIndicator.InstrumentState.AftLeftFuelQuantityPounds =flightData.aft/10.0f;
            fuelQuantityIndicator.InstrumentState.ForeRightFuelQuantityPounds =flightData.fwd/10.0f;
            fuelQuantityIndicator.InstrumentState.TotalFuelQuantityPounds =flightData.total;

        }
    }
}
