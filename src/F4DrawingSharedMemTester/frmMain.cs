using F4SharedMem.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMSDrawingSharedMemTester
{
    public partial class frmMain : Form
    {
        private Color _foreColor = Color.Green;
        private Color _backColor = Color.Black;

        private Brush _brush = new SolidBrush(Color.Green);
        private Pen _pen = new Pen(Color.Green, width: 1);
        private ImageAttributes _imageAttrs;

        private string _fontTexture;
        private HashSet<BmsFont> _bmsFonts=new HashSet<BmsFont>();
        private Image _HUDImage;
        private Image _RWRImage;
        private Image _HMSImage;
        private string _fontDir;

        private string _HUDCommands;
        private string _RWRCommands;
        private string _HMSCommands;
        private Dictionary<string, string> _previousCommands = new Dictionary<string, string>();
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

        private void DrawCommands(Graphics g, string commands)
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
                                try { SetFont(RemoveSurroundingQuotes(args[0])); } catch { };
                            }
                            else if (command.StartsWith("P:"))
                            {
                                var args = command.Replace("P:", "").TrimEnd(';').Split(',');
                                try { DrawPoint(float.Parse(args[0]), float.Parse(args[1]), g); } catch { };
                            }
                            else if (command.StartsWith("L:"))
                            {
                                var args = command.Replace("L:", "").TrimEnd(';').Split(',');
                                try { DrawLine(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("T:"))
                            {
                                var args = command.Replace("T:", "").TrimEnd(';').Split(',');
                                try { DrawTri(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]), g); } catch { };
                            }
                            else if (command.StartsWith("S:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("S:", "").TrimEnd(';').Split(',');
                                try { DrawString(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), byte.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("SR:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("SR:", "").TrimEnd(';').Split(',');
                                try { DrawStringRotated(float.Parse(args[0]), float.Parse(args[1]), RemoveSurroundingQuotes(args[2]), float.Parse(args[3]), g); } catch { };
                            }
                            else if (command.StartsWith("FG:"))
                            {
                                var args = command.Replace("FG:", "").TrimEnd(';').Split(',');
                                try { SetForegroundColor(uint.Parse(args[0])); } catch { };
                            }
                            else if (command.StartsWith("BG:"))
                            {
                                var args = command.Replace("BG:", "").TrimEnd(';').Split(',');
                                try { SetBackgroundColor(uint.Parse(args[0])); } catch { };
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
        private void SetBackgroundColor(uint packedABGR)
        {
            var backgroundColor = ColorFromPackedABGR(packedABGR);
            SetBackgroundColor(backgroundColor);
        }
        private void SetBackgroundColor(Color backgroundColor)
        {
            _backColor = backgroundColor;
        }
        private void SetForegroundColor(uint packedABGR)
        {
            var foregroundColor = ColorFromPackedABGR(packedABGR);
            SetForegroundColor(foregroundColor);
        }

        private void SetForegroundColor(Color foregroundColor)
        {
            _foreColor = foregroundColor;
            _pen = new Pen(_foreColor, width: 1);
            _brush = new SolidBrush(_foreColor);
            _imageAttrs = new ImageAttributes();
            var colorMatrix = new ColorMatrix
            (
                new float[][]
                {
                        new float[] {_foreColor.R/255.0f, 0, 0, 0, 0}, //red %
                        new float[] { 0,_foreColor.G/255.0f, 0, 0, 0 }, //green
                        new float[] {0, 0, _foreColor.B/255.0f, 0, 0}, //blue %
                        new float[] {0, 0, 0, _foreColor.A/255.0f, 0}, //alpha %
                        new float[] {0, 0, 0, 0, 1} //add
                }
            );
            _imageAttrs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default);
        }

        private void SetFont(string fontFile)
        {
            LoadBmsFont(fontFile);
            _fontTexture = fontFile;
        }
        private void DrawPoint(float x1, float y1, Graphics g)
        {
            g.DrawLine(_pen, x1, y1, x1, y1);
        }
        private void DrawLine(float x1, float y1, float x2, float y2, Graphics g)
        {
            g.DrawLine(_pen, x1, y1, x2, y2);
        }
        private void DrawTri(float x1, float y1, float x2, float y2, float x3, float y3, Graphics g)
        {
            g.FillPolygon(_brush, new[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) });
        }
        private void DrawString(float xLeft, float yTop, string textString, byte invert, Graphics g)
        {
            if (xLeft < -10000 || yTop < -10000) return; //prevent overflow errors when exiting BMS flying
            var curX = xLeft;
            var curY = yTop;
            var font = _bmsFonts.Where(x=>string.Equals(Path.GetFileName(x.TextureFile), _fontTexture, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (font == null) return;
            var originalForegroundColor = _foreColor;
            var originalBackgroundColor = _backColor;
            if (invert == 1) //invert text
            {
                //swap foreground and background colors
                SetForegroundColor(originalBackgroundColor);
                SetBackgroundColor(originalForegroundColor);
            }
            foreach (var character in UnescapeComma(textString).ToCharArray())
            {
                var charMetric = font.FontMetrics.Where(x => x.idx == character).First();
                curX += charMetric.lead;
                var destRect = new Rectangle((int)curX, (int)curY, charMetric.width, charMetric.height);
                var srcRect = new RectangleF(charMetric.left, charMetric.top, charMetric.width, charMetric.height);
                if (invert == 1)
                {
                    g.FillRectangle(new SolidBrush(originalForegroundColor), destRect);
                }
                g.DrawImage(font.Texture, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, _imageAttrs);
                curX += charMetric.width;
                curX += charMetric.trail;
            }
            if (invert == 1) //invert text
            {
                //swap foreground and background colors back to originals
                SetForegroundColor(originalForegroundColor);
                SetBackgroundColor(originalBackgroundColor);
            }
        }
        private void DrawStringRotated(float xLeft, float yTop, string textString, float angle, Graphics g)
        {
            var origTransform = g.Transform;
            g.TranslateTransform(xLeft, yTop);
            g.RotateTransform((float)(angle * (180.0 / Math.PI)));
            g.TranslateTransform(-xLeft, -yTop);
            DrawString(xLeft, yTop, textString, 0, g);
            g.Transform = origTransform;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            var curData = _smReader.GetCurrentData();
            var drawingData = curData != null ? curData.DrawingData : null;
            var stringData = curData != null ? curData.StringData : null;
            var stringDataData = stringData !=null ? stringData.data : null;
            _HUDCommands= drawingData !=null && !(string.IsNullOrWhiteSpace(drawingData.HUD_commands))
                                ? drawingData.HUD_commands
                                : "";

           _RWRCommands = drawingData != null && !(string.IsNullOrWhiteSpace(drawingData.RWR_commands))
                                ? drawingData.RWR_commands
                                : "";

            _HMSCommands = drawingData != null && !(string.IsNullOrWhiteSpace(drawingData.HMS_commands))
                                ? drawingData.HMS_commands
                                : "";

            var cockpitArtDir = stringDataData != null && stringDataData.Any(sd => sd.strId == (uint)StringIdentifier.ThrCockpitdir)
                                ? stringDataData.Where(sd => sd.strId == (uint)StringIdentifier.ThrCockpitdir).First().value
                                : "";
            _fontDir = Path.Combine(cockpitArtDir, "3DFont");

            Process(_HUDCommands, "HUD", txtHUD, lblHUDDataSize, pbHUD, ref _HUDImage);
            Draw(_HUDImage, _HUDCommands, pbHUD);
            Process(_RWRCommands, "RWR", txtRWR, lblRWRDataSize, pbRWR, ref _RWRImage);
            Draw(_RWRImage, _RWRCommands, pbRWR);
            Process(_HMSCommands, "HMS", txtHMS, lblHMSDataSize, pbHMS, ref _HMSImage);
            Draw(_HMSImage, _HMSCommands, pbHMS);
        }

        private void Process(string commands, string displayName, TextBox textBox, Label dataSizeLabel, PictureBox pictureBox, ref Image displayImage)
        {
            if (textBox != null)
            {
                var toDisplay = commands.Replace(";", ";\r\n");
                var previousCommands = _previousCommands.ContainsKey(displayName) ? _previousCommands[displayName] : "";
                if (toDisplay != previousCommands)
                {
                    var newVal = commands != null ? toDisplay : "";
                    textBox.Text = newVal;
                    _previousCommands[displayName] = newVal;
                }
            }
            var dataSizeInKB = (commands ?? "").Length / 1024.0;
            if (dataSizeLabel !=null) dataSizeLabel.Text = $"Data Size: {dataSizeInKB.ToString("0.00")} KB";
            if (commands !=null)
            {
                var resStart = commands.IndexOf("R:") + 2;
                var resEnd = resStart > 1 ? commands.IndexOf(";", resStart) : resStart;
                if (resStart > 1 && resEnd > resStart)
                {
                    var resX = int.Parse(commands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                    var resY = int.Parse(commands.Substring(resStart, resEnd - resStart).Split(',')[1]);
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
            if (target == null || commands ==null || pictureBox == null) return;
            using (var g = Graphics.FromImage(target))
            {

                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawCommands(g, commands);
                pictureBox.Update();
                pictureBox.Refresh();
            }

        }
        private BmsFont LoadBmsFont(string fontFile)
        {
            var alreadyLoadedFont = _bmsFonts.Where(x => String.Equals(Path.GetFileName(x.TextureFile), fontFile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (alreadyLoadedFont != null) return alreadyLoadedFont;
            var fontFullPath = Path.Combine(_fontDir, fontFile);
            var rctPath = Path.Combine(_fontDir, Path.GetFileNameWithoutExtension(fontFile) + ".rct");
            var bmsFont = new BmsFont(fontFullPath, rctPath);
            _bmsFonts.Add(bmsFont);
            return bmsFont;
        }
    }
}
