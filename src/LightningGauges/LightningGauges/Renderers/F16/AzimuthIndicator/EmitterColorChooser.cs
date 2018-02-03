using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EmitterColorChooser
    {
        internal static Color GetEmitterColor(
            Color scopeGreenColor, Color missileColor, Color airborneThreatColor, Color groundThreatColor, Color searchThreatColor, Color navalThreatColor, Color unknownThreatColor, Blip blip,
            Options options)
        {
            var emitterColor = scopeGreenColor;
            if (options.Style == AzimuthIndicator.InstrumentStyle.AdvancedThreatDisplay)
            {
                var category = EmitterCategoryRetriever.GetEmitterCategory(blip.SymbolID);
                switch (category)
                {
                    case EmitterCategory.AirborneThreat:
                        emitterColor = airborneThreatColor;
                        break;
                    case EmitterCategory.GroundThreat:
                        emitterColor = groundThreatColor;
                        break;
                    case EmitterCategory.Search:
                        emitterColor = searchThreatColor;
                        break;
                    case EmitterCategory.Missile:
                        emitterColor = missileColor;
                        break;
                    case EmitterCategory.Naval:
                        emitterColor = navalThreatColor;
                        break;
                    case EmitterCategory.Unknown:
                        emitterColor = unknownThreatColor;
                        break;
                }
            }

            return emitterColor;
        }
    }
}