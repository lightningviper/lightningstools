using System;
using System.Xml.Serialization;
using Henkie.Common;
using Henkie.SDI;

namespace SimLinkup.HardwareSupport.Henk.QuadSinCos
{
    [Serializable]
    [XmlRoot("HenkieQuadSinCos")]
    public class HenkieQuadSinCosBoardHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkieQuadSinCosBoardHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkieQuadSinCosBoardHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }

    [Serializable]
    public class DeviceConfig
    {
        public string Address { get; set; }
        public string COMPort { get; set; }
        public ConnectionType? ConnectionType { get; set; }
        public DiagnosticLEDMode? DiagnosticLEDMode { get; set; }
    }
}