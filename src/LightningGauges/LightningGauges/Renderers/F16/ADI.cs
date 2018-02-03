using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IADI : IInstrumentRenderer
    {
        ADI.ADIInstrumentState InstrumentState { get; set; }
    }

    public class ADI : InstrumentRendererBase, IADI
    {
        private const string ADI_BACKGROUND_IMAGE_FILENAME = "adi.bmp";
        private const string ADI_BACKGROUND_MASK_FILENAME = "adi_mask.bmp";
        private const string ADI_BALL_IMAGE_FILENAME = "adiball.bmp";
        private const string ADI_ARROWS_IMAGE_FILENAME = "adiarrows.bmp";
        private const string ADI_ARROWS_MASK_FILENAME = "adiarrows_mask.bmp";
        private const string ADI_AUX_FLAG_IMAGE_FILENAME = "adiaux.bmp";
        private const string ADI_AUX_FLAG_MASK_FILENAME = "adiflags_mask.bmp";
        private const string ADI_GLIDESLOPE_FLAG_IMAGE_FILENAME = "adigsflag.bmp";
        private const string ADI_GLIDESLOPE_FLAG_MASK_FILENAME = "adiflags_mask.bmp";
        private const string ADI_LOCALIZER_FLAG_IMAGE_FILENAME = "adiloc.bmp";
        private const string ADI_LOCALIZER_FLAG_MASK_FILENAME = "adiflags_mask.bmp";
        private const string ADI_OFF_FLAG_IMAGE_FILENAME = "adioff.bmp";
        private const string ADI_OFF_FLAG_MASK_FILENAME = "adiflags_mask.bmp";
        private const string ADI_HORIZONTAL_BAR_IMAGE_FILENAME = "adihorbar.bmp";
        private const string ADI_HORIZONTAL_BAR_MASK_FILENAME = "adihorbar_mask.bmp";
        private const string ADI_VERTICAL_BAR_IMAGE_FILENAME = "adiverbar.bmp";
        private const string ADI_VERTICAL_BAR_MASK_FILENAME = "adiverbar_mask.bmp";
        private const string ADI_AIRPLANE_SYMBOL_IMAGE_FILENAME = "adiplane.bmp";
        private const string ADI_AIRPLANE_SYMBOL_MASK_FILENAME = "adiplane_mask.bmp";
        private const string ADI_SLIP_INDICATOR_BALL_IMAGE_FILENAME = "adislip.bmp";
        private const string ADI_SLIP_INDICATOR_BALL_MASK_FILENAME = "adislip_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static Image _ball;
        private static ImageMaskPair _arrows;
        private static ImageMaskPair _auxFlag;
        private static ImageMaskPair _glideslopeFlag;
        private static ImageMaskPair _localizerFlag;
        private static ImageMaskPair _offFlag;
        private static ImageMaskPair _horizontalBar;
        private static ImageMaskPair _verticalBar;
        private static ImageMaskPair _airplaneSymbol;
        private static ImageMaskPair _slipIndicatorBall;
        private static bool _imagesLoaded;

        public ADI() { InstrumentState = new ADIInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_BACKGROUND_MASK_FILENAME);
            }

            if (_ball == null) _ball = Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_BALL_IMAGE_FILENAME);

            if (_arrows == null)
            {
                _arrows = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_ARROWS_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_ARROWS_MASK_FILENAME);
            }

            if (_auxFlag == null)
            {
                _auxFlag = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_AUX_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_AUX_FLAG_MASK_FILENAME);
            }

            if (_glideslopeFlag == null)
            {
                _glideslopeFlag = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_GLIDESLOPE_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_GLIDESLOPE_FLAG_MASK_FILENAME);
            }

            if (_localizerFlag == null)
            {
                _localizerFlag = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_LOCALIZER_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_LOCALIZER_FLAG_MASK_FILENAME);
            }

            if (_offFlag == null)
            {
                _offFlag = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_OFF_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_OFF_FLAG_MASK_FILENAME);
            }
            if (_horizontalBar == null)
            {
                _horizontalBar = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_HORIZONTAL_BAR_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_HORIZONTAL_BAR_MASK_FILENAME);
            }
            if (_verticalBar == null)
            {
                _verticalBar = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_VERTICAL_BAR_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_VERTICAL_BAR_MASK_FILENAME);
            }
            if (_airplaneSymbol == null)
            {
                _airplaneSymbol = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_AIRPLANE_SYMBOL_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_AIRPLANE_SYMBOL_MASK_FILENAME);
            }

            if (_slipIndicatorBall == null)
            {
                _slipIndicatorBall = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_SLIP_INDICATOR_BALL_IMAGE_FILENAME,
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + ADI_SLIP_INDICATOR_BALL_MASK_FILENAME);
            }
            _imagesLoaded = true;
        }

        public ADIInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class ADIInstrumentState : InstrumentStateBase
        {
            private const float MIN_PITCH = -90;
            private const float MAX_PITCH = 90;
            private const float MIN_ROLL = -180;
            private const float MAX_ROLL = 180;
            private const float DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES = 1.0F;
            private const float DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES = 5.0F;
            private float _glideslopeDeviationDegrees;
            private float _glideslopeDeviationLimitDegrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
            private float _localizerDeviationDegrees;
            private float _localizerDeviationLimitDegrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
            private float _pitchDegrees;
            private float _rollDegrees;

            public ADIInstrumentState()
            {
                PitchDegrees = 0;
                RollDegrees = 0;
                GlideslopeDeviationLimitDegrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
                GlideslopeDeviationDegrees = 0;
                LocalizerDeviationLimitDegrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
                LocalizerDeviationDegrees = 0;
                OffFlag = false;
                AuxFlag = false;
                GlideslopeFlag = false;
                LocalizerFlag = false;
                ShowCommandBars = false;
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


            public float LocalizerDeviationDegrees
            {
                get => _localizerDeviationDegrees;
                set
                {
                    var degrees = value;
                    degrees %= 360.0f;
                    if (float.IsNaN(degrees) || float.IsInfinity(degrees)) degrees = 0;
                    _localizerDeviationDegrees = degrees;
                }
            }


            public float LocalizerDeviationLimitDegrees
            {
                get => _localizerDeviationLimitDegrees;
                set
                {
                    var degrees = value;
                    degrees %= 360.0f;
                    if (float.IsInfinity(degrees) || float.IsNaN(degrees) || degrees == 0) degrees = DEFAULT_LOCALIZER_DEVIATION_LIMIT_DEGREES;
                    _localizerDeviationLimitDegrees = degrees;
                }
            }


            public float GlideslopeDeviationLimitDegrees
            {
                get => _glideslopeDeviationLimitDegrees;
                set
                {
                    var degrees = value;
                    degrees %= 360.0f;
                    if (float.IsNaN(degrees) || float.IsInfinity(degrees) || degrees == 0) degrees = DEFAULT_GLIDESLOPE_DEVIATION_LIMIT_DEGREES;
                    _glideslopeDeviationLimitDegrees = degrees;
                }
            }


            public float GlideslopeDeviationDegrees
            {
                get => _glideslopeDeviationDegrees;
                set
                {
                    var degrees = value;
                    degrees %= 360.0f;
                    if (float.IsInfinity(degrees) || float.IsNaN(degrees)) degrees = 0;
                    _glideslopeDeviationDegrees = degrees;
                }
            }


            public bool OffFlag { get; set; }
            public bool AuxFlag { get; set; }
            public bool GlideslopeFlag { get; set; }
            public bool LocalizerFlag { get; set; }
            public bool ShowCommandBars { get; set; }
        }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Image.Width - 66;
                var height = _background.Image.Height - 55;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-32, -31);

                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the ball
                const float pixelsPerDegreePitch = 2.0f;
                var pitchDegrees = InstrumentState.PitchDegrees;
                var rollDegrees = InstrumentState.RollDegrees;
                var centerPixelY = _ball.Height / 2.0f - pixelsPerDegreePitch * pitchDegrees;
                var topPixelY = centerPixelY - 80;
                var leftPixelX = _ball.Width / 2.0f - 80;
                var sourceRect = new RectangleF(leftPixelX, topPixelY, 160, 160);
                var destRect = new RectangleF(48, 40, sourceRect.Width, sourceRect.Height);

                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(128, 118);
                destinationGraphics.RotateTransform(-rollDegrees);
                destinationGraphics.TranslateTransform(-128, -118);
                destinationGraphics.TranslateTransform(-2, 0);
                destinationGraphics.DrawImageFast(_ball, destRect, sourceRect, GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the arrows
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(128, 118);
                destinationGraphics.RotateTransform(-rollDegrees);
                destinationGraphics.TranslateTransform(-128, -118);
                destinationGraphics.TranslateTransform(0, -10);
                destinationGraphics.DrawImageFast(_arrows.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the airplane symbol
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(0, -5);
                destinationGraphics.DrawImageFast(_airplaneSymbol.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                if (InstrumentState.ShowCommandBars && !InstrumentState.OffFlag)
                {
                    //draw the localizer bar
                    {
                        var positionPct = InstrumentState.LocalizerDeviationDegrees / InstrumentState.LocalizerDeviationLimitDegrees;
                        if (Math.Abs(positionPct) <= 1.0f)
                        {
                            const float canvasRange = 36.0f;
                            var pos = canvasRange * positionPct;
                            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                            destinationGraphics.TranslateTransform(pos, -10.0f);
                            destinationGraphics.DrawImageFast(_verticalBar.MaskedImage, new Point(0, 0));
                            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                        }
                    }

                    //draw the glideslope bar
                    {
                        var positionPct = InstrumentState.GlideslopeDeviationDegrees / InstrumentState.GlideslopeDeviationLimitDegrees;
                        if (Math.Abs(positionPct) <= 1.0f)
                        {
                            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                            const float canvasRange = 46.0f;
                            var pos = -canvasRange * positionPct;
                            destinationGraphics.TranslateTransform(0.0f, pos);
                            destinationGraphics.DrawImageFast(_horizontalBar.MaskedImage, new Point(0, 0));
                            GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                        }
                    }
                }

                //draw the localizer flag
                //if (InstrumentState.LocalizerFlag || InstrumentState.OffFlag)
                if (InstrumentState.LocalizerFlag) //Falcas 28-10-2012 LOC flag is not linked to the OffFlag
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.RotateTransform(25);
                    destinationGraphics.TranslateTransform(85, -133);
                    destinationGraphics.DrawImageFast(_localizerFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the glideslope flag
                //if (InstrumentState.GlideslopeFlag || InstrumentState.OffFlag)
                if (InstrumentState.GlideslopeFlag) //Falcas 28-10-2012 GS flag is not linked to the OffFlag
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.RotateTransform(-25);
                    destinationGraphics.TranslateTransform(-113, -25);
                    destinationGraphics.DrawImageFast(_glideslopeFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the aux flag
                if (InstrumentState.AuxFlag || InstrumentState.OffFlag)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.RotateTransform(-25);
                    destinationGraphics.TranslateTransform(-12, 102);
                    destinationGraphics.DrawImageFast(_auxFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the off flag
                if (InstrumentState.OffFlag)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.RotateTransform(25);
                    destinationGraphics.TranslateTransform(-15, -8);
                    destinationGraphics.DrawImageFast(_offFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //draw the slip indicator ball
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_slipIndicatorBall.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}