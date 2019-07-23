using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BMSVectorsharedMemTestTool
{
    public partial class frmMain : Form
    {
        private Color _color = Color.Green;
        private Brush _brush = new SolidBrush(Color.Green);
        private Pen _pen = new Pen(Color.Green, width: 1);
        private static Font _smallFont = new Font(FontFamily.GenericSansSerif, 14);
        private static Font _bigFont = new Font(FontFamily.GenericSansSerif, 16);
        private static Font _warnFont = new Font(FontFamily.GenericSansSerif, 18);
        private Font _font = _smallFont;

        private Image _HUDImage;
        private Image _RWRImage;
        private Image _HMSImage;

        private string _HUDCommands;
        private string _RWRCommands;
        private string _HMSCommands;

        private F4SharedMem.Reader _smReader = new F4SharedMem.Reader();

        public frmMain()
        {
            InitializeComponent();
            pbHUD.Image = _HUDImage;
            pbRWR.Image = _RWRImage;
            pbHMS.Image = _HMSImage;

            pbHUD.Refresh();
            pbRWR.Refresh();
            pbHMS.Refresh();

            timer1.Start();
        }

        private void DrawOnto(Graphics g, string commands)
        {
            using (var sr = new StringReader(commands))
            {
                var cmdBuilder = new StringBuilder();
                int thisChar = 0;
                bool insideQuotes = false;
                var command = "";
                while ((thisChar = sr.Read()) > 0)
                {
                    cmdBuilder.Append((char)thisChar);
                    if ((char)thisChar == '"')
                    {
                        insideQuotes = !insideQuotes;
                    }
                    else if ((char)thisChar == ';' && !insideQuotes)
                    {
                        command = cmdBuilder.ToString();
                        cmdBuilder.Clear();
                        if (!command.TrimEnd().EndsWith(";")) continue;
                        try
                        {
                            if (command.StartsWith("FONT:"))
                            {
                                var args = command.Replace("FONT:", "").TrimEnd(';').Split(',');
                                try { SetFont(int.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("FONTEX:"))
                            {
                                var args = command.Replace("FONTEX:", "").TrimEnd(';').Split(',');
                                try { SetFontEx(int.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("POINT:"))
                            {
                                var args = command.Replace("POINT:", "").TrimEnd(';').Split(',');
                                try { Render2DPoint(float.Parse(args[0]), float.Parse(args[1]), g); } catch { };
                            }
                            else if (command.StartsWith("LINE:"))
                            {
                                var args = command.Replace("LINE:", "").TrimEnd(';').Split(',');
                                try { Render2DLine(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("TRI:"))
                            {
                                var args = command.Replace("TRI:", "").TrimEnd(';').Split(',');
                                try { Render2DTri(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]), g); } catch { };
                            }
                            else if (command.StartsWith("TEXT:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("TEXT:", "").TrimEnd(';').Split(',');
                                try { ScreenText(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), int.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("TEXTROTATED:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("TEXTROTATED:", "").TrimEnd(';').Split(',');
                                try { ScreenTextRotated(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("FORECOLOR:"))
                            {
                                var args = command.Replace("FORECOLOR:", "").TrimEnd(';').Split(',');
                                try { SetColor(uint.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("BACKCOLOR:"))
                            {
                                var args = command.Replace("BACKCOLOR:", "").TrimEnd(';').Split(',');
                                try { SetBackground(uint.Parse(args[0]), g); } catch { };
                            }
                        }
                        catch { }
                    }



                }
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
            _pen = new Pen(_color, width: 1);
            _brush = new SolidBrush(_color);
        }
        private void SetFont(int newFont)
        {
            
        }
        private void SetFontEx(int newFontEx)
        {

        }
        private void Render2DPoint(float x1, float y1, Graphics g)
        {
            g.DrawLine(_pen, x1, y1, x1, y1);
        }
        private void Render2DLine(float x1, float y1, float x2, float y2, Graphics g)
        {
            g.DrawLine(_pen, x1, y1, x2, y2);
        }
        private void Render2DTri(float x1, float y1, float x2, float y2, float x3, float y3, Graphics g)
        {
            g.FillPolygon(_brush, new[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) });
        }
        private void ScreenText(float xLeft, float yTop, string textString, int boxed, Graphics g)
        {
            g.DrawString(UnescapeComma(textString), _font, _brush, xLeft, yTop);
        }
        private void ScreenTextRotated(float xLeft, float yTop, string textString, float angle, Graphics g)
        {
            var origTransform = g.Transform;
            g.TranslateTransform(xLeft, yTop);
            g.RotateTransform((float)(angle * (180.0 / Math.PI)));
            g.TranslateTransform(-xLeft, -yTop);
            g.DrawString(UnescapeComma(textString), _font, _brush, xLeft, yTop);
            g.Transform = origTransform;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            var rawVectorData = _smReader.GetRawVectorsData();
            rawVectorData[0] = 0x32;
            rawVectorData[1] = 0x32;
            rawVectorData[2] = 0x32;
            rawVectorData[3] = 0x32;
            if (rawVectorData == null) return;
            var commands = Encoding.Default.GetString(rawVectorData);
            var hudStart = commands.IndexOf("START:HUD");
            var hudEnd = commands.IndexOf("END:HUD;") + 8;
            var rwrStart = commands.IndexOf("START:RWR");
            var rwrEnd = commands.IndexOf("END:RWR;") + 8;
            var hmsStart = commands.IndexOf("START:HMS");
            var hmsEnd = commands.IndexOf("END:HMS;") + 8;

            _HUDCommands = hudStart >=0 && hudEnd >= hudStart ? commands.Substring(hudStart, hudEnd - hudStart) : "";
            txtHUD.Text = _HUDCommands.Replace(";", ";\r\n");
            txtHUD.Update();
            lblHUDDataSize.Text = $"Data Size: {(int)(_HUDCommands.Length / 1024)} KB";
            if (!string.IsNullOrWhiteSpace(_HUDCommands))
            {
                var resStart = _HUDCommands.IndexOf("(")+1;
                var resEnd = _HUDCommands.IndexOf(")") ;
                var resX = int.Parse(_HUDCommands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                var resY = int.Parse(_HUDCommands.Substring(resStart, resEnd - resStart).Split(',')[1]);
                if (_HUDImage == null || _HUDImage.Width != resX || _HUDImage.Height != resY)
                {
                    _HUDImage = new Bitmap(resX, resY);
                    pbHUD.Image = _HUDImage;
                    pbHUD.Refresh();
                }
            }

            _RWRCommands = rwrStart >=0 && rwrEnd >= rwrStart ? commands.Substring(rwrStart, rwrEnd - rwrStart) : "";
            txtRWR.Text = _RWRCommands.Replace(";", ";\r\n");
            txtRWR.Update();
            lblRWRDataSize.Text = $"Data Size: {(int)(_RWRCommands.Length / 1024)} KB";
            if (!string.IsNullOrWhiteSpace(_RWRCommands))
            {
                var resStart = _RWRCommands.IndexOf("(") + 1;
                var resEnd = _RWRCommands.IndexOf(")") ;
                var resX = int.Parse(_RWRCommands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                var resY = int.Parse(_RWRCommands.Substring(resStart, resEnd - resStart).Split(',')[1]);
                if (_RWRImage == null || _RWRImage.Width != resX || _RWRImage.Height != resY)
                {
                    _RWRImage = new Bitmap(resX, resY);
                    pbRWR.Image = _RWRImage;
                    pbRWR.Refresh();
                }
            }

            _HMSCommands = hmsStart >= 0 && hmsEnd >= hmsStart ? commands.Substring(hmsStart, hmsEnd - hmsStart) : "";
            txtHMS.Text = _HMSCommands.Replace(";", ";\r\n");
            txtHMS.Update();
            lblHMSDataSize.Text = $"Data Size: {(int)(_HMSCommands.Length / 1024)} KB";
            if (!string.IsNullOrWhiteSpace(_HMSCommands))
            {
                var resStart = _HMSCommands.IndexOf("(") + 1;
                var resEnd = _HMSCommands.IndexOf(")") ;
                var resX = int.Parse(_HMSCommands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                var resY = int.Parse(_HMSCommands.Substring(resStart, resEnd - resStart).Split(',')[1]);
                if (_HMSImage == null || _HMSImage.Width != resX || _HMSImage.Height != resY)
                {
                    _HMSImage = new Bitmap(resX, resY);
                    pbHMS.Image = _HMSImage;
                    pbHMS.Refresh();
                }
            }

            using (var g = Graphics.FromImage(_HUDImage))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawOnto(g, _HUDCommands);
                pbHUD.Update();
                pbHUD.Refresh();
            }
            using (var g = Graphics.FromImage(_RWRImage))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawOnto(g, _RWRCommands);
                pbRWR.Update();
                pbRWR.Refresh();
            }
            using (var g = Graphics.FromImage(_HMSImage))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawOnto(g, _HMSCommands);
                pbHMS.Update();
                pbHMS.Refresh();
            }
        }
    }
}
