using System;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    [Serializable]
    public class TeensyRWRHardwareSupportModuleConfig
    {
        public string COMPort { get; set; }

        public static TeensyRWRHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util
                .DeserializeFromXmlFile<TeensyRWRHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}
