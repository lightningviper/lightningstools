using System;
using Common.Networking;
using F4KeyFile;
using F4Utils.Process;
using MFDExtractor.Networking;
using MFDExtractor.Properties;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IEHSIRightKnobIncreasedByOneEventHandler:IInputEventHandlerEventHandler{}
	internal class EHSIRightKnobIncreasedByOneEventHandler : IEHSIRightKnobIncreasedByOneEventHandler
	{
		private readonly IEHSIStateTracker _ehsiStateTracker;
		public EHSIRightKnobIncreasedByOneEventHandler(IEHSIStateTracker ehsiStateTracker)
		{
			_ehsiStateTracker = ehsiStateTracker;
		}


		public void Handle(bool forwardEvent)
		{
		    var ehsi = _ehsiStateTracker.EHSI;
		    _ehsiStateTracker.RightKnobLastActivityTime = DateTime.UtcNow;
		    if (ehsi.InstrumentState.ShowBrightnessLabel)
		    {
		        var newBrightness =
		            (int) Math.Floor((ehsi.InstrumentState.Brightness + (ehsi.InstrumentState.MaxBrightness*(1.0f/32.0f))));
		        ehsi.InstrumentState.Brightness = newBrightness;
		        Settings.Default.EHSIBrightness = newBrightness;
		    }
		    else
		    {
		        if (Extractor.State.NetworkMode != NetworkMode.Client)
		        {
		            var useIncrementByOne = false;
		            var incByOneCallback = KeyFileUtils.FindKeyBinding("SimHsiCrsIncBy1");
		            if (incByOneCallback != null && incByOneCallback.Key.ScanCode != (int) ScanCodes.NotAssigned)
		            {
		                useIncrementByOne = true;
		            }
		            KeyFileUtils.SendCallbackToFalcon(useIncrementByOne ? "SimHsiCrsIncBy1" : "SimHsiCourseInc");
		        }
		    }
		    if (!forwardEvent) return;
            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.EHSIRightKnobIncrease));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.EHSIRightKnobIncrease));
                    break;
            }

		}
	}
}
