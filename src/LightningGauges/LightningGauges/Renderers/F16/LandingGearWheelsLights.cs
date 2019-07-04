using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface ILandingGearWheelsLights : IInstrumentRenderer
    {
        LandingGearWheelsLights.LandingGearWheelsLightsInstrumentState InstrumentState { get; set; }
    }

    public class LandingGearWheelsLights : InstrumentRendererBase, ILandingGearWheelsLights
    {
        private const string GEAR_BACKGROUND_IMAGE_FILENAME = "gear.bmp";
        private const string GEAR_BACKGROUND_MASK_FILENAME = "gear_mask.bmp";

        private const string GEAR_LEFT_GEAR_LIGHT_IMAGE_FILENAME = "leftgr.bmp";
        private const string GEAR_LEFT_GEAR_LIGHT_MASK_FILENAME = "leftgr_mask.bmp";
        private const string GEAR_RIGHT_GEAR_LIGHT_IMAGE_FILENAME = "rightgr.bmp";
        private const string GEAR_RIGHT_GEAR_LIGHT_MASK_FILENAME = "rightgr_mask.bmp";
        private const string GEAR_NOSE_GEAR_LIGHT_IMAGE_FILENAME = "nosegr.bmp";
        private const string GEAR_NOSE_GEAR_LIGHT_MASK_FILENAME = "nosegr_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static ImageMaskPair _noseGearLight;
        private static ImageMaskPair _leftGearLight;
        private static ImageMaskPair _rightGearLight;
        private static bool _imagesLoaded;

        public LandingGearWheelsLights() { InstrumentState = new LandingGearWheelsLightsInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_BACKGROUND_MASK_FILENAME);
                _background.Use1BitAlpha = true;
            }
            if (_leftGearLight == null)
            {
                _leftGearLight = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_LEFT_GEAR_LIGHT_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_LEFT_GEAR_LIGHT_MASK_FILENAME);
            }

            if (_rightGearLight == null)
            {
                _rightGearLight = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_RIGHT_GEAR_LIGHT_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_RIGHT_GEAR_LIGHT_MASK_FILENAME);
            }

            if (_noseGearLight == null)
            {
                _noseGearLight = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_NOSE_GEAR_LIGHT_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + GEAR_NOSE_GEAR_LIGHT_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public LandingGearWheelsLightsInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class LandingGearWheelsLightsInstrumentState : InstrumentStateBase
        {
            public bool LeftGearDown { get; set; }
            public bool RightGearDown { get; set; }
            public bool NoseGearDown { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                const int width = 142;
                const int height = 168;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-56, -29);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the left gear light
                if (InstrumentState.LeftGearDown)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_leftGearLight.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the right gear light
                if (InstrumentState.RightGearDown)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_rightGearLight.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the nose gear light
                if (InstrumentState.NoseGearDown)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_noseGearLight.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}