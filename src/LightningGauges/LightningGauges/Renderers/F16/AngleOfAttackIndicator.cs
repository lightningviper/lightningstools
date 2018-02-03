using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IAngleOfAttackIndicator : IInstrumentRenderer

    {
        AngleOfAttackIndicator.AngleOfAttackIndicatorInstrumentState InstrumentState { get; set; }
    }

    public class AngleOfAttackIndicator : InstrumentRendererBase, IAngleOfAttackIndicator
    {
        private const string AOA_BACKGROUND_IMAGE_FILENAME = "aoa.bmp";
        private const string AOA_BACKGROUND_MASK_FILENAME = "aoa_mask.bmp";
        private const string AOA_OFF_FLAG_IMAGE_FILENAME = "aoaflag.bmp";
        private const string AOA_OFF_FLAG_MASK_FILENAME = "aoaflag_mask.bmp";
        private const string AOA_INDICATOR_LINE_IMAGE_FILENAME = "aoastrip.bmp";
        private const string AOA_INDICATOR_LINE_MASK_FILENAME = "aoastrip_mask.bmp";

        private const string AOA_NUMBER_TAPE_IMAGE_FILENAME = "aoanum.bmp";

        private static readonly string IMAGES_FOLDER_NAME = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + Path.DirectorySeparatorChar + "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static ImageMaskPair _offFlag;
        private static ImageMaskPair _indicatorLine;
        private static Bitmap _numberTape;
        private static bool _imagesLoaded;

        public AngleOfAttackIndicator() { InstrumentState = new AngleOfAttackIndicatorInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_BACKGROUND_MASK_FILENAME);
            }
            if (_offFlag == null)
            {
                _offFlag = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_OFF_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_OFF_FLAG_MASK_FILENAME);
            }
            if (_indicatorLine == null)
            {
                _indicatorLine = ImageMaskPair.CreateFromFiles(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_INDICATOR_LINE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_INDICATOR_LINE_MASK_FILENAME);
            }
            if (_numberTape == null) _numberTape = (Bitmap) Util.LoadBitmapFromFile(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + AOA_NUMBER_TAPE_IMAGE_FILENAME);
            _imagesLoaded = true;
        }

        public AngleOfAttackIndicatorInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class AngleOfAttackIndicatorInstrumentState : InstrumentStateBase
        {
            private const float MIN_AOA = -35.0F;
            private const float MAX_AOA = 35.0f;
            private float _angleOfAttackDegrees;

            public AngleOfAttackIndicatorInstrumentState()
            {
                OffFlag = false;
                AngleOfAttackDegrees = 0.0f;
            }


            public float AngleOfAttackDegrees
            {
                get => _angleOfAttackDegrees;
                set
                {
                    var degrees = value;
                    if (float.IsNaN(degrees) || float.IsInfinity(degrees)) degrees = 0;
                    degrees %= 360.0F;
                    if (degrees < MIN_AOA) degrees = MIN_AOA;
                    if (degrees > MAX_AOA) degrees = MAX_AOA;
                    _angleOfAttackDegrees = degrees;
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
                var width = _background.Image.Width;
                width -= 153;
                var height = _background.Image.Height - 28;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 

                destinationGraphics.TranslateTransform(0, -12);
                destinationGraphics.TranslateTransform(-75, 0);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                if (!InstrumentState.OffFlag)
                {
                    //draw the number tape
                    var aoaDegrees = InstrumentState.AngleOfAttackDegrees;
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    const float translateX = 105;
                    float translateY = 253;
                    const float pixelsPerDegreeAoa = 10f;
                    translateY -= -pixelsPerDegreeAoa * aoaDegrees;
                    translateY -= _numberTape.Height / 2.0f;
                    destinationGraphics.TranslateTransform(translateX, translateY);
                    destinationGraphics.ScaleTransform(0.75f, 0.75f);
                    destinationGraphics.DrawImageFast(_numberTape, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }
                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the indicator line
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(0, 0);
                destinationGraphics.DrawImageFast(_indicatorLine.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the OFF flag
                if (InstrumentState.OffFlag)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.TranslateTransform(0, 0);
                    destinationGraphics.DrawImageFast(_offFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}