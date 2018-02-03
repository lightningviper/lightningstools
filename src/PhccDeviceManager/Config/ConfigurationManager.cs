using System.Collections.Generic;
using System.Xml.Serialization;
using Common.Serialization;

namespace Phcc.DeviceManager.Config
{
    [XmlRoot("PhccDeviceManagerConfiguration")]
    public class ConfigurationManager
    {
        private List<Motherboard> _motherboards = new List<Motherboard>();

        [XmlArray("Devices")]
        [XmlArrayItem("Motherboard")]
        public List<Motherboard> Motherboards
        {
            get { return _motherboards; }
            set { _motherboards = value; }
        }

        public static ConfigurationManager Load(string fileName)
        {
            return Util.DeserializeFromXmlFile<ConfigurationManager>(fileName);
        }

        public void Save(string fileName)
        {
            Util.SerializeToXmlFile(this, fileName);
        }
    }
}