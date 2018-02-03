using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F4SharedMem;
using System.Runtime.InteropServices;
namespace F4SharedMemoryRecorder.Runtime
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SharedMemorySample
    {
        public ushort PrimaryFlightDataLength;
        public byte[] PrimaryFlightData;
        public ushort FlightData2Length;
        public byte[] FlightData2;
        public ushort OSBDataLength;
        public byte[] OSBData;
        public ushort IntellivibeDataLength;
        public byte[] IntellivibeData;
        public ushort RadioClientStatusDataLength;
        public byte[] RadioClientStatusData;
        public ushort RadioClientControlDataLength;
        public byte[] RadioClientControlData;
    }
}
