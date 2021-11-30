using Common.MacroProgramming;
using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.NiclasMorin.DTSCard
{
    [Serializable]
    [XmlRoot("DTSCard")]
    public class DTSCardHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem("Device")]
        public DeviceConfig[] Devices { get; set; }

        public static DTSCardHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util.DeserializeFromXmlFile<DTSCardHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }

    [Serializable]
    public class DeviceConfig
    {
        public string Serial { get; set; }
        [XmlArray("CalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] CalibrationData { get; set; }

    }
}