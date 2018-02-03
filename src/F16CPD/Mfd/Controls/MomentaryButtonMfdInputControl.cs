using System;

namespace F16CPD.Mfd.Controls
{
    public class MomentaryButtonPressedEventArgs : EventArgs
    {
        private readonly DateTime _whenPressed;

        public MomentaryButtonPressedEventArgs()
            : this(DateTime.UtcNow)
        {
        }

        public MomentaryButtonPressedEventArgs(DateTime whenPressed)
        {
            _whenPressed = whenPressed;
        }

        public DateTime WhenPressed
        {
            get { return _whenPressed; }
        }
    }

    public class MomentaryButtonMfdInputControl : MfdInputControl
    {
        public event EventHandler<MomentaryButtonPressedEventArgs> Pressed;

        public void Press(DateTime whenPressed)
        {
            OnPressed(whenPressed);
        }

        protected virtual void OnPressed(DateTime whenPressed)
        {
            if (Pressed != null)
            {
                Pressed(this, new MomentaryButtonPressedEventArgs(whenPressed));
            }
        }
    }
}