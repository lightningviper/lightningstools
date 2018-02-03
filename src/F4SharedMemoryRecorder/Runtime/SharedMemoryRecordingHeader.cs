using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace F4SharedMemoryRecorder.Runtime
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    struct SharedMemoryRecordingHeader
    {
        public byte[] Magic;
        public ulong NumSamples;
        public ushort SampleInterval;
    }
}
