using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;
using MFDExtractor.Properties;

namespace MFDExtractor.RendererFactories
{
	internal interface IFuelQualityIndicatorRendererFactory
	{
		IFuelQuantityIndicator Create();
	}

	class FuelQualityIndicatorRendererFactory : IFuelQualityIndicatorRendererFactory
	{
		public IFuelQuantityIndicator Create()
		{
			return new FuelQuantityIndicator
			{
				Options =
				{
					NeedleType =
						Settings.Default.FuelQuantityIndicator_NeedleCModel
							? FuelQuantityIndicator.FuelQuantityIndicatorOptions.F16FuelQuantityNeedleType.CModel
							: FuelQuantityIndicator.FuelQuantityIndicatorOptions.F16FuelQuantityNeedleType.DModel
				}
			};
		}
	}
}
