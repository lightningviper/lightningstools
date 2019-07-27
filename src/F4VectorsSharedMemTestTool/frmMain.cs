using F4SharedMem.Headers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMSVectorsharedMemTestTool
{
    public partial class frmMain : Form
    {
        private Color _color = Color.Green;
        private Brush _brush = new SolidBrush(Color.Green);
        private Pen _pen = new Pen(Color.Green, width: 1);
        private Font _font = new Font(FontFamily.GenericSansSerif, 12);

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
                            if (command.StartsWith("F:"))
                            {
                                var args = command.Replace("F:", "").TrimEnd(';').Split(',');
                                try { SetFont(int.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("FX:"))
                            {
                                var args = command.Replace("FX:", "").TrimEnd(';').Split(',');
                                try { SetFontEx(int.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("P:"))
                            {
                                var args = command.Replace("P:", "").TrimEnd(';').Split(',');
                                try { Render2DPoint(float.Parse(args[0]), float.Parse(args[1]), g); } catch { };
                            }
                            else if (command.StartsWith("L:"))
                            {
                                var args = command.Replace("L:", "").TrimEnd(';').Split(',');
                                try { Render2DLine(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("T:"))
                            {
                                var args = command.Replace("T:", "").TrimEnd(';').Split(',');
                                try { Render2DTri(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]), g); } catch { };
                            }
                            else if (command.StartsWith("S:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("S:", "").TrimEnd(';').Split(',');
                                try { ScreenText(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), int.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("SR:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("SR:", "").TrimEnd(';').Split(',');
                                try { ScreenTextRotated(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("FG:"))
                            {
                                var args = command.Replace("FG:", "").TrimEnd(';').Split(',');
                                try { SetColor(uint.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("BG:"))
                            {
                                var args = command.Replace("BG:", "").TrimEnd(';').Split(',');
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
            for (var i = 0; i < line.Length; i++)
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
            return Color.FromArgb(alpha: (int)((packedABGR & 0xFF000000) >> 24), blue: (int)((packedABGR & 0xFF0000) >> 16), green: (int)((packedABGR & 0xFF00) >> 8), red: (int)(packedABGR & 0xFF)); ;
        }
        private void SetBackground(uint packedABGR, Graphics g)
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
            var curData = _smReader.GetCurrentData();
            var stringData = curData != null ? curData.StringData : null;
            var stringDataData = stringData != null ? stringData.data : null;
            var hudCommands = stringDataData !=null && stringDataData.Any(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForHUD) 
                                ? stringDataData.Where(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForHUD).First().value
                                : "";

            var rwrCommands = stringDataData != null && stringDataData.Any(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForRWR)
                                ? stringDataData.Where(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForRWR).First().value
                                : "";

            var hmsCommands = stringDataData != null && stringDataData.Any(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForHMS)
                                ? stringDataData.Where(sd => sd.strId == (uint)StringIdentifier.DrawingCommandsForHMS).First().value
                                : "";

            Process(hudCommands, "HUD", txtHUD, lblHUDDataSize, pbHUD, ref _HUDImage, out _HUDCommands);
            Draw(_HUDImage, _HUDCommands, pbHUD);
            Process(rwrCommands, "RWR", txtRWR, lblRWRDataSize, pbRWR, ref _RWRImage, out _RWRCommands);
            Draw(_RWRImage, _RWRCommands, pbRWR);
            Process(hmsCommands, "HMS", txtHMS, lblHMSDataSize, pbHMS, ref _HMSImage, out _HMSCommands);
            Draw(_HMSImage, _HMSCommands, pbHMS);
        }
        private void Process(string allCommands, string displayName, TextBox textBox, Label dataSizeLabel, PictureBox pictureBox, ref Image displayImage, out string displayCommands)
        {
            var start = allCommands.IndexOf($"START:{displayName}");
            var end = allCommands.IndexOf($"END:{displayName};") + 8;

            displayCommands = start >= 0 && end >= start ? allCommands.Substring(start, end - start) : "";
            if (textBox != null)
            {
                textBox.Text = displayCommands.Replace(";", ";\r\n");
                textBox.Update();
            }
            if (dataSizeLabel !=null) dataSizeLabel.Text = $"Data Size: {(int)(displayCommands.Length / 1024)} KB";
            if (!string.IsNullOrWhiteSpace(displayCommands))
            {
                var resStart = displayCommands.IndexOf("(") + 1;
                var resEnd = displayCommands.IndexOf(")");
                if (resStart > -1 && resEnd > resStart)
                {
                    var resX = int.Parse(displayCommands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                    var resY = int.Parse(displayCommands.Substring(resStart, resEnd - resStart).Split(',')[1]);
                    if (displayImage == null || displayImage.Width != resX || displayImage.Height != resY)
                    {
                        displayImage = new Bitmap(resX, resY);
                        if (pictureBox != null)
                        {
                            pictureBox.Image = displayImage;
                            pictureBox.Refresh();
                        }
                    }
                }
            }
        }
        private void Draw(Image target, string commands, PictureBox pictureBox)
        {
            if (target == null || string.IsNullOrWhiteSpace(commands) || pictureBox == null) return;
            using (var g = Graphics.FromImage(target))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawOnto(g, commands);
                pictureBox.Update();
                pictureBox.Refresh();
            }

        }

    }
}
