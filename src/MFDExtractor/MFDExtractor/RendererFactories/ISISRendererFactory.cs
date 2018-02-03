using System;
using LightningGauges.Renderers.F16.ISIS;
using MFDExtractor.Properties;

namespace MFDExtractor.RendererFactories
{
	public interface IISISRendererFactory
	{
		IISIS Create(); 
	}
	class ISISRendererFactory:IISISRendererFactory
	{
		public IISIS Create()
		{
			var toReturn = new ISIS
			{
				Options = new Options
				{
					PressureAltitudeUnits = (PressureUnits)Enum.Parse(typeof(PressureUnits), Settings.Default.ISIS_PressureUnits),
				}
			};
			return toReturn;
		}
	}
}
