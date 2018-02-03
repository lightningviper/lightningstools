using Common.InputSupport;
using MFDExtractor.Configuration;
using MFDExtractor.EventSystem.Handlers;

namespace MFDExtractor.EventSystem
{
	internal interface IMediatorStateChangeHandler
	{
		void HandleStateChange(object sender, PhysicalControlStateChangedEventArgs e);
	}

	class MediatorStateChangeHandler : IMediatorStateChangeHandler
	{
		private readonly IDirectInputEventHotkeyFilter _directInputEventHotkeyFilter;
		private readonly IDIHotkeyDetection _diHotkeyDetection;
		private readonly IInputEvents _inputEvents;
	    private readonly IKeySettingsReader _keySettingsReader;
		public MediatorStateChangeHandler(
			IDIHotkeyDetection diHotkeyDetection,
			IInputEvents inputEvents,
            IDirectInputEventHotkeyFilter directInputEventHotkeyFilter=null,
            IKeySettingsReader keySettingsReader = null)
		{
            
			_directInputEventHotkeyFilter = directInputEventHotkeyFilter ?? new DirectInputEventHotkeyFilter();
		    _diHotkeyDetection = diHotkeyDetection;
            _inputEvents = inputEvents;
            _keySettingsReader = keySettingsReader ?? new KeySettingsReader();
        }
		public void HandleStateChange(object sender, PhysicalControlStateChangedEventArgs e)
		{
		    var keySettings = _keySettingsReader.Read();
			if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.NVISKey))
			{
				_inputEvents.NightVisionModeToggled.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.AirspeedIndexIncreaseKey))
			{
                _inputEvents.AirspeedIndexIncreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.AirspeedIndexDecreaseKey))
			{
                _inputEvents.AirspeedIndexDecreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSIHeadingDecreaseKey))
			{
                _inputEvents.EHSILeftKnobDecreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSIHeadingIncreaseKey))
			{
                _inputEvents.EHSILeftKnobIncreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSICourseDecreaseKey))
			{
                _inputEvents.EHSIRightKnobDecreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSICourseIncreaseKey))
			{
                _inputEvents.EHSIRightKnobIncreasedByOne.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSICourseDepressedKey))
			{
			    if (e.Control.ControlType == ControlType.Button && e.CurrentState == 0 && e.PreviousState == 1)
			    {
			        _inputEvents.EHSIRightKnobReleased.Handle(true);
			    }
			    else
			    {
			        _inputEvents.EHSIRightKnobDepressed.Handle(true);
			    }
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.EHSIMenuButtonDepressedKey))
			{
                _inputEvents.EHSIMenuButtonDepressed.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.ISISBrightButtonKey))
			{
                _inputEvents.ISISBrightButtonDepressed.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.ISISStandardButtonKey))
			{
                _inputEvents.ISISStandardButtonDepressed.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.AzimuthIndicatorBrightnessIncreaseKey))
			{
                _inputEvents.AzimuthIndicatorBrightnessIncreased.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.AzimuthIndicatorBrightnessDecreaseKey))
			{
                _inputEvents.AzimuthIndicatorBrightnessDecreased.Handle(true);
			}
			else if (_directInputEventHotkeyFilter.CheckIfMatches(e, keySettings.AccelerometerResetKey))
			{
                _inputEvents.AccelerometerReset.Handle(true);
			}
		}
	}
}
