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
        RWRRenderer CreateRenderer(RWRType rwrType);
    }
    public class RWRRendererFactory:IRWRRendererFactory
    {
        public RWRRenderer CreateRenderer(RWRType rwrType)
        {
            switch (rwrType)
            {
                case RWRType.ALR56:
                    return new RWRScopeALR56Renderer();
                case RWRType.ALR69:
                    return new RWRScopeALR69Renderer();
                case RWRType.ALR93:
                    return new RWRScopeALR93Renderer();
                case RWRType.SPS1000:
                    return new RWRScopeSPS1000Renderer();
                case RWRType.ALR67:
                    return new RWRScopeALR67Renderer();
                case RWRType.CARAPACE:
                    return new RWRScopeCARAPACERenderer();
                default:
                    throw new ArgumentException();
            }
           
        }

    }
}
