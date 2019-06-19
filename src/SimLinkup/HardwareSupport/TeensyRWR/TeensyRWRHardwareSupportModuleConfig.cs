using System;
using System.Xml.Serialization;
using Common.MacroProgramming;

namespace SimLinkup.HardwareSupport.TeensyRWR
{
    [Serializable]
    public class TeensyRWRHardwareSupportModuleConfig
    {
        public string COMPort { get; set; }
        public float RotationDegrees { get; set; }

        [XmlArray("XAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] XAxisCalibrationData { get; set; }

        [XmlArray("YAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] YAxisCalibrationData { get; set; }

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
