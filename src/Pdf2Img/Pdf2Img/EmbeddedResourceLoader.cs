using System.IO;
using System.Reflection;

namespace Pdf2Img
{
    internal interface IEmbeddedResourceLoader
    {
        byte[] LoadEmbeddedResource(string name);
    }

    internal class EmbeddedResourceLoader : IEmbeddedResourceLoader
    {
        public EmbeddedResourceLoader()
        {
            AssemblyContext = Assembly.GetExecutingAssembly();
        }

        internal Assembly AssemblyContext { get; set; }

        public byte[] LoadEmbeddedResource(string name)
        {
            using (var embeddedResourceStream = AssemblyContext.GetManifestResourceStream(name) ?? new MemoryStream())
            {
                var bytes = new byte[embeddedResourceStream.Length];
                embeddedResourceStream.Read(bytes, 0, (int) embeddedResourceStream.Length);
                return bytes;
            }
        }
    }
}