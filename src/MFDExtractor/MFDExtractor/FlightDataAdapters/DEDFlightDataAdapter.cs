using System.Text;
using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IDEDFlightDataAdapter
    {
        void Adapt(IDataEntryDisplayPilotFaultList ded, FlightData flightData);
    }

    class DEDFlightDataAdapter : IDEDFlightDataAdapter
    {
        public void Adapt(IDataEntryDisplayPilotFaultList ded, FlightData flightData)
        {
            if (flightData.DEDLines != null)
            {
                ded.InstrumentState.Line1 = Encoding.Default.GetBytes(flightData.DEDLines[0] ?? "");
                ded.InstrumentState.Line2 = Encoding.Default.GetBytes(flightData.DEDLines[1] ?? "");
                ded.InstrumentState.Line3 = Encoding.Default.GetBytes(flightData.DEDLines[2] ?? "");
                ded.InstrumentState.Line4 = Encoding.Default.GetBytes(flightData.DEDLines[3] ?? "");
                ded.InstrumentState.Line5 = Encoding.Default.GetBytes(flightData.DEDLines[4] ?? "");
            }
            if (flightData.Invert != null)
            {
                ded.InstrumentState.Line1Invert = Encoding.Default.GetBytes(flightData.Invert[0] ?? "");
                ded.InstrumentState.Line2Invert = Encoding.Default.GetBytes(flightData.Invert[1] ?? "");
                ded.InstrumentState.Line3Invert = Encoding.Default.GetBytes(flightData.Invert[2] ?? "");
                ded.InstrumentState.Line4Invert = Encoding.Default.GetBytes(flightData.Invert[3] ?? "");
                ded.InstrumentState.Line5Invert = Encoding.Default.GetBytes(flightData.Invert[4] ?? "");
            }
        }
    }
}
