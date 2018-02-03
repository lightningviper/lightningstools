namespace AnalogDevices.DeviceCommands
{
    internal interface IReadSPI
    {
        ushort ReadSPI();
    }

    internal class ReadSPICommand : IReadSPI
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly IInitializeSPIPins _initializeSPIPinsCommand;
        private readonly ILockFactory _lockFactory;
        private readonly IUSBControlTransfer _usbControlTransferCommand;

        public ReadSPICommand(
            IDenseDacEvalBoard evalBoard,
            IUSBControlTransfer usbControlTransferCommand = null,
            IInitializeSPIPins initializeSPIPinsCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _usbControlTransferCommand = usbControlTransferCommand ?? new USBControlTransferCommand(evalBoard);
            _initializeSPIPinsCommand = initializeSPIPinsCommand ?? new InitializeSPIPinsCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public ushort ReadSPI()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                if (!_evalBoard.DeviceState.SPI_Initialized)
                {
                    _initializeSPIPinsCommand.InitializeSPIPins();
                }

                const ushort len = 3;
                var buf = new byte[len];

                _usbControlTransferCommand.UsbControlTransfer(
                    (byte) UsbRequestType.TypeVendor | (byte) UsbCtrlFlags.Direction_In,
                    (byte) DeviceCommand.SendSPI,
                    0,
                    0,
                    buf,
                    len
                );
                return (ushort) (buf[0] | (buf[1] << 8));
            }
        }
    }
}