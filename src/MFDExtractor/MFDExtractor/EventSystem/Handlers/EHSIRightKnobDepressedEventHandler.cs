using System;
using Common.Networking;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	public interface IEHSIRightKnobDepressedEventHandler:IInputEventHandlerEventHandler {}
	class EHSIRightKnobDepressedEventHandler : IEHSIRightKnobDepressedEventHandler
	{
		private readonly IEHSIStateTracker _ehsiStateTracker;
		public EHSIRightKnobDepressedEventHandler(IEHSIStateTracker ehsiStateTracker)
		{
			_ehsiStateTracker = ehsiStateTracker;
		}

		public void Handle(bool forwardEvent)
		{
            _ehsiStateTracker.RightKnobDepressedTime = DateTime.UtcNow;
            _ehsiStateTracker.RightKnobReleasedTime = null;
            _ehsiStateTracker.RightKnobLastActivityTime = DateTime.UtcNow;
		    if (!forwardEvent) return;
		    switch (Extractor.State.NetworkMode)
		    {
		        case NetworkMode.Client:
		            ExtractorClient.SendMessageToServer(new Message(MessageType.EHSIRightKnobDepressed));
		            break;
		        case NetworkMode.Server:
		            ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSIRightKnobDepressed));
		            break;
		    }
		}
	}
}
