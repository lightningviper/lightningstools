using System.Collections.Generic;
using Common.Networking;
using LightningGauges.Renderers.F16;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IAccelerometerResetEventHandler:IInputEventHandlerEventHandler {}
	internal class AccelerometerResetEventHandler : IAccelerometerResetEventHandler
	{
        private readonly IDictionary<InstrumentType, IInstrument> _instruments;
        public AccelerometerResetEventHandler(IDictionary<InstrumentType, IInstrument> instruments)
        {
            _instruments = instruments;
        }
		public void Handle(bool forwardEvent)
		{
		    var accelerometer = _instruments[InstrumentType.Accelerometer].Renderer as IAccelerometer;
		    if (accelerometer != null) accelerometer.InstrumentState.ResetMinAndMaxGs();
		    if (!forwardEvent) return;
            
            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.AccelerometerIsReset));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.AccelerometerIsReset));
                    break;
            }

		}
	}
}
