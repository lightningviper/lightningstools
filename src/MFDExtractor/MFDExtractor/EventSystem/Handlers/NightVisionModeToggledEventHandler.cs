using Common.Networking;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	public interface INightVisionModeToggledEventHandler:IInputEventHandlerEventHandler {}
	public class NightVisionModeToggledEventHandler : INightVisionModeToggledEventHandler
	{
        public void Handle(bool forwardEvent)
		{
            Extractor.State.NightMode = !Extractor.State.NightMode;
            if (!forwardEvent) return;
            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.ToggleNightMode));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.ToggleNightMode));
                    break;
            }
		}
	}
}
