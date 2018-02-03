using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;

namespace LightningGauges.Renderers.F16.ISIS
{
    internal static class PitchLadderRenderer
    {
        private static readonly ThreadLocal<StringFormat> PitchDigitFormat = new ThreadLocal<StringFormat>(()=>new StringFormat
        {
            Alignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        });

        private const int MinorLineWidth = 15;
        private const int PitchBarPenWidth = 2;
        private static readonly Color GroundColor = Color.FromArgb(111, 72, 31);
        private static readonly ThreadLocal<Brush> GroundBrush = new ThreadLocal<Brush>(()=>new SolidBrush(GroundColor));
        private static readonly Color SkyColor = Color.FromArgb(3, 174, 252);
        private static readonly ThreadLocal<Brush> SkyBrush = new ThreadLocal<Brush>(() => new SolidBrush(SkyColor));
        private static readonly Color PitchBarColor = Color.White;
        private static readonly Color PitchDigitColor = Color.White;
        private static readonly ThreadLocal<Brush> PitchDigitBrush = new ThreadLocal<Brush>(()=>new SolidBrush(PitchDigitColor));
        private static readonly ThreadLocal<Pen> PitchBarPen = new ThreadLocal<Pen>(()=>new Pen(PitchBarColor) {Width = PitchBarPenWidth});

        private static ThreadLocal<Font> _pitchDigitFont;

        internal static void DrawPitchLadder(Graphics g, RectangleF bounds, float pitchDegrees, PrivateFontCollection fonts)
        {
            if (_pitchDigitFont == null) _pitchDigitFont = new ThreadLocal<Font>(()=>new Font(fonts.Families[0], 12, FontStyle.Bold, GraphicsUnit.Point));

            var pitchLadderWidth = bounds.Width;
            var pitchLadderHeight = bounds.Height;
            var startingTransform = g.Transform;
            var startingClip = g.Clip;

            var pixelsPerDegree = pitchLadderHeight / (180.0f + 90);

            //draw the ground
            g.FillRectangle(GroundBrush.Value, new RectangleF(0, pitchLadderHeight / 2.0f, pitchLadderWidth, pitchLadderHeight / 2.0f));

            //draw the sky
            g.FillRectangle(SkyBrush.Value, new RectangleF(0, 0, pitchLadderWidth, pitchLadderHeight / 2.0f));

            //draw the horizon line
            g.DrawLineFast(PitchBarPen.Value, new PointF(0, pitchLadderHeight / 2.0f), new PointF(pitchLadderWidth, pitchLadderHeight / 2.0f));

            //draw zenith/nadir symbol
            const float zenithNadirSymbolWidth = MinorLineWidth;
            ZenithNadirSymbolRenderer.DrawZenithNadirSymbol(g, pitchLadderHeight, pixelsPerDegree, pitchLadderWidth, zenithNadirSymbolWidth, PitchBarPen.Value);
            ClimbPitchBarsRenderer.DrawClimbPitchBars(
                g, pitchDegrees, MinorLineWidth, pitchLadderWidth, pitchLadderHeight, pixelsPerDegree, PitchBarPen.Value, _pitchDigitFont.Value, PitchDigitBrush.Value, PitchDigitFormat.Value);

            g.Transform = startingTransform;
            g.Clip = startingClip;
        }
    }
}