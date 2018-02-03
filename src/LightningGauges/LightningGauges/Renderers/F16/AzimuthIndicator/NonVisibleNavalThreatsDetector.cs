using System.Linq;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class NonVisibleNavalThreatsDetector
    {
        internal static bool AreNonVisibleNavalThreatsDetected(InstrumentState instrumentState)
        {
            if (instrumentState?.Blips == null) return false;

            return (
                from thisBlip 
                in instrumentState.Blips
                where thisBlip != null && thisBlip.Lethality != 0 && !thisBlip.Visible
                select thisBlip.SymbolID).Any(symbolId => symbolId == 18);
        }
    }
}