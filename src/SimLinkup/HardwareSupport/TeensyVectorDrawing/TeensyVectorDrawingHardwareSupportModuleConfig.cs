﻿using System;
using System.Xml.Serialization;
using Common.MacroProgramming;

namespace SimLinkup.HardwareSupport.TeensyVectorDrawing
{
    [Serializable]
    public class TeensyVectorDrawingHardwareSupportModuleConfig
    {
        [XmlElement("COMPort")]
        public string COMPort { get; set; }

        [XmlElement("RotationDegrees")]
        public float RotationDegrees { get; set; }

        [XmlElement("TestPattern")]
        public int TestPattern { get; set; }


        [XmlArray("XAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] XAxisCalibrationData { get; set; }

        [XmlArray("YAxisCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] YAxisCalibrationData { get; set; }

        [XmlElement("Centering")]
        public CenteringConfig Centering { get; set; } = new CenteringConfig {OffsetX = 0, OffsetY = 0};

        [XmlElement("Scaling")]
        public ScalingConfig Scaling { get; set; } = new ScalingConfig { ScaleX = 1, ScaleY = 1 };

        public class CenteringConfig
        {
            [XmlElement("OffsetX")]
            public short OffsetX { get; set; }

            [XmlElement("OffsetY")]
            public short OffsetY { get; set; }
        }

        public class ScalingConfig
        {
            [XmlElement("ScaleX")]
            public double ScaleX { get; set; }

            [XmlElement("ScaleY")]
            public double ScaleY { get; set; }
        }
        public static TeensyVectorDrawingHardwareSupportModuleConfig Load(string filePath)
        {
            return Common.Serialization.Util
                .DeserializeFromXmlFile<TeensyVectorDrawingHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }
}
