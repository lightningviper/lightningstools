using System.Text;
using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IPFLFlightDataAdapter
    {
        void Adapt(IDataEntryDisplayPilotFaultList pfl, FlightData flightData);
    }

    class PFLFlightDataAdapter : IPFLFlightDataAdapter
    {
        public void Adapt(IDataEntryDisplayPilotFaultList pfl, FlightData flightData)
        {
            if (flightData.PFLLines != null)
            {
                pfl.InstrumentState.Line1 = Encoding.Default.GetBytes(flightData.PFLLines[0] ?? "");
                pfl.InstrumentState.Line2 = Encoding.Default.GetBytes(flightData.PFLLines[1] ?? "");
                pfl.InstrumentState.Line3 = Encoding.Default.GetBytes(flightData.PFLLines[2] ?? "");
                pfl.InstrumentState.Line4 = Encoding.Default.GetBytes(flightData.PFLLines[3] ?? "");
                pfl.InstrumentState.Line5 = Encoding.Default.GetBytes(flightData.PFLLines[4] ?? "");
            }
            if (flightData.PFLInvert != null)
            {
                pfl.InstrumentState.Line1Invert = Encoding.Default.GetBytes(flightData.PFLInvert[0] ?? "");
                pfl.InstrumentState.Line2Invert = Encoding.Default.GetBytes(flightData.PFLInvert[1] ?? "");
                pfl.InstrumentState.Line3Invert = Encoding.Default.GetBytes(flightData.PFLInvert[2] ?? "");
                pfl.InstrumentState.Line4Invert = Encoding.Default.GetBytes(flightData.PFLInvert[3] ?? "");
                pfl.InstrumentState.Line5Invert = Encoding.Default.GetBytes(flightData.PFLInvert[4] ?? "");
            }

        }
    }
}
