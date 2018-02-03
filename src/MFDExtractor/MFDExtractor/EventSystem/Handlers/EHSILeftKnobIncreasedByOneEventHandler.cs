using Common.Networking;
using F4KeyFile;
using F4Utils.Process;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	public interface IEHSILeftKnobIncreasedByOneEventHandler:IInputEventHandlerEventHandler {}
	public class EHSILeftKnobIncreasedByOneEventHandler : IEHSILeftKnobIncreasedByOneEventHandler
	{

		public void Handle(bool forwardEvent)
		{
		    if (Extractor.State.NetworkMode != NetworkMode.Client)
		    {
		        var useIncrementByOne = false;
		        var incByOneCallback = KeyFileUtils.FindKeyBinding("SimHsiHdgIncBy1");
		        if (incByOneCallback != null && incByOneCallback.Key.ScanCode != (int) ScanCodes.NotAssigned)
		        {
		            useIncrementByOne = true;
		        }
		        KeyFileUtils.SendCallbackToFalcon(useIncrementByOne ? "SimHsiHdgIncBy1" : "SimHsiHeadingInc");
		    }
		    if (!forwardEvent) return;
		    switch (Extractor.State.NetworkMode)
		    {
		        case NetworkMode.Client:
		            ExtractorClient.SendMessageToServer(new Message(MessageType.EHSILeftKnobIncrease));
		            break;
		        case NetworkMode.Server:
		            ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSILeftKnobIncrease));
		            break;
		    }
		}
	}
}
