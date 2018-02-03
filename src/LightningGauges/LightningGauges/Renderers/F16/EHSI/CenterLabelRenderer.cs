using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class CenterLabelRenderer
    {
        private static ThreadLocal<Font> _labelFont;

        internal static void DrawCenterLabel(Graphics g, RectangleF outerBounds, string label, PrivateFontCollection fonts)
        {
            if (_labelFont == null)
            {
                var fontFamily = fonts.Families[0];
                _labelFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 25, FontStyle.Bold, GraphicsUnit.Point));
            }

            var labelStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var charSize = g.MeasureString("A", _labelFont.Value);
            var labelRect = new RectangleF(new PointF(0, (outerBounds.Y + outerBounds.Height - charSize.Height) / 2.0f), new SizeF(outerBounds.Width, charSize.Height));
            labelRect.Offset(0, -40);

            g.DrawStringFast(label, _labelFont.Value, Brushes.White, labelRect, labelStringFormat);
        }
    }
}