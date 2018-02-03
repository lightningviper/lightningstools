using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class TextWarningFlagRenderer
    {
        private static ThreadLocal<Font> _inuFlagFont;

        internal static void DrawTextWarningFlag(Graphics g, string flagText, Color flagColor, Color textColor, PrivateFontCollection fonts)
        {
            if (_inuFlagFont == null)
            {
                var fontFamily = fonts.Families[0];
                _inuFlagFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 20, FontStyle.Bold, GraphicsUnit.Point));
            }
            using (var inuFlagStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            })
            using (var flagBrush = new SolidBrush(flagColor)) 
            {
                var flagLocation = new PointF(12, 75);
                var flagSize = new SizeF(25, 75);
                var flagRect = new RectangleF(flagLocation, flagSize);
                g.FillRectangle(flagBrush, flagRect);
                using (var textBrush = new SolidBrush(textColor)) { g.DrawStringFast(flagText, _inuFlagFont.Value, textBrush, flagRect, inuFlagStringFormat); }
            }
        }
    }
}