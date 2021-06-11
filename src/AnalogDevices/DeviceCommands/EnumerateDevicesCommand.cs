using System.Collections.Generic;
using System.Linq;

namespace AnalogDevices.DeviceCommands
{
    internal interface IEnumerateDevices
    {
        IDenseDacEvalBoard[] EnumerateDevices();
    }

    internal class EnumerateDevicesCommand : IEnumerateDevices
    {
        public IDenseDacEvalBoard[] EnumerateDevices()
        {
            var discoveredDevices = new List<string>();
            var toReturn = new List<IDenseDacEvalBoard>();
            var devs = LibUsbDevice.AllDevices.ToArray();
            //var devs = WinUsbDevice.AllDevices.ToArray();
            foreach (var device in devs)
            {
                try
                {
                    if (device == null || device.Vid != 0x0456 || (ushort)device.Pid != 0xB20F && (ushort)device.Pid != 0xB20E)
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                if (!discoveredDevices.Contains(device.SymbolicName))
                {
                    var newDevice = new DenseDacEvalBoard(device);
                    toReturn.Add(newDevice);
                }
                discoveredDevices.Add(device.SymbolicName);
            }
            return toReturn.OrderByDescending(x => x.UsbDevice.SymbolicName).ToArray();
        }
    }
}