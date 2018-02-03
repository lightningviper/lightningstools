using System;
using System.Text;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Imaging;
using Common.Imaging;

namespace LightningGauges
{
    internal class FontGraphic : IDisposable
    {
        private readonly Bitmap[] _charBitmaps = new Bitmap[256];
        private static readonly ThreadLocal<Brush> _charBrush = new ThreadLocal<Brush>(()=>Brushes.Black);
        private readonly Bitmap _fontBitmap;
        private bool _disposed;
        public FontGraphic(string fileName) { _fontBitmap = (Bitmap) Util.LoadBitmapFromFile(fileName); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Bitmap GetCharImage(byte someByte)
        {
            someByte -= 32;
            if (_charBitmaps[someByte] == null)
            {
                var glyphWidth = _fontBitmap.Width / 16;
                var glyphHeight = _fontBitmap.Height / 16;
                var thisCharBitmap = new Bitmap(glyphWidth, glyphHeight, PixelFormat.Format16bppRgb565);
                var leftX = someByte % 16 * glyphWidth;
                var topY = someByte / 16 * glyphHeight;
                var toCut = new Rectangle(new Point(leftX, topY), new Size(glyphWidth, glyphHeight));
                using (var g = Graphics.FromImage(thisCharBitmap))
                {
                    g.FillRectangle(_charBrush.Value, new Rectangle(0, 0, glyphWidth, glyphHeight));
                    g.DrawImageFast(_fontBitmap, new Rectangle(0, 0, glyphWidth, glyphHeight), toCut, GraphicsUnit.Pixel);
                }
                thisCharBitmap.MakeTransparent(Color.Black);
                _charBitmaps[someByte] = thisCharBitmap;
            }
            return _charBitmaps[someByte];
        }

        public Bitmap GetCharImage(char someChar)
        {
            var thisCharByte = Encoding.ASCII.GetBytes(new[] {someChar})[0];
            return GetCharImage(thisCharByte);
        }

        ~FontGraphic() { Dispose(false); }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Common.Util.DisposeObject(_fontBitmap);
                if (_charBitmaps != null) foreach (var t in _charBitmaps) Common.Util.DisposeObject(t);
            }

            _disposed = true;
        }
    }
}