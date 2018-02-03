namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class NonVisiblePriorityThreatsDetector
    {
        internal static bool AreNonVisiblePriorityThreatsDetected(InstrumentState instrumentState)
        {
            if (instrumentState?.Blips == null) return false;

            if (!instrumentState.PriorityMode) return false;

            var trackingOwnshipCount = 0;
            var visibleCount = 0;
            foreach (var thisBlip in instrumentState.Blips)
            {
                if (thisBlip == null || thisBlip.Lethality == 0) continue;

                trackingOwnshipCount++;
                if (thisBlip.Visible) visibleCount++;
            }

            return visibleCount == 5 && trackingOwnshipCount > visibleCount;
        }
    }
}