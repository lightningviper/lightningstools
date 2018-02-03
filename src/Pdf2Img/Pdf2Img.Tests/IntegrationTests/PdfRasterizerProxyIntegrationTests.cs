using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pdf2Img.Tests.Helpers;

namespace Pdf2Img.Tests.IntegrationTests
{
    [TestClass]
    public class PdfTests
    {
        private int _pageCount;
        private Image _pageImage;
        private IEnumerable<Image> _pageImages;
        private IPdfRasterizer _pdfRasterizer;
        private Stream _pdfStream;
        private const string SourceMaterialFileName = "Test1.pdf";
        private static string PdfFileFromTheTestCorpus => Path.Combine(SourceMaterialFolder, SourceMaterialFileName);
        private static string SourceMaterialFolder => Path.Combine("TestCorpus", "SourceMaterial");
        private static string ExpectedResultsFolder => Path.Combine("TestCorpus", "ExpectedResults");
        private static string ExpectedResultFileNameTemplate => "{0}_Page{1}.bmp";
        private const int ExpectedPageCount = 2;
        private const int DefaultXDpi = 92;
        private const int DefaultYDpi = 92;

        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ShouldGetMultiplePageImages()
        {
            GivenAPdfToConvert(PdfFileFromTheTestCorpus);
            WhenGetPages(DefaultXDpi, DefaultYDpi);
            ThenImagesShouldBeReturnedForAllPages();
        }

        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ShouldGetSinglePageImage()
        {
            GivenAPdfToConvert(PdfFileFromTheTestCorpus);
            WhenGetPage(DefaultXDpi, DefaultYDpi, 1);
            ThenAnImageShouldBeReturnedForThatPage();
        }

        [TestCategory("Integration Tests")]
        [TestMethod]
        public void PageCountShouldMatchWithKnownPageCountFromTestCorpusMaterial()
        {
            GivenAPdfToConvert(PdfFileFromTheTestCorpus);
            WhenGetPageCount();
            ThenReturnedPageCountShouldMatchExpectedResult();
        }

        [TestCategory("Integration Tests")]
        [TestMethod]
        public void ImagesShouldMatchExpectedResultsFromTestCorpus()
        {
            GivenAPdfToConvert(PdfFileFromTheTestCorpus);
            WhenGetPages(DefaultXDpi, DefaultYDpi);
            ThenReturnedImagesShouldMatchExpectedResults();
        }

        private void GivenAPdfToConvert(string testCorpusPdfFilePath)
        {
            var currentExecutingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pdfFile = Path.Combine(currentExecutingAssemblyLocation, testCorpusPdfFilePath);
            using (var fileStream = File.OpenRead(pdfFile))
            {
                _pdfStream = new MemoryStream();
                fileStream.CopyTo(_pdfStream);
                fileStream.Close();
                _pdfRasterizer.Open(_pdfStream);
            }
        }

        private void WhenGetPages(int xDpi, int yDpi)
        {
            var images = new List<Image>();
            for (var pageNumber = 1; pageNumber <= _pdfRasterizer.PageCount; pageNumber++)
            {
                images.Add(_pdfRasterizer.GetPage(xDpi, yDpi, pageNumber));
            }
            _pageImages = images;
        }

        private void WhenGetPage(int xDpi, int yDpi, int pageNumber)
        {
            _pageImage = _pdfRasterizer.GetPage(xDpi, yDpi, pageNumber);
        }

        private void WhenGetPageCount()
        {
            _pageCount = _pdfRasterizer.PageCount;
        }

        private void ThenImagesShouldBeReturnedForAllPages()
        {
            CollectionAssert.AllItemsAreInstancesOfType(_pageImages.ToList(), typeof(Image));
        }

        private void ThenReturnedImagesShouldMatchExpectedResults()
        {
            var pageImages = _pageImages.ToList();
            Assert.AreEqual(ExpectedPageCount, pageImages.Count);
            for (var pageNumber = 1; pageNumber <= pageImages.Count; pageNumber++)
            {
                var currentExecutingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var expectedResultsFolder = Path.Combine(currentExecutingLocation, ExpectedResultsFolder);
                var expectedResultImageFileName = Path.Combine(expectedResultsFolder,
                    string.Format(ExpectedResultFileNameTemplate,
                        Path.GetFileNameWithoutExtension(SourceMaterialFileName), pageNumber));
                var expectedResultImage = Image.FromFile(expectedResultImageFileName);
                var expectedResultImageBytes = expectedResultImage.ToByteArray();
                var convertedImage = pageImages[pageNumber-1];
                var convertedImageBytes = convertedImage.ToByteArray();
                CollectionAssert.AreEqual(expectedResultImageBytes, convertedImageBytes);
            }
        }

        private void ThenReturnedPageCountShouldMatchExpectedResult()
        {
            Assert.AreEqual(ExpectedPageCount, _pageCount);
        }

        private void ThenAnImageShouldBeReturnedForThatPage()
        {
            Assert.IsInstanceOfType(_pageImage, typeof(Image));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _pdfRasterizer = new PdfRasterizerProxy();
        }
    }
}