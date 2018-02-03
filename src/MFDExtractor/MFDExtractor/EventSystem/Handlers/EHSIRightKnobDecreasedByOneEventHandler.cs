using System;
using Common.Networking;
using F4KeyFile;
using F4Utils.Process;
using MFDExtractor.Networking;
using MFDExtractor.Properties;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IEHSIRightKnobDecreasedByOneEventHandler:IInputEventHandlerEventHandler{}
	internal class EHSIRightKnobDecreasedByOneEventHandler : IEHSIRightKnobDecreasedByOneEventHandler
	{
	    private readonly IEHSIStateTracker _ehsiStateTracker;
		public EHSIRightKnobDecreasedByOneEventHandler(IEHSIStateTracker ehsiStateTracker)
		{
		    _ehsiStateTracker = ehsiStateTracker;
		}

		public void Handle(bool forwardEvent)
		{
		    _ehsiStateTracker.RightKnobLastActivityTime = DateTime.UtcNow;
		    var ehsi = _ehsiStateTracker.EHSI;
		    if (ehsi.InstrumentState.ShowBrightnessLabel)
		    {
		        var newBrightness =
		            (int) Math.Floor((ehsi.InstrumentState.Brightness - (ehsi.InstrumentState.MaxBrightness*(1.0f/32.0f))));
		        ehsi.InstrumentState.Brightness = newBrightness;
		        Settings.Default.EHSIBrightness = newBrightness;
		    }
		    else
		    {
		        if (Extractor.State.NetworkMode != NetworkMode.Client)
		        {
		            var useDecrementByOne = false;
		            var decByOneCallback = KeyFileUtils.FindKeyBinding("SimHsiCrsDecBy1");
		            if (decByOneCallback != null && decByOneCallback.Key.ScanCode != (int) ScanCodes.NotAssigned)
		            {
		                useDecrementByOne = true;
		            }
		            KeyFileUtils.SendCallbackToFalcon(useDecrementByOne ? "SimHsiCrsDecBy1" : "SimHsiCourseDec");
		        }
		    }
		    if (!forwardEvent) return;
            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.EHSIRightKnobDecrease));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSIRightKnobDecrease));
                    break;
            }

		}
	}

}
