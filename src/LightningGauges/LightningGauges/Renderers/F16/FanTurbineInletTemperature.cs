using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IFanTurbineInletTemperature : IInstrumentRenderer
    {
        FanTurbineInletTemperature.FanTurbineInletTemperatureInstrumentState InstrumentState { get; set; }
        FanTurbineInletTemperature.FanTurbineInletTemperatureOptions Options { get; set; }
    }

    public class FanTurbineInletTemperature : InstrumentRendererBase, IFanTurbineInletTemperature
    {
        private const string FTIT_BACKGROUND_IMAGE_FILENAME = "ftit.bmp";
        private const string FTIT_BACKGROUND2_IMAGE_FILENAME = "ftit2.bmp";

        private const string FTIT_NEEDLE_IMAGE_FILENAME = "arrow_rpm.bmp";
        private const string FTIT_NEEDLE_MASK_FILENAME = "arrow_rpmmask.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static Bitmap _background;
        private static Bitmap _background2;
        private static ImageMaskPair _needle;
        private static bool _imagesLoaded;
        private static readonly object _imagesLock = new object();

        public FanTurbineInletTemperature()
        {
            Options = new FanTurbineInletTemperatureOptions();
            InstrumentState = new FanTurbineInletTemperatureInstrumentState();
        }

        private static void LoadImageResources()
        {
            lock (_imagesLock)
            {
                if (_background == null) _background = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + FTIT_BACKGROUND_IMAGE_FILENAME);
                if (_background2 == null) _background2 = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + FTIT_BACKGROUND2_IMAGE_FILENAME);
                if (_needle == null)
                {
                    _needle = ImageMaskPair.CreateFromFiles(
                        IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + FTIT_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + FTIT_NEEDLE_MASK_FILENAME);
                }
            }
            _imagesLoaded = true;
        }

        public FanTurbineInletTemperatureInstrumentState InstrumentState { get; set; }
        public FanTurbineInletTemperatureOptions Options { get; set; }

        [Serializable]
        public class FanTurbineInletTemperatureInstrumentState : InstrumentStateBase
        {
            private const float MIN_TEMP_CELCIUS = 0.0f;
            private float _inletTemperatureDegreesCelcius = 1200.0f;


            public float InletTemperatureDegreesCelcius
            {
                get => _inletTemperatureDegreesCelcius;
                set
                {
                    var temp = value;
                    if (float.IsNaN(temp) || float.IsInfinity(temp)) temp = 0;
                    if (temp < MIN_TEMP_CELCIUS) temp = MIN_TEMP_CELCIUS;
                    _inletTemperatureDegreesCelcius = temp;
                }
            }
        }

        public class FanTurbineInletTemperatureOptions
        {
            public FanTurbineInletTemperatureOptions() { IsSecondary = false; }

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
                var angle = 116f + GetAngle(InstrumentState.InletTemperatureDegreesCelcius);

                destinationGraphics.TranslateTransform(_background.Width / 2.0f, _background.Width / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-(float) _background.Width / 2.0f, -(float) _background.Width / 2.0f);
                destinationGraphics.DrawImageFast(_needle.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }

        private static float GetAngle(float inletTemperature)
        {
            float angle = 0;
            if (inletTemperature <= 200.0f) { angle = 0; }
            else if (inletTemperature > 200.0f && inletTemperature <= 700.0f)
            {
                angle = (inletTemperature - 200.0f) / 50.0f * 10.5f;
                //10.5 degrees of space for 50 degrees Celcius of readout
            }
            else if (inletTemperature > 700.0f && inletTemperature <= 1000.0f)
            {
                angle = 105 + (inletTemperature - 700.0f) / 10.0f * 5.4f;
                //5.5 degrees of space for 10 degrees Celcius of readout
            }
            else if (inletTemperature > 1000.0f && inletTemperature <= 1200.0f)
            {
                angle = 266 + (inletTemperature - 1000.0f) / 50.0f * 10.5f;
                //10.5 degrees of space for 50 degrees Celcius of readout
            }

            return angle;
        }
    }
}