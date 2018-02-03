using System;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.EHSI
{
    internal static class InstrumentModeRenderer
    {
        private static ThreadLocal<Font> _labelFont;

        internal static void DrawInstrumentMode(Graphics g, RectangleF outerBounds, PrivateFontCollection fonts, InstrumentState instrumentState)
        {
            if (_labelFont == null)
            {
                var fontFamily = fonts.Families[0];
                _labelFont = new ThreadLocal<Font>(()=>new Font(fontFamily, 25, FontStyle.Bold, GraphicsUnit.Point));
            }
            var labelStringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };

            const float letterHeight = 20;
            const float margin = 8;
            const float labelWidth = 50;

            var howLongSinceInstrumentModeChanged = DateTime.UtcNow.Subtract(instrumentState.WhenInstrumentModeLastChanged);
            if (howLongSinceInstrumentModeChanged.TotalMilliseconds <= 2000)
            {
                var toDisplay = string.Empty;
                switch (instrumentState.InstrumentMode)
                {
                    case InstrumentModes.Unknown: break;
                    case InstrumentModes.PlsTacan:
                        toDisplay = "PLS/TACAN";
                        break;
                    case InstrumentModes.Tacan:
                        toDisplay = "TACAN";
                        break;
                    case InstrumentModes.Nav:
                        toDisplay = "NAV";
                        break;
                    case InstrumentModes.PlsNav:
                        toDisplay = "PLS/NAV";
                        break;
                }

                if (!instrumentState.ShowBrightnessLabel) CenterLabelRenderer.DrawCenterLabel(g, outerBounds, toDisplay, fonts);
            }

            //draw PLS label
            if (instrumentState.InstrumentMode == InstrumentModes.PlsNav || instrumentState.InstrumentMode == InstrumentModes.PlsTacan)
            {
                var plsLabelRect = new RectangleF(outerBounds.Width * 0.25f, outerBounds.Height - letterHeight - margin, labelWidth, letterHeight);
                g.DrawStringFast("PLS", _labelFont.Value, Brushes.White, plsLabelRect, labelStringFormat);
            }

            if (instrumentState.InstrumentMode == InstrumentModes.PlsNav || instrumentState.InstrumentMode == InstrumentModes.Nav)
            {
                var navLabelRect = new RectangleF(outerBounds.Width * 0.7f, outerBounds.Height - letterHeight - margin, labelWidth, letterHeight);
                g.DrawStringFast("NAV", _labelFont.Value, Brushes.White, navLabelRect, labelStringFormat);
            }

            if (instrumentState.InstrumentMode == InstrumentModes.PlsTacan || instrumentState.InstrumentMode == InstrumentModes.Tacan)
            {
                var tacanLabelRect = new RectangleF(outerBounds.Width * 0.7f, outerBounds.Height - letterHeight - margin, labelWidth, letterHeight);
                g.DrawStringFast("TCN", _labelFont.Value, Brushes.White, tacanLabelRect, labelStringFormat);
            }
        }
    }
}