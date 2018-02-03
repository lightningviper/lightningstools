using System;
using Common.Networking;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IEHSIRightKnobReleasedEventHandler:IInputEventHandlerEventHandler {}
	internal class EHSIRightKnobReleasedEventHandler : IEHSIRightKnobReleasedEventHandler
	{
		private readonly IEHSIStateTracker _ehsiStateTracker;
		public EHSIRightKnobReleasedEventHandler(IEHSIStateTracker ehsiStateTracker)
		{
			_ehsiStateTracker = ehsiStateTracker;
		}

		public void Handle(bool forwardEvent)
		{
            if (!_ehsiStateTracker.RightKnobIsPressed) return;
            _ehsiStateTracker.RightKnobDepressedTime = null;
            _ehsiStateTracker.RightKnobReleasedTime = DateTime.UtcNow;
            _ehsiStateTracker.RightKnobLastActivityTime = DateTime.UtcNow;
		    if (!forwardEvent) return;
		    switch (Extractor.State.NetworkMode)
		    {
		        case NetworkMode.Client:
		            ExtractorClient.SendMessageToServer(new Message(MessageType.EHSIRightKnobReleased));
		            break;
		        case NetworkMode.Server:
		            ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSIRightKnobReleased));
		            break;
		    }
		}
	}
}
