using SimLinkup.HardwareSupport.TeensyEWMU;
using System;
using System.Xml.Serialization;

namespace SimLinkup.HardwareSupport.ArduinoSeat
{
    [Serializable]
    public class ArduinoSeatHardwareSupportModuleConfig
    {
        [XmlElement("COMPort")]
        public string COMPort { get; set; }

        
        [XmlElement("MotorByte1")]
        public byte MotorByte1 { get; set; }

        [XmlElement("MotorByte2")]
        public byte MotorByte2 { get; set; }
        [XmlElement("MotorByte3")]
        public byte MotorByte3 { get; set; }
        [XmlElement("MotorByte4")]
        public byte MotorByte4 { get; set; }

        [XmlElement("ForceSlight")]
        public byte ForceSlight { get; set; }

        [XmlElement("ForceRumble")]
        public byte ForceRumble { get; set; }

        [XmlElement("ForceMedium")]
        public byte ForceMedium { get; set; }

        [XmlElement("ForceHard")]
        public byte ForceHard { get; set; }

        
        [XmlArray("SeatOutputs"), XmlArrayItem("Output")]
        public SeatOutput[] SeatOutputs { get; set; }

        [XmlIgnore]
        public string FilePath { get; set; }

        public static ArduinoSeatHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util
                .DeserializeFromXmlFile<ArduinoSeatHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}
