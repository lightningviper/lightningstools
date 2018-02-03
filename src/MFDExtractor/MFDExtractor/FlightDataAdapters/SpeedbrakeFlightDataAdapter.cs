using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface ISpeedbrakeFlightDataAdapter
    {
        void Adapt(ISpeedbrakeIndicator speedbrakeIndicator, FlightData flightData);
    }

    class SpeedbrakeFlightDataAdapter : ISpeedbrakeFlightDataAdapter
    {
        public void Adapt(ISpeedbrakeIndicator speedbrakeIndicator, FlightData flightData)
        {
            speedbrakeIndicator.InstrumentState.PercentOpen = flightData.speedBrake * 100.0f;
            speedbrakeIndicator.InstrumentState.PowerLoss = ((flightData.lightBits3 & (int)LightBits3.Power_Off) == (int)LightBits3.Power_Off);
        }
    }
}
