using LightningGauges.Renderers.F16;
using MFDExtractor.Properties;

namespace MFDExtractor.RendererFactories
{
    internal interface ITachometerRendererFactory
    {
        ITachometer Create();
    }


    class TachometerRendererFactory : ITachometerRendererFactory
    {
       public ITachometer Create()
        {
            return new Tachometer
            {
                Options =
                {
                    EngineType = Settings.Default.IsPwEngine
                    ?Tachometer.TachometerOptions.TachometerEngineType.PWEngine
                    :Tachometer.TachometerOptions.TachometerEngineType.GEEngine,
                    IsSecondary=false
                }
            };
        }
    }
   
}
