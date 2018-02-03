using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pdf2Img.Fakes;

namespace Pdf2Img.Tests.UnitTests
{
    [TestClass]
    public class GhostscriptNativeDllLoaderUnitTests
    {
        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldRetrieve64BitNativeDll()
        {
            GivenA64BitGhostscriptNativeDllIsPackagedAsAnEmbeddedResource();
            WhenGetFromEmbeddedResource(true);
            Then64BitNativeDllIsRetrieved();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldRetrieve32BitNativeDll()
        {
            GivenA32BitGhostscriptNativeDllIsPackagedAsAnEmbeddedResource();
            WhenGetFromEmbeddedResource(false);
            Then32BitNativeDllIsRetrieved();
        }

        private void GivenA64BitGhostscriptNativeDllIsPackagedAsAnEmbeddedResource()
        {
            _embeddedResourceLoaderStub.LoadEmbeddedResourceString = get64BitVersion => new byte[] {64};
        }

        private void GivenA32BitGhostscriptNativeDllIsPackagedAsAnEmbeddedResource()
        {
            _embeddedResourceLoaderStub.LoadEmbeddedResourceString = get64BitVersion => new byte[] {32};
        }

        private void WhenGetFromEmbeddedResource(bool get64BitVersion)
        {
            _retrievedNativeDll = _ghostscriptNativeDllRetriever.GetGhostscriptNativeDll(get64BitVersion);
        }

        private void Then64BitNativeDllIsRetrieved()
        {
            Assert.IsNotNull(_retrievedNativeDll);
            Assert.AreEqual(64, _retrievedNativeDll[0]);
        }

        private void Then32BitNativeDllIsRetrieved()
        {
            Assert.IsNotNull(_retrievedNativeDll);
            Assert.AreEqual(32, _retrievedNativeDll[0]);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _embeddedResourceLoaderStub = new StubIEmbeddedResourceLoader();
            _ghostscriptNativeDllRetriever = new GhostscriptNativeDllRetriever(_embeddedResourceLoaderStub);
        }

        private StubIEmbeddedResourceLoader _embeddedResourceLoaderStub;
        private IGhostscriptNativeDllRetriever _ghostscriptNativeDllRetriever;
        private byte[] _retrievedNativeDll;

    }
}