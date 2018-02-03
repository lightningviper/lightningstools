using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IVerticalVelocityIndicator : IInstrumentRenderer
    {
        VerticalVelocityIndicatorInstrumentState InstrumentState { get; set; }
    }
}