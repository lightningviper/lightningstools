using Common.UI;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    public class Options
    {
        public Options() { Style = AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay; }

        public AzimuthIndicator.InstrumentStyle Style { get; set; }
        public bool HideBezel { get; set; }
    }
}