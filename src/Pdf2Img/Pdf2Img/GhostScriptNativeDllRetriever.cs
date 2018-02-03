using System.Reflection;

namespace Pdf2Img
{
    internal interface IGhostscriptNativeDllRetriever
    {
        byte[] GetGhostscriptNativeDll(bool get64BitVersion);
    }

    internal class GhostscriptNativeDllRetriever : IGhostscriptNativeDllRetriever
    {
        internal const string EmbeddedResourceNameTemplate = "{0}.lib.gs.gs{1}.dll";
        private static byte[] _nativeDll;
        private static bool _nativeDllArchitectureIs64Bit;
        private readonly IEmbeddedResourceLoader _embeddedResourceLoader;

        public GhostscriptNativeDllRetriever(IEmbeddedResourceLoader embeddedResourceLoader = null)
        {
            _embeddedResourceLoader = embeddedResourceLoader ?? new EmbeddedResourceLoader();
        }

        public byte[] GetGhostscriptNativeDll(bool get64BitVersion)
        {
            if (_nativeDll != null && (_nativeDllArchitectureIs64Bit == get64BitVersion))
            {
                return _nativeDll;
            }
            var resourceName = GetEmbeddedResourceName(get64BitVersion);
            _nativeDll = _embeddedResourceLoader.LoadEmbeddedResource(resourceName);
            _nativeDllArchitectureIs64Bit = get64BitVersion;
            return _nativeDll;
        }

        internal static string GetEmbeddedResourceName(bool get64BitVersion)
        {
            var thisAssemblyShortName = Assembly.GetExecutingAssembly().GetName().Name;
            return string.Format(EmbeddedResourceNameTemplate,
                thisAssemblyShortName, get64BitVersion ? "64" : "32");
        }
    }
}