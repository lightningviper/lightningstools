using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface ICabinPressureAltitudeIndicator : IInstrumentRenderer
    {
        CabinPressureAltitudeIndicator.CabinPressureAltitudeIndicatorInstrumentState InstrumentState { get; set; }
        CabinPressureAltitudeIndicator.CabinPressureAltitudeIndicatorOptions Options { get; set; }
    }

    public class CabinPressureAltitudeIndicator : InstrumentRendererBase, ICabinPressureAltitudeIndicator
    {
        private const string CABINPRESS_BACKGROUND_IMAGE_FILENAME = "cabinpress.bmp";

        private const string CABINPRESS_NEEDLE_IMAGE_FILENAME = "cabprneed.bmp";
        private const string CABINPRESS_NEEDLE_MASK_FILENAME = "cabprneed_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static Bitmap _background;
        private static ImageMaskPair _needle;
        private static readonly object _imagesLock = new object();
        private static bool _imagesLoaded;

        public CabinPressureAltitudeIndicator()
        {
            InstrumentState = new CabinPressureAltitudeIndicatorInstrumentState();
            Options = new CabinPressureAltitudeIndicatorOptions();
        }

        private static void LoadImageResources()
        {
            lock (_imagesLock)
            {
                if (_background == null) _background = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CABINPRESS_BACKGROUND_IMAGE_FILENAME);
                if (_needle == null)
                {
                    _needle = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                        IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CABINPRESS_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + CABINPRESS_NEEDLE_MASK_FILENAME);
                    _needle.Use1BitAlpha = true;
                }
            }
            _imagesLoaded = true;
        }

        public CabinPressureAltitudeIndicatorInstrumentState InstrumentState { get; set; }
        public CabinPressureAltitudeIndicatorOptions Options { get; set; }

        [Serializable]
        public class CabinPressureAltitudeIndicatorInstrumentState : InstrumentStateBase
        {
            private float _CabinPressureAltitudeFeet;

            public CabinPressureAltitudeIndicatorInstrumentState() { CabinPressureAltitudeFeet = 0; }


            public float CabinPressureAltitudeFeet
            {
                get => _CabinPressureAltitudeFeet;
                set
                {
                    var pressureAltitude = value;
                    if (float.IsNaN(pressureAltitude) || float.IsInfinity(pressureAltitude)) pressureAltitude = 0;
                    if (pressureAltitude < 0) pressureAltitude = 0;
                    if (pressureAltitude > 50000) pressureAltitude = 50000;
                    _CabinPressureAltitudeFeet = pressureAltitude;
                }
            }
        }

        public class CabinPressureAltitudeIndicatorOptions
        {
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Width - 12;
                var height = _background.Height - 12;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-6, -5);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the needle 
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                var angle = InstrumentState.CabinPressureAltitudeFeet / 50000.0f * 298.0f - 3;
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