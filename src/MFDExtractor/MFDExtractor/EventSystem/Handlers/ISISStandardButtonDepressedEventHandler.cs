using System;
using System.Collections.Generic;
using Common.Networking;
using LightningGauges.Renderers.F16.ISIS;
using MFDExtractor.Networking;

namespace MFDExtractor.EventSystem.Handlers
{
	internal interface IISISStandardButtonDepressedEventHandler:IInputEventHandlerEventHandler{}
	internal class ISISStandardButtonDepressedEventHandler : IISISStandardButtonDepressedEventHandler
	{
		private readonly IDictionary<InstrumentType, IInstrument> _instruments;
		public ISISStandardButtonDepressedEventHandler(IDictionary<InstrumentType, IInstrument> instruments)
		{
			_instruments = instruments;
		}

		public void Handle(bool forwardEvent)
		{
		    var isis = _instruments[InstrumentType.ISIS].Renderer as IISIS;
		    if (isis != null)
		        isis.InstrumentState.Brightness = (int) Math.Floor((isis.InstrumentState.MaxBrightness)*0.5f);
		    if (!forwardEvent) return;
            switch (Extractor.State.NetworkMode)
            {
                case NetworkMode.Client:
                    ExtractorClient.SendMessageToServer(new Message(MessageType.ISISStandardButtonDepressed));
                    break;
                case NetworkMode.Server:
                    ExtractorServer.SubmitMessageToClientFromServer(new Message(MessageType.ISISStandardButtonDepressed));
                    break;
            }
		}
	}
}
