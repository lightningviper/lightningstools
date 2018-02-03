using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IRollTrimIndicatorFlightDataAdapter
    {
        void Adapt(IRollTrimIndicator rollTrimIndicator, FlightData flightData);
    }

    class RollTrimIndicatorFlightDataAdapter : IRollTrimIndicatorFlightDataAdapter
    {
        public void Adapt(IRollTrimIndicator rollTrimIndicator, FlightData flightData)
        {
            var rolltrim = flightData.TrimRoll;
            rollTrimIndicator.InstrumentState.RollTrimPercent = rolltrim * 2.0f * 100.0f;

        }
    }
}
