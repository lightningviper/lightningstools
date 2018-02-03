using System;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;
using MFDExtractor.Properties;
using MFDExtractor.UI;

namespace MFDExtractor.RendererFactories
{
	internal interface IVVIRendererFactory
	{
		IVerticalVelocityIndicator Create();
	}

	class VVIRendererFactory : IVVIRendererFactory
	{
		public IVerticalVelocityIndicator Create()
		{
			var vviStyleString = Settings.Default.VVI_Style;
			var vviStyle = (VVIStyles)Enum.Parse(typeof(VVIStyles), vviStyleString);
			switch (vviStyle)
			{
				case VVIStyles.Tape:
					return new VerticalVelocityIndicatorUSA();
				case VVIStyles.Needle:
					return new VerticalVelocityIndicatorEU();
				default:
					return new VerticalVelocityIndicatorUSA();
			}
		}
	}
}
