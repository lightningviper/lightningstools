using System;
using System.Xml.Serialization;
using AnalogDevices;

namespace SimLinkup.HardwareSupport.AnalogDevices
{
    [Serializable]
    [XmlRoot(nameof(AnalogDevices))]
    public class AnalogDevicesHardwareSupportModuleConfig
    {
        [XmlArray("Devices")]
        [XmlArrayItem("Device")]
        public DeviceConfig[] Devices { get; set; }

        public bool? EnableTemperatureShutdown { get; set; }

        public static AnalogDevicesHardwareSupportModuleConfig Load(string filePath)
        {
            return
                Common.Serialization.Util.DeserializeFromXmlFile<AnalogDevicesHardwareSupportModuleConfig>(filePath);
        }

        public void Save(string filePath)
        {
            Common.Serialization.Util.SerializeToXmlFile(this, filePath);
        }
    }


    [Serializable]
    public class DeviceConfig
    {
        public DeviceCalibration Calibration { get; set; }
        public DACChannelConfigurations DACChannelConfig { get; set; }
        public DacPrecision? DACPrecision { get; set; }
    }

    [Serializable]
    public class DeviceCalibration
    {
        public ushort? OffsetDAC0 { get; set; }
        public ushort? OffsetDAC1 { get; set; }
        public ushort? OffsetDAC2 { get; set; }
    }

    [Serializable]
    public class DACChannelConfigurations
    {
        public DACChannelConfiguration DAC0 { get; set; }
        public DACChannelConfiguration DAC1 { get; set; }
        public DACChannelConfiguration DAC10 { get; set; }
        public DACChannelConfiguration DAC11 { get; set; }
        public DACChannelConfiguration DAC12 { get; set; }
        public DACChannelConfiguration DAC13 { get; set; }
        public DACChannelConfiguration DAC14 { get; set; }
        public DACChannelConfiguration DAC15 { get; set; }
        public DACChannelConfiguration DAC16 { get; set; }
        public DACChannelConfiguration DAC17 { get; set; }
        public DACChannelConfiguration DAC18 { get; set; }
        public DACChannelConfiguration DAC19 { get; set; }
        public DACChannelConfiguration DAC2 { get; set; }
        public DACChannelConfiguration DAC20 { get; set; }
        public DACChannelConfiguration DAC21 { get; set; }
        public DACChannelConfiguration DAC22 { get; set; }
        public DACChannelConfiguration DAC23 { get; set; }
        public DACChannelConfiguration DAC24 { get; set; }
        public DACChannelConfiguration DAC25 { get; set; }
        public DACChannelConfiguration DAC26 { get; set; }
        public DACChannelConfiguration DAC27 { get; set; }
        public DACChannelConfiguration DAC28 { get; set; }
        public DACChannelConfiguration DAC29 { get; set; }
        public DACChannelConfiguration DAC3 { get; set; }
        public DACChannelConfiguration DAC30 { get; set; }
        public DACChannelConfiguration DAC31 { get; set; }
        public DACChannelConfiguration DAC32 { get; set; }
        public DACChannelConfiguration DAC33 { get; set; }
        public DACChannelConfiguration DAC34 { get; set; }
        public DACChannelConfiguration DAC35 { get; set; }
        public DACChannelConfiguration DAC36 { get; set; }
        public DACChannelConfiguration DAC37 { get; set; }
        public DACChannelConfiguration DAC38 { get; set; }
        public DACChannelConfiguration DAC39 { get; set; }
        public DACChannelConfiguration DAC4 { get; set; }
        public DACChannelConfiguration DAC5 { get; set; }
        public DACChannelConfiguration DAC6 { get; set; }
        public DACChannelConfiguration DAC7 { get; set; }
        public DACChannelConfiguration DAC8 { get; set; }
        public DACChannelConfiguration DAC9 { get; set; }
    }

    [Serializable]
    public class DACChannelConfiguration
    {
        public DACChannelCalibration Calibration { get; set; }
        public InitialDACChannelState InitialState { get; set; }
    }

    [Serializable]
    public class InitialDACChannelState
    {
        public ushort? DataValueA { get; set; }
        public ushort? DataValueB { get; set; }
    }

    [Serializable]
    public class DACChannelCalibration
    {
        public ushort? Gain { get; set; }
        public ushort? Offset { get; set; }
    }
}