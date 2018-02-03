using Common.Networking;
using F4Utils.Process;
using LightningGauges.Renderers.F16.EHSI;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IEHSIMenuButtonDepressedEventHandler:IInputEventHandlerEventHandler{}
	internal class EHSIMenuButtonDepressedEventHandler : IEHSIMenuButtonDepressedEventHandler
	{
		public void Handle(bool forwardEvent)
		{

		    if (Extractor.State.NetworkMode != NetworkMode.Client)
		    {
		        KeyFileUtils.SendCallbackToFalcon("SimStepHSIMode");
		    }
		    if (!forwardEvent) return;

            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.EHSIMenuButtonDepressed));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSIMenuButtonDepressed));
                    break;
            }
		}
	}
}
