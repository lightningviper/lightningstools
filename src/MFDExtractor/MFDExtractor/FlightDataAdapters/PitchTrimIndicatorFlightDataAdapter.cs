using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IPitchTrimIndicatorFlightDataAdapter
    {
        void Adapt(IPitchTrimIndicator pitchTrimIndicator, FlightData flightData);
    }

    class PitchTrimIndicatorFlightDataAdapter : IPitchTrimIndicatorFlightDataAdapter
    {
        public void Adapt(IPitchTrimIndicator pitchTrimIndicator, FlightData flightData)
        {
            var pitchTrim = flightData.TrimPitch;
            pitchTrimIndicator.InstrumentState.PitchTrimPercent = pitchTrim * 2.0f * 100.0f;        }
    }
}
