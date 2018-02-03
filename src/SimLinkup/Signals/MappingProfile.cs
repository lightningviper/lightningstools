using System;
using System.Collections.Generic;

namespace SimLinkup.Signals
{
    [Serializable]
    public class MappingProfile
    {
        private List<SignalMapping> _signalMappings = new List<SignalMapping>();

        public List<SignalMapping> SignalMappings
        {
            get => _signalMappings;
            set => _signalMappings = value;
        }

        public static MappingProfile Load(string filePath)
        {
            return Common.Serialization.Util.DeserializeFromXmlFile<MappingProfile>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}