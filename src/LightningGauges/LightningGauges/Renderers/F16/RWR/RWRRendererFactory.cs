using LightningGauges.Renderers.F16.RWR.ALR56;
using LightningGauges.Renderers.F16.RWR.ALR67;
using LightningGauges.Renderers.F16.RWR.ALR69;
using LightningGauges.Renderers.F16.RWR.ALR93;
using LightningGauges.Renderers.F16.RWR.CARAPACE;
using LightningGauges.Renderers.F16.RWR.SPS1000;
using System;

namespace LightningGauges.Renderers.F16.RWR
{
    public interface IRWRRendererFactory
    {
        RWRRenderer CreateRenderer(RWRType rwrType, bool useVectorFont=false);
    }
    public class RWRRendererFactory:IRWRRendererFactory
    {
        public RWRRenderer CreateRenderer(RWRType rwrType, bool useVectorFont=false)
        {
            RWRRenderer toReturn=null;
            switch (rwrType)
            {
                case RWRType.ALR56:
                    toReturn= new RWRScopeALR56Renderer();
                    break;
                case RWRType.ALR69:
                    toReturn= new RWRScopeALR69Renderer();
                    break;
                case RWRType.ALR93:
                    toReturn= new RWRScopeALR93Renderer();
                    break;
                case RWRType.SPS1000:
                    toReturn= new RWRScopeSPS1000Renderer();
                    break;
                case RWRType.ALR67:
                    toReturn= new RWRScopeALR67Renderer();
                    break;
                case RWRType.CARAPACE:
                    toReturn= new RWRScopeCARAPACERenderer();
                    break;
                default:
                    throw new ArgumentException();
            }
            toReturn.FormatForVectorDisplay = useVectorFont;
            return toReturn;
           
        }

    }
}
