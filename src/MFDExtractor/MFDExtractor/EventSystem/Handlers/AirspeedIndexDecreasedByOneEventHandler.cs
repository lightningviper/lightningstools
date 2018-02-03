using System.Collections.Generic;
using Common.Networking;
using LightningGauges.Renderers.F16;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IAirspeedIndexDecreasedByOneEventHandler:IInputEventHandlerEventHandler {}
	internal class AirspeedIndexDecreasedByOneEventHandler : IAirspeedIndexDecreasedByOneEventHandler
	{
		private readonly IDictionary<InstrumentType, IInstrument> _instruments;
		public AirspeedIndexDecreasedByOneEventHandler(IDictionary<InstrumentType, IInstrument> instruments)
		{
		    _instruments = instruments;
		}

		public void Handle(bool forwardEvent)
		{
			var airspeedIndicator = (_instruments[InstrumentType.ASI].Renderer as IAirspeedIndicator);
            if (airspeedIndicator !=null) airspeedIndicator.InstrumentState.AirspeedIndexKnots -= 2.5F;
		    if (!forwardEvent) return;

            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.AirspeedIndexDecrease));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.AirspeedIndexDecrease));
                    break;
            }

		}
	}
}
