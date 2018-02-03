using F4SharedMem;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;

namespace MFDExtractor.FlightDataAdapters
{
    internal interface IAccelerometerFlightDataAdapter
    {
        void Adapt(IAccelerometer accelerometer, FlightData flightData);
    }

    class AccelerometerFlightDataAdapter : IAccelerometerFlightDataAdapter
    {
        public void Adapt(IAccelerometer accelerometer, FlightData flightData)
        {
            var gs = flightData.gs;
            if (gs == 0) //ignore exactly zero g's
            {
                gs = 1;
            }
            accelerometer.InstrumentState.AccelerationInGs = gs;
        }
    }
}
