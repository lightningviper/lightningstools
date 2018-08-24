using System.Drawing;
using Common.Win32.Paths;
using Pdf2Img;

namespace Common.PDF
{
    public static class PdfRenderEngine
    {
        public const int DEFAULT_DPI= 72;
        public static Bitmap GeneratePageBitmap(string pdfPath, int pageNum, Size size, int dpi=DEFAULT_DPI)
        {
            var inputPath = Util.GetShortPathName(pdfPath);
            using (var rasterizer = new PdfRasterizerProxy())
            {
                rasterizer.Open(pdfPath);
                using (var rasterizedPage = rasterizer.GetPage(72, 72, pageNum))
                {
                    return new Bitmap(original: rasterizedPage, width:size.Width, height:size.Height);
                }
            }
        }

        public static int NumPagesInPdf(string fileName)
        {
            using (var rasterizer = new PdfRasterizerProxy()) {
                rasterizer.Open(fileName);
                return rasterizer.PageCount;
            }
        }
    }
}