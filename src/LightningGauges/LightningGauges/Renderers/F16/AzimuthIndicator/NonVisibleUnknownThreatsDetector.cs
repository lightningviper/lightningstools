using System.Linq;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class NonVisibleUnknownThreatsDetector
    {
        internal static bool AreNonVisibleUnknownThreatsDetected(InstrumentState instrumentState)
        {
            return instrumentState?.Blips != null && (
                from thisBlip 
                in instrumentState.Blips
                where thisBlip != null && !thisBlip.Visible && thisBlip.Lethality != 0
                select thisBlip.SymbolID)
                .Any(symbolId => symbolId < 0 || symbolId == 1 || symbolId == 27 || symbolId == 28 || symbolId == 29);
        }
    }
}