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
            var tokens = rctLine.Split(new[] { ' ', '\t'}, System.StringSplitOptions.RemoveEmptyEntries);
            var fontMetric = new FontMetric();
            fontMetric.idx = tokens.Length > 0 ? int.Parse(tokens[0]) : 0;
            fontMetric.left = tokens.Length > 1 ? int.Parse(tokens[1]) : 0;
            fontMetric.top = tokens.Length > 2 ? int.Parse(tokens[2]) : 0;
            fontMetric.width = tokens.Length > 3 ? int.Parse(tokens[3]) : 0;
            fontMetric.height = tokens.Length > 4 ? int.Parse(tokens[4]) : 0;
            fontMetric.lead = tokens.Length > 5 ? int.Parse(tokens[5]) : 0;
            fontMetric.trail = tokens.Length > 6 ? int.Parse(tokens[6]) : 0;
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
