using Common.InputSupport;
using Common.InputSupport.UI;

namespace MFDExtractor.EventSystem
{
	internal interface IDirectInputEventHotkeyFilter
	{
		bool CheckIfMatches(PhysicalControlStateChangedEventArgs directInputEvent, InputControlSelection inputControlSelection);
	}

	class DirectInputEventHotkeyFilter : IDirectInputEventHotkeyFilter
	{
		public bool CheckIfMatches(PhysicalControlStateChangedEventArgs directInputEvent, InputControlSelection inputControlSelection)
		{
			if (directInputEvent == null) return false;
			if (directInputEvent.Control == null) return false;
			if (inputControlSelection == null) return false;

			if (
				inputControlSelection.ControlType != ControlType.Axis
				&&
				inputControlSelection.ControlType != ControlType.Button
				&&
				inputControlSelection.ControlType != ControlType.Pov
				)
			{
				return false;
			}
			if (inputControlSelection.DirectInputControl == null) return false;
			if (inputControlSelection.DirectInputDevice == null) return false;

			if (
				directInputEvent.Control.ControlType == inputControlSelection.DirectInputControl.ControlType
				&&
				directInputEvent.Control.ControlNum == inputControlSelection.DirectInputControl.ControlNum
				&&
				(
					(directInputEvent.Control.ControlType == ControlType.Axis &&
					 directInputEvent.Control.AxisType == inputControlSelection.DirectInputControl.AxisType)
					||
					(directInputEvent.Control.ControlType != ControlType.Axis)
				)
				&&
				Equals(directInputEvent.Control.Parent.Key, inputControlSelection.DirectInputDevice.Key)
				&&
				(
					directInputEvent.Control.ControlType != ControlType.Pov
					||
					(
						inputControlSelection.ControlType == ControlType.Pov
						&&
						inputControlSelection.PovDirection == Util.GetPovDirection(directInputEvent.CurrentState)
					)
				)
				&&
				(
					directInputEvent.Control.ControlType != ControlType.Button
					||
					(
						directInputEvent.Control.ControlType == ControlType.Button
						&&
						directInputEvent.CurrentState == 1
					)
				)
				)
			{
				return true;
			}
			return false;
		}
		
	}
}
