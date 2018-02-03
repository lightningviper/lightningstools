using System;
using System.Drawing;
using System.IO;
using Ghostscript.NET.Rasterizer.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pdf2Img.Fakes;

namespace Pdf2Img.Tests.UnitTests
{
    [TestClass]
    public class PdfRasterizerProxyUnitTests
    {
        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldOpenStreamUsingGhostscriptNativeDll()
        {
            GivenAnNativeDll(_fakeNativeDll);
            GivenAStreamToOpen(_fakePdfStream);
            WhenOpenStream();
            ThenGhostscriptRasterizerOpenStreamMethodIsCalledUsingNativeDll();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldOpenPathUsingGhostscriptNativeDll()
        {
            GivenAnNativeDll(_fakeNativeDll);
            GivenAPathToOpen(FakePdfPath);
            WhenOpenPath();
            ThenGhostscriptRasterizerOpenPathMethodIsCalledUsingNativeDll();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldDelegateCloseCallToUnderlyingRasterer()
        {
            WhenClose();
            ThenGhostscriptRasterizerCloseMethodIsCalled();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldDelegateGetPageCallToUnderlyingRasterer()
        {
            WhenGetPage(XDpi, YDpi, PageNumber);
            ThenGhostscriptRasterizerGetPageMethodIsCalled(XDpi, YDpi, PageNumber);
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldReturnImageReturnedByGhostscriptRasterizerToCaller()
        {
            WhenGetPage(XDpi, YDpi, PageNumber);
            ThenImageReturnedByGhostscriptRasterizerIsReturnedToCaller();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldDelegateGetPageCountCallToUnderlyingRasterer()
        {
            WhenGetPageCount();
            ThenGhostscriptRasterizerGetPageCountMethodIsCalled();
        }

        [TestCategory("Unit Tests")]
        [TestMethod]
        public void ShouldReturnPageCountReturnedByGhostscriptRasterizerToCaller()
        {
            WhenGetPageCount();
            ThenPageCountReturnedByGhostscriptRasterizerIsReturnedToCaller();
        }

        private void GivenAnNativeDll(byte[] nativeDll)
        {
            _nativeDll = nativeDll;
        }

        private void GivenAPathToOpen(string path)
        {
            _pathToOpen = path;
        }

        private void GivenAStreamToOpen(Stream streamToOpen)
        {
            _streamToOpen = streamToOpen;
        }

        private void WhenOpenPath()
        {
            _pdfRasterizerProxy.Open(_pathToOpen);
        }

        private void WhenOpenStream()
        {
            _pdfRasterizerProxy.Open(_streamToOpen);
        }

        private void WhenClose()
        {
            _pdfRasterizerProxy.Close();
        }

        private void WhenGetPage(int xDpi, int yDpi, int pageNumber)
        {
            _returnValueFromCallToGetPageOnProxy = _pdfRasterizerProxy.GetPage(xDpi, yDpi, pageNumber);
        }

        private void WhenGetPageCount()
        {
            _returnValueFromCallToGetPageCountOnProxy = _pdfRasterizerProxy.PageCount;
        }

        private void ThenGhostscriptRasterizerOpenStreamMethodIsCalledUsingNativeDll()
        {
            Assert.IsTrue(_openStreamWasCalledOnUnderlyingGhostscriptRasterizer);
            Assert.AreEqual(_streamToOpen, _streamPassedToUnderlyingRasterizersOpenStreamMethod);
            CollectionAssert.AreEqual(_nativeDll, _dllImageByteArrayPassedToOpenCall);
        }

        private void ThenGhostscriptRasterizerOpenPathMethodIsCalledUsingNativeDll()
        {
            Assert.IsTrue(_openPathWasCalledOnUnderlyingGhostscriptRasterizer);
            Assert.AreEqual(_pathToOpen, _pathPassedToUnderlyingRasterizersOpenPathMethod);
            CollectionAssert.AreEqual(_nativeDll, _dllImageByteArrayPassedToOpenCall);
        }

        private void ThenGhostscriptRasterizerCloseMethodIsCalled()
        {
            Assert.IsTrue(_closeWasCalledOnUnderlyingGhostscriptRasterizer);
        }

        private void ThenGhostscriptRasterizerGetPageMethodIsCalled(int expectedXDpi, int expectedYDpi,
            int expectedPageNum)
        {
            Assert.IsTrue(_getPageWasCalledOnUnderlyingGhostscriptRasterizer);
            Assert.AreEqual(expectedXDpi, _lastGetPageCallXDpi);
            Assert.AreEqual(expectedYDpi, _lastGetPageCallYDpi);
            Assert.AreEqual(expectedPageNum, _lastGetPageCallPageNum);
        }

        private void ThenGhostscriptRasterizerGetPageCountMethodIsCalled()
        {
            Assert.IsTrue(_getPageCountWasCalledOnUnderlyingGhostscriptRasterizer);
        }

        private void ThenImageReturnedByGhostscriptRasterizerIsReturnedToCaller()
        {
            Assert.AreEqual(_fakePdfPageImage, _returnValueFromCallToGetPageOnProxy);
        }

        private void ThenPageCountReturnedByGhostscriptRasterizerIsReturnedToCaller()
        {
            Assert.AreEqual(FakePageCount, _returnValueFromCallToGetPageCountOnProxy);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _shimsContext = ShimsContext.Create();

            _ghostscriptNativeDllRetrieverStub = new StubIGhostscriptNativeDllRetriever
            {
                GetGhostscriptNativeDllBoolean = get64BitVersion => _nativeDll
            };
            _ghostscriptRasterizerShim = new ShimGhostscriptRasterizer
            {
                OpenStreamByteArray = (stream, byteArray) =>
                {
                    _openStreamWasCalledOnUnderlyingGhostscriptRasterizer = true;
                    _streamPassedToUnderlyingRasterizersOpenStreamMethod = stream;
                    _dllImageByteArrayPassedToOpenCall = byteArray;
                },
                OpenStringByteArray = (path, byteArray) =>
                {
                    _openPathWasCalledOnUnderlyingGhostscriptRasterizer = true;
                    _pathPassedToUnderlyingRasterizersOpenPathMethod = path;
                    _dllImageByteArrayPassedToOpenCall = byteArray;
                },
                GetPageInt32Int32Int32 = (xDpi, yDpi, pageNumber) =>
                {
                    _getPageWasCalledOnUnderlyingGhostscriptRasterizer = true;
                    _lastGetPageCallXDpi = xDpi;
                    _lastGetPageCallYDpi = yDpi;
                    _lastGetPageCallPageNum = pageNumber;
                    return _fakePdfPageImage;
                },
                PageCountGet = () =>
                {
                    _getPageCountWasCalledOnUnderlyingGhostscriptRasterizer = true;
                    return FakePageCount;
                },
                Close = () => { _closeWasCalledOnUnderlyingGhostscriptRasterizer = true; }
            };


            _pdfRasterizerProxy = new PdfRasterizerProxy(_ghostscriptNativeDllRetrieverStub, _ghostscriptRasterizerShim);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _shimsContext.Dispose();
        }

        private const string FakePdfPath = "<fake pdf path>";
        private const int XDpi = -1;
        private const int YDpi = -2;
        private const int PageNumber = -3;
        private const int FakePageCount = -99;
        private static IDisposable _shimsContext;
        private readonly byte[] _fakeNativeDll = { 0xD, 0xE, 0xA, 0xD, 0xB, 0xE, 0xE, 0xF };
        private readonly Image _fakePdfPageImage = new Bitmap(128, 128);
        private readonly Stream _fakePdfStream = new MemoryStream();
        private bool _closeWasCalledOnUnderlyingGhostscriptRasterizer;
        private byte[] _dllImageByteArrayPassedToOpenCall;
        private bool _getPageCountWasCalledOnUnderlyingGhostscriptRasterizer;
        private bool _getPageWasCalledOnUnderlyingGhostscriptRasterizer;
        private StubIGhostscriptNativeDllRetriever _ghostscriptNativeDllRetrieverStub;
        private ShimGhostscriptRasterizer _ghostscriptRasterizerShim;
        private int _lastGetPageCallPageNum;
        private int _lastGetPageCallXDpi;
        private int _lastGetPageCallYDpi;
        private byte[] _nativeDll;
        private bool _openPathWasCalledOnUnderlyingGhostscriptRasterizer;
        private bool _openStreamWasCalledOnUnderlyingGhostscriptRasterizer;
        private string _pathPassedToUnderlyingRasterizersOpenPathMethod;
        private string _pathToOpen;
        private IPdfRasterizer _pdfRasterizerProxy;
        private int _returnValueFromCallToGetPageCountOnProxy;
        private Image _returnValueFromCallToGetPageOnProxy;
        private Stream _streamPassedToUnderlyingRasterizersOpenStreamMethod;
        private Stream _streamToOpen;
    }
}