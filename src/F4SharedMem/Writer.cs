using System;
using System.Runtime.InteropServices;

namespace F4SharedMem
{
    public sealed class Writer : IDisposable
    {
        private bool _disposed;
        private IntPtr _hPrimarySharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _hSecondarySharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _hOsbSharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _hIntellivibeSharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _hRadioClientControlSharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _hRadioClientStatusSharedMemoryAreaFileMappingObject = IntPtr.Zero;
        private IntPtr _lpPrimarySharedMemoryAreaBaseAddress = IntPtr.Zero;
        private IntPtr _lpSecondarySharedMemoryAreaBaseAddress = IntPtr.Zero;
        private IntPtr _lpOsbSharedMemoryAreaBaseAddress = IntPtr.Zero;
        private IntPtr _lpIntellivibeSharedMemoryAreaBaseAddress = IntPtr.Zero;
        private IntPtr _lpRadioClientControlSharedMemoryAreaBaseAddress = IntPtr.Zero;
        private IntPtr _lpRadioClientStatusSharedMemoryAreaBaseAddress = IntPtr.Zero;

        private const string _primarySharedMemoryAreaFileName = "FalconSharedMemoryArea";
        private const string _secondarySharedMemoryFileName = "FalconSharedMemoryArea2";
        private const string _osbSharedMemoryAreaFileName = "FalconSharedOsbMemoryArea";
        private const string _intellivibeSharedMemoryAreaFileName = "FalconIntellivibeSharedMemoryArea";
        private const string _radioClientControlSharedMemoryAreaFileName = "FalconRccSharedMemoryArea";
        private const string _radioClientStatusSharedMemoryAreaFileName = "FalconRcsSharedMemoryArea";

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void WritePrimaryFlightData(byte[] data)
        {
            WriteSharedMemData(data, _primarySharedMemoryAreaFileName, ref _hPrimarySharedMemoryAreaFileMappingObject, ref _lpPrimarySharedMemoryAreaBaseAddress);
        }

        public void WriteOSBData(byte[] data)
        {
            WriteSharedMemData(data, _osbSharedMemoryAreaFileName, ref _hOsbSharedMemoryAreaFileMappingObject, ref _lpOsbSharedMemoryAreaBaseAddress);
        }

        public void WriteFlightData2(byte[] data)
        {
            WriteSharedMemData(data, _secondarySharedMemoryFileName, ref _hSecondarySharedMemoryAreaFileMappingObject, ref _lpSecondarySharedMemoryAreaBaseAddress);
        }

        public void WriteIntellivibeData(byte[] data)
        {
            WriteSharedMemData(data, _intellivibeSharedMemoryAreaFileName, ref _hIntellivibeSharedMemoryAreaFileMappingObject, ref _lpIntellivibeSharedMemoryAreaBaseAddress);
        }
        public void WriteRadioClientControlData(byte[] data)
        {
            WriteSharedMemData(data, _radioClientControlSharedMemoryAreaFileName, ref _hRadioClientControlSharedMemoryAreaFileMappingObject, ref _lpRadioClientControlSharedMemoryAreaBaseAddress);
        }
        public void WriteRadioClientStatusData(byte[] data)
        {
            WriteSharedMemData(data, _radioClientStatusSharedMemoryAreaFileName, ref _hRadioClientStatusSharedMemoryAreaFileMappingObject, ref _lpRadioClientStatusSharedMemoryAreaBaseAddress);
        }
        private static void WriteSharedMemData(byte[] data, string sharedMemoryAreaFileName, ref IntPtr hSharedMemoryFileMappingObject, ref IntPtr lpDestination)
        {
            if (data == null || data.Length == 0) return;
            if (hSharedMemoryFileMappingObject.Equals(IntPtr.Zero))
            {
                CreateSharedMemoryArea(sharedMemoryAreaFileName, (ushort)data.Length, out hSharedMemoryFileMappingObject, out lpDestination);
            }
            if (hSharedMemoryFileMappingObject.Equals(IntPtr.Zero))
            {
                return;
            }
            if (!hSharedMemoryFileMappingObject.Equals(IntPtr.Zero))
            {
                Marshal.Copy(data, 0, lpDestination, data.Length);
            }
        }
        private static void CreateSharedMemoryArea(string sharedMemoryAreaFileName, ushort length, out IntPtr hFileMappingObject, out IntPtr lpBaseAddress)
        {
            hFileMappingObject =
                F4SharedMem.Win32.NativeMethods.CreateFileMapping(new IntPtr(F4SharedMem.Win32.NativeMethods.INVALID_HANDLE_VALUE), IntPtr.Zero,
                                                F4SharedMem.Win32.NativeMethods.PageProtection.ReadWrite, 0, length,
                                                sharedMemoryAreaFileName);
            lpBaseAddress =
                F4SharedMem.Win32.NativeMethods.MapViewOfFile(hFileMappingObject,
                                            F4SharedMem.Win32.NativeMethods.SECTION_MAP_READ | F4SharedMem.Win32.NativeMethods.SECTION_MAP_WRITE, 0, 0,
                                            IntPtr.Zero);
        }


        private void CloseSharedMemFiles()
        {
            CloseSharedMemFile(ref _lpPrimarySharedMemoryAreaBaseAddress, ref _hPrimarySharedMemoryAreaFileMappingObject);
            CloseSharedMemFile(ref _lpSecondarySharedMemoryAreaBaseAddress, ref _hSecondarySharedMemoryAreaFileMappingObject);
            CloseSharedMemFile(ref _lpOsbSharedMemoryAreaBaseAddress, ref _hOsbSharedMemoryAreaFileMappingObject);
            CloseSharedMemFile(ref _lpIntellivibeSharedMemoryAreaBaseAddress, ref _hIntellivibeSharedMemoryAreaFileMappingObject);
            CloseSharedMemFile(ref _lpRadioClientControlSharedMemoryAreaBaseAddress, ref _hRadioClientControlSharedMemoryAreaFileMappingObject);
            CloseSharedMemFile(ref _lpRadioClientStatusSharedMemoryAreaBaseAddress, ref _hRadioClientStatusSharedMemoryAreaFileMappingObject);
        }
        private static void CloseSharedMemFile(ref IntPtr lpBaseAddress, ref IntPtr hFileMappingObject) 
        {
            if (!hFileMappingObject.Equals(IntPtr.Zero))
            {
                F4SharedMem.Win32.NativeMethods.UnmapViewOfFile(lpBaseAddress);
                F4SharedMem.Win32.NativeMethods.CloseHandle(hFileMappingObject);
            }
            lpBaseAddress = IntPtr.Zero;
            hFileMappingObject = IntPtr.Zero;
        }

        internal void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseSharedMemFiles();
                }

                _disposed = true;
            }
        }
    }
}