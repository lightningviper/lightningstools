using System.Collections.Generic;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IInputEvents
	{
		INightVisionModeToggledEventHandler NightVisionModeToggled { get;  }
		IAirspeedIndexDecreasedByOneEventHandler AirspeedIndexDecreasedByOne { get;  }
		IAirspeedIndexIncreasedByOneEventHandler AirspeedIndexIncreasedByOne { get;  }
		IEHSILeftKnobDecreasedByOneEventHandler EHSILeftKnobDecreasedByOne { get;  }
		IEHSILeftKnobIncreasedByOneEventHandler EHSILeftKnobIncreasedByOne { get;  }
		IEHSIRightKnobDecreasedByOneEventHandler EHSIRightKnobDecreasedByOne { get;  }
		IEHSIRightKnobIncreasedByOneEventHandler EHSIRightKnobIncreasedByOne { get;  }
		IEHSIRightKnobDepressedEventHandler EHSIRightKnobDepressed { get;  }
		IEHSIRightKnobReleasedEventHandler EHSIRightKnobReleased { get;  }
		IEHSIMenuButtonDepressedEventHandler EHSIMenuButtonDepressed { get;  }
		IISISBrightButtonDepressedEventHandler ISISBrightButtonDepressed { get;  }
		IISISStandardButtonDepressedEventHandler ISISStandardButtonDepressed { get;  }
		IAzimuthIndicatorBrightnessIncreasedEventHandler AzimuthIndicatorBrightnessIncreased { get;  }
		IAzimuthIndicatorBrightnessDecreasedEventHandler AzimuthIndicatorBrightnessDecreased { get;  }
		IAccelerometerResetEventHandler AccelerometerReset { get;  }

	}
	internal class InputEvents : IInputEvents
	{
			public INightVisionModeToggledEventHandler NightVisionModeToggled { get; private set; }
			public IAirspeedIndexDecreasedByOneEventHandler AirspeedIndexDecreasedByOne { get; private set; }
			public IAirspeedIndexIncreasedByOneEventHandler AirspeedIndexIncreasedByOne { get; private set; }
			public IEHSILeftKnobDecreasedByOneEventHandler EHSILeftKnobDecreasedByOne { get; private set; }
			public IEHSILeftKnobIncreasedByOneEventHandler EHSILeftKnobIncreasedByOne { get; private set; }
			public IEHSIRightKnobDecreasedByOneEventHandler EHSIRightKnobDecreasedByOne { get; private set; }
			public IEHSIRightKnobIncreasedByOneEventHandler EHSIRightKnobIncreasedByOne { get; private set; }
			public IEHSIRightKnobDepressedEventHandler EHSIRightKnobDepressed { get; private set; }
			public IEHSIRightKnobReleasedEventHandler EHSIRightKnobReleased { get; private set; }
			public IEHSIMenuButtonDepressedEventHandler EHSIMenuButtonDepressed { get; private set; }
			public IISISBrightButtonDepressedEventHandler ISISBrightButtonDepressed { get; private set; }
			public IISISStandardButtonDepressedEventHandler ISISStandardButtonDepressed { get; private set; }
			public IAzimuthIndicatorBrightnessIncreasedEventHandler AzimuthIndicatorBrightnessIncreased { get; private set; }
			public IAzimuthIndicatorBrightnessDecreasedEventHandler AzimuthIndicatorBrightnessDecreased { get; private set; }
			public IAccelerometerResetEventHandler AccelerometerReset { get; private set; }

		public InputEvents(
            IDictionary<InstrumentType, IInstrument>  instruments,
            IEHSIStateTracker ehsiStateTracker)
		{
			NightVisionModeToggled =  new NightVisionModeToggledEventHandler();
            AirspeedIndexIncreasedByOne = new AirspeedIndexIncreasedByOneEventHandler(instruments);
            AirspeedIndexDecreasedByOne = new AirspeedIndexDecreasedByOneEventHandler(instruments);
            EHSILeftKnobDecreasedByOne = new EHSILeftKnobDecreasedByOneEventHandler();
            EHSILeftKnobIncreasedByOne = new EHSILeftKnobIncreasedByOneEventHandler();
            EHSIRightKnobDecreasedByOne = new EHSIRightKnobDecreasedByOneEventHandler(ehsiStateTracker);
            EHSIRightKnobIncreasedByOne = new EHSIRightKnobIncreasedByOneEventHandler(ehsiStateTracker);
            EHSIRightKnobDepressed = new EHSIRightKnobDepressedEventHandler(ehsiStateTracker);
            EHSIRightKnobReleased = new EHSIRightKnobReleasedEventHandler(ehsiStateTracker);
            EHSIMenuButtonDepressed = new EHSIMenuButtonDepressedEventHandler();
            ISISBrightButtonDepressed = new ISISBrightButtonDepressedEventHandler(instruments);
            ISISStandardButtonDepressed = new ISISStandardButtonDepressedEventHandler(instruments);
            AzimuthIndicatorBrightnessIncreased = new AzimuthIndicatorBrightnessIncreasedEventHandler(instruments);
            AzimuthIndicatorBrightnessDecreased = new AzimuthIndicatorBrightnessDecreasedEventHandler(instruments);
            AccelerometerReset = new AccelerometerResetEventHandler(instruments);
		}
	}
}
