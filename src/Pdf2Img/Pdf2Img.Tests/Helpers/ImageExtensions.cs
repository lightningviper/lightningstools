using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Pdf2Img.Tests.Helpers
{
    internal static class ImageExtensions
    {
        public static byte[] ToByteArray(this Image image)
        {
            using (var bmp = new Bitmap(image))
            {
                var bounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
                byte[] bmpData;
                BitmapData bmpLock = null;
                try
                {
                    bmpLock = bmp.LockBits(bounds, ImageLockMode.ReadOnly, bmp.PixelFormat);
                    bmpData = new byte[bmpLock.Height*bmpLock.Stride];
                    Marshal.Copy(bmpLock.Scan0, bmpData, 0, bmpData.Length);
                }
                finally
                {
                    if (bmpLock != null)
                    {
                        bmp.UnlockBits(bmpLock);
                    }
                }
                return bmpData;
            }
        }
    }
}