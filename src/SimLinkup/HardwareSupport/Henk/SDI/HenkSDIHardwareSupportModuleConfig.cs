using System;
using System.Xml.Serialization;
using Common.MacroProgramming;
using Henkie.Common;
using Henkie.SDI;

namespace SimLinkup.HardwareSupport.Henk.SDI
{
    [Serializable]
    [XmlRoot("HenkSDI")]
    public class HenkSDIHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem(nameof(Device))]
        public DeviceConfig[] Devices { get; set; }

        public static HenkSDIHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<HenkSDIHardwareSupportModuleConfig>(filePath);
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
        public int? InitialIndicatorPosition { get; set; }
        public MovementLimitsConfig MovementLimitsConfig { get; set; }
        public OutputChannelsConfig OutputChannelsConfig { get; set; }
        public PowerDownConfig PowerDownConfig { get; set; }
        public StatorBaseAnglesConfig StatorBaseAnglesConfig { get; set; }
        public UpdateRateControlConfig UpdateRateControlConfig { get; set; }
    }

    [Serializable]
    public class PowerDownConfig
    {
        public short? DelayTimeMilliseconds { get; set; }
        public bool? Enabled { get; set; }
        public PowerDownLevel? Level { get; set; }
    }

    [Serializable]
    public class StatorBaseAnglesConfig
    {
        public ushort? S1BaseAngleDegrees { get; set; }
        public ushort? S2BaseAngleDegrees { get; set; }
        public ushort? S3BaseAngleDegrees { get; set; }
    }

    [Serializable]
    public class MovementLimitsConfig
    {
        public byte? Max { get; set; }
        public byte? Min { get; set; }
    }

    [Serializable]
    public class OutputChannelsConfig
    {
        public OutputChannelConfig DIG_PWM_1 { get; set; }
        public OutputChannelConfig DIG_PWM_2 { get; set; }
        public OutputChannelConfig DIG_PWM_3 { get; set; }
        public OutputChannelConfig DIG_PWM_4 { get; set; }
        public OutputChannelConfig DIG_PWM_5 { get; set; }
        public OutputChannelConfig DIG_PWM_6 { get; set; }
        public OutputChannelConfig DIG_PWM_7 { get; set; }
        public OutputChannelConfig PWM_OUT { get; set; }
    }

    [Serializable]
    public class OutputChannelConfig
    {
        [XmlArray("CalibrationData")]
        [XmlArrayItem(nameof(CalibrationPoint))]
        public CalibrationPoint[] CalibrationData { get; set; }

        public byte? InitialValue { get; set; }
        public OutputChannelMode? Mode { get; set; }
    }

    [Serializable]
    public class UpdateRateControlConfig
    {
        public UpdateRateControlModes? Mode { get; set; }
        public UpdateRateControlModeSettings ModeSettings { get; set; }
        public ushort? StepUpdateDelayMillis { get; set; }
        public bool? UseShortestPath { get; set; }
    }

    [Serializable]
    [XmlInclude(typeof(LimitModeSettings))]
    [XmlInclude(typeof(SmoothingModeSettings))]
    public abstract class UpdateRateControlModeSettings
    {
    }

    [Serializable]
    public class LimitModeSettings : UpdateRateControlModeSettings
    {
        public byte? LimitThreshold { get; set; }
    }

    [Serializable]
    public class SmoothingModeSettings : UpdateRateControlModeSettings
    {
        public byte? SmoothingMinimumThreshold { get; set; }
        public UpdateRateControlSmoothingMode? SmoothingMode { get; set; }
    }
}