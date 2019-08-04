using System;
using System.Runtime.InteropServices;
namespace F4SharedMemoryRecorder.Runtime
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SharedMemorySample
    {
        public uint PrimaryFlightDataLength;
        public byte[] PrimaryFlightData;
        public uint FlightData2Length;
        public byte[] FlightData2;
        public uint OSBDataLength;
        public byte[] OSBData;
        public uint IntellivibeDataLength;
        public byte[] IntellivibeData;
        public uint RadioClientStatusDataLength;
        public byte[] RadioClientStatusData;
        public uint RadioClientControlDataLength;
        public byte[] RadioClientControlData;
        public uint StringDataLength;
        public byte[] StringData;
        public uint DrawingDataLength;
        public byte[] DrawingData;
    }
}
