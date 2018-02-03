using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IOilPressureGauge : IInstrumentRenderer
    {
        OilPressureGauge.OilPressureGaugeInstrumentState InstrumentState { get; set; }
        OilPressureGauge.OilPressureGaugeOptions Options { get; set; }
    }

    public class OilPressureGauge : InstrumentRendererBase, IOilPressureGauge
    {
        private const string OIL_BACKGROUND_IMAGE_FILENAME = "oil.bmp";
        private const string OIL_BACKGROUND2_IMAGE_FILENAME = "oil2.bmp";

        private const string OIL_NEEDLE_IMAGE_FILENAME = "arrow_rpm.bmp";
        private const string OIL_NEEDLE_MASK_FILENAME = "arrow_rpmmask.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static Bitmap _background;
        private static Bitmap _background2;
        private static ImageMaskPair _needle;
        private static readonly object _imagesLock = new object();
        private static bool _imagesLoaded;

        public OilPressureGauge()
        {
            InstrumentState = new OilPressureGaugeInstrumentState();
            Options = new OilPressureGaugeOptions();
        }

        private static void LoadImageResources()
        {
            lock (_imagesLock)
            {
                if (_background == null)
                {
                    _background = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + OIL_BACKGROUND_IMAGE_FILENAME);
                    _background.MakeTransparent(Color.Black);
                }
                if (_background2 == null)
                {
                    _background2 = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + OIL_BACKGROUND2_IMAGE_FILENAME);
                    _background.MakeTransparent(Color.Black);
                }
                if (_needle == null)
                {
                    _needle = ImageMaskPair.CreateFromFiles(
                        IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + OIL_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + OIL_NEEDLE_MASK_FILENAME);
                    _needle.Use1BitAlpha = true;
                }
            }
            _imagesLoaded = true;
        }

        public OilPressureGaugeInstrumentState InstrumentState { get; set; }
        public OilPressureGaugeOptions Options { get; set; }

        [Serializable]
        public class OilPressureGaugeInstrumentState : InstrumentStateBase
        {
            private float _oilPressurePercent;

            public OilPressureGaugeInstrumentState() { OilPressurePercent = 0; }


            public float OilPressurePercent
            {
                get => _oilPressurePercent;
                set
                {
                    var pct = value;
                    if (float.IsInfinity(pct) || float.IsNaN(pct)) pct = 0;
                    if (pct < 0) pct = 0;
                    if (pct > 100) pct = 100;
                    _oilPressurePercent = pct;
                }
            }
        }

        public class OilPressureGaugeOptions
        {
            public OilPressureGaugeOptions() { IsSecondary = false; }

            public bool IsSecondary { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                const int width = 178;
                const int height = 178;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-39, -39);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                if (Options.IsSecondary)
                {
                    destinationGraphics.DrawImageFast(
                        _background2, new Rectangle(0, 0, _background2.Width, _background2.Height), new Rectangle(0, 0, _background2.Width, _background2.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    destinationGraphics.DrawImageFast(
                        _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                }
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the needle 
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                float angle;
                if (InstrumentState.OilPressurePercent < 50.0f) { angle = -86 + 3.23f * (InstrumentState.OilPressurePercent - 50.0f); }
                else { angle = -86 + 3.16f * (InstrumentState.OilPressurePercent - 50.0f); }

                destinationGraphics.TranslateTransform(_background.Width / 2.0f, _background.Width / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-(float) _background.Width / 2.0f, -(float) _background.Width / 2.0f);
                destinationGraphics.DrawImageFast(_needle.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}