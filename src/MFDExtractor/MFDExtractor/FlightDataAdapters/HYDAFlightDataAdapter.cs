using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IHYDAFlightDataAdapter
    {
        void Adapt(IHydraulicPressureGauge hydraulicPressureGaugeA, FlightData flightData);
    }

    class HYDAFlightDataAdapter : IHYDAFlightDataAdapter
    {
        public void Adapt(IHydraulicPressureGauge hydraulicPressureGaugeA, FlightData flightData)
        {
            hydraulicPressureGaugeA.InstrumentState.HydraulicPressurePoundsPerSquareInch = flightData.hydPressureA;
        }
    }
}
