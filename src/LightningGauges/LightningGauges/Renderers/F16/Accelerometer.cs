using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IAccelerometer : IInstrumentRenderer
    {
        Accelerometer.AccelerometerInstrumentState InstrumentState { get; set; }
        Accelerometer.AccelerometerOptions Options { get; set; }
    }

    public class Accelerometer : InstrumentRendererBase, IAccelerometer
    {
        private const string ACCELEROMETER_BACKGROUND_IMAGE_FILENAME = "accelerometerface.bmp";

        private const string ACCELEROMETER_NEEDLE_IMAGE_FILENAME = "accelerometerneed.bmp";
        private const string ACCELEROMETER_NEEDLE_MASK_FILENAME = "accelerometerneed_mask.bmp";
        private const string ACCELEROMETER_NEEDLE2_IMAGE_FILENAME = "accelerometerneed2.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static Bitmap _background;
        private static ImageMaskPair _needle;
        private static ImageMaskPair _needle2;
        private static readonly object _imagesLock = new object();

        public Accelerometer()
        {
            InstrumentState = new AccelerometerInstrumentState();
            Options = new AccelerometerOptions();
            LoadImageResources();
        }

        private static void LoadImageResources()
        {
            lock (_imagesLock)
            {
                if (_background == null) _background = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ACCELEROMETER_BACKGROUND_IMAGE_FILENAME);
                if (_needle == null)
                {
                    _needle = ImageMaskPair.CreateFromFiles(
                        IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ACCELEROMETER_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ACCELEROMETER_NEEDLE_MASK_FILENAME);
                    _needle.Use1BitAlpha = true;
                }
                if (_needle2 == null)
                {
                    _needle2 = ImageMaskPair.CreateFromFiles(
                        IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ACCELEROMETER_NEEDLE2_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ACCELEROMETER_NEEDLE_MASK_FILENAME);
                    _needle2.Use1BitAlpha = true;
                }
            }
        }

        public AccelerometerInstrumentState InstrumentState { get; set; }
        public AccelerometerOptions Options { get; set; }

        [Serializable]
        public class AccelerometerInstrumentState : InstrumentStateBase
        {
            private const float MIN_INDICATABLE_GS = -5.0f;
            private const float MAX_INDICATABLE_GS = 10.0f;
            private float _accelerationInGs = 1;


            public float AccelerationInGs
            {
                get => _accelerationInGs;
                set
                {
                    var acceleration = value;
                    if (float.IsNaN(acceleration) || float.IsInfinity(acceleration)) acceleration = 1;
                    if (acceleration < MIN_INDICATABLE_GS) acceleration = MIN_INDICATABLE_GS;
                    if (acceleration > MAX_INDICATABLE_GS) acceleration = MAX_INDICATABLE_GS;
                    if (acceleration > MaxGs) MaxGs = acceleration;
                    if (acceleration < MinGs) MinGs = acceleration;
                    _accelerationInGs = acceleration;
                }
            }


            public float MinGs { get; private set; } = 1;

            public float MaxGs { get; private set; } = 1;

            public void ResetMinAndMaxGs()
            {
                MinGs = 1;
                MaxGs = 1;
            }
        }

        public class AccelerometerOptions
        {
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Width;
                var height = _background.Height;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the min G needle
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                var angle = -43 + (InstrumentState.MinGs + 5) / 15.0f * 340;
                destinationGraphics.TranslateTransform(_background.Width / 2.0f, _background.Width / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-(float) _background.Width / 2.0f, -(float) _background.Width / 2.0f);
                destinationGraphics.DrawImageFast(_needle2.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the max G needle
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                angle = -43 + (InstrumentState.MaxGs + 5) / 15.0f * 340;
                destinationGraphics.TranslateTransform(_background.Width / 2.0f, _background.Width / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-(float) _background.Width / 2.0f, -(float) _background.Width / 2.0f);
                destinationGraphics.DrawImageFast(_needle2.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the actual G needle
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                angle = -43 + (InstrumentState.AccelerationInGs + 5) / 15.0f * 340;
                destinationGraphics.TranslateTransform(_background.Width / 2.0f, _background.Width / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-(float) _background.Width / 2.0f, -(float) _background.Width / 2.0f);
                destinationGraphics.DrawImageFast(_needle.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }

        ~Accelerometer() { Dispose(false); }
    }
}