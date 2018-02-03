using System;

namespace F16CPD.Mfd.Controls
{
    public class ToggleSwitchPositionMfdInputControl : MfdInputControl
    {
        public ToggleSwitchPositionMfdInputControl()
        {
        }

        public ToggleSwitchPositionMfdInputControl(string positionName, ToggleSwitchMfdInputControl parent)
        {
            PositionName = positionName;
            Parent = parent;
        }

        public String PositionName { get; set; }
        public ToggleSwitchMfdInputControl Parent { get; set; }

        public void Activate()
        {
            var parent = Parent;
            if (parent != null)
            {
                parent.CurrentPosition = this;
            }
        }
    }
}