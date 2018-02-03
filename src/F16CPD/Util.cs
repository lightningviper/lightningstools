using F16CPD.Properties;

namespace F16CPD
{
    internal static class Util
    {
        internal static void SaveCurrentProperties()
        {
            Settings.Default.Save();
            Settings.Default.Reload();
        }
    }
}