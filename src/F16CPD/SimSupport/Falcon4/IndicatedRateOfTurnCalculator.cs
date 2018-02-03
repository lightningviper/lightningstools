using Common.Statistics;
using F16CPD.FlightInstruments;
using System;
using System.Collections.Generic;
using F16CPD.FlightInstruments.Pfd;

namespace F16CPD.SimSupport.Falcon4
{
    internal interface IIndicatedRateOfTurnCalculator
    {
        void DetermineIndicatedRateOfTurn(FlightData flightData);
        void Reset();
    }
    class IndicatedRateOfTurnCalculator:IIndicatedRateOfTurnCalculator
    {
        private TimestampedFloatValue _lastHeadingSample;
        private List<TimestampedFloatValue> _lastInstantaneousRatesOfTurn = new List<TimestampedFloatValue>();
        private readonly IMedianOfSamplesCalculator _medianOfSamplesCalculator = new MedianOfSamplesCalculator();
        
        public void DetermineIndicatedRateOfTurn(FlightData flightData)
        {
            //capture the current time
            var curTime = DateTime.UtcNow;

            //determine how many seconds it's been since our last "current heading" datum snapshot?
            var dT = (float)((curTime.Subtract(_lastHeadingSample.Timestamp)).TotalMilliseconds);

            //determine the change in heading since our last snapshot
            var currentHeading = flightData.MagneticHeadingInDecimalDegrees;
            var headingDelta = Common.Math.Util.AngleDelta(_lastHeadingSample.Value, currentHeading);

            //now calculate the instantaneous rate of turn
            var currentInstantaneousRateOfTurn = (headingDelta / dT) * 1000;
            if (Math.Abs(currentInstantaneousRateOfTurn) > 30 || float.IsInfinity(currentInstantaneousRateOfTurn) ||
                float.IsNaN(currentInstantaneousRateOfTurn)) currentInstantaneousRateOfTurn = 0; //noise
            if (Math.Abs(currentInstantaneousRateOfTurn) >
                (Pfd.MAX_INDICATED_RATE_OF_TURN_DECIMAL_DEGREES_PER_SECOND + 0.5f))
            {
                currentInstantaneousRateOfTurn = (Pfd.MAX_INDICATED_RATE_OF_TURN_DECIMAL_DEGREES_PER_SECOND + 0.5f) *
                                                 Math.Sign(currentInstantaneousRateOfTurn);
            }

            var sample = new TimestampedFloatValue { Timestamp = curTime, Value = currentInstantaneousRateOfTurn };

            //cull historic rate-of-turn samples older than n seconds
            var replacementList = new List<TimestampedFloatValue>();
            for (var i = 0; i < _lastInstantaneousRatesOfTurn.Count; i++)
            {
                if (!(Math.Abs(curTime.Subtract(_lastInstantaneousRatesOfTurn[i].Timestamp).TotalMilliseconds) > 1000))
                {
                    replacementList.Add(_lastInstantaneousRatesOfTurn[i]);
                }
            }
            _lastInstantaneousRatesOfTurn = replacementList;

            _lastInstantaneousRatesOfTurn.Add(sample);

            var medianRateOfTurn = (float)Math.Round(_medianOfSamplesCalculator.MedianSampleValue(_lastInstantaneousRatesOfTurn), 1);
            const float minIncrement = 0.1f;
            while (medianRateOfTurn < flightData.RateOfTurnInDecimalDegreesPerSecond - minIncrement)
            {
                flightData.RateOfTurnInDecimalDegreesPerSecond -= minIncrement;
            }
            while (medianRateOfTurn > flightData.RateOfTurnInDecimalDegreesPerSecond + minIncrement)
            {
                flightData.RateOfTurnInDecimalDegreesPerSecond += minIncrement;
            }

            if (Math.Round(medianRateOfTurn, 1) == 0)
            {
                flightData.RateOfTurnInDecimalDegreesPerSecond = 0;
            }
            else if (medianRateOfTurn == flightData.RateOfTurnInDecimalDegreesPerSecond - minIncrement)
            {
                flightData.RateOfTurnInDecimalDegreesPerSecond = medianRateOfTurn;
            }
            else if (medianRateOfTurn == flightData.RateOfTurnInDecimalDegreesPerSecond + minIncrement)
            {
                flightData.RateOfTurnInDecimalDegreesPerSecond = medianRateOfTurn;
            }

            _lastHeadingSample = new TimestampedFloatValue
            {
                Timestamp = curTime,
                Value = flightData.MagneticHeadingInDecimalDegrees
            };
        }
        public void Reset()
        {
            _lastInstantaneousRatesOfTurn = new List<TimestampedFloatValue>();
        }
    }
}
