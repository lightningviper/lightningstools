using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface ICMDSFlightDataAdapter
    {
        void Adapt(ICMDSPanel cmdsPanel, FlightData flightData);
    }

    class CMDSFlightDataAdapter : ICMDSFlightDataAdapter
    {
        public void Adapt(ICMDSPanel cmdsPanel, FlightData flightData)
        {
            cmdsPanel.InstrumentState.Degraded = ((flightData.lightBits2 & (int)LightBits2.Degr) == (int)LightBits2.Degr);
            cmdsPanel.InstrumentState.ChaffCount = (int)flightData.ChaffCount;
            cmdsPanel.InstrumentState.ChaffLow = ((flightData.lightBits2 & (int)LightBits2.ChaffLo) == (int)LightBits2.ChaffLo);
            cmdsPanel.InstrumentState.DispenseReady = ((flightData.lightBits2 & (int)LightBits2.Rdy) == (int)LightBits2.Rdy);
            cmdsPanel.InstrumentState.FlareCount = (int)flightData.FlareCount;
            cmdsPanel.InstrumentState.FlareLow = ((flightData.lightBits2 & (int)LightBits2.FlareLo) == (int)LightBits2.FlareLo);
            cmdsPanel.InstrumentState.Go = (((flightData.lightBits2 & (int)LightBits2.Go) == (int)LightBits2.Go));
            cmdsPanel.InstrumentState.NoGo = (((flightData.lightBits2 & (int)LightBits2.NoGo) == (int)LightBits2.NoGo));
            cmdsPanel.InstrumentState.Other1Count = 0;
            cmdsPanel.InstrumentState.Other1Low = true;
            cmdsPanel.InstrumentState.Other2Count = 0;
            cmdsPanel.InstrumentState.Other2Low = true;
        }
    }
}
