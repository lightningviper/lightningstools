using Common.Math;
using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IStandbyADIFlightDataAdapter
    {
        void Adapt(IStandbyADI standbyADI, FlightData flightData);
    }

    class StandbyADIFlightDataAdapter : IStandbyADIFlightDataAdapter
    {
        public void Adapt(IStandbyADI standbyADI, FlightData flightData)
        {
            var hsibits = (HsiBits)flightData.hsiBits;
           standbyADI.InstrumentState.OffFlag = ((hsibits & HsiBits.BUP_ADI_OFF) == HsiBits.BUP_ADI_OFF);
            if (((hsibits & HsiBits.BUP_ADI_OFF) == HsiBits.BUP_ADI_OFF))
            {
                //if the standby ADI is off
                standbyADI.InstrumentState.PitchDegrees = 0;
                standbyADI.InstrumentState.RollDegrees = 0;
                standbyADI.InstrumentState.OffFlag = true;
            }
            else
            {
                standbyADI.InstrumentState.PitchDegrees = ((flightData.pitch / Common.Math.Constants.RADIANS_PER_DEGREE));
                standbyADI.InstrumentState.RollDegrees = ((flightData.roll / Common.Math.Constants.RADIANS_PER_DEGREE));
                standbyADI.InstrumentState.OffFlag = false;
            }
        }
    }
}
