using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IEPUFuelGauge : IInstrumentRenderer
    {
        EPUFuelGauge.EPUFuelGaugeInstrumentState InstrumentState { get; set; }
    }

    public class EPUFuelGauge : InstrumentRendererBase, IEPUFuelGauge
    {
        private const string EPU_BACKGROUND_IMAGE_FILENAME = "epu.bmp";

        private const string EPU_NEEDLE_IMAGE_FILENAME = "arrow_rpm.bmp";
        private const string EPU_NEEDLE_MASK_FILENAME = "arrow_rpmmask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static Bitmap _background;
        private static ImageMaskPair _needle;
        private static bool _imagesLoaded;

        public EPUFuelGauge() { InstrumentState = new EPUFuelGaugeInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null) _background = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + EPU_BACKGROUND_IMAGE_FILENAME);
            if (_needle == null)
            {
                _needle = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + EPU_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + EPU_NEEDLE_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public EPUFuelGaugeInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class EPUFuelGaugeInstrumentState : InstrumentStateBase
        {
            private float _fuelRemainingPercent;


            public float FuelRemainingPercent
            {
                get => _fuelRemainingPercent;
                set
                {
                    var percent = value;
                    if (float.IsNaN(percent) || float.IsNegativeInfinity(percent)) percent = 0;
                    if (percent < 0) percent = 0;
                    if (percent > 100) percent = 100;
                    _fuelRemainingPercent = percent;
                }
            }
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
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the needle 
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                var angle = -147f + 2.96f * InstrumentState.FuelRemainingPercent;

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