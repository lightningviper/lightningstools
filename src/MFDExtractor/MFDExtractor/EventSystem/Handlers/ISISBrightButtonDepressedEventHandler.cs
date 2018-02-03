using System.Collections.Generic;
using Common.Networking;
using LightningGauges.Renderers.F16.ISIS;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IISISBrightButtonDepressedEventHandler:IInputEventHandlerEventHandler{}
	internal class ISISBrightButtonDepressedEventHandler : IISISBrightButtonDepressedEventHandler
	{
		private readonly IDictionary<InstrumentType, IInstrument> _instruments;

		public ISISBrightButtonDepressedEventHandler(IDictionary<InstrumentType, IInstrument> instruments)
		{
		    _instruments = instruments;
		}

		public void Handle(bool forwardEvent)
		{
            var isis = _instruments[InstrumentType.ISIS].Renderer as IISIS;
            if (isis != null) isis.InstrumentState.Brightness = isis.InstrumentState.MaxBrightness;

		    if (!forwardEvent) return;
		    switch (Extractor.State.NetworkMode)
		    {
		        case NetworkMode.Client:
		            ExtractorClient.SendMessageToServer(new Message(MessageType.ISISBrightButtonDepressed));
		            break;
		        case NetworkMode.Server:
		            ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.ISISBrightButtonDepressed));
		            break;
		    }
		}
	}
}
