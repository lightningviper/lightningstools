using System.Runtime.InteropServices;

namespace Henkie.Common
{
    [ComVisible(true)]
    public enum ConnectionType:byte
    {
        USB=0,
        PHCC=1
    }
}
