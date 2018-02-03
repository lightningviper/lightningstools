using Common.InputSupport.UI;

namespace MFDExtractor.Configuration
{
	public class KeySettings
	{
		public InputControlSelection NVISKey { get; internal set; }
		public InputControlSelection AirspeedIndexIncreaseKey { get; internal set; }
		public InputControlSelection AirspeedIndexDecreaseKey { get; internal set; }
		public InputControlSelection EHSIHeadingIncreaseKey { get; internal set; }
		public InputControlSelection EHSIHeadingDecreaseKey { get; internal set; }
		public InputControlSelection EHSICourseDecreaseKey { get; internal set; }
		public InputControlSelection EHSICourseDepressedKey { get; internal set; }
		public InputControlSelection EHSIMenuButtonDepressedKey { get; internal set; }
		public InputControlSelection EHSICourseIncreaseKey { get; internal set; }
		public InputControlSelection ISISBrightButtonKey { get; internal set; }
		public InputControlSelection ISISStandardButtonKey { get; internal set; }
		public InputControlSelection AzimuthIndicatorBrightnessIncreaseKey { get; internal set; }
		public InputControlSelection AccelerometerResetKey { get; internal set; }
		public InputControlSelection AzimuthIndicatorBrightnessDecreaseKey { get; internal set; }

	}
}
