using F4SharedMem;
using LightningGauges.Renderers.F16.RWR;
using System;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IRWRFlightDataAdapter
    {
        void Adapt(IRWRRenderer scopeRenderer, FlightData flightData);
    }

    class RWRFlightDataAdapter : IRWRFlightDataAdapter
    {
        public void Adapt(IRWRRenderer scopeRenderer, FlightData flightData)
        {
            scopeRenderer.InstrumentState.yaw = flightData.yaw;
            scopeRenderer.InstrumentState.RwrObjectCount = flightData.RwrObjectCount;
            scopeRenderer.InstrumentState.RWRsymbol = flightData.RWRsymbol ?? Array.Empty<int>();
            scopeRenderer.InstrumentState.bearing = flightData.bearing ?? Array.Empty<float>();
            scopeRenderer.InstrumentState.missileActivity = flightData.missileActivity ?? Array.Empty<int>();
            scopeRenderer.InstrumentState.missileLaunch = flightData.missileLaunch ?? Array.Empty<int>();
            scopeRenderer.InstrumentState.selected = flightData.selected ?? Array.Empty<int>();
            scopeRenderer.InstrumentState.lethality = flightData.lethality ?? Array.Empty<float>();
            scopeRenderer.InstrumentState.newDetection = flightData.newDetection ?? Array.Empty<int>();
            scopeRenderer.InstrumentState.RwrInfo = flightData.RwrInfo ?? Array.Empty<byte>();
            scopeRenderer.InstrumentState.ChaffCount = flightData.ChaffCount;
            scopeRenderer.InstrumentState.FlareCount = flightData.FlareCount;
    }
    }
}
