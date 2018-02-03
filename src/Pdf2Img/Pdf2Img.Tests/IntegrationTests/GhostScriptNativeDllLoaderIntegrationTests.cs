using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pdf2Img.Tests.Helpers;

namespace Pdf2Img.Tests.IntegrationTests
{
    [TestClass]
    public class GhostscriptNativeDllLoaderIntegrationTests
    {
        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ShouldRetrieveNative64BitDll()
        {
            WhenGetFromEmbeddedResource(true);
            Then64BitDllIsRetrieved();
        }

        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ShouldRetrieve32BitNativeDll()
        {
            WhenGetFromEmbeddedResource(false);
            Then32BitDllIsRetrieved();
        }

        private void WhenGetFromEmbeddedResource(bool get64BitVersion)
        {
            _retrievedNativeDll = _ghostscriptNativeDllRetriever.GetGhostscriptNativeDll(get64BitVersion);
        }

        private void Then64BitDllIsRetrieved()
        {
            Assert.IsNotNull(_retrievedNativeDll);
            Assert.AreEqual(NativeDllArchitectureTypeDetector.SixtyFourBitArchitecture,
                NativeDllArchitectureTypeDetector.DetectNativeDllArchitecture(_retrievedNativeDll));
        }

        private void Then32BitDllIsRetrieved()
        {
            Assert.IsNotNull(_retrievedNativeDll);
            Assert.AreEqual(NativeDllArchitectureTypeDetector.ThirtyTwoBitArchitecture,
                NativeDllArchitectureTypeDetector.DetectNativeDllArchitecture(_retrievedNativeDll));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _ghostscriptNativeDllRetriever = new GhostscriptNativeDllRetriever();
        }


        private IGhostscriptNativeDllRetriever _ghostscriptNativeDllRetriever;
        private byte[] _retrievedNativeDll;

    }
}