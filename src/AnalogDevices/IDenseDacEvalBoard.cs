namespace AnalogDevices
{
    public interface IDenseDacEvalBoard
    {
        DacPrecision DacPrecision { get; set; }
        string SymbolicName { get; }

        bool IsOverTemperature { get; }
        bool IsThermalShutdownEnabled { get; set; }

        ushort OffsetDAC0 { get; set; }
        ushort OffsetDAC1 { get; set; }

        bool PacketErrorCheckErrorOccurred { get; }

        DeviceState DeviceState { get; }
        IUsbDevice UsbDevice { get; }

        DacChannelDataSource GetDacChannelDataSource(ChannelAddress channel);
        void SetDacChannelDataSource(ChannelAddress channel, DacChannelDataSource value);

        void SetDacChannelDataSource(ChannelGroup group, DacChannelDataSource channel0, DacChannelDataSource channel1,
            DacChannelDataSource channel2, DacChannelDataSource channel3, DacChannelDataSource channel4,
            DacChannelDataSource channel5, DacChannelDataSource channel6, DacChannelDataSource channel7);

        void SetDacChannelDataSourceAllChannels(DacChannelDataSource source);

        ushort GetDacChannelDataValueA(ChannelAddress channel);
        void SetDacChannelDataValueA(ChannelAddress channel, ushort value);

        ushort GetDacChannelDataValueB(ChannelAddress channel);
        void SetDacChannelDataValueB(ChannelAddress channel, ushort value);

        ushort GetDacChannelGain(ChannelAddress channel);
        void SetDacChannelGain(ChannelAddress channel, ushort value);

        ushort GetDacChannelOffset(ChannelAddress channel);
        void SetDacChannelOffset(ChannelAddress channel, ushort value);

        void PerformSoftPowerDown();
        void PerformSoftPowerUp();

        void SetLDACPinHigh();
        void SetLDACPinLow();
        void PulseLDACPin();

        void SuspendAllDacOutputs();
        void ResumeAllDacOutputs();

        void Reset();

        void UpdateAllDacOutputs();

        void UploadFirmware(IhxFile ihxFile);
    }
}