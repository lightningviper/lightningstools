using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.TeensyEWMU
{
    [Serializable]
    public class TeensyEWMUHardwareSupportModuleConfig
    {
        [XmlElement("COMPort")]
        public string COMPort { get; set; }

        [XmlArray("DXOutputs"), XmlArrayItem("Output")]
        public DXOutput[] DXOutputs { get; set; }

        public static TeensyEWMUHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util
                .DeserializeFromXmlFile<TeensyEWMUHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}
