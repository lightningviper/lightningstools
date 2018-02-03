namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EmitterCategoryRetriever
    {
        internal static EmitterCategory GetEmitterCategory(int symbolId)
        {
            EmitterCategory category;
            switch (symbolId)
            {
                case (int) ThreatSymbols.RWRSYM_ADVANCED_INTERCEPTOR:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_BASIC_INTERCEPTOR:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_ACTIVE_MISSILE:
                    category = EmitterCategory.Missile;
                    break;
                case (int) ThreatSymbols.RWRSYM_HAWK:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_PATRIOT:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA2:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA3:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA4:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA5:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA6:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA8:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA9:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA10:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA13:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_AAA:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SEARCH:
                    category = EmitterCategory.Search;
                    break;
                case (int) ThreatSymbols.RWRSYM_NAVAL:
                    category = EmitterCategory.Naval;
                    break;
                case (int) ThreatSymbols.RWRSYM_CHAPARAL:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_SA15:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_NIKE:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_A1:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_A2:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_A3:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_PDOT:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_PSLASH:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_UNK1:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_UNK2:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_UNK3:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_KSAM:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V1:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V4:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V5:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V6:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V14:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V15:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V16:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V18:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V19:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V20:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V21:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V22:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V23:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V25:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V27:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V29:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V30:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_V31:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_VP:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_VPD:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_VA:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_VB:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_VS:
                    category = EmitterCategory.AirborneThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_Aa:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_Ab:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_Ac:
                    category = EmitterCategory.GroundThreat;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_F_S:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_F_A:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_F_M:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_F_U:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_F_BW:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_BW_S:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_BW_A:
                    category = EmitterCategory.Unknown;
                    break;
                case (int) ThreatSymbols.RWRSYM_MIB_BW_M:
                    category = EmitterCategory.Unknown;
                    break;
                default:
                    category = EmitterCategory.Unknown;
                    break;
            }

            return category;
        }
    }
}