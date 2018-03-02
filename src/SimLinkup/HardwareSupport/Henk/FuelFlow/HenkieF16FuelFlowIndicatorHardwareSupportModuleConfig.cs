using System;
using System.Xml.Serialization;
using Common.MacroProgramming;
using Henkie.Common;
using Henkie.FuelFlow;

namespace SimLinkup.HardwareSupport.Henk.FuelFlow
{
    [Serializable]
    [XmlRoot("HenkieF16FuelFlowIndicator")]
    public class HenkieF16FuelFlowIndicatorHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkieF16FuelFlowIndicatorHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkieF16FuelFlowIndicatorHardwareSupportModuleConfig>(filePath);
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
        public StatorBaseAnglesConfig StatorBaseAnglesConfig { get; set; }
        [XmlArray("CalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] CalibrationData { get; set; }
    }

    [Serializable]
    public class StatorBaseAnglesConfig
    {
        public ushort? S1BaseAngleDegrees { get; set; }
        public ushort? S2BaseAngleDegrees { get; set; }
        public ushort? S3BaseAngleDegrees { get; set; }
    }

    [Serializable]
    public class OutputChannelsConfig
    {
        public OutputChannelConfig DIG_OUT_1 { get; set; }
        public OutputChannelConfig DIG_OUT_2 { get; set; }
        public OutputChannelConfig DIG_OUT_3 { get; set; }
        public OutputChannelConfig DIG_OUT_4 { get; set; }
        public OutputChannelConfig DIG_OUT_5 { get; set; }
    }

    [Serializable]
    public class OutputChannelConfig
    {

        public bool InitialValue { get; set; }
    }

}