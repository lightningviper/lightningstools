using F4SharedMem;
using F4SharedMem.Headers;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IAngleOfAttackIndicatorFlightDataAdapter
    {
        void Adapt(IAngleOfAttackIndicator angleOfAttackIndicator, FlightData flightData);
    }

    class AngleOfAttackIndicatorFlightDataAdapter : IAngleOfAttackIndicatorFlightDataAdapter
    {
        public void Adapt(IAngleOfAttackIndicator angleOfAttackIndicator, FlightData flightData)
        {
            var hsibits = (HsiBits)flightData.hsiBits;
            if (((hsibits & HsiBits.AOA) == HsiBits.AOA))
            {
                angleOfAttackIndicator.InstrumentState.OffFlag = true;
                angleOfAttackIndicator.InstrumentState.AngleOfAttackDegrees = 0;
            }
            else
            {
                angleOfAttackIndicator.InstrumentState.OffFlag = false;
                angleOfAttackIndicator.InstrumentState.AngleOfAttackDegrees = flightData.alpha;
            }

        }
    }
}
