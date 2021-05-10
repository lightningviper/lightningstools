using System;
using Common.SimSupport;

namespace LightningGauges.Renderers.VectorDrawing
{
    [Serializable]
    public class InstrumentState : InstrumentStateBase
    {
        public InstrumentState(){}
        public string DrawingCommands { get; set; }
        public string FontDirectory { get; set; }

    }

}