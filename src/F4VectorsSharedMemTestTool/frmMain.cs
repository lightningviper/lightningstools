using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMSVectorsharedMemTestTool
{
    public partial class frmMain : Form
    {
        private Color _color = Color.Green;
        private static Font _smallFont = new Font(FontFamily.GenericSansSerif, 14);
        private static Font _bigFont = new Font(FontFamily.GenericSansSerif, 16);
        private static Font _warnFont = new Font(FontFamily.GenericSansSerif, 18);
        private Font _font = _smallFont;
        private Image _image;
        private string _commands;
        private F4SharedMem.Reader _smReader = new F4SharedMem.Reader();
        public frmMain()
        {
            InitializeComponent();
            _image = new Bitmap(600, 600);
            pictureBox1.Image = _image;
            pictureBox1.Refresh();
            timer1.Start();
        }

        private void DrawOnto(Graphics g)
        {
            int lineCount = 0;
            foreach (var command in _commands.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                lineCount++;
                if (!command.TrimEnd().EndsWith(")")) continue;
                try
                {


                    if (command.StartsWith("SetFontEx"))
                    {
                        var keyValuePairs = Parse(command.Replace("SetFontEx(", "").Replace(")", ""));
                        try { SetFontEx(int.Parse(keyValuePairs["newFont"])); } catch { };
                    }
                    else if (command.StartsWith("SetFont"))
                    {
                        var keyValuePairs = Parse(command.Replace("SetFont(", "").Replace(")", ""));
                        try { SetFont(int.Parse(keyValuePairs["newFont"])); } catch { };
                    }
                    else if (command.StartsWith("Render2DPoint"))
                    {
                        var keyValuePairs = Parse(command.Replace("Render2DPoint(", "").Replace(")", ""));
                        try { Render2DPoint(float.Parse(keyValuePairs["x1"]), float.Parse(keyValuePairs["y1"]), g); } catch { };
                    }
                    else if (command.StartsWith("Render2DLine"))
                    {
                        var keyValuePairs = Parse(command.Replace("Render2DLine(", "").Replace(")", ""));
                        try { Render2DLine(float.Parse(keyValuePairs["x1"]), float.Parse(keyValuePairs["y1"]), float.Parse(keyValuePairs["x2"]), float.Parse(keyValuePairs["y2"]), g); } catch { };
                    }
                    else if (command.StartsWith("Render2DTri"))
                    {
                        var keyValuePairs = Parse(command.Replace("Render2DTri(", "").Replace(")", ""));
                        try { Render2DTri(float.Parse(keyValuePairs["x1"]), float.Parse(keyValuePairs["y1"]), float.Parse(keyValuePairs["x2"]), float.Parse(keyValuePairs["y2"]), float.Parse(keyValuePairs["x3"]), float.Parse(keyValuePairs["y3"]), g); } catch { };
                    }
                    else if (command.StartsWith("ScreenTextRotated"))
                    {
                        var keyValuePairs = Parse(EscapeQuotedComma(command).Replace("ScreenTextRotated(", "").Replace(")", ""));
                        try { ScreenTextRotated(float.Parse(keyValuePairs["xLeft"]), float.Parse(keyValuePairs["yTop"]), RemoveSurroundingQuotes(keyValuePairs["string"]), float.Parse(keyValuePairs["angle"]), g); } catch { };
                    }
                    else if (command.StartsWith("ScreenText"))
                    {
                        var keyValuePairs = Parse(EscapeQuotedComma(command).Replace("ScreenText(", "").Replace(")", ""));
                        try { ScreenText(float.Parse(keyValuePairs["xLeft"]), float.Parse(keyValuePairs["yTop"]), RemoveSurroundingQuotes(keyValuePairs["string"]), int.Parse(keyValuePairs["boxed"]), g); } catch { };
                    }
                    else if (command.StartsWith("SetBackground"))
                    {
                        var keyValuePairs = Parse(command.Replace("SetBackground(", "").Replace(")", ""));
                        try { SetBackground(uint.Parse(keyValuePairs["packedABGR"]), g); } catch { };
                    }
                    else if (command.StartsWith("SetColor"))
                    {
                        var keyValuePairs = Parse(command.Replace("SetColor(", "").Replace(")", ""));
                        try { SetColor(uint.Parse(keyValuePairs["packedABGR"])); } catch { };
                    }
                }
                catch { }
            }
        }
        private string RemoveSurroundingQuotes(string text)
        {
            var toReturn = text;
            if (toReturn.StartsWith("\""))
            {
                toReturn = toReturn.Substring(1, toReturn.Length - 1);
            }
            if (toReturn.EndsWith("\""))
            {
                toReturn = toReturn.Substring(0, toReturn.Length - 1);
            }
            return toReturn;
        }
        private string UnescapeComma(string line)
        {
            return line.Replace("&comma;", ",");
        }
        private string EscapeQuotedComma(string line)
        {
            bool quoteOpen = false;
            var toReturn = new StringBuilder();
            for (var i=0;i<line.Length;i++)
            {
               
                if (line[i] == '"')
                {
                    quoteOpen = !quoteOpen;
                    toReturn.Append(line[i]);
                }
                else if (line[i] == ',' && quoteOpen)
                {
                    toReturn.Append("&comma;");
                }
                else
                {
                    toReturn.Append(line[i]);
                }
            }
            return toReturn.ToString();
        }
        private IDictionary<string,string> Parse(string line)
        {
            return line
                      .Split(',')
                      .Select(value => value.Split('='))
                      .ToDictionary(pair => pair[0], pair => pair[1]);

        }
        private Color ColorFromPackedABGR(uint packedABGR)
        {
            return Color.FromArgb(alpha: (int)((packedABGR & 0xFF000000) >> 24), blue: (int)((packedABGR & 0xFF0000)>>16), green: (int)((packedABGR & 0xFF00)>>8), red: (int)(packedABGR & 0xFF)); ;
        }
        private void SetBackground (uint packedABGR, Graphics g)
        {
            g.Clear(ColorFromPackedABGR(packedABGR));
        }
        private void SetColor(uint packedABGR)
        {
            _color = ColorFromPackedABGR(packedABGR);
        }
        private void SetFont(int newFont)
        {
            
        }
        private void SetFontEx(int newFontEx)
        {

        }
        private void Render2DPoint(float x1, float y1, Graphics g)
        {
            using (var pen = new Pen(_color, width:1))
            {
                g.DrawLine(pen, x1, y1, x1, y1);
            }
        }
        private void Render2DLine(float x1, float y1, float x2, float y2, Graphics g)
        {
            using (var pen = new Pen(_color, width: 1))
            {
                g.DrawLine(pen, x1, y1, x2, y2);
            }

        }
        private void Render2DTri(float x1, float y1, float x2, float y2, float x3, float y3, Graphics g)
        {
            using (var brush = new SolidBrush (_color))
            {
                g.FillPolygon(brush, new[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) });
            }
        }
        private void ScreenText(float xLeft, float yTop, string textString, int boxed, Graphics g)
        {
            using (var pen = new Pen(_color, width: 1))
            using (var brush = new SolidBrush(_color))
            {
                g.DrawString(UnescapeComma(textString), _font, brush, xLeft, yTop);
            }

        }
        private void ScreenTextRotated(float xLeft, float yTop, string textString, float angle, Graphics g)
        {
            using (var pen = new Pen(_color, width: 1))
            using (var brush = new SolidBrush(_color))
            {
                var origTransform = g.Transform;
                g.TranslateTransform(xLeft, yTop);
                g.RotateTransform((float)(angle * (180.0 / Math.PI)));
                g.TranslateTransform(-xLeft, -yTop);
                g.DrawString(UnescapeComma(textString), _font, brush, xLeft, yTop);
                g.Transform = origTransform;
            }

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            var VectorsData = _smReader.GetRawVectorsData();
            if (VectorsData == null) return;
            _commands = Encoding.Default.GetString(VectorsData);
            textBox1.Text = _commands;
            using (var g = Graphics.FromImage(_image))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawOnto(g);
                pictureBox1.Refresh();
            }
        }
    }
}
