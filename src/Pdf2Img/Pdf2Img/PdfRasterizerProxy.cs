using System;
using System.Drawing;
using System.IO;
using Ghostscript.NET.Rasterizer;

namespace Pdf2Img
{
    public interface IPdfRasterizer : IDisposable
    {
        int PageCount { get; }
        void Open(Stream stream);
        void Open(string path);
        void Close();
        Image GetPage(int xDpi, int yDpi, int pageNumber);
    }

    public class PdfRasterizerProxy : IPdfRasterizer
    {
        private readonly Lazy<byte[]> _ghostscriptNativeDll;
        private readonly IGhostscriptNativeDllRetriever _ghostscriptNativeDllRetriever;
        private readonly GhostscriptRasterizer _ghostscriptRasterizer;

        internal PdfRasterizerProxy(IGhostscriptNativeDllRetriever ghostscriptNativeDllRetriever = null,
            GhostscriptRasterizer ghostscriptRasterizer = null)
        {
            _ghostscriptNativeDllRetriever = ghostscriptNativeDllRetriever ?? new GhostscriptNativeDllRetriever();
            _ghostscriptRasterizer = ghostscriptRasterizer ?? new GhostscriptRasterizer();
            _ghostscriptNativeDll =
                new Lazy<byte[]>(
                    () => _ghostscriptNativeDllRetriever.GetGhostscriptNativeDll(Environment.Is64BitProcess));
        }

        public void Open(Stream stream)
        {
            _ghostscriptRasterizer.Open(stream, _ghostscriptNativeDll.Value);
        }

        public void Open(string path)
        {
            _ghostscriptRasterizer.Open(path, _ghostscriptNativeDll.Value);
        }

        public Image GetPage(int xDpi, int yDpi, int pageNumber)
        {
            return _ghostscriptRasterizer.GetPage(xDpi, yDpi, pageNumber);
        }

        public int PageCount => _ghostscriptRasterizer.PageCount;

        public void Close()
        {
            _ghostscriptRasterizer.Close();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            _ghostscriptRasterizer.Dispose();
        }
    }
}