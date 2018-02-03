using System.Threading;

namespace AnalogDevices.DeviceCommands
{
    internal interface IUploadFirmware
    {
        void UploadFirmware(IhxFile ihxFile);
    }

    internal class UploadFirmwareCommand : IUploadFirmware
    {
        private readonly ILockFactory _lockFactory;
        private readonly IResetDevice _resetDeviceCommand;
        private readonly IUSBControlTransfer _usbControlTransferCommand;

        public UploadFirmwareCommand(
            IDenseDacEvalBoard evalBoard,
            IResetDevice resetDeviceCommand = null,
            IUSBControlTransfer usbControlTransferCommand = null,
            ILockFactory lockFactory = null)
        {
            _resetDeviceCommand = resetDeviceCommand ?? new ResetDeviceCommand(evalBoard);
            _usbControlTransferCommand = usbControlTransferCommand ?? new USBControlTransferCommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void UploadFirmware(IhxFile ihxFile)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                const int transactionBytes = 256;
                var buffer = new byte[transactionBytes];

                _resetDeviceCommand.ResetDevice(true); // reset = 1

                var j = 0;
                for (var i = 0; i <= ihxFile.IhxData.Length; i++)
                {
                    if (i >= ihxFile.IhxData.Length || ihxFile.IhxData[i] < 0 || j >= transactionBytes)
                    {
                        if (j > 0)
                        {
                            _usbControlTransferCommand.UsbControlTransfer(
                                (byte) UsbRequestType.TypeVendor,
                                0xA0,
                                i - j,
                                0,
                                buffer,
                                j);
                            Thread.Sleep(1); // to avoid package loss
                        }
                        j = 0;
                    }

                    if (i >= ihxFile.IhxData.Length || ihxFile.IhxData[i] < 0 || ihxFile.IhxData[i] > 255)
                    {
                        continue;
                    }
                    buffer[j] = (byte) ihxFile.IhxData[i];
                    j += 1;
                }
                _resetDeviceCommand.ResetDevice(false); //error (may caused re-numeration) can be ignored
            }
        }
    }
}