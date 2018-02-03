using System;
using LightningGauges.Renderers;
using LightningGauges.Renderers.F16;
using MFDExtractor.Properties;

namespace MFDExtractor.RendererFactories
{
	internal interface IAltimeterRendererFactory
	{
		IAltimeter Create();
	}

	class AltimeterRendererFactory : IAltimeterRendererFactory
	{
		public IAltimeter Create()
		{
			return new Altimeter
			{
				Options = new Altimeter.AltimeterOptions
				{

					Style =
						(Altimeter.AltimeterOptions.F16AltimeterStyle)
							Enum.Parse(typeof (Altimeter.AltimeterOptions.F16AltimeterStyle), Settings.Default.Altimeter_Style),
					PressureAltitudeUnits =
						(Altimeter.AltimeterOptions.PressureUnits)
							Enum.Parse(typeof (Altimeter.AltimeterOptions.PressureUnits), Settings.Default.Altimeter_PressureUnits)
				}
			};
		}
	}
}
