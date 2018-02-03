using Common.InputSupport;
using Common.InputSupport.DirectInput;
using Common.InputSupport.UI;

namespace MFDExtractor
{
    public interface IDIHotkeyDetection
    {
        bool DirectInputHotkeyIsTriggering(InputControlSelection hotkey);
    }

    public sealed class DIHotkeyDetection : IDIHotkeyDetection
    {
        private readonly Mediator _mediator;

        public DIHotkeyDetection(Mediator mediator)
        {
            _mediator = mediator;
        }

        public bool DirectInputHotkeyIsTriggering(InputControlSelection hotkey)
        {
            if (hotkey == null || hotkey.DirectInputControl == null) return false;
            int? currentVal = _mediator.GetPhysicalControlValue(hotkey.DirectInputControl, StateType.Current);
            int? prevVal = _mediator.GetPhysicalControlValue(hotkey.DirectInputControl, StateType.Previous);

            switch (hotkey.ControlType)
            {
                case ControlType.Unknown:
                    break;
                case ControlType.Axis:
                    if (currentVal.HasValue && !prevVal.HasValue)
                    {
                        return true;
                    }
                    if (!currentVal.HasValue && prevVal.HasValue)
                    {
                        return true;
                    }
                    return (currentVal.Value != prevVal.Value);
                case ControlType.Button:
                    return (currentVal.HasValue && currentVal.Value == 1);
                case ControlType.Pov:
                    if (currentVal.HasValue)
                    {
                        return Common.InputSupport.Util.GetPovDirection(currentVal.Value) == hotkey.PovDirection;
                    }
                    return false;
                case ControlType.Key:
                    return (currentVal.HasValue && currentVal.Value == 1);
            }
            return false;
        }
    }
}