using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IStandbyADI : IInstrumentRenderer
    {
        StandbyADI.StandbyADIInstrumentState InstrumentState { get; set; }
    }

    public class StandbyADI : InstrumentRendererBase, IStandbyADI
    {
        private const string BUADI_BACKGROUND_IMAGE_FILENAME = "buadi.bmp";
        private const string BUADI_BACKGROUND_MASK_FILENAME = "buadi_mask.bmp";
        private const string BUADI_BALL_IMAGE_FILENAME = "buadiball.bmp";
        private const string BUADI_ARROWS_IMAGE_FILENAME = "buadislip.bmp";
        private const string BUADI_ARROWS_MASK_FILENAME = "buadislip_mask.bmp";
        private const string BUADI_OFF_FLAG_IMAGE_FILENAME = "buadiflag.bmp";
        private const string BUADI_OFF_FLAG_MASK_FILENAME = "buadiflag_mask.bmp";
        private const string BUADI_AIRPLANE_SYMBOL_IMAGE_FILENAME = "buadiplane.bmp";
        private const string BUADI_AIRPLANE_SYMBOL_MASK_FILENAME = "buadiplane_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static Image _ball;
        private static ImageMaskPair _arrows;
        private static ImageMaskPair _offFlag;
        private static ImageMaskPair _airplaneSymbol;
        private static bool _imagesLoaded;

        public StandbyADI() { InstrumentState = new StandbyADIInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_BACKGROUND_MASK_FILENAME);
            }

            if (_ball == null) _ball = ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_BALL_IMAGE_FILENAME);

            if (_arrows == null)
            {
                _arrows = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_ARROWS_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_ARROWS_MASK_FILENAME);
            }

            if (_offFlag == null)
            {
                _offFlag = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_OFF_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_OFF_FLAG_MASK_FILENAME);
            }
            if (_airplaneSymbol == null)
            {
                _airplaneSymbol = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_AIRPLANE_SYMBOL_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + BUADI_AIRPLANE_SYMBOL_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public StandbyADIInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class StandbyADIInstrumentState : InstrumentStateBase
        {
            private const float MIN_PITCH = -90;
            private const float MAX_PITCH = 90;
            private const float MIN_ROLL = -180;
            private const float MAX_ROLL = 180;
            private float _pitchDegrees;
            private float _rollDegrees;

            public StandbyADIInstrumentState()
            {
                PitchDegrees = 0;
                RollDegrees = 0;
                OffFlag = false;
            }


            public float PitchDegrees
            {
                get => _pitchDegrees;
                set
                {
                    var pitch = value;
                    if (pitch < MIN_PITCH) pitch = MIN_PITCH;
                    if (pitch > MAX_PITCH) pitch = MAX_PITCH;
                    if (float.IsNaN(pitch) || float.IsInfinity(pitch)) pitch = 0;
                    _pitchDegrees = pitch;
                }
            }


            public float RollDegrees
            {
                get => _rollDegrees;
                set
                {
                    var roll = value;
                    if (roll < MIN_ROLL) roll = MIN_ROLL;
                    if (roll > MAX_ROLL) roll = MAX_ROLL;
                    if (float.IsInfinity(roll) || float.IsNaN(roll)) roll = 0;
                    _rollDegrees = roll;
                }
            }


            public bool OffFlag { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Image.Width - 84;
                var height = _background.Image.Height - 84;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-42, -42);

                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the ball
                const float pixelsPerDegreePitch = 2.0f;
                var pitch = InstrumentState.PitchDegrees;
                var roll = InstrumentState.RollDegrees;
                var centerPixelY = _ball.Height / 2.0f - pixelsPerDegreePitch * pitch;
                var topPixelY = centerPixelY - 80;
                var leftPixelX = _ball.Width / 2.0f - 73;
                var sourceRect = new RectangleF(leftPixelX, topPixelY, 160, 160);
                var destRect = new RectangleF(48, 48, sourceRect.Width, sourceRect.Height);

                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                const float translateX = 127;
                const float translateY = 125;

                destinationGraphics.TranslateTransform(0, 0);
                destinationGraphics.TranslateTransform(translateX, translateY);
                destinationGraphics.RotateTransform(-roll);
                destinationGraphics.TranslateTransform(-translateX, -translateY);
                destinationGraphics.DrawImageFast(_ball, destRect, sourceRect, GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the arrows
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(0, 0);
                destinationGraphics.TranslateTransform(translateX, translateY);
                destinationGraphics.RotateTransform(-roll);
                destinationGraphics.TranslateTransform(-translateX, -translateY);
                destinationGraphics.DrawImageFast(_arrows.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the airplane symbol
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(0, 3);
                destinationGraphics.DrawImageFast(_airplaneSymbol.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the off flag
                if (InstrumentState.OffFlag)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.DrawImageFast(_offFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}