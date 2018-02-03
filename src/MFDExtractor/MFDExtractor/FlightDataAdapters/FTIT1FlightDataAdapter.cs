using F4SharedMem;
using F4Utils.SimSupport;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IFTIT1FlightDataAdapter
    {
        void Adapt(IFanTurbineInletTemperature ftit1, FlightData flightData);
    }

    class FTIT1FlightDataAdapter : IFTIT1FlightDataAdapter
    {
        public void Adapt(IFanTurbineInletTemperature ftit1, FlightData flightData)
        {
            ftit1.InstrumentState.InletTemperatureDegreesCelcius = flightData.ftit * 100.0f;
        }
    }
}
