using System;
using System.IO;
using System.Threading;
using Common.Drawing;
using Common.Drawing.Text;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface ICMDSPanel : IInstrumentRenderer
    {
        CMDSPanel.CMDSPanelInstrumentState InstrumentState { get; set; }
    }

    public class CMDSPanel : InstrumentRendererBase, ICMDSPanel
    {
        private const string CMDS_BACKGROUND_IMAGE_FILENAME = "cmds.bmp";
        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static Bitmap _background;
        private static bool _imagesLoaded;
        private static readonly Color _digitColor = Color.FromArgb(255, 63, 250, 63);
        private static readonly ThreadLocal<Brush> _digitBrush= new ThreadLocal<Brush>(() => new SolidBrush(_digitColor));
        private static readonly ThreadLocal<Font> _digitFont = new ThreadLocal<Font>(() => new Font(_pfc.Value.Families[0], 12, FontStyle.Regular, GraphicsUnit.Point));
        private static readonly ThreadLocal<Font> _legendFont = new ThreadLocal<Font>(() => new Font("Lucida Console", 10, FontStyle.Bold, GraphicsUnit.Point));
        private static readonly ThreadLocal<PrivateFontCollection> _pfc = new ThreadLocal<PrivateFontCollection>(
            () =>
            {
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile("lcddot.ttf");
                return pfc;
            });
        private static readonly ThreadLocal<StringFormat> _digitStringFormat = new ThreadLocal<StringFormat>(() => new StringFormat
        {
            Alignment = StringAlignment.Far,
            FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        });

        public CMDSPanel()
        {
            InstrumentState = new CMDSPanelInstrumentState();
        }

        private static void LoadImageResources()
        {
            if (_background == null) _background = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CMDS_BACKGROUND_IMAGE_FILENAME);
            _imagesLoaded = true;
        }

        public CMDSPanelInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class CMDSPanelInstrumentState : InstrumentStateBase
        {
            private const int MAX_CHAFF = 99;
            private const int MAX_FLARE = 99;
            private const int MAX_OTHER1 = 99;
            private const int MAX_OTHER2 = 99;

            private int _chaffCount;
            private int _flareCount;
            private int _other1Count;
            private int _other2Count;


            public int ChaffCount
            {
                get => _chaffCount;
                set
                {
                    var chaff = value;
                    if (chaff < 0) chaff = 0;
                    if (chaff > MAX_CHAFF) chaff = MAX_CHAFF;
                    _chaffCount = chaff;
                }
            }


            public int FlareCount
            {
                get => _flareCount;
                set
                {
                    var flare = value;
                    if (flare < 0) flare = 0;
                    if (flare > MAX_FLARE) flare = MAX_FLARE;
                    _flareCount = flare;
                }
            }


            public int Other1Count
            {
                get => _other1Count;
                set
                {
                    var other1 = value;
                    if (other1 < 0) other1 = 0;
                    if (other1 > MAX_OTHER1) other1 = MAX_OTHER1;
                    _other1Count = other1;
                }
            }


            public int Other2Count
            {
                get => _other2Count;
                set
                {
                    var other2 = value;
                    if (other2 < 0) other2 = 0;
                    if (other2 > MAX_OTHER2) other2 = MAX_OTHER2;
                    _other2Count = other2;
                }
            }


            public bool ChaffLow { get; set; }
            public bool FlareLow { get; set; }
            public bool Other1Low { get; set; }
            public bool Other2Low { get; set; }
            public bool Degraded { get; set; }
            public bool Go { get; set; }
            public bool NoGo { get; set; }
            public bool DispenseReady { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                const int width = 336;
                const int height = 88;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-90, -29);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                float translateX = 162;
                const float translateY = 72;

                const float digitBoxWidth = 57;
                const float digitBoxHeight = 35;
                //draw the Other1 count
                {
                    var countString = $"{InstrumentState.Other1Count:00}";
                    if (InstrumentState.Other1Low) countString = "Lo" + countString;
                    if (InstrumentState.Degraded) countString = "PGR";
                    var countLocation = new RectangleF(translateX, translateY, digitBoxWidth, digitBoxHeight);
                    destinationGraphics.DrawStringFast(countString, _digitFont.Value, _digitBrush.Value, countLocation, _digitStringFormat.Value);
                }

                //draw the Other2 count
                {
                    translateX += digitBoxWidth;
                    var countString = $"{InstrumentState.Other2Count:00}";
                    if (InstrumentState.Other2Low) countString = "Lo" + countString;
                    if (InstrumentState.Degraded) countString = "FAIL";
                    var countLocation = new RectangleF(translateX, translateY, digitBoxWidth, digitBoxHeight);
                    destinationGraphics.DrawStringFast(countString, _digitFont.Value, _digitBrush.Value, countLocation, _digitStringFormat.Value);
                }

                //draw the Chaff count
                {
                    translateX += digitBoxWidth;
                    var countString = $"{InstrumentState.ChaffCount:00}";
                    if (InstrumentState.ChaffLow) countString = "Lo" + countString;
                    if (InstrumentState.Degraded) countString = "GO";
                    var countLocation = new RectangleF(translateX, translateY, digitBoxWidth, digitBoxHeight);
                    destinationGraphics.DrawStringFast(countString, _digitFont.Value, _digitBrush.Value, countLocation, _digitStringFormat.Value);
                }

                //draw the Flare count
                {
                    translateX += digitBoxWidth;
                    var countString = $"{InstrumentState.FlareCount:00}";
                    if (InstrumentState.FlareLow) countString = "Lo" + countString;
                    if (InstrumentState.Degraded) countString = "BYP";
                    var countLocation = new RectangleF(translateX, translateY, digitBoxWidth, digitBoxHeight);
                    destinationGraphics.DrawStringFast(countString, _digitFont.Value, _digitBrush.Value, countLocation, _digitStringFormat.Value);
                }

                RectangleF dispenseReadyRectangle = new Rectangle(283, 36, 115, 21);
                destinationGraphics.FillRectangle(Brushes.Black, dispenseReadyRectangle);
                if (InstrumentState.DispenseReady) destinationGraphics.DrawStringFast("DISPENSE RDY", _legendFont.Value, _digitBrush.Value, dispenseReadyRectangle, _digitStringFormat.Value);

                RectangleF goRectangle = new Rectangle(220, 37, 25, 18);
                destinationGraphics.FillRectangle(Brushes.Black, goRectangle);
                if (InstrumentState.Go) destinationGraphics.DrawStringFast("GO", _legendFont.Value, _digitBrush.Value, goRectangle, _digitStringFormat.Value);

                RectangleF noGoRectangle = new Rectangle(166, 36, 45, 21);
                destinationGraphics.FillRectangle(Brushes.Black, noGoRectangle);
                if (InstrumentState.NoGo) destinationGraphics.DrawStringFast("NO GO", _legendFont.Value, _digitBrush.Value, noGoRectangle, _digitStringFormat.Value);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}