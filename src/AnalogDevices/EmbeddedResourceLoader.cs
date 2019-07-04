using System.IO;
using System.Reflection;

namespace AnalogDevices
{
    internal static class EmbeddedResourceLoader
    {
        public static Stream GetResourceStream(string path)
        {
            var resourceName = Assembly.GetExecutingAssembly().GetName().Name + "." + path.Replace("\\", ".");
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}
