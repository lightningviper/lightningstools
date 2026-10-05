using System;

namespace SimLinkup.HardwareSupport.Henk.HSI.HeadingBug
{
    public interface IHsiHeadingBugProcessor
    {
        double Process(double magneticHeadingDegrees, double datumPlusMinusZeroToNinetyDegrees);
        void Reset();
    }

    public class HsiHeadingBugProcessor : IHsiHeadingBugProcessor
    {
        private double _previousHeading = -1.0;
        private double _previousDatum = 0.0;
        private double _smoothedHeadingBug = -1.0;
        private int _currentQuadrant = 0; // 0=Front-Right, 1=Aft-Right, 2=Aft-Left, 3=Front-Left
        private bool _isFirstRun = true;

        // --- TUNING PARAMETERS ---
        private const double FilterSmoothingFactor = 0.20;
        private const double NoiseThresholdDegrees = 0.25;

        /// <summary>
        /// Inputs raw, jittery HSI data and outputs a debounced, stable 1-360° absolute heading bug.
        /// </summary>
        public double Process(double magneticHeadingDegrees, double datumPlusMinusZeroToNinetyDegrees)
        {
            // 1. Manual Clamp for compatibility
            if (datumPlusMinusZeroToNinetyDegrees < -90.0) datumPlusMinusZeroToNinetyDegrees = -90.0;
            if (datumPlusMinusZeroToNinetyDegrees > 90.0) datumPlusMinusZeroToNinetyDegrees = 90.0;

            if (_isFirstRun)
            {
                _currentQuadrant = datumPlusMinusZeroToNinetyDegrees >= 0 ? 0 : 3;
                _previousHeading = magneticHeadingDegrees;
                _previousDatum = datumPlusMinusZeroToNinetyDegrees;

                double initialBug = (magneticHeadingDegrees + datumPlusMinusZeroToNinetyDegrees + 360) % 360;
                _smoothedHeadingBug = initialBug <= 0.1 ? 360.0 : initialBug;

                _isFirstRun = false;
                return Math.Round(_smoothedHeadingBug, 1);
            }

            // 2. Identify if the change is a pilot knob turn or aircraft yaw
            double deltaHeading = magneticHeadingDegrees - _previousHeading;
            if (deltaHeading > 180) deltaHeading -= 360;
            if (deltaHeading < -180) deltaHeading += 360;

            double actualDeltaDatum = datumPlusMinusZeroToNinetyDegrees - _previousDatum;
            double expectedDeltaDatum = -deltaHeading;

            bool isKnobTurning = Math.Abs(actualDeltaDatum - expectedDeltaDatum) > NoiseThresholdDegrees;

            // 3. Track Quadrant State Machine
            if (isKnobTurning)
            {
                double deltaDatum = datumPlusMinusZeroToNinetyDegrees - _previousDatum;

                if (_currentQuadrant == 0 && datumPlusMinusZeroToNinetyDegrees > 84.0 && deltaDatum < -0.3) _currentQuadrant = 1;
                else if (_currentQuadrant == 1 && datumPlusMinusZeroToNinetyDegrees > 84.0 && deltaDatum > 0.3) _currentQuadrant = 0;
                else if (_currentQuadrant == 3 && datumPlusMinusZeroToNinetyDegrees < -84.0 && deltaDatum > 0.3) _currentQuadrant = 2;
                else if (_currentQuadrant == 2 && datumPlusMinusZeroToNinetyDegrees < -84.0 && deltaDatum < -0.3) _currentQuadrant = 3;

                if (_currentQuadrant == 0 && datumPlusMinusZeroToNinetyDegrees < -0.1) _currentQuadrant = 3;
                if (_currentQuadrant == 3 && datumPlusMinusZeroToNinetyDegrees > 0.1) _currentQuadrant = 0;
                if (_currentQuadrant == 1 && datumPlusMinusZeroToNinetyDegrees < -0.1) _currentQuadrant = 2;
                if (_currentQuadrant == 2 && datumPlusMinusZeroToNinetyDegrees > 0.1) _currentQuadrant = 1;
            }

            _previousHeading = magneticHeadingDegrees;
            _previousDatum = datumPlusMinusZeroToNinetyDegrees;

            // 4. Reconstruct raw relative offset 
            double relativeOffset;
            switch (_currentQuadrant)
            {
                case 0: // Front-Right
                    relativeOffset = datumPlusMinusZeroToNinetyDegrees;
                    break;
                case 1: // Aft-Right
                    relativeOffset = 180.0 - datumPlusMinusZeroToNinetyDegrees;
                    break;
                case 2: // Aft-Left
                    relativeOffset = -180.0 - datumPlusMinusZeroToNinetyDegrees;
                    break;
                case 3: // Front-Left
                    relativeOffset = datumPlusMinusZeroToNinetyDegrees;
                    break;
                default:
                    relativeOffset = datumPlusMinusZeroToNinetyDegrees;
                    break;
            }

            // 5. Build raw absolute heading bug target
            var rawAbsoluteBug = (magneticHeadingDegrees + relativeOffset + 360) % 360;
            if (rawAbsoluteBug <= 0.1) rawAbsoluteBug = 360.0;

            // 6. Apply Low-Pass Filter Debounce
            var diff = rawAbsoluteBug - _smoothedHeadingBug;
            if (diff > 180) diff -= 360;
            if (diff < -180) diff += 360;

            _smoothedHeadingBug += diff * FilterSmoothingFactor;
            _smoothedHeadingBug = (_smoothedHeadingBug + 360) % 360;
            if (_smoothedHeadingBug <= 0.1) _smoothedHeadingBug = 360.0;

            return Math.Round(_smoothedHeadingBug, 1);
        }

        public void Reset()
        {
            _isFirstRun = true;
        }
    }
}