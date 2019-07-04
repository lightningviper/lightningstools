using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IVerticalVelocityIndicatorUSA : IVerticalVelocityIndicator
    {
    }

    public class VerticalVelocityIndicatorUSA : InstrumentRendererBase, IVerticalVelocityIndicatorUSA
    {
        private const string VVI_BACKGROUND_IMAGE_FILENAME = "vvi.bmp";
        private const string VVI_BACKGROUND_MASK_FILENAME = "vvi_mask.bmp";
        private const string VVI_OFF_FLAG_IMAGE_FILENAME = "vviflag.bmp";
        private const string VVI_OFF_FLAG_MASK_FILENAME = "vviflag_mask.bmp";
        private const string VVI_INDICATOR_LINE_IMAGE_FILENAME = "vvistrip.bmp";
        private const string VVI_INDICATOR_LINE_MASK_FILENAME = "vvistrip_mask.bmp";

        private const string VVI_NUMBER_TAPE_IMAGE_FILENAME = "vvinum.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static ImageMaskPair _background;
        private static ImageMaskPair _offFlag;
        private static ImageMaskPair _indicatorLine;
        private static Bitmap _numberTape;
        private static bool _imagesLoaded;

        public VerticalVelocityIndicatorUSA() { InstrumentState = new VerticalVelocityIndicatorInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null)
            {
                _background = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_BACKGROUND_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_BACKGROUND_MASK_FILENAME);
            }
            if (_offFlag == null)
            {
                _offFlag = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_OFF_FLAG_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_OFF_FLAG_MASK_FILENAME);
            }
            if (_indicatorLine == null)
            {
                _indicatorLine = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_INDICATOR_LINE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_INDICATOR_LINE_MASK_FILENAME);
            }
            if (_numberTape == null) _numberTape = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + VVI_NUMBER_TAPE_IMAGE_FILENAME);
            _imagesLoaded = true;
        }

        public VerticalVelocityIndicatorInstrumentState InstrumentState { get; set; }

        public override void Render(Graphics destinationGraphics, Rectangle destinationRectangle)
        {
            if (!_imagesLoaded) LoadImageResources();
            lock (_imagesLock)
            {
                //store the canvas's transform and clip settings so we can restore them later
                var initialState = destinationGraphics.Save();

                //set up the canvas scale and clipping region
                var width = _background.Image.Width;
                width -= 154;
                var height = _background.Image.Height - 29;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-76, 0);
                destinationGraphics.TranslateTransform(0, -15);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                if (!InstrumentState.OffFlag)
                {
                    //draw the number tape
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    const float translateX = 110;
                    float translateY = 236;
                    const float pixelsPerHundredFeet = 4.75f;
                    var vv = InstrumentState.VerticalVelocityFeet;
                    if (Math.Abs(vv) > 6000.0) vv = Math.Sign(vv) * 6000.0f;
                    var verticalVelocityThousands = vv / 1000.0f;
                    translateY -= -pixelsPerHundredFeet * verticalVelocityThousands * 10.0f;
                    translateY -= _numberTape.Height / 2.0f;
                    destinationGraphics.TranslateTransform(translateX, translateY);
                    destinationGraphics.ScaleTransform(0.79f, 0.79f);
                    destinationGraphics.DrawImageFast(_numberTape, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }
                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(_background.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the indicator line
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(0, 1);
                destinationGraphics.DrawImageFast(_indicatorLine.MaskedImage, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the OFF flag
                if (InstrumentState.OffFlag)
                {
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                    destinationGraphics.TranslateTransform(0, -3);
                    destinationGraphics.DrawImageFast(_offFlag.MaskedImage, new Point(0, 0));
                    GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                }

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}