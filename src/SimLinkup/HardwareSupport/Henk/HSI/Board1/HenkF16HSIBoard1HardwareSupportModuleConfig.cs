using System;
using System.Xml.Serialization;
using Common.MacroProgramming;
using Henkie.Common;
using Henkie.HSI.Board1;

namespace SimLinkup.HardwareSupport.Henk.HSI.Board1
{
    [Serializable]
    [XmlRoot("Henk_F16_HS1_Board1")]
    public class HenkieF16HSIBoard1HardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkieF16HSIBoard1HardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkieF16HSIBoard1HardwareSupportModuleConfig>(filePath);
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
        public OutputChannelsConfig OutputChannelsConfig { get; set; }
        public StatorOffsetsConfig StatorOffsetsConfig { get; set; }

        [XmlArray("HeadingCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] HeadingCalibrationData { get; set; }

        [XmlArray("BearingCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] BearingCalibrationData { get; set; }

        [XmlArray("RangeOnesDigitCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] RangeOnesDigitCalibrationData { get; set; }

        [XmlArray("RangeTensDigitCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] RangeTensDigitCalibrationData { get; set; }

        [XmlArray("RangeHundredsDigitCalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] RangeHundredsDigitCalibrationData { get; set; }

    }

    [Serializable]
    public class StatorOffsetsConfig
    {
        public ushort? HeadingS1Offset { get; set; }
        public ushort? HeadingS2Offset { get; set; }
        public ushort? HeadingS3Offset { get; set; }
        public ushort? BearingS1Offset { get; set; }
        public ushort? BearingS2Offset { get; set; }
        public ushort? BearingS3Offset { get; set; }
        public byte? RangeOnesDigitXOffset { get; set; }
        public byte? RangeOnesDigitYOffset { get; set; }
        public byte? RangeTensDigitXOffset { get; set; }
        public byte? RangeTensDigitYOffset { get; set; }
        public byte? RangeHundredsDigitXOffset { get; set; }
        public byte? RangeHundredsDigitYOffset { get; set; }
    }

    [Serializable]
    public class OutputChannelsConfig
    {
        public OutputChannelConfig DIG_OUT_1 { get; set; }
        public OutputChannelConfig DIG_OUT_2 { get; set; }
    }

    [Serializable]
    public class OutputChannelConfig
    {

        public bool InitialValue { get; set; }
    }

}