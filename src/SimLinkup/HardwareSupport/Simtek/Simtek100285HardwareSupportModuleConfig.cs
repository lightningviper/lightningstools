using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.Simtek
{
    [Serializable]
    [XmlRoot(nameof(Simtek100285HardwareSupportModule))]
    public class Simtek100285HardwareSupportModuleConfig
    {
        public double? MinBaroPressureInHg { get; set; } = 28.09;
        public double? MaxBaroPressureInHg { get; set; } = 31.025;
        public double? IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro { get; set; } = 2800;
        public double? AltitudeZeroOffsetInFeet { get; set; } = 0;
        public static Simtek100285HardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<Simtek100285HardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }


}