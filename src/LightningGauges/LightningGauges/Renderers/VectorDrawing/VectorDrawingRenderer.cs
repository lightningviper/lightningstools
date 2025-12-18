using Common.SimSupport;
using F4Utils.SimSupport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LightningGauges.Renderers.VectorDrawing
{
    public interface IVectorDrawingRenderer : IInstrumentRenderer
    {
        void Render(DrawingContext drawingContext);
        InstrumentState InstrumentState { get; set; }
        Size GetResolution();
    }
    public partial class VectorDrawingRenderer : InstrumentRendererBase, IVectorDrawingRenderer
    {
        private Color _foreColor = Colors.Green;
        private Color _backColor = Colors.Black;

        private Brush _brush = new SolidColorBrush(Colors.Green);
        private Pen _pen = new Pen(new SolidColorBrush(Colors.Green), thickness: 1);
        private int _offsetX = 0;
        private int _offsetY = 2;

        private string _currentFontFile;
        private HashSet<BmsFont> _bmsFonts = new HashSet<BmsFont>();

        public InstrumentState InstrumentState { get; set; }
        public virtual void Render(DrawingContext drawingContext) 
        {
            DrawCommands(drawingContext, InstrumentState.DrawingCommands);
        }
        public override void Render(Common.Drawing.Graphics destinationGraphics, Common.Drawing.Rectangle destinationRectangle)
        {
            var resolution = GetResolution();
            if ((int)resolution.Width == 0 || (int)resolution.Height == 0) return;
            DrawingVisual dv = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(dv, BitmapScalingMode.Fant);
            RenderOptions.SetClearTypeHint(dv, ClearTypeHint.Enabled);
            RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased);
            DrawingContext dc = dv.RenderOpen();
            Render(dc);
            dc.Close();
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)resolution.Width, (int)resolution.Height, 96, 96, PixelFormats.Default);
            rtb.Render(dv);
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using (Common.Drawing.Bitmap bmp = new Common.Drawing.Bitmap(ms))
                {
                    destinationGraphics.DrawImage(bmp, destinationRectangle, new Common.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), Common.Drawing.GraphicsUnit.Pixel);
                }
            }
        }

        private void DrawCommands(DrawingContext dcx, string commands)
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
                                try { SetFontFile(RemoveSurroundingQuotes(args[0])); } catch { };
                            }
                            else if (command.StartsWith("P:"))
                            {
                                var args = command.Replace("P:", "").TrimEnd(';').Split(',');
                                try { DrawPoint(ParseFloat(args[0]), ParseFloat(args[1]), dcx); } catch { };
                            }
                            else if (command.StartsWith("L:"))
                            {
                                var args = command.Replace("L:", "").TrimEnd(';').Split(',');
                                try { DrawLine(ParseFloat(args[0]), ParseFloat(args[1]), ParseFloat(args[2]), ParseFloat(args[3]), dcx); } catch { };
                            }
                            else if (command.StartsWith("T:"))
                            {
                                var args = command.Replace("T:", "").TrimEnd(';').Split(',');
                                try { DrawTri(ParseFloat(args[0]), ParseFloat(args[1]), ParseFloat(args[2]), ParseFloat(args[3]), ParseFloat(args[4]), ParseFloat(args[5]), dcx); } catch { };
                            }
                            else if (command.StartsWith("S:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("S:", "").TrimEnd(';').Split(',');
                                try { DrawString(ParseFloat(args[0]), ParseFloat(args[1]), RemoveSurroundingQuotes(args[2]), byte.Parse(args[3]), dcx); } catch { };
                            }
                            else if (command.StartsWith("SR:"))
                            {
                                var args = EscapeQuotedComma(command).Replace("SR:", "").TrimEnd(';').Split(',');
                                try { DrawStringRotated(ParseFloat(args[0]), ParseFloat(args[1]), RemoveSurroundingQuotes(args[2]), ParseFloat(args[3]), dcx); } catch { };
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
            return Color.FromArgb(a: (byte)((packedABGR & 0xFF000000) >> 24), b: (byte)((packedABGR & 0xFF0000) >> 16), g: (byte)((packedABGR & 0xFF00) >> 8), r: (byte)(packedABGR & 0xFF)); ;
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
            _brush = new SolidColorBrush(_foreColor);
            _pen = new Pen(_brush, thickness: 1);
        }

        private void SetFontFile(string fontFile)
        {
            if (string.IsNullOrWhiteSpace(fontFile) || string.IsNullOrWhiteSpace(InstrumentState.FontDirectory)) return;
            LoadBmsFont(fontFile);
            _currentFontFile = fontFile;
        }
        private void DrawPoint(float x1, float y1, DrawingContext dcx )
        {
            if (IsAnyOutOfRange(x1, y1)) return;
            dcx.DrawLine(_pen, new Point(x1, y1), new Point( x1, y1)); 
        }
        private void DrawLine(float x1, float y1, float x2, float y2, DrawingContext dcx)
        {
            if (IsAnyOutOfRange(x1, y1, x2, y2)) return;
            dcx.DrawLine(_pen, new Point(x1, y1), new Point(x2, y2));
        }
        
        private void DrawTri(float x1, float y1, float x2, float y2, float x3, float y3, DrawingContext dcx)
        {
            if (IsAnyOutOfRange(x1, y1, x2, y2, x3, y3)) return;
            TriangleFillHelper.DrawTriangleFilledByLines(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), (Point p1, Point p2) => { DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, dcx); }, yStep: 1.0);
            //var streamGeometry = new StreamGeometry();
            //using (var geometryContext = streamGeometry.Open())
            //{
            //    geometryContext.BeginFigure(new Point(x1,y1), true, true);
            //    var points = new PointCollection { new Point(x2, y2), new Point(x3, y3) };
            //    geometryContext.PolyLineTo(points, true, true);
            //}
            //dcx.DrawGeometry(_brush, _pen, streamGeometry);
        }
        
        private void DrawString(float xLeft, float yTop, string textString, byte invert, DrawingContext dcx)
        {
            if (IsAnyOutOfRange(xLeft, yTop)) return;
            if (xLeft < 0 || yTop < 0 || float.IsNaN(xLeft) || float.IsNaN(yTop)) return;

            var curX = xLeft;
            var curY = yTop;
            var font = _bmsFonts.Where(x => string.Equals(Path.GetFileName(x.TextureFile), _currentFontFile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (font == null) return;
            var originalForegroundColor = _foreColor;
            var originalBackgroundColor = _backColor;
            if (invert == 1) //invert text
            {
                //swap foreground and background colors
                SetForegroundColor(originalBackgroundColor);
                SetBackgroundColor(originalForegroundColor);
            }
            var unescapedString = UnescapeComma(textString);
            var textAsCharArray = unescapedString.ToCharArray();
            if (invert == 1) //draw inverted-color bounding rectangle 
            {
                var left = xLeft;
                var top = yTop;
                var width = 0f;
                var height = 0f;
                foreach (var character in textAsCharArray)
                {
                    var charMetric = font.FontMetrics.Where(x => x.idx == character).First();
                    width += charMetric.lead + charMetric.width + charMetric.trail;
                    height = Math.Max(height, charMetric.height);
                }
                var brush = new SolidColorBrush(originalForegroundColor);
                var pen = new System.Windows.Media.Pen(brush, 1);
                dcx.DrawRectangle(brush,pen, new System.Windows.Rect(xLeft - 1 + _offsetX, yTop - 1 + _offsetY, width + 2, height));
            }


            foreach (var character in textAsCharArray)
            {
                var charMetric = font.FontMetrics.Where(x => x.idx == character).First();
                curX += charMetric.lead;
                var pen = new Pen { Thickness = 1, Brush = _brush };
                VectorStrokeFont.DrawText(dcx, pen, character.ToString(), new Point(curX + _offsetX, curY + _offsetY), charMetric.width / VectorStrokeFont.MaxCharacterWidth(), charMetric.height / VectorStrokeFont.MaxCharacterHeight());
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
        private void DrawStringRotated(float xLeft, float yTop, string textString, float angle, DrawingContext dcx)
        {
            if (IsAnyOutOfRange(xLeft, yTop)) return;
            dcx.PushTransform(new TranslateTransform(xLeft, yTop));
            dcx.PushTransform(new RotateTransform((float)(angle * (180.0 / Math.PI))));
            dcx.PushTransform(new TranslateTransform(-xLeft, -yTop));
            DrawString(xLeft, yTop, textString, 0, dcx);
            dcx.Pop();
            dcx.Pop();
            dcx.Pop();
        }
        private bool IsAnyOutOfRange(params float[] parms)
        {
            foreach (var parm in parms)
            {
                if (parm < 0 || float.IsNaN(parm) || float.IsInfinity(parm)) return true;
            }
            return false;
        }
        public Size GetResolution()
        {
            string commands = InstrumentState?.DrawingCommands;
            if (!string.IsNullOrWhiteSpace(commands))
            {
                var resStart = commands.IndexOf("R:") + 2;
                var resEnd = resStart > 1 ? commands.IndexOf(";", resStart) : resStart;
                if (resStart > 1 && resEnd > resStart)
                {
                    var resX = int.Parse(commands.Substring(resStart, resEnd - resStart).Split(',')[0]);
                    var resY = int.Parse(commands.Substring(resStart, resEnd - resStart).Split(',')[1]);
                    return new Size(resX, resY);
                }
            }
            return new Size(0, 0);
        }

        private void LoadBmsFont(string fontFile)
        {
            var alreadyLoadedFont = _bmsFonts.Where(x => String.Equals(Path.GetFileName(x.TextureFile), fontFile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (alreadyLoadedFont != null) return;
            var fontFullPath = Path.Combine(InstrumentState.FontDirectory, fontFile);
            var rctPath = Path.Combine(InstrumentState.FontDirectory, Path.GetFileNameWithoutExtension(fontFile) + ".rct");
            var bmsFont = new BmsFont(fontFullPath, rctPath);
            _bmsFonts.Add(bmsFont);
        }

        private static readonly CultureInfo USCultureInfo = new CultureInfo("en-US");
        private float ParseFloat(string toParse)
        {
            float.TryParse(toParse, NumberStyles.Float, USCultureInfo, out float toReturn);
            return toReturn;
        }

    }
}
