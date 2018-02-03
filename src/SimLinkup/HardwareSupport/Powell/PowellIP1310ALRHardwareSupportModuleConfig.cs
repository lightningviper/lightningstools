using System;

namespace SimLinkup.HardwareSupport.Powell
{
    [Serializable]
    public class PowellIP1310ALRHardwareSupportModuleConfig
    {
        public string COMPort { get; set; }
        public uint DelayBetweenCharactersMillis { get; set; }
        public uint DelayBetweenCommandsMillis { get; set; }
        public string DeviceID { get; set; }

        public static PowellIP1310ALRHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util
                .DeserializeFromXmlFile<PowellIP1310ALRHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}