using System;
using System.Xml.Serialization;
using Common.MacroProgramming;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    [Serializable]
    public class TeensyRWRHardwareSupportModuleConfig
    {
        [XmlElement("COMPort")]
        public string COMPort { get; set; }

        [XmlElement("RotationDegrees")]
        public float RotationDegrees { get; set; }

        [XmlArray("XAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] XAxisCalibrationData { get; set; }

        [XmlArray("YAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] YAxisCalibrationData { get; set; }

        [XmlElement("Centering")]
        public CenteringConfig Centering { get; set; } = new CenteringConfig {OffsetX = 0, OffsetY = 0};

        public class CenteringConfig
        {
            [XmlElement("OffsetX")]
            public short OffsetX { get; set; }

            [XmlElement("OffsetY")]
            public short OffsetY { get; set; }
        }

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
