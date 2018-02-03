using System;

namespace F16CPD.Mfd.Controls
{
    public class RotaryEncoderPositionChangedEventArgs : EventArgs
    {
        protected RotationDirection RotationDirection = RotationDirection.Empty;

        public RotaryEncoderPositionChangedEventArgs(RotationDirection direction)
        {
            RotationDirection = direction;
        }
    }

    public class RotaryEncoderMfdInputControl : MfdInputControl
    {
        protected MomentaryButtonMfdInputControl ClockwiseMomentary;
        protected MomentaryButtonMfdInputControl CounterClockwiseMomentary;

        public MomentaryButtonMfdInputControl ClockwiseMomentaryInputControl
        {
            get { return ClockwiseMomentary; }
        }

        public MomentaryButtonMfdInputControl CounterclockwiseMomentaryInputControl
        {
            get { return CounterClockwiseMomentary; }
        }

        public event EventHandler<RotaryEncoderPositionChangedEventArgs> Rotated;

        public MomentaryButtonMfdInputControl GetMomentaryInputControl(RotationDirection direction)
        {
            MomentaryButtonMfdInputControl toReturn = null;
            switch (direction)
            {
                case RotationDirection.Empty:
                    break;
                case RotationDirection.Counterclockwise:
                    toReturn = CounterClockwiseMomentary;
                    break;
                case RotationDirection.Clockwise:
                    toReturn = ClockwiseMomentary;
                    break;
                default:
                    break;
            }
            return toReturn;
        }

        public void RotateClockwise()
        {
            OnRotated(RotationDirection.Clockwise);
        }

        public void RotateCounterclockwise()
        {
            OnRotated(RotationDirection.Counterclockwise);
        }

        protected virtual void OnRotated(RotationDirection direction)
        {
            if (Rotated != null)
            {
                Rotated(this, new RotaryEncoderPositionChangedEventArgs(direction));
            }
        }
    }
}