using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IAirspeedIndicator : IInstrumentRenderer
    {
        AirspeedIndicator.AirspeedIndicatorInstrumentState InstrumentState { get; set; }
        float GetMachAngle(float machNumber);
    }

    public class AirspeedIndicator : InstrumentRendererBase, IAirspeedIndicator
    {
        private const string AIRSPEED_BACKGROUND_IMAGE_FILENAME = "asi.bmp";
        private const string AIRSPEED_MACH_WHEEL_IMAGE_FILENAME = "asiinner.bmp";
        private const string AIRSPEED_MACH_WHEEL_MASK_FILENAME = "asiinnermask.bmp";
        private const string AIRSPEED_SPEED_BUG_IMAGE_FILENAME = "asispeed.bmp";
        private const string AIRSPEED_SPEED_BUG_MASK_FILENAME = "asispeed_mask.bmp";
        private const string AIRSPEED_POINTER_IMAGE_FILENAME = "arrow_asi.bmp";
        private const string AIRSPEED_POINTER_MASK_FILENAME = "arrow_asimask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static Bitmap _background;
        private static ImageMaskPair _machWheel;
        private static ImageMaskPair _speedBug;
        private static ImageMaskPair _airspeedPointerWheel;
        private static bool _imagesLoaded;

        public AirspeedIndicator() { InstrumentState = new AirspeedIndicatorInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null) _background = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_BACKGROUND_IMAGE_FILENAME);
            if (_machWheel == null)
            {
                _machWheel = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_MACH_WHEEL_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_MACH_WHEEL_MASK_FILENAME);
                _machWheel.Use1BitAlpha = true;
            }
            if (_speedBug == null)
            {
                _speedBug = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_SPEED_BUG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_SPEED_BUG_MASK_FILENAME);
            }

            if (_airspeedPointerWheel == null)
            {
                _airspeedPointerWheel = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_POINTER_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AIRSPEED_POINTER_MASK_FILENAME);
                _airspeedPointerWheel.Use1BitAlpha = true;
            }
            _imagesLoaded = true;
        }

        public AirspeedIndicatorInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class AirspeedIndicatorInstrumentState : InstrumentStateBase
        {
            private const float MAX_MACH = 3.0F;
            private const float MAX_AIRSPEED = 850.0f;
            private const float MAX_AIRSPEED_INDEX_KNOTS = 850.0f;
            private const float MIN_AIRSPEED_INDEX_KNOTS = 80.0f;
            private const float DEFAULT_AIRSPEED_INDEX_KNOTS = 250;
            private float _airspeedIndexKnots;
            private float _airspeedKnots;
            private float _machNumber;
            private float _neverExceedSpeedKnots;

            public AirspeedIndicatorInstrumentState()
            {
                AirspeedIndexKnots = DEFAULT_AIRSPEED_INDEX_KNOTS;
                AirspeedKnots = 0;
                MachNumber = 0;
                NeverExceedSpeedKnots = 0;
            }


            public float AirspeedKnots
            {
                get => _airspeedKnots;
                set
                {
                    var knots = value;
                    if (float.IsNaN(knots) || float.IsInfinity(knots) || knots < 0) knots = 0;
                    if (knots > MAX_AIRSPEED) knots = MAX_AIRSPEED;
                    _airspeedKnots = knots;
                }
            }


            public float MachNumber
            {
                get => _machNumber;
                set
                {
                    var mach = value;
                    if (mach < 0) mach = 0;
                    if (mach > MAX_MACH) mach = MAX_MACH;
                    _machNumber = mach;
                }
            }


            public float AirspeedIndexKnots
            {
                get => _airspeedIndexKnots;
                set
                {
                    var knots = value;
                    if (knots < MIN_AIRSPEED_INDEX_KNOTS) knots = MIN_AIRSPEED_INDEX_KNOTS;
                    if (knots > MAX_AIRSPEED_INDEX_KNOTS) knots = MAX_AIRSPEED_INDEX_KNOTS;
                    _airspeedIndexKnots = knots;
                }
            }


            public float NeverExceedSpeedKnots
            {
                get => _neverExceedSpeedKnots;
                set
                {
                    var vne = value;
                    if (vne < 0) vne = 0;
                    if (vne > MAX_AIRSPEED) vne = MAX_AIRSPEED;
                    _neverExceedSpeedKnots = vne;
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
                const int width = 191;
                const int height = 191;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-31, -34);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                var airspeed = InstrumentState.AirspeedKnots;
                var airspeedIndex = InstrumentState.AirspeedIndexKnots;
                var mach = InstrumentState.MachNumber;
                //calculate airspeed and mach wheel angles
                var airspeedAngle = GetPointerAngle(airspeed);
                var airspeedIndexAngle = GetPointerAngle(airspeedIndex);
                var machAngle = -GetMachAngle(mach);
                machAngle = (machAngle + 240.5f + airspeedAngle) % 360.0f;
                airspeedAngle = (airspeedAngle + 128.0f) % 360.0f;
                //compensate for pointer image not being oriented with zero up

                //draw the mach wheel
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.ScaleTransform(1.13f, 1.13f);
                destinationGraphics.TranslateTransform(-13.5f, -12.5f);
                destinationGraphics.TranslateTransform(125.5F, 127.5F);
                destinationGraphics.RotateTransform(machAngle);
                destinationGraphics.TranslateTransform(-125.5F, -127.5F);
                destinationGraphics.DrawImageFast(
                    _machWheel.MaskedImage, new Rectangle(0, 0, _machWheel.MaskedImage.Width, _machWheel.MaskedImage.Height),
                    new Rectangle(0, 0, _machWheel.MaskedImage.Width, _machWheel.MaskedImage.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the airspeed pointer wheel
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.ScaleTransform(1.13f, 1.13f);
                destinationGraphics.TranslateTransform(-13.5f, -12.5f);
                destinationGraphics.TranslateTransform(126F, 127F);
                destinationGraphics.RotateTransform(airspeedAngle);
                destinationGraphics.TranslateTransform(-126F, -127F);
                destinationGraphics.DrawImageFast(
                    _airspeedPointerWheel.MaskedImage, new Rectangle(0, 0, _airspeedPointerWheel.MaskedImage.Width, _airspeedPointerWheel.MaskedImage.Height),
                    new Rectangle(0, 0, _airspeedPointerWheel.MaskedImage.Width, _airspeedPointerWheel.MaskedImage.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the airspeed index bug
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(126, 130);
                destinationGraphics.RotateTransform(airspeedIndexAngle);
                destinationGraphics.TranslateTransform(-126, -130);
                destinationGraphics.DrawImageFast(
                    _speedBug.MaskedImage, new Rectangle(0, 0, _speedBug.MaskedImage.Width, _speedBug.MaskedImage.Height),
                    new Rectangle(0, 0, _speedBug.MaskedImage.Width, _speedBug.MaskedImage.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }

        private static float GetPointerAngle(float airspeed)
        {
            float angle;
            if (airspeed >= 0.0f && airspeed <= 80.0f)
            {
                angle = airspeed / 80.0f * 15.0f; //15 degrees of space for 80 knots of readout
            }
            else if (airspeed > 80.0f && airspeed <= 100.0f)
            {
                angle = 15 + (airspeed - 80.0f) / 20.0f * 20.0f; //20 degrees of space for 20 knots of readout
            }
            else if (airspeed > 100.0f && airspeed <= 150.0f)
            {
                angle = 35 + (airspeed - 100.0f) / 50.0f * 54.0f; //54 degrees of space for 50 knots of readout
            }
            else if (airspeed > 150.0f && airspeed <= 180.0f)
            {
                angle = 89 + (airspeed - 150.0f) / 30.0f * 32.0f; //32.0 degrees of space for 30 knots of readout
            }
            else if (airspeed > 180.0f && airspeed <= 200.0f)
            {
                angle = 121 + (airspeed - 180.0f) / 20.0f * 17.0f; //17.0 degrees of space for 20 knots of readout
            }
            else if (airspeed > 200.0f && airspeed <= 250.0f)
            {
                angle = 138 + (airspeed - 200.0f) / 50.0f * 29.0f; //29 degrees of space for 50 knots of readout
            }
            else if (airspeed > 250.0f && airspeed <= 300.0f)
            {
                angle = 167 + (airspeed - 250.0f) / 50.0f * 25.0f; //25 degrees of space for 50 knots of readout
            }
            else if (airspeed > 300.0f && airspeed <= 350.0f)
            {
                angle = 192 + (airspeed - 300.0f) / 50.0f * 21f; //21 degrees of space for 50 knots of readout
            }
            else if (airspeed > 350.0f && airspeed <= 400.0f)
            {
                angle = 213f + (airspeed - 350.0f) / 50.0f * 18f; //18 degrees of space for 50 knots of readout
            }
            else if (airspeed > 400.0f && airspeed <= 450.0f)
            {
                angle = 231 + (airspeed - 400.0f) / 50.0f * 17.0f; //17 degrees of space for 50 knots of readout
            }
            else if (airspeed > 450.0f && airspeed <= 500.0f)
            {
                angle = 248 + (airspeed - 450.0f) / 50.0f * 15.0f; //15 degrees of space for 50 knots of readout
            }
            else if (airspeed > 500.0f && airspeed <= 600.0f)
            {
                angle = 263 + (airspeed - 500.0f) / 100.0f * 27.0f; //27 degrees of space for 100 knots of readout
            }
            else if (airspeed > 600.0f && airspeed <= 700.0f)
            {
                angle = 290 + (airspeed - 600.0f) / 100.0f * 24.0f; //24 degrees of space for 100 knots of readout
            }
            else if (airspeed > 700.0f && airspeed <= 800.0f)
            {
                angle = 314 + (airspeed - 700.0f) / 100.0f * 21.5f; //21.5 degrees of space for 100 knots of readout
            }
            else if (airspeed > 800.0f && airspeed <= 850.0f)
            {
                angle = 337.5f + (airspeed - 800.0f) / 50.0f * 8.0f; //8 degrees of space for 50 knots of readout
            }
            else if (airspeed > 850.0f) { angle = 345.5f; }
            else { angle = 0; }
            return angle;
        }

        public float GetMachAngle(float machNumber)
        {
            float angle;
            if (machNumber >= 0.0f && machNumber <= 0.5f)
            {
                angle = machNumber / 0.5f * 32.0f; //32 degrees of space for 0.5 mach of readout
            }
            else if (machNumber > 0.50f && machNumber <= 0.55f)
            {
                angle = 32 + (machNumber - 0.50f) / 0.05f * 14.0f; //14 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.55f && machNumber <= 0.60f)
            {
                angle = 46 + (machNumber - 0.55f) / 0.05f * 13.0f; //13 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.60f && machNumber <= 0.65f)
            {
                angle = 59 + (machNumber - 0.60f) / 0.05f * 12.5f; //12.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.65f && machNumber <= 0.70f)
            {
                angle = 71.5f + (machNumber - 0.65f) / 0.05f * 11.5f; //11.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.70f && machNumber <= 0.75f)
            {
                angle = 84f + (machNumber - 0.70f) / 0.05f * 11.0f; //11 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.75f && machNumber <= 0.80f)
            {
                angle = 94f + (machNumber - 0.75f) / 0.05f * 10.5f; //10.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.80f && machNumber <= 0.85f)
            {
                angle = 104.5f + (machNumber - 0.80f) / 0.05f * 8.0f; //8 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.85f && machNumber <= 0.90f)
            {
                angle = 112.5f + (machNumber - 0.85f) / 0.05f * 7f; //7 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 0.90f && machNumber <= 1.00f)
            {
                angle = 120.5f + (machNumber - 0.90f) / 0.10f * 15.5f; //15.5 degrees of space for 0.10 mach of readout
            }
            else if (machNumber > 1.00f && machNumber <= 1.05f)
            {
                angle = 136.0f + (machNumber - 1.00f) / 0.05f * 8.0f; //8 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.05f && machNumber <= 1.10f)
            {
                angle = 144.0f + (machNumber - 1.05f) / 0.05f * 8.0f; //8 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.10f && machNumber <= 1.15f)
            {
                angle = 152.0f + (machNumber - 1.10f) / 0.05f * 7f; //7 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.15f && machNumber <= 1.20f)
            {
                angle = 159f + (machNumber - 1.15f) / 0.05f * 8.5f; //8.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.20f && machNumber <= 1.25f)
            {
                angle = 167.5f + (machNumber - 1.20f) / 0.05f * 6.5f; //6.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.25f && machNumber <= 1.30f)
            {
                angle = 174.0f + (machNumber - 1.25f) / 0.05f * 6.0f; //6 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.30f && machNumber <= 1.35f)
            {
                angle = 180.0f + (machNumber - 1.30f) / 0.05f * 5.5f; //5.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.35f && machNumber <= 1.40f)
            {
                angle = 185.5f + (machNumber - 1.35f) / 0.05f * 5.5f; //5.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.40f && machNumber <= 1.45f)
            {
                angle = 191f + (machNumber - 1.40f) / 0.05f * 5.0f; //5.0 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.45f && machNumber <= 1.50f)
            {
                angle = 196.5f + (machNumber - 1.45f) / 0.05f * 4.5f; //4.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.50f && machNumber <= 1.55f)
            {
                angle = 201f + (machNumber - 1.50f) / 0.05f * 5.5f; //5.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.55f && machNumber <= 1.60f)
            {
                angle = 206.5f + (machNumber - 1.55f) / 0.05f * 5.0f; //5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.60f && machNumber <= 1.65f)
            {
                angle = 211.5f + (machNumber - 1.60f) / 0.05f * 4.0f; //4 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.65f && machNumber <= 1.70f)
            {
                angle = 215.5f + (machNumber - 1.65f) / 0.05f * 4.5f; //4.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.70f && machNumber <= 1.75f)
            {
                angle = 220.0f + (machNumber - 1.70f) / 0.05f * 5.0f; //5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.75f && machNumber <= 1.80f)
            {
                angle = 225.0f + (machNumber - 1.75f) / 0.05f * 4.5f; //4.5 degrees of space for 0.05 mach of readout
            }
            else if (machNumber > 1.80) { angle = 229.5f; }
            else { angle = 0; }
            return angle;
        }
    }
}