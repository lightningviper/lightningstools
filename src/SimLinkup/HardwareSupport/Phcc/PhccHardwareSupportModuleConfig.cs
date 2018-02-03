using System;

namespace SimLinkup.HardwareSupport.Phcc
{
    [Serializable]
    public class PhccHardwareSupportModuleConfig
    {
        public string PhccDeviceManagerConfigFilePath { get; set; }

        public static PhccHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util.DeserializeFromXmlFile<PhccHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}