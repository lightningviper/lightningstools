using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.Simtek
{
    [Serializable]
    [XmlRoot(nameof(Simtek100294HardwareSupportModule))]
    public class Simtek100294HardwareSupportModuleConfig
    {
        public uint? MaxPoundsTotalFuel { get; set; } = 9200;

        public static Simtek100294HardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<Simtek100294HardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }


}