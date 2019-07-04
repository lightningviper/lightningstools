using System;
using System.IO;
using Common.Drawing;
using Common.Imaging;
using Common.SimSupport;

namespace LightningGauges.Renderers.F16
{
    public interface IPitchTrimIndicator : IInstrumentRenderer
    {
        PitchTrimIndicator.PitchTrimIndicatorInstrumentState InstrumentState { get; set; }
    }

    public class PitchTrimIndicator : InstrumentRendererBase, IPitchTrimIndicator
    {
        private const string PITCHTRIM_BACKGROUND_IMAGE_FILENAME = "pitchtrim.bmp";
        private const string PITCHTRIM_NEEDLE_IMAGE_FILENAME = "pitchtrimneedle.bmp";
        private const string PITCHTRIM_NEEDLE_MASK_FILENAME = "pitchtrimneedle_mask.bmp";

        private static readonly string IMAGES_FOLDER_NAME =  "images";

        private static readonly object _imagesLock = new object();
        private static Bitmap _background;
        private static Bitmap _needle;
        private static bool _imagesLoaded;

        public PitchTrimIndicator() { InstrumentState = new PitchTrimIndicatorInstrumentState(); }

        private static void LoadImageResources()
        {
            if (_background == null) _background = (Bitmap) ResourceUtil.LoadBitmapFromEmbeddedResource(IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + PITCHTRIM_BACKGROUND_IMAGE_FILENAME);
            if (_needle == null)
            {
                using (var needleWithMask = ResourceUtil.CreateImageMaskPairFromEmbeddedResources(
                    IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + PITCHTRIM_NEEDLE_IMAGE_FILENAME, IMAGES_FOLDER_NAME + Path.DirectorySeparatorChar + PITCHTRIM_NEEDLE_MASK_FILENAME))
                {
                    needleWithMask.Use1BitAlpha = true;
                    _needle = (Bitmap) Util.CropBitmap(needleWithMask.MaskedImage, new Rectangle(97, 68, 60, 60));
                    _needle.MakeTransparent(Color.Black);
                }
            }
            _imagesLoaded = true;
        }

        public PitchTrimIndicatorInstrumentState InstrumentState { get; set; }

        [Serializable]
        public class PitchTrimIndicatorInstrumentState : InstrumentStateBase
        {
            private float _pitchTrimPercent;

            public PitchTrimIndicatorInstrumentState() { PitchTrimPercent = 0; }


            public float PitchTrimPercent
            {
                get => _pitchTrimPercent;
                set
                {
                    var pct = value;
                    if (float.IsInfinity(pct) || float.IsNaN(pct)) pct = 0;
                    if (Math.Abs(pct) > 100.0f) pct = Math.Sign(pct) * 100.0f;
                    _pitchTrimPercent = pct;
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
                var width = _background.Width - 148;
                var height = _background.Height - 148;
                destinationGraphics.ResetTransform(); //clear any existing transforms
                destinationGraphics.SetClip(destinationRectangle); //set the clipping region on the graphics object to our render rectangle's boundaries
                destinationGraphics.FillRectangle(Brushes.Black, destinationRectangle);
                destinationGraphics.ScaleTransform(destinationRectangle.Width / (float) width, destinationRectangle.Height / (float) height);
                //set the initial scale transformation 
                destinationGraphics.TranslateTransform(-75, -68);
                //save the basic canvas transform and clip settings so we can revert to them later, as needed
                var basicState = destinationGraphics.Save();

                //draw the background image
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.DrawImageFast(
                    _background, new Rectangle(0, 0, _background.Width, _background.Height), new Rectangle(0, 0, _background.Width, _background.Height), GraphicsUnit.Pixel);
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //draw the needle
                var pt = InstrumentState.PitchTrimPercent;
                var angle = pt / 100.0f * 90;

                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);
                destinationGraphics.TranslateTransform(78, 95);
                destinationGraphics.TranslateTransform(20, -25);
                destinationGraphics.TranslateTransform(_needle.Width / 2.0f, _needle.Height / 2.0f);
                destinationGraphics.RotateTransform(angle);
                destinationGraphics.TranslateTransform(-_needle.Width / 2.0f, -_needle.Height / 2.0f);
                destinationGraphics.DrawImageFast(_needle, new Point(0, 0));
                GraphicsUtil.RestoreGraphicsState(destinationGraphics, ref basicState);

                //restore the canvas's transform and clip settings to what they were when we entered this method
                destinationGraphics.Restore(initialState);
            }
        }
    }
}