using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IHYDBFlightDataAdapter
    {
        void Adapt(IHydraulicPressureGauge hydraulicPressureGaugeB, FlightData flightData);
    }

    class HYDBFlightDataAdapter : IHYDBFlightDataAdapter
    {
        public void Adapt(IHydraulicPressureGauge hydraulicPressureGaugeB, FlightData flightData)
        {
            hydraulicPressureGaugeB.InstrumentState.HydraulicPressurePoundsPerSquareInch = flightData.hydPressureB;
        }
    }
}
