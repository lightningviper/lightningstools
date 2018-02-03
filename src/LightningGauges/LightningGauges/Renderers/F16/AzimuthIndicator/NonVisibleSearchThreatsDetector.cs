namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class NonVisibleSearchThreatsDetector
    {
        internal static bool AreNonVisibleSearchThreatsDetected(InstrumentState instrumentState)
        {
            if (instrumentState?.Blips == null) return false;
            if (instrumentState.SearchMode) return false;

            foreach (var thisBlip in instrumentState.Blips)
            {
                if (thisBlip == null || thisBlip.Lethality == 0 || thisBlip.Visible) continue;

                var symbolId = thisBlip.SymbolID;
                if (symbolId >= 5 && symbolId <= 17 || symbolId >= 19 && symbolId <= 26 || symbolId == 30 || symbolId >= 54 && symbolId <= 56) return true;
            }

            return false;
        }
    }
}