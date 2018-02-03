using AnalogDevices.DeviceCommands;
using Resourcer;

namespace AnalogDevices
{
    public class DenseDacEvalBoard : IDenseDacEvalBoard
    {
        private readonly IGetDacChannelDataSource _getDacChannelDataSourceCommand;
        private readonly IGetDacChannelDataValueA _getDacChannelDataValueACommand;
        private readonly IGetDacChannelDataValueB _getDacChannelDataValueBCommand;
        private readonly IGetDacChannelGain _getDacChannelGainCommand;
        private readonly IGetDacChannelOffset _getDacChannelOffsetCommand;
        private readonly IGetDeviceSymbolicName _getDeviceSymbolicNameCommand;
        private readonly IGetIsOverTemperature _getIsOverTemperatureCommand;
        private readonly IGetPacketErrorCheckErrorOccurredStatus _getPacketErrorCheckErrorOccurredStatusCommand;
        private readonly IGetThermalShutdownEnabled _getThermalShutdownEnabledCommand;
        private readonly IPerformSoftPowerDown _performSoftPowerDownCommand;
        private readonly IPerformSoftPowerUp _performSoftPowerUpCommand;
        private readonly IPulseLDACPin _pulseLDACPinCommand;
        private readonly IReadbackOFS0Register _readbackOFS0RegisterCommand;
        private readonly IReadbackOFS1Register _readbackOFS1RegisterCommand;
        private readonly ISetCLRPinHigh _setCLRPinHighCommand;
        private readonly ISetCLRPinLow _setCLRPinLowCommand;
        private readonly ISetDacChannelDataSourceAllChannels _setDacChannelDataSourceAllChannelsCommand;
        private readonly ISetDacChannelDataSource _setDacChannelDataSourceCommand;
        private readonly ISetDacChannelDataValueA _setDacChannelDataValueACommand;
        private readonly ISetDacChannelDataValueB _setDacChannelDataValueBCommand;
        private readonly ISetDacChannelGain _setDacChannelGainCommand;
        private readonly ISetDacChannelOffset _setDacChannelOffsetCommand;
        private readonly ISetLDACPinHigh _setLDACPinHighCommand;
        private readonly ISetLDACPinLow _setLDACPinLowCommand;
        private readonly ISetOffsetDAC0 _setOffsetDAC0Command;
        private readonly ISetOffsetDAC1 _setOffsetDAC1Command;
        private readonly ISetThermalShutdownEnabled _setThermalShutdownEnabledCommand;
        private readonly IToggleReset _toggleResetCommand;
        private readonly IUploadFirmware _uploadFirmwareCommand;

        public DenseDacEvalBoard(IUsbDevice usbDevice) : this(usbDevice, null)
        {
            UsbDevice = UsbDevice;
            UploadFirmware(new IhxFile(Resource.AsStream("AD5371SPI.hex")));
        }

        internal DenseDacEvalBoard(
            IUsbDevice usbDevice,
            IGetDeviceSymbolicName getDeviceSymbolicNameCommand = null,
            IUploadFirmware uploadFirmwareCommand = null,
            IGetPacketErrorCheckErrorOccurredStatus getPacketErrorCheckErrorOccurredStatusCommand = null,
            IPerformSoftPowerDown performSoftPowerDownCommand = null,
            IPerformSoftPowerUp performSoftPowerUpCommand = null,
            IGetThermalShutdownEnabled getThermalShutdownEnabledCommand = null,
            ISetThermalShutdownEnabled setThermalShutdownEnabledCommand = null,
            IGetIsOverTemperature getIsOverTemperatureCommand = null,
            IGetDacChannelDataSource getDacChannelDataSourceCommand = null,
            ISetDacChannelDataSource setDacChannelDataSourceCommand = null,
            ISetDacChannelDataSourceAllChannels setDacChannelDataSourceAllChannelsCommand = null,
            ISetDacChannelOffset setDacChannelOffsetCommand = null,
            IGetDacChannelOffset getDacChannelOffsetCommand = null,
            ISetDacChannelGain setDacChannelGainCommand = null,
            IGetDacChannelGain getDacChannelGainCommand = null,
            IGetDacChannelDataValueA getDacChannelDataValueACommand = null,
            ISetDacChannelDataValueA setDacChannelDataValueACommand = null,
            IGetDacChannelDataValueB getDacChannelDataValueBCommand = null,
            ISetDacChannelDataValueB setDacChannelDataValueBCommand = null,
            ISetOffsetDAC0 setOffsetDAC0Command = null,
            ISetOffsetDAC1 setOffsetDAC1Command = null,
            IReadbackOFS0Register readbackOFS0RegisterCommand = null,
            IReadbackOFS1Register readbackOFS1RegisterCommand = null,
            ISetCLRPinLow setCLRPinLowCommand = null,
            ISetCLRPinHigh setCLRPinHighCommand = null,
            ISetLDACPinHigh setLDACPinHighCommand = null,
            ISetLDACPinLow setLDACPinLowCommand = null,
            IPulseLDACPin pulseLDACPinCommand = null,
            IToggleReset toggleResetCommand = null
        )
        {
            UsbDevice = usbDevice;
            _getDeviceSymbolicNameCommand = getDeviceSymbolicNameCommand ??
                                            new GetDeviceSymbolicNameCommand(this);
            _uploadFirmwareCommand = uploadFirmwareCommand ?? new UploadFirmwareCommand(this);
            _getPacketErrorCheckErrorOccurredStatusCommand =
                getPacketErrorCheckErrorOccurredStatusCommand ??
                new GetPacketErrorCheckErrorOccurredStatusCommand(this);
            _performSoftPowerDownCommand = performSoftPowerDownCommand ??
                                           new PerformSoftPowerDownCommand(this);
            _performSoftPowerUpCommand = performSoftPowerUpCommand ?? new PerformSoftPowerUpCommand(this);
            _getThermalShutdownEnabledCommand = getThermalShutdownEnabledCommand ??
                                                new GetThermalShutdownEnabledCommand(this);
            _setThermalShutdownEnabledCommand = setThermalShutdownEnabledCommand ??
                                                new SetThermalShutdownEnabledCommand(this);
            _getIsOverTemperatureCommand = getIsOverTemperatureCommand ??
                                           new GetIsOverTemperatureCommand(this);
            _getDacChannelDataSourceCommand = getDacChannelDataSourceCommand ??
                                              new GetDacChannelDataSourceCommand(this);
            _setDacChannelDataSourceCommand = setDacChannelDataSourceCommand ??
                                              new SetDacChannelDataSourceCommand(this);
            _setDacChannelDataSourceAllChannelsCommand = setDacChannelDataSourceAllChannelsCommand ??
                                                         new SetDacChannelDataSourceAllChannelsCommand(this);
            _getDacChannelOffsetCommand = getDacChannelOffsetCommand ?? new GetDacChannelOffsetCommand(this);
            _setDacChannelOffsetCommand = setDacChannelOffsetCommand ?? new SetDacChannelOffsetCommand(this);
            _getDacChannelGainCommand = getDacChannelGainCommand ?? new GetDacChannelGainCommand(this);
            _setDacChannelGainCommand = setDacChannelGainCommand ?? new SetDacChannelGainCommand(this);
            _getDacChannelDataValueACommand = getDacChannelDataValueACommand ??
                                              new GetDacChannelDataValueACommand(this);
            _setDacChannelDataValueACommand = setDacChannelDataValueACommand ??
                                              new SetDacChannelDataValueACommand(this);
            _getDacChannelDataValueBCommand = getDacChannelDataValueBCommand ??
                                              new GetDacChannelDataValueBCommand(this);
            _setDacChannelDataValueBCommand = setDacChannelDataValueBCommand ??
                                              new SetDacChannelDataValueBCommand(this);
            _setOffsetDAC0Command = setOffsetDAC0Command ?? new SetOffsetDAC0Command(this);
            _setOffsetDAC1Command = setOffsetDAC1Command ?? new SetOffsetDAC1Command(this);
            _readbackOFS0RegisterCommand = readbackOFS0RegisterCommand ??
                                           new ReadbackOFS0RegisterCommand(this);
            _readbackOFS1RegisterCommand = readbackOFS1RegisterCommand ??
                                           new ReadbackOFS1RegisterCommand(this);
            _setCLRPinLowCommand = setCLRPinLowCommand ?? new SetCLRPinLowCommand(this);
            _setCLRPinHighCommand = setCLRPinHighCommand ?? new SetCLRPinHighCommand(this);
            _setLDACPinHighCommand = setLDACPinHighCommand ?? new SetLDACPinHighCommand(this);
            _setLDACPinLowCommand = setLDACPinLowCommand ?? new SetLDACPinLowCommand(this);
            _pulseLDACPinCommand = pulseLDACPinCommand ?? new PulseLDACPinCommand(this);
            _toggleResetCommand = toggleResetCommand ?? new ToggleResetCommand(this);
        }

        public string SymbolicName => _getDeviceSymbolicNameCommand.GetDeviceSymbolicName();

        public ushort OffsetDAC0
        {
            get => _readbackOFS0RegisterCommand.ReadbackOFS0Register();
            set => _setOffsetDAC0Command.SetOffsetDAC0(value);
        }

        public ushort OffsetDAC1
        {
            get => _readbackOFS1RegisterCommand.ReadbackOFS1Register();
            set => _setOffsetDAC1Command.SetOffsetDAC1(value);
        }

        public bool IsThermalShutdownEnabled
        {
            get => _getThermalShutdownEnabledCommand.GetThermalShutdownEnabled();
            set => _setThermalShutdownEnabledCommand.SetThermalShutdownEnabled(value);
        }

        public DacPrecision DacPrecision
        {
            get => DeviceState.Precision;
            set => DeviceState.Precision = value;
        }

        public DeviceState DeviceState { get; } = new DeviceState();

        public bool PacketErrorCheckErrorOccurred => _getPacketErrorCheckErrorOccurredStatusCommand
            .GetPacketErrorCheckErrorOccurredStatus();

        public bool IsOverTemperature => _getIsOverTemperatureCommand.GetIsOverTemperature();

        public DacChannelDataSource GetDacChannelDataSource(ChannelAddress channel)
        {
            return _getDacChannelDataSourceCommand.GetDacChannelDataSource(channel);
        }

        public void SetDacChannelDataSource(ChannelAddress channel, DacChannelDataSource value)
        {
            _setDacChannelDataSourceCommand.SetDacChannelDataSource(channel, value);
        }

        public void SetDacChannelDataSource(ChannelGroup group, DacChannelDataSource channel0,
            DacChannelDataSource channel1, DacChannelDataSource channel2, DacChannelDataSource channel3,
            DacChannelDataSource channel4, DacChannelDataSource channel5, DacChannelDataSource channel6,
            DacChannelDataSource channel7)
        {
            _setDacChannelDataSourceCommand.SetDacChannelDataSource(group, channel0, channel1, channel2, channel3,
                channel4, channel5, channel6, channel7);
        }

        public void SetDacChannelDataSourceAllChannels(DacChannelDataSource source)
        {
            _setDacChannelDataSourceAllChannelsCommand.SetDacChannelDataSourceAllChannels(source);
        }

        public void Reset()
        {
            _toggleResetCommand.ToggleReset();
        }

        public void SuspendAllDacOutputs()
        {
            _setCLRPinLowCommand.SetCLRPinLow();
        }

        public void ResumeAllDacOutputs()
        {
            _setCLRPinHighCommand.SetCLRPinHigh();
        }

        public void UpdateAllDacOutputs()
        {
            _pulseLDACPinCommand.PulseLDACPin();
        }

        public ushort GetDacChannelDataValueA(ChannelAddress channel)
        {
            return _getDacChannelDataValueACommand.GetDacChannelDataValueA(channel);
        }

        public void SetDacChannelDataValueA(ChannelAddress channel, ushort value)
        {
            _setDacChannelDataValueACommand.SetDacChannelDataValueA(channel, value);
        }

        public ushort GetDacChannelDataValueB(ChannelAddress channel)
        {
            return _getDacChannelDataValueBCommand.GetDacChannelDataValueB(channel);
        }

        public void SetDacChannelDataValueB(ChannelAddress channel, ushort value)
        {
            _setDacChannelDataValueBCommand.SetDacChannelDataValueB(channel, value);
        }

        public ushort GetDacChannelOffset(ChannelAddress channel)
        {
            return _getDacChannelOffsetCommand.GetDacChannelOffset(channel);
        }

        public void SetDacChannelOffset(ChannelAddress channel, ushort value)
        {
            _setDacChannelOffsetCommand.SetDacChannelOffset(channel, value);
        }

        public ushort GetDacChannelGain(ChannelAddress channel)
        {
            return _getDacChannelGainCommand.GetDacChannelGain(channel);
        }

        public void SetDacChannelGain(ChannelAddress channel, ushort value)
        {
            _setDacChannelGainCommand.SetDacChannelGain(channel, value);
        }

        public void PulseLDACPin()
        {
            _pulseLDACPinCommand.PulseLDACPin();
        }

        public void SetLDACPinLow()
        {
            _setLDACPinLowCommand.SetLDACPinLow();
        }

        public void SetLDACPinHigh()
        {
            _setLDACPinHighCommand.SetLDACPinHigh();
        }

        public void UploadFirmware(IhxFile ihxFile)
        {
            _uploadFirmwareCommand.UploadFirmware(ihxFile);
        }

        public void PerformSoftPowerDown()
        {
            _performSoftPowerDownCommand.PerformSoftPowerDown();
        }

        public void PerformSoftPowerUp()
        {
            _performSoftPowerUpCommand.PerformSoftPowerUp();
        }

        public IUsbDevice UsbDevice { get; }

        public static IDenseDacEvalBoard[] Enumerate()
        {
            return new EnumerateDevicesCommand().EnumerateDevices();
        }
    }
}