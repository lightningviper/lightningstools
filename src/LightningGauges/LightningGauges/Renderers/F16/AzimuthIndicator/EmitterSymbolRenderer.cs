using System.Text;
using Common.Drawing;

namespace LightningGauges.Renderers.F16.AzimuthIndicator
{
    internal static class EmitterSymbolRenderer
    {
        internal static void DrawEmitterSymbol(int symbolId, Graphics g, RectangleF bounds, bool largeSize, bool primarySymbol, Color color, Font largeFont, Font smallFont, int width)
        {
            var originalTransform = g.Transform;
            var symbolFont = largeSize ? largeFont : smallFont;

            var x = bounds.X;
            var y = bounds.Y;

            var basicInterceptorPoints = GetBasicInterceptorPoints(bounds, x, y);
            var advancedInterceptorPoints = GetAdvancedInterceptorPoints(bounds, x, y);
            var airborneThreatSymbolPoints = GetAirborneThreatSymbolPoints(bounds, x, y);
            var shipSymbolPoints = GetShipSymbolPoints(bounds, width, x, y);

            bounds.Offset(0, 2);
            var sf = new StringFormat(StringFormatFlags.NoWrap) {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            const int lineSpacing = 4;
            using (Brush brush = new SolidBrush(color))
            using (var pen = new Pen(color))
            {
                switch (symbolId)
                {
                    case 0: break;
                    case 1:
                        StringRenderer.DrawString(g, "U", symbolFont, brush, bounds, sf);
                        break;
                    case 2:
                        g.DrawPolygon(pen, advancedInterceptorPoints);
                        break;
                    case 3:
                        g.DrawPolygon(pen, basicInterceptorPoints);
                        break;
                    case 4:
                        StringRenderer.DrawString(g, "M", symbolFont, brush, bounds, sf);
                        break;
                    case 5:
                        StringRenderer.DrawString(g, "H", symbolFont, brush, bounds, sf);
                        break;
                    case 6:
                        StringRenderer.DrawString(g, "P", symbolFont, brush, bounds, sf);
                        break;
                    case 7:
                        StringRenderer.DrawString(g, "2", symbolFont, brush, bounds, sf);
                        break;
                    case 8:
                        StringRenderer.DrawString(g, "3", symbolFont, brush, bounds, sf);
                        break;
                    case 9:
                        StringRenderer.DrawString(g, "4", symbolFont, brush, bounds, sf);
                        break;
                    case 10:
                        StringRenderer.DrawString(g, "5", symbolFont, brush, bounds, sf);
                        break;
                    case 11:
                        StringRenderer.DrawString(g, "6", symbolFont, brush, bounds, sf);
                        break;
                    case 12:
                        StringRenderer.DrawString(g, "8", symbolFont, brush, bounds, sf);
                        break;
                    case 13:
                        StringRenderer.DrawString(g, "9", symbolFont, brush, bounds, sf);
                        break;
                    case 14:
                        StringRenderer.DrawString(g, "10", symbolFont, brush, bounds, sf);
                        break;
                    case 15:
                        StringRenderer.DrawString(g, "13", symbolFont, brush, bounds, sf);
                        break;
                    case 16:
                        StringRenderer.DrawString(g, primarySymbol ? "A" : "S", symbolFont, brush, bounds, sf);
                        break;
                    case 17:
                        StringRenderer.DrawString(g, "S", symbolFont, brush, bounds, sf);
                        break;
                    case 18:
                        g.DrawPolygon(pen, shipSymbolPoints);
                        break;
                    case 19:
                        StringRenderer.DrawString(g, "C", symbolFont, brush, bounds, sf);
                        break;
                    case 20:
                        StringRenderer.DrawString(g, primarySymbol ? "15" : "M", symbolFont, brush, bounds, sf);
                        break;
                    case 21:
                        StringRenderer.DrawString(g, "N", symbolFont, brush, bounds, sf);
                        break;
                    case 22:
                        StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, ".", symbolFont, brush, bounds, sf);
                        break;
                    case 23:
                        StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, "..", symbolFont, brush, bounds, sf);
                        break;
                    case 24:
                        StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, "...", symbolFont, brush, bounds, sf);
                        break;
                    case 25:
                        StringRenderer.DrawString(g, "P", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, ".", symbolFont, brush, bounds, sf);
                        break;
                    case 26:
                        StringRenderer.DrawString(g, "P|", symbolFont, brush, bounds, sf);
                        break;
                    case 27:
                        StringRenderer.DrawString(g, "U", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, ".", symbolFont, brush, bounds, sf);
                        break;
                    case 28:
                        StringRenderer.DrawString(g, "U", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, "..", symbolFont, brush, bounds, sf);
                        break;
                    case 29:
                        StringRenderer.DrawString(g, "U", symbolFont, brush, bounds, sf);
                        bounds.Offset(0, lineSpacing);
                        StringRenderer.DrawString(g, "...", symbolFont, brush, bounds, sf);
                        break;
                    case 30:
                        StringRenderer.DrawString(g, "C", symbolFont, brush, bounds, sf);
                        break;
                    case 31:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "1", symbolFont, brush, bounds, sf);
                        break;
                    case 32:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "4", symbolFont, brush, bounds, sf);
                        break;
                    case 33:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "5", symbolFont, brush, bounds, sf);
                        break;
                    case 34:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "6", symbolFont, brush, bounds, sf);
                        break;
                    case 35:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "14", symbolFont, brush, bounds, sf);
                        break;
                    case 36:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "15", symbolFont, brush, bounds, sf);
                        break;
                    case 37:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "16", symbolFont, brush, bounds, sf);
                        break;
                    case 38:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "18", symbolFont, brush, bounds, sf);
                        break;
                    case 39:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "19", symbolFont, brush, bounds, sf);
                        break;
                    case 40:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "20", symbolFont, brush, bounds, sf);
                        break;
                    case 41:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "21", symbolFont, brush, bounds, sf);
                        break;
                    case 42:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "22", symbolFont, brush, bounds, sf);
                        break;
                    case 43:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "23", symbolFont, brush, bounds, sf);
                        break;
                    case 44:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "25", symbolFont, brush, bounds, sf);
                        break;
                    case 45:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "27", symbolFont, brush, bounds, sf);
                        break;
                    case 46:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "29", symbolFont, brush, bounds, sf);
                        break;
                    case 47:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "30", symbolFont, brush, bounds, sf);
                        break;
                    case 48:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "31", symbolFont, brush, bounds, sf);
                        break;
                    case 49:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "P", symbolFont, brush, bounds, sf);
                        break;
                    case 50:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "PD", symbolFont, brush, bounds, sf);
                        break;
                    case 51:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf);
                        break;
                    case 52:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "B", symbolFont, brush, bounds, sf);
                        break;
                    case 53:
                        g.DrawLines(pen, airborneThreatSymbolPoints);
                        StringRenderer.DrawString(g, "S", symbolFont, brush, bounds, sf);
                        break;
                    case 54:
                        StringRenderer.DrawString(g, " A|", symbolFont, brush, bounds, sf);
                        break;
                    case 55:
                        StringRenderer.DrawString(g, "|A|", symbolFont, brush, bounds, sf);
                        break;
                    case 56:
                        StringRenderer.DrawString(g, "|||", symbolFont, brush, bounds, sf);
                        StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf);
                        break;
                    case 57:
                        StringRenderer.DrawString(g, primarySymbol ? "F" : "S", symbolFont, brush, bounds, sf);
                        break;
                    case 58:
                        StringRenderer.DrawString(g, primarySymbol ? "F" : "A", symbolFont, brush, bounds, sf);
                        break;
                    case 59:
                        StringRenderer.DrawString(g, primarySymbol ? "F" : "M", symbolFont, brush, bounds, sf);
                        break;
                    case 60:
                        StringRenderer.DrawString(g, primarySymbol ? "F" : "U", symbolFont, brush, bounds, sf);
                        break;
                    case 61:
                        if (primarySymbol) { StringRenderer.DrawString(g, "F", symbolFont, brush, bounds, sf); }
                        else { g.DrawPolygon(pen, basicInterceptorPoints); }
                        break;
                    case 62:
                        if (primarySymbol) { StringRenderer.DrawString(g, "S", symbolFont, brush, bounds, sf); }
                        else { g.DrawPolygon(pen, basicInterceptorPoints); }
                        break;
                    case 63:
                        if (primarySymbol) { StringRenderer.DrawString(g, "A", symbolFont, brush, bounds, sf); }
                        else { g.DrawPolygon(pen, basicInterceptorPoints); }
                        break;
                    case 64:
                        if (primarySymbol) { StringRenderer.DrawString(g, "M", symbolFont, brush, bounds, sf); }
                        else { g.DrawPolygon(pen, basicInterceptorPoints); }
                        break;

                    default:
                        if (symbolId >= 100)
                        {
                            var symbolString = (symbolId - 100).ToString();
                            StringRenderer.DrawString(g, symbolString, symbolFont, brush, bounds, sf);
                        }
                        else if (symbolId >= 65)
                        {
                            var symbolString = Encoding.ASCII.GetString(new[] {(byte) symbolId});
                            StringRenderer.DrawString(g, symbolString, symbolFont, brush, bounds, sf);
                        }
                        else if (symbolId < 0) StringRenderer.DrawString(g, "U", symbolFont, brush, bounds, sf);
                        break;
                }
            }

            g.Transform = originalTransform;
        }

        private static PointF[] GetShipSymbolPoints(RectangleF bounds, int width, float x, float y)
        {
            var shipSymbolPoints = new[]
            {
                new PointF(x + 6.5f, -3 + y + width / 4.0f), new PointF(x + 6.5f, -3 + y + 1.5f + width / 4.0f), new PointF(x, -3 + y + 1.5f + width / 4.0f),
                new PointF(x + 3.5f, -3 + y + 4.5f + width / 4.0f), new PointF(x + 15, -3 + y + 4.5f + width / 4.0f), new PointF(x + 15, -3 + y + 1.5f + width / 4.0f),
                new PointF(x + 11, -3 + y + 1.5f + width / 4.0f), new PointF(x + 11, -3 + y + width / 4.0f)
            };
            for (var i = 0; i < shipSymbolPoints.Length; i++)
            {
                var p = shipSymbolPoints[i];
                var p2 = new PointF(p.X + bounds.Width / 4.0f, p.Y + bounds.Height / 4.0f);
                shipSymbolPoints[i] = p2;
            }

            return shipSymbolPoints;
        }

        private static PointF[] GetAirborneThreatSymbolPoints(RectangleF bounds, float x, float y)
        {
            var airborneThreatSymbolPoints = new[] {new PointF(x + 4, y), new PointF(x + 8, y - 4), new PointF(x + 12, y)};
            for (var i = 0; i < airborneThreatSymbolPoints.Length; i++)
            {
                var p = airborneThreatSymbolPoints[i];
                var p2 = new PointF(p.X + bounds.Width / 4.0f, p.Y + bounds.Height / 4.0f);
                airborneThreatSymbolPoints[i] = p2;
            }

            return airborneThreatSymbolPoints;
        }

        private static PointF[] GetAdvancedInterceptorPoints(RectangleF bounds, float x, float y)
        {
            var advancedInterceptorPoints = new[]
            {
                new PointF(x + 8, y + 2), //top center
                new PointF(x - 1, y + 8), //left bottom
                new PointF(x + 5, y + 8), //left of center on bottom
                new PointF(x + 8, y + 12), //center point on bottom
                new PointF(x + 11, y + 8), //right of center on bottom
                new PointF(x + 17, y + 8) //right bottom
            };
            for (var i = 0; i < advancedInterceptorPoints.Length; i++)
            {
                var p = advancedInterceptorPoints[i];
                var p2 = new PointF(p.X + bounds.Width / 4.0f, p.Y + bounds.Height / 4.0f);
                advancedInterceptorPoints[i] = p2;
            }

            return advancedInterceptorPoints;
        }

        private static PointF[] GetBasicInterceptorPoints(RectangleF bounds, float x, float y)
        {
            var basicInterceptorPoints = new[]
            {
                new PointF(x + 8, y + 2), //top center
                new PointF(x - 1, y + 8), //left bottom
                new PointF(x + 1, y + 8), //left of center on bottom
                new PointF(x + 8, y + 6), //center point on bottom
                new PointF(x + 15, y + 8), //right of center on bottom
                new PointF(x + 17, y + 8) //right bottom
            };
            for (var i = 0; i < basicInterceptorPoints.Length; i++)
            {
                var p = basicInterceptorPoints[i];
                var p2 = new PointF(p.X + bounds.Width / 4.0f, p.Y + bounds.Height / 4.0f);
                basicInterceptorPoints[i] = p2;
            }

            return basicInterceptorPoints;
        }
    }
}