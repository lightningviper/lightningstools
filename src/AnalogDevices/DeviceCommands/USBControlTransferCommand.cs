namespace AnalogDevices.DeviceCommands
{
    internal interface IUSBControlTransfer
    {
        void UsbControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer = null,
            int? length = null);
    }

    internal class USBControlTransferCommand : IUSBControlTransfer
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;

        public USBControlTransferCommand(
            IDenseDacEvalBoard evalBoard,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void UsbControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer = null,
            int? length = null)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _evalBoard.UsbDevice.ControlTransfer(requestType, request, value, index, buffer, length);
            }
        }
    }
}