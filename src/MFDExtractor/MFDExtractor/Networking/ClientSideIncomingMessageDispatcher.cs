using MFDExtractor.EventSystem.Handlers;

namespace MFDExtractor.Networking
{
	internal interface IClientSideIncomingMessageDispatcher
	{
		void ProcessPendingMessages();
	}

	class ClientSideIncomingMessageDispatcher : IClientSideIncomingMessageDispatcher
	{
		private readonly IInputEvents _inputEvents;

		public ClientSideIncomingMessageDispatcher(
			IInputEvents inputEvents)
		{
			_inputEvents = inputEvents;
		}

		public void ProcessPendingMessages()
		{
			var pendingMessage = ExtractorClient.GetNextMessageToClientFromServer();
			while (pendingMessage != null)
			{
				var messageType = pendingMessage.MessageType;
				switch (messageType)
				{
					case MessageType.ToggleNightMode:
						_inputEvents.NightVisionModeToggled.Handle(false);
						break;
					case MessageType.AirspeedIndexIncrease:
                        _inputEvents.AirspeedIndexIncreasedByOne.Handle(false);
						break;
					case MessageType.AirspeedIndexDecrease:
                        _inputEvents.AirspeedIndexDecreasedByOne.Handle(false);
						break;
					case MessageType.EHSILeftKnobIncrease:
                        _inputEvents.EHSILeftKnobIncreasedByOne.Handle(false);
						break;
					case MessageType.EHSILeftKnobDecrease:
                        _inputEvents.EHSILeftKnobDecreasedByOne.Handle(false);
						break;
					case MessageType.EHSIRightKnobIncrease:
                        _inputEvents.EHSIRightKnobIncreasedByOne.Handle(false);
						break;
					case MessageType.EHSIRightKnobDecrease:
                        _inputEvents.EHSIRightKnobDecreasedByOne.Handle(false);
						break;
					case MessageType.EHSIRightKnobDepressed:
                        _inputEvents.EHSIRightKnobDepressed.Handle(false);
						break;
					case MessageType.EHSIRightKnobReleased:
                        _inputEvents.EHSIRightKnobReleased.Handle(false);
						break;
					case MessageType.EHSIMenuButtonDepressed:
                        _inputEvents.EHSIMenuButtonDepressed.Handle(false);
						break;
					case MessageType.AccelerometerIsReset:
                        _inputEvents.AccelerometerReset.Handle(false);
						break;
                    case MessageType.ISISBrightButtonDepressed:
                        _inputEvents.ISISBrightButtonDepressed.Handle(false);
                        break;
                    case MessageType.ISISStandardButtonDepressed:
                        _inputEvents.ISISStandardButtonDepressed.Handle(false);
                        break;
                    case MessageType.AzimuthIndicatorBrightnessDecrease:
                        _inputEvents.AzimuthIndicatorBrightnessDecreased.Handle(false);
                        break;
                    case MessageType.AzimuthIndicatorBrightnessIncrease:
                        _inputEvents.AzimuthIndicatorBrightnessIncreased.Handle(false);
                        break;
                }
				pendingMessage = ExtractorClient.GetNextMessageToClientFromServer();
			}
		}
	}
}
