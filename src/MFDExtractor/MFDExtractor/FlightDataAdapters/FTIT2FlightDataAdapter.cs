using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IFTIT2FlightDataAdapter
    {
        void Adapt(IFanTurbineInletTemperature ftit2, FlightData flightData);
    }

    class FTIT2FlightDataAdapter : IFTIT2FlightDataAdapter
    {
        public void Adapt(IFanTurbineInletTemperature ftit2, FlightData flightData)
        {
            ftit2.InstrumentState.InletTemperatureDegreesCelcius = flightData.ftit2 * 100.0f;
        }
    }
}
