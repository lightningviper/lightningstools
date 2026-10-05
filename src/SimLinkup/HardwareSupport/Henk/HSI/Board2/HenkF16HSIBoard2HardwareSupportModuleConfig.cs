using System;
using System.Xml.Serialization;
using Common.MacroProgramming;
using Henkie.Common;
using Henkie.HSI.Board2;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board2
{
    [Serializable]
    [XmlRoot("Henk_F16_HSI_Board2")]
    public class HenkieF16HSIBoard2HardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkieF16HSIBoard2HardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkieF16HSIBoard2HardwareSupportModuleConfig>(filePath);
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
        public ConnectionType? ConnectionType { get; set; } = Henkie.Common.ConnectionType.USB;
        public DiagnosticLEDMode? DiagnosticLEDMode { get; set; } = Henkie.Common.DiagnosticLEDMode.Heartbeat;
        public OutputChannelsConfig OutputChannelsConfig { get; set; } = new OutputChannelsConfig();
        public StatorOffsetsConfig StatorOffsetsConfig { get; set; } = new StatorOffsetsConfig();

        public byte HeadingValueHysteresisThreshold { get; set; } = 0;
        public byte CourseValueHysteresisThreshold { get; set; } = 0;
        public byte Course45DegreeSinCosCrossover { get; set; } = 177;

        [XmlArray("CourseDeviationIndicatorCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] CourseDeviationIndicatorCalibrationData { get; set; } = Array.Empty<CalibrationPoint>();

        [XmlArray("HeadingDatumCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] HeadingDatumCalibrationData { get; set; } = Array.Empty<CalibrationPoint>();

        [XmlArray("DesiredCourseCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] DesiredCourseCalibrationData { get; set; } = Array.Empty<CalibrationPoint>();
    }

    [Serializable]
    public class StatorOffsetsConfig
    {
        public ushort? CourseExciterS1Offset { get; set; } = 156;
        public ushort? CourseExciterS2Offset { get; set; } = 497;
        public ushort? CourseExciterS3Offset { get; set; } = 838;
    }

    [Serializable]
    public class OutputChannelsConfig
    {
        public OutputChannelConfig DIG_OUT_A { get; set; } = new OutputChannelConfig();
        public OutputChannelConfig DIG_OUT_B { get; set; } = new OutputChannelConfig();
        public OutputChannelConfig DIG_OUT_X { get; set; } = new OutputChannelConfig();
    }

    [Serializable]
    public class OutputChannelConfig
    {
        public bool InitialValue { get; set; } = false;
    }

}