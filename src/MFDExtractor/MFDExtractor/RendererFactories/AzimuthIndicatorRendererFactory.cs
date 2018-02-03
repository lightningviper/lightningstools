using System;
using LightningGauges.Renderers.F16.AzimuthIndicator;
using MFDExtractor.Properties;

namespace MFDExtractor.RendererFactories
{
	internal interface IAzimuthIndicatorRendererFactory
	{
		IAzimuthIndicator Create();
	}

	class AzimuthIndicatorRendererFactory : IAzimuthIndicatorRendererFactory
	{
		public IAzimuthIndicator Create()
		{
			return new AzimuthIndicator
			{
				Options = new Options
				{
					Style = (AzimuthIndicator.InstrumentStyle)
						Enum.Parse(typeof (AzimuthIndicator.InstrumentStyle),
							Settings.Default.AzimuthIndicatorType),
					HideBezel = !Settings.Default.AzimuthIndicator_ShowBezel,
				}
			};
		}
	}
}
