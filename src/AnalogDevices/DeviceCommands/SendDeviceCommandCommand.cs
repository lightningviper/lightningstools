namespace AnalogDevices.DeviceCommands
{
    internal interface ISendDeviceCommand
    {
        void SendDeviceCommand(DeviceCommand deviceCommand, uint setupData);
    }

    internal class SendDeviceCommandCommand : ISendDeviceCommand
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly IInitializeSPIPins _initializeSPIPinsCommand;
        private readonly ILockFactory _lockFactory;
        private readonly IUSBControlTransfer _usbControlTransferCommand;

        public SendDeviceCommandCommand(
            IDenseDacEvalBoard evalBoard,
            IUSBControlTransfer usbControlTransferCommand = null,
            IInitializeSPIPins initializeSPIPinsCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _usbControlTransferCommand = usbControlTransferCommand ?? new USBControlTransferCommand(evalBoard);
            _initializeSPIPinsCommand = initializeSPIPinsCommand ??
                                        new InitializeSPIPinsCommand(evalBoard, this);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SendDeviceCommand(DeviceCommand deviceCommand, uint setupData)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (!_evalBoard.DeviceState.SPI_Initialized && deviceCommand != DeviceCommand.InitializeSPIPins)
                {
                    _initializeSPIPinsCommand.InitializeSPIPins();
                }

                _usbControlTransferCommand.UsbControlTransfer(
                    (byte) UsbRequestType.TypeVendor,
                    (byte) deviceCommand,
                    (ushort) (setupData & (uint) BasicMasks.SixteenBits),
                    (ushort) ((setupData & 0xFF0000) / 0x10000),
                    null,
                    0);
            }
        }
    }
}