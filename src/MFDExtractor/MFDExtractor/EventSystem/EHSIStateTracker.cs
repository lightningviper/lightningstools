using System;
using System.Collections.Generic;
using LightningGauges.Renderers.F16.EHSI;

namespace MFDExtractor.EventSystem
{
	internal interface IEHSIStateTracker
	{
		DateTime? RightKnobDepressedTime { get; set; }
		DateTime? RightKnobLastActivityTime { get; set; }
		DateTime? RightKnobReleasedTime { get; set; }
		bool RightKnobIsPressed { get; }
		void UpdateEHSIBrightnessLabelVisibility();
        IEHSI EHSI { get; }
	}

	internal class EHSIStateTracker : IEHSIStateTracker
	{
		public DateTime? RightKnobDepressedTime { get; set; }
		public DateTime? RightKnobLastActivityTime { get; set; }
		public DateTime? RightKnobReleasedTime { get; set; }
		public bool RightKnobIsPressed {  get { return RightKnobDepressedTime.HasValue; } }
		private readonly IDictionary<InstrumentType, IInstrument> _instruments;

		public EHSIStateTracker(IDictionary<InstrumentType, IInstrument> instruments)
		{
		    _instruments = instruments;
		}

		public void UpdateEHSIBrightnessLabelVisibility()
		{
			var showBrightnessLabel = false;
			if (RightKnobIsPressed)
			{
				if (RightKnobDepressedTime.HasValue)
				{
					var howLongPressed = DateTime.UtcNow.Subtract(RightKnobDepressedTime.Value);
					if (howLongPressed.TotalMilliseconds > 2000)
					{
						showBrightnessLabel = true;
					}
				}
			}
			else
			{
				if (RightKnobReleasedTime.HasValue && RightKnobLastActivityTime.HasValue)
				{
					var howLongAgoReleased = DateTime.UtcNow.Subtract(RightKnobReleasedTime.Value);
					var howLongAgoLastActivity = DateTime.UtcNow.Subtract(RightKnobLastActivityTime.Value);
					if (howLongAgoReleased.TotalMilliseconds < 2000 || howLongAgoLastActivity.TotalMilliseconds < 2000)
					{
						showBrightnessLabel = EHSI.InstrumentState.ShowBrightnessLabel;
					}
				}
			}
			EHSI.InstrumentState.ShowBrightnessLabel = showBrightnessLabel;
		}
        public IEHSI EHSI { get { return (_instruments[InstrumentType.EHSI].Renderer as IEHSI); } }
	}
}
