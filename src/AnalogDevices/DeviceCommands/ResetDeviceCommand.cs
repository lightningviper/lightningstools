using System.Threading;

namespace AnalogDevices.DeviceCommands
{
    internal interface IResetDevice
    {
        void ResetDevice(bool r);
    }

    internal class ResetDeviceCommand : IResetDevice
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly IUSBControlTransfer _usbControlTransferCommand;

        public ResetDeviceCommand(
            IDenseDacEvalBoard evalBoard,
            IUSBControlTransfer usbControlTransferCommand = null,
            ILockFactory lockFactory = null)
        {
            _evalBoard = evalBoard;
            _usbControlTransferCommand = usbControlTransferCommand ?? new USBControlTransferCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void ResetDevice(bool r)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _evalBoard.DeviceState.ClearRegisterCache();
                byte[] buffer = {(byte) (r ? 1 : 0)};
                _usbControlTransferCommand.UsbControlTransfer(
                    (byte) UsbRequestType.TypeVendor,
                    0xA0,
                    0xE600,
                    0,
                    buffer,
                    buffer.Length);
                Thread.Sleep(r ? 50 : 400); // give the firmware some time for initialization
            }
        }
    }
}