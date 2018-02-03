using Common.UI;

namespace LightningGauges.Renderers.F16.ISIS
{
    public class Options
    {
        public Options()
        {
            PressureAltitudeUnits = PressureUnits.InchesOfMercury;
            RadarAltitudeUnits = AltitudeUnits.Feet;
        }

        public AltitudeUnits RadarAltitudeUnits { get; set; }
        public PressureUnits PressureAltitudeUnits { get; set; }
    }
}