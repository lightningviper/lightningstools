using System.Runtime.InteropServices;

namespace Henkie.Common
{
    [ComVisible(true)]
    public enum SerialPortDataReceivedEventType
    {
        Chars = 1,
        Eof
    }
}
