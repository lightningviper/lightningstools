using System;
using System.Xml.Serialization;
using Common.MacroProgramming;
using Henkie.Common;
using Henkie.Altimeter;

namespace SimLinkup.HardwareSupport.Henk.Altimeter
{
    [Serializable]
    [XmlRoot("HenkieF16Altimeter")]
    public class HenkieF16AltimeterHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkieF16AltimeterHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkieF16AltimeterHardwareSupportModuleConfig>(filePath);
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
        public double? MinBaroPressureInHg { get; set; } = 28.09;
        public double? MaxBaroPressureInHg { get; set; } = 31.025;
        public double? IndicatedAltitudeDifferenceInFeetFromMinBaroToMaxBaro { get; set; } = 2800;
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
    }

    [Serializable]
    public class OutputChannelConfig
    {

        public bool InitialValue { get; set; }
    }

}