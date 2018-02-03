using Common.Networking;
using F4KeyFile;
using F4Utils.Process;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	public interface IEHSILeftKnobDecreasedByOneEventHandler:IInputEventHandlerEventHandler{}

	public class EHSILeftKnobDecreasedByOneEventHandler : IEHSILeftKnobDecreasedByOneEventHandler
	{
		public void Handle(bool forwardEvent)
		{
		    if (Extractor.State.NetworkMode != NetworkMode.Client)
		    {
		        var useDecrementByOne = false;
		        var decByOneCallback = KeyFileUtils.FindKeyBinding("SimHsiHdgDecBy1");
		        if (decByOneCallback != null && decByOneCallback.Key.ScanCode != (int) ScanCodes.NotAssigned)
		        {
		            useDecrementByOne = true;
		        }
		        KeyFileUtils.SendCallbackToFalcon(useDecrementByOne ? "SimHsiHdgDecBy1" : "SimHsiHeadingDec");
		    }
		    if (!forwardEvent) return;

            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.EHSILeftKnobDecrease));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSILeftKnobDecrease));
                    break;
            }
		}
	}
}
