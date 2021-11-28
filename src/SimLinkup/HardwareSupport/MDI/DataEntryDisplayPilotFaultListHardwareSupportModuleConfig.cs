using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.MDI
{
    [Serializable]
    [XmlRoot("Microchip_DataEntryDisplayPilotFaultList")]
    public class DataEntryDisplayPilotFaultListHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem("Device")]
        public DeviceConfig[] Devices { get; set; }

        public static DataEntryDisplayPilotFaultListHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<DataEntryDisplayPilotFaultListHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }

    }


    [Serializable]
    public class DeviceConfig
    {
        public string COMPort { get; set; }
        public DeviceType? DeviceType { get; set; } = MDI.DeviceType.DED;
    }

    [Serializable]
    public enum DeviceType
    {
       DED,
       PFL
    }

}