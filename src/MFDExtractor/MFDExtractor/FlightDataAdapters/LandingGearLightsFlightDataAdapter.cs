using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface ILandingGearLightsFlightDataAdapter
    {
        void Adapt(ILandingGearWheelsLights landingGearLights, FlightData flightData);
    }

    class LandingGearLightsFlightDataAdapter : ILandingGearLightsFlightDataAdapter
    {
        public void Adapt(ILandingGearWheelsLights landingGearLights, FlightData flightData)
        {
            landingGearLights.InstrumentState.LeftGearDown = ((flightData.lightBits3 & (int)LightBits3.LeftGearDown) == (int)LightBits3.LeftGearDown);
            landingGearLights.InstrumentState.NoseGearDown = ((flightData.lightBits3 & (int)LightBits3.NoseGearDown) == (int)LightBits3.NoseGearDown);
            landingGearLights.InstrumentState.RightGearDown = ((flightData.lightBits3 & (int)LightBits3.RightGearDown) == (int)LightBits3.RightGearDown);
        }
    }
}
