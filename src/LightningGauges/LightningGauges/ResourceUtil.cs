using Common.Drawing;
using Common.Drawing.Imaging;
using Common.Imaging;
using System.IO;
using System.Reflection;

namespace LightningGauges
{
    internal static class ResourceUtil
    {
        public static Image LoadBitmapFromEmbeddedResource(string imagePath)
        {
            var resourceName = Assembly.GetExecutingAssembly().GetName().Name + "." +  imagePath.Replace("\\", ".");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ?? new MemoryStream())
            {
                var temp = Image.FromStream(stream);
                Common.Imaging.Util.ConvertPixelFormat(ref temp, Common.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                return temp;
            }
        }
        public static ImageMaskPair CreateImageMaskPairFromEmbeddedResources(string imagePath, string maskPath)
        {
            var image = LoadBitmapFromEmbeddedResource(imagePath);
            var mask = LoadBitmapFromEmbeddedResource(maskPath);
            Util.ConvertPixelFormat(ref image, PixelFormat.Format32bppArgb);
            Util.ConvertPixelFormat(ref mask, PixelFormat.Format32bppArgb);
            return new ImageMaskPair((Bitmap)image, (Bitmap)mask);
        }
    }
}
