using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pdf2Img.Tests.IntegrationTests
{
    [TestClass]
    public class EmbeddedResourceLoaderIntegrationTests
    {
        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ShouldLoadEmbeddedResource()
        {
            GivenAnEmbeddedResourceNameToLoad(_testEmbeddedResource);
            WhenLoadEmbeddedResource();
            ThenTheSpecifiedEmbeddedResourceIsRetrieved();
        }

        private void GivenAnEmbeddedResourceNameToLoad(string embeddedResourceNameToLoad)
        {
            _embeddedResourceNameToLoad = embeddedResourceNameToLoad;
        }

        private void WhenLoadEmbeddedResource()
        {
            _loadedResource = _embeddedResourceLoader.LoadEmbeddedResource(_embeddedResourceNameToLoad);
        }

        private void ThenTheSpecifiedEmbeddedResourceIsRetrieved()
        {
            Assert.IsTrue(Encoding.ASCII.GetString(_loadedResource).Contains(ExpectedContents));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _embeddedResourceLoader = new EmbeddedResourceLoader
            {
                AssemblyContext = Assembly.GetExecutingAssembly()
            };
        }

        private const string ExpectedContents = "{CFD12EF3-D80B-4954-A28C-6EB789F3F32C}";
        private readonly string _testEmbeddedResource =
            $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.TestableEmbeddedResource.txt";
        private IEmbeddedResourceLoader _embeddedResourceLoader;
        private string _embeddedResourceNameToLoad;
        private byte[] _loadedResource;

    }
}