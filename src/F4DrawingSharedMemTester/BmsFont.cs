using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BMSDrawingSharedMemTester
{
    public class BmsFont
    {
        public BmsFont(string texturePath, string rctPath)
        {
            Load(texturePath, rctPath);
        }

        public string MetricsFile { get; private set; }
        public string TextureFile { get; private set; }
        public Image Texture { get; private set; }
        public IEnumerable<FontMetric> FontMetrics { get; private set; } = new List<FontMetric>();
        private void Load(string texturePath, string rctPath)
        {
            var sourceTexture = new Bitmap(Image.FromFile(texturePath));
            sourceTexture.MakeTransparent(Color.Black);
            Texture = sourceTexture;
            TextureFile = texturePath;
            using (var reader = new StreamReader(rctPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fontMetric = parseRctLine(line);
                    (FontMetrics as List<FontMetric>).Add(fontMetric);
                }
            }
            MetricsFile = rctPath;
        }
        private FontMetric parseRctLine(string rctLine )
        {
            var tokens = rctLine.Split(' ');
            var fontMetric = new FontMetric();
            fontMetric.idx = int.Parse(tokens[0]);
            fontMetric.left = int.Parse(tokens[1]);
            fontMetric.top = int.Parse(tokens[2]);
            fontMetric.width = int.Parse(tokens[3]);
            fontMetric.height = int.Parse(tokens[4]);
            fontMetric.lead = int.Parse(tokens[5]);
            fontMetric.trail = int.Parse(tokens[6]);
            return fontMetric;
        }
        public struct FontMetric
        {
            public int idx;
            public int left;
            public int top;
            public int width;
            public int height;
            public int lead;
            public int trail;
        }
    }
}
